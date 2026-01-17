using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;


namespace HopeTools
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class HopeUdonDebug : UdonSharpBehaviour
    {
        [Header("Debug Settings")]
        [Tooltip("是否启用调试日志")]
        public bool enableDebugLogs = true;

        [Tooltip("是否在屏幕上显示调试信息")]
        public bool enableScreenLogs = true;

        [Tooltip("屏幕日志最大行数")]
        public int maxScreenLogLines = 10;

        [Tooltip("日志保留时间(秒)")]
        public float logLifetime = 30f;

        [Header("UI References")]
        [Tooltip("用于显示日志的Text组件")]
        public UnityEngine.UI.Text debugText;

        // 日志存储
        private string[] logMessages;
        private float[] logTimes;
        private int logCount = 0;
        private int currentIndex = 0;
        private bool _is_init = false;
        // 初始化

        public LineRenderer lineRenderer;
        void Start()
        {
            Init();
        }
        public void Init()
        {
            if (this._is_init)
                return;
            this._is_init = true;
            {
                // 初始化日志数组
                logMessages = new string[maxScreenLogLines];
                logTimes = new float[maxScreenLogLines];

                if (debugText == null && enableScreenLogs)
                {
                    Debug.LogWarning("[UdonDebugger] 没有分配Text组件，屏幕日志将不可用");
                    enableScreenLogs = false;
                }

                LogSystemInfo();
            }
        }

        private void Update()
        {
            if (enableScreenLogs && debugText != null)
            {
                UpdateScreenLogs();
            }
        }

        /// <summary>
        /// 记录一条普通日志
        /// </summary>
        public void Log(object message)
        {
            if (!enableDebugLogs) return;

            string formattedMessage = $"hugf_log :[{Time.time:F2}] {message}";
            Debug.Log(formattedMessage);
            AddScreenLog(formattedMessage);
        }

        public void LogUdonMsg(UdonSharpBehaviour udon, object message)
        {
            if (!enableDebugLogs) return;

            string formattedMessage = $"hugf_log :[{Time.time:F2}] {udon.name} : {message}";
            Debug.Log(formattedMessage);
            AddScreenLog(formattedMessage);
        }

        /// <summary>
        /// 记录一条警告日志
        /// </summary>
        public void LogWarning(object message)
        {
            if (!enableDebugLogs) return;

            string formattedMessage = $"[{Time.time:F2}] WARNING: {message}";
            Debug.LogWarning(formattedMessage);
            AddScreenLog($"<color=yellow>{formattedMessage}</color>");
        }

        /// <summary>
        /// 记录一条错误日志
        /// </summary>
        public void LogError(object message)
        {
            if (!enableDebugLogs) return;

            string formattedMessage = $"[{Time.time:F2}] ERROR: {message}";
            Debug.LogError(formattedMessage);
            AddScreenLog($"<color=red>{formattedMessage}</color>");
        }

        public void DrawDayHit(Vector3 rayOrigin, Quaternion rayRotation, float l)
        {
            if (!enableDebugLogs) return;
            Vector3 rayDirection = rayRotation * Vector3.forward * l;
            lineRenderer.SetPosition(0, rayOrigin);
            lineRenderer.SetPosition(1, rayOrigin + rayDirection);
        }
        /// <summary>
        /// 添加日志到屏幕显示
        /// </summary>
        private void AddScreenLog(string message)
        {
            if (!enableScreenLogs || debugText == null) return;

            logMessages[currentIndex] = message;
            logTimes[currentIndex] = Time.time;
            currentIndex = (currentIndex + 1) % maxScreenLogLines;
            logCount = Mathf.Min(logCount + 1, maxScreenLogLines);
        }

        /// <summary>
        /// 更新屏幕日志显示
        /// </summary>
        private void UpdateScreenLogs()
        {
            var sb = "";
            int displayCount = 0;

            for (int i = 0; i < logCount; i++)
            {
                int index = (currentIndex - logCount + i + maxScreenLogLines) % maxScreenLogLines;

                // 检查日志是否过期
                if (Time.time - logTimes[index] > logLifetime) continue;

                sb = sb + "\r\n" + logMessages[index];
                displayCount++;
            }

            // 如果没有日志显示，显示占位符
            if (displayCount == 0)
            {
                sb = sb + "\r\n" + "No debug messages...";
            }

            debugText.text = sb.ToString();
        }

        /// <summary>
        /// 记录系统信息
        /// </summary>
        private void LogSystemInfo()
        {
            if (!enableDebugLogs) return;

            Debug.Log("[UdonDebugger] System Info:");
            //Debug.Log($"- Unity Version: {Application.unityVersion}");
            //Debug.Log($"- Platform: {Application.platform}");
            //Debug.Log($"- VRChat SDK Version: {VRC.SDK_Version}");

            if (Networking.LocalPlayer != null)
            {
                Debug.Log($"- Player: {Networking.LocalPlayer.displayName}");
            }
        }

        /// <summary>
        /// 显示对象信息
        /// </summary>
        public void InspectObject(GameObject obj)
        {
            if (obj == null)
            {
                LogError("InspectObject: 对象为null");
                return;
            }

            Log($"Inspecting: {obj.name}");
            Log($"- Active: {obj.activeInHierarchy}");
            Log($"- Layer: {LayerMask.LayerToName(obj.layer)}");
            Log($"- Position: {obj.transform.position}");
            Log($"- Rotation: {obj.transform.rotation.eulerAngles}");
            Log($"- Scale: {obj.transform.localScale}");

            // 获取所有组件
            Component[] components = obj.GetComponents<Component>();
            Log($"- Components ({components.Length}):");
            foreach (Component comp in components)
            {
                Log($"  - {comp.GetType().Name}");
            }
        }

        /// <summary>
        /// 清空所有日志
        /// </summary>
        public void ClearLogs()
        {
            logCount = 0;
            currentIndex = 0;

            if (debugText != null)
            {
                debugText.text = "Debug logs cleared";
            }

            Log("Debug logs cleared");
        }

        /// <summary>
        /// 切换调试日志开关
        /// </summary>
        public void ToggleDebugLogs()
        {
            enableDebugLogs = !enableDebugLogs;
            Log($"Debug logs {(enableDebugLogs ? "enabled" : "disabled")}");
        }

        /// <summary>
        /// 切换屏幕日志开关
        /// </summary>
        public void ToggleScreenLogs()
        {
            enableScreenLogs = !enableScreenLogs;
            Log($"Screen logs {(enableScreenLogs ? "enabled" : "disabled")}");

            if (!enableScreenLogs && debugText != null)
            {
                debugText.text = "Screen logs disabled";
            }
        }
    }
}