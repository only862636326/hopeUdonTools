using System;
using System.Net;
using System.Text;
using System.Threading;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 简易本地 HTTP Server 编辑器扩展
/// - 监听 5004 端口
/// - 输入框 Enter → 添加到缓冲区(bufferText) → 清空输入框
/// - 收到 GET 请求时 → 返回整个缓冲区内容 → 清空缓冲区 → 追加到 historyText
/// </summary>
public class HtoolSimpleWebSever : EditorWindow
{
    #region Fields

    private string inputText = "";
    private string bufferText = "";
    private string historyText = "";
    private Vector2 historyScroll;
    private Vector2 bufferScroll;

    private HttpListener listener;
    private Thread serverThread;
    private bool isRunning;

    #endregion

    #region Menu Entry

    [MenuItem("HopeTools/Htool Simple Web Server")]
    private static void ShowWindow()
    {
        var win = GetWindow<HtoolSimpleWebSever>("Htool Web Server");
        win.minSize = new Vector2(400, 400);
        win.Show();
    }

    #endregion

    #region Window GUI

    private void OnGUI()
    {
        EditorGUILayout.Space(10);

        // === Server control ===
        EditorGUILayout.BeginHorizontal();

        GUI.enabled = !isRunning;
        if (GUILayout.Button("Start Server (5004)", GUILayout.Height(30)))
            StartServer();

        GUI.enabled = isRunning;
        if (GUILayout.Button("Stop Server", GUILayout.Height(30)))
            StopServer();

        GUI.enabled = true;

        EditorGUILayout.EndHorizontal();

        // Server status indicator
        EditorGUILayout.Space(4);
        EditorGUILayout.LabelField("Status", isRunning ? "Listening on http://localhost:5004/" : "Stopped");
        EditorGUILayout.Space(10);

        // === History (sent items - displayed first) ===
        EditorGUILayout.LabelField("History (sent items):", EditorStyles.boldLabel);
        historyScroll = EditorGUILayout.BeginScrollView(historyScroll, GUILayout.ExpandHeight(true));
        EditorGUILayout.SelectableLabel(historyText, EditorStyles.textArea, GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Clear History", GUILayout.Width(100)))
        {
            historyText = "";
            historyScroll = Vector2.zero;
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(10);

        // === Buffer (accumulated items waiting to be sent) ===
        EditorGUILayout.LabelField("Buffer (accumulated, returned on next request):", EditorStyles.boldLabel);
        bufferScroll = EditorGUILayout.BeginScrollView(bufferScroll, GUILayout.MinHeight(60), GUILayout.MaxHeight(120));
        EditorGUILayout.SelectableLabel(bufferText, EditorStyles.textArea, GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Clear Buffer", GUILayout.Width(100)))
            bufferText = "";
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(10);

        // === Input (multi-line, supports paste with line breaks, Enter to submit) ===
        EditorGUILayout.LabelField("Input text (Enter to add, paste multi-line ok):", EditorStyles.boldLabel);

        GUI.SetNextControlName("inputField");

        // Intercept Enter to submit instead of adding newline
        if (Event.current.isKey && Event.current.keyCode == KeyCode.Return
            && Event.current.type == EventType.KeyDown
            && GUI.GetNameOfFocusedControl() == "inputField")
        {
            if (!string.IsNullOrEmpty(inputText))
            {
                bufferText = string.IsNullOrEmpty(bufferText)
                    ? inputText
                    : bufferText + "\n" + inputText;
                inputText = "";
            }
            Event.current.Use();
            GUI.FocusControl("inputField");
            Repaint();
        }

        inputText = EditorGUILayout.TextArea(inputText, GUILayout.MinHeight(60));

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add to Buffer", GUILayout.Width(110)))
        {
            AddInputToBuffer();
            GUI.FocusControl("inputField");
        }

        if (GUILayout.Button("Clear Input", GUILayout.Width(100)))
            inputText = "";
        EditorGUILayout.EndHorizontal();

        // Keep repainting to handle async UI updates from server thread
        if (isRunning)
            Repaint();
    }

    public void AppendToBuffer(string text)
    {
        if (string.IsNullOrEmpty(text))
            return;

        bufferText = string.IsNullOrEmpty(bufferText)
            ? text
            : bufferText + "\n" + text;
        Repaint();
    }

    private void AddInputToBuffer()
    {
        if (string.IsNullOrEmpty(inputText))
            return;

        AppendToBuffer(inputText);
        inputText = "";
    }

    #endregion

    #region Server Lifecycle

    private void StartServer()
    {
        if (isRunning) return;

        try
        {
            listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:5004/");
            listener.Start();
            isRunning = true;

            serverThread = new Thread(ServerLoop)
            {
                IsBackground = true,
                Name = "HtoolWebServer"
            };
            serverThread.Start();

            Debug.Log("[HtoolWebServer] Started on http://localhost:5004/");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[HtoolWebServer] Failed to start: {ex.Message}");
            isRunning = false;
            listener = null;
        }
    }

    private void StopServer()
    {
        if (!isRunning) return;

        isRunning = false;

        try { listener?.Stop(); } catch { }
        try { listener?.Close(); } catch { }
        listener = null;

        serverThread = null;

        Debug.Log("[HtoolWebServer] Stopped.");
    }

    private void ServerLoop()
    {
        while (isRunning && listener != null && listener.IsListening)
        {
            try
            {
                var context = listener.GetContext();
                HandleRequest(context);
            }
            catch (ObjectDisposedException)
            {
                break;
            }
            catch (HttpListenerException)
            {
                break;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[HtoolWebServer] Error: {ex.Message}");
            }
        }
    }

    private void HandleRequest(HttpListenerContext context)
    {
        var response = context.Response;

        // Ignore favicon requests from browsers
        if (context.Request.Url.AbsolutePath != "/")
        {
            byte[] empty = new byte[0];
            response.ContentLength64 = 0;
            response.OutputStream.Close();
            return;
        }

        // Capture current buffer and schedule UI update on main thread
        string dataToSend = bufferText;

        EditorApplication.delayCall += () =>
        {
            // Clear buffer
            bufferText = "";

            // Append sent data to history (with timestamp)
            if (!string.IsNullOrEmpty(dataToSend))
            {
                string timestamp = DateTime.Now.ToString("HH:mm:ss");
                historyText = $"[{timestamp}]\n{dataToSend}\n\n" + historyText;
            }

            GUI.FocusControl("inputField");
        };

        // Build response
        byte[] buffer = Encoding.UTF8.GetBytes(dataToSend ?? "");
        response.ContentType = "text/plain; charset=utf-8";
        response.ContentLength64 = buffer.Length;
        response.OutputStream.Write(buffer, 0, buffer.Length);
        response.OutputStream.Close();
    }

    #endregion

    #region Cleanup

    private void OnDestroy()
    {
        StopServer();
    }

    #endregion

    // [MenuItem("CONTEXT/Transform/Copy Hierarchy Path")]
    // private static void CopyHierarchyPath(MenuCommand cmd)
    // {
    //     var t = cmd.context as Transform;
    //     var path = "/" + t.name;
    //     var parent = t.parent;
    //     while (parent != null)
    //     {
    //         path = parent.name + "/" + path;
    //         parent = parent.parent;
    //     }
    //     path = "cd \"" + path + "\"";

    //     var win = GetWindow<HtoolSimpleWebSever>("Htool Web Server", false);
    //     if (win != null)
    //         win.AppendToBuffer(path);
    // }
}


