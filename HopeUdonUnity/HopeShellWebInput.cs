
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.Components;
using VRC.SDK3.StringLoading;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

/// <summary>
/// HopeShellWebInput — VRCStringDownloader 下载字符串 Demo
/// 参考 url_down.cs 实现:
///   - Start 时自动下载一次指定 URL 的文本内容
///   - Interact 手动重新下载
///   - 下载结果写入 displayText
/// </summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class HopeShellWebInput : UdonSharpBehaviour
{

    [Header("Download Settings")]
    [SerializeField] private VRCUrl targetUrl = new VRCUrl("http://localhost:5004/");
    [Tooltip("Start 时自动下载")]
    [SerializeField] private bool auto_down = true;

    private bool isLoading;
    
    private string lastResult;



    void Start()
    {
        ;
    }

    void Update()
    {
        if (auto_down)           
        {
            LoadString();
        }
    }


    /// <summary>手动触发下载 (可通过 Interact 或外部脚本调用)</summary>
    public void LoadString()
    {
        if (isLoading)
        {
            return;
        }

        if (targetUrl == null || string.IsNullOrEmpty(targetUrl.Get()))
        {
            LogMsg("URL is null or empty.");
            return;
        }

        isLoading = true;
        LogIsDown("Down");
        // LogMsg($" {targetUrl.Get()}");
        VRCStringDownloader.LoadUrl(targetUrl, (IUdonEventReceiver)this);
    }


    public override void Interact()
    {
        LoadString();
    }


    /// <summary>下载成功回调</summary>
    public override void OnStringLoadSuccess(IVRCStringDownload result)
    {
        isLoading = false;
        LogIsDown("");
        lastResult = result.Result;
        LogMsg($"Load success, length={lastResult.Length}");
        OnLoadString(lastResult);
    }

    /// <summary>下载失败回调</summary>
    public override void OnStringLoadError(IVRCStringDownload result)
    {
        isLoading = false;
        LogIsDown("");
        LogMsg($"Load failed — ErrorCode={result.ErrorCode}, Error={result.Error}");
    }



    public UdonSharpBehaviour _tar_duon;

    public void OnLoadString(string text)
    {
        if (_tar_duon != null)
        {
            _tar_duon.SetProgramVariable("eventData", text);
            _tar_duon.SendCustomEvent("ex");
        }
    }

    [Header("url down UI, 可为空")]
    public Text tips_text;
    public Text tipsdown_text;

    public void LogMsg(string msg)
    {
        //Debug.Log($"[HopeShellWebInput] {msg}");
        if (tips_text != null)
        {
            tips_text.text = msg;
        }
    }
    public void LogIsDown(string msg)
    {
        if (tipsdown_text != null)
        {
            tipsdown_text.text = msg;
        }
    }

    public VRCUrlInputField urlInputField;
    public void UrlInputEditEnd()
    {
        targetUrl = urlInputField.GetUrl();
    }

    public  Toggle atuto_down_toggle;
    public void ToggleEvn_AutoDown()
    {
        auto_down = atuto_down_toggle.isOn;
    }

    public void ToggleEvn_MaulDown()
    {
        LoadString();
    }
}



