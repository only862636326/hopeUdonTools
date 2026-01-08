using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using static UnityEngine.UI.CanvasScaler;
using static VRC.Core.ApiInfoPushSystem;

namespace HopeTools
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]

    public class HopeUdonEvn : UdonSharpBehaviour
    {
        // 事件监听者字典（UdonSharp不支持真正的字典，我们用两个数组模拟）
        //[HideInInspector]
        public string[] eventNames;
        //[HideInInspector]
        public UdonSharpBehaviour[][] eventListeners;

        public HopeUdonFramework hugf;
        // 网络同步相关变量
        [UdonSynced]
        private string syncedEventName;
        [UdonSynced]
        private int syncedEventData;
        [UdonSynced]
        private int syncedEventId;
        private int locEventId;


        private void Start()
        {
            Init();
        }
        private bool _is_init = false;
        // 初始化
        public void Init()
        {
            if (this._is_init)
                return;
            this._is_init = true;
            this.hugf = this.GetComponentInParent<HopeUdonFramework>();
            if (eventNames == null)
            {
                eventNames = new string[0];
                eventListeners = new UdonSharpBehaviour[0][];
            }
        }

        // 注册监听者到特定事件
        public void RegisterListener(string eventName, UdonSharpBehaviour listener)
        {
            this.hugf.udondebug.Log($"{this.name}:RegisterListener-{listener.name}-{eventName}");
            // 检查是否已存在该事件
            int eventIndex = -1;
            for (int i = 0; i < eventNames.Length; i++)
            {
                if (eventNames[i] == eventName)
                {
                    eventIndex = i;
                    break;
                }
            }

            // 如果是新事件
            if (eventIndex == -1)
            {
                // 扩展事件数组
                string[] newEventNames = new string[eventNames.Length + 1];
                UdonSharpBehaviour[][] newEventListeners = new UdonSharpBehaviour[eventNames.Length + 1][];

                // 复制旧数据
                for (int i = 0; i < eventNames.Length; i++)
                {
                    newEventNames[i] = eventNames[i];
                    newEventListeners[i] = eventListeners[i];
                }

                // 添加新事件
                newEventNames[eventNames.Length] = eventName;
                newEventListeners[eventNames.Length] = new UdonSharpBehaviour[] { listener };

                eventNames = newEventNames;
                eventListeners = newEventListeners;
            }
            else
            {
                // 检查是否已注册
                foreach (var l in eventListeners[eventIndex])
                {
                    if (l == listener) return;
                }

                // 添加到现有事件
                UdonSharpBehaviour[] newListeners = new UdonSharpBehaviour[eventListeners[eventIndex].Length + 1];
                eventListeners[eventIndex].CopyTo(newListeners, 0);
                newListeners[eventListeners[eventIndex].Length] = listener;
                eventListeners[eventIndex] = newListeners;
            }
        }

        // 注销监听者
        public void UnregisterListener(string eventName, UdonSharpBehaviour listener)
        {
            for (int i = 0; i < eventNames.Length; i++)
            {
                if (eventNames[i] == eventName)
                {
                    // 查找监听者
                    int listenerIndex = -1;
                    for (int j = 0; j < eventListeners[i].Length; j++)
                    {
                        if (eventListeners[i][j] == listener)
                        {
                            listenerIndex = j;
                            break;
                        }
                    }

                    if (listenerIndex == -1) return;

                    // 移除监听者
                    UdonSharpBehaviour[] newListeners = new UdonSharpBehaviour[eventListeners[i].Length - 1];
                    for (int j = 0, k = 0; j < eventListeners[i].Length; j++)
                    {
                        if (j != listenerIndex)
                        {
                            newListeners[k++] = eventListeners[i][j];
                        }
                    }

                    eventListeners[i] = newListeners;

                    // 如果该事件没有监听者了，移除整个事件
                    if (newListeners.Length == 0)
                    {
                        RemoveEvent(i);
                    }

                    return;
                }
            }
        }

        // 移除事件
        private void RemoveEvent(int index)
        {
            string[] newEventNames = new string[eventNames.Length - 1];
            UdonSharpBehaviour[][] newEventListeners = new UdonSharpBehaviour[eventNames.Length - 1][];

            for (int i = 0, j = 0; i < eventNames.Length; i++)
            {
                if (i != index)
                {
                    newEventNames[j] = eventNames[i];
                    newEventListeners[j] = eventListeners[i];
                    j++;
                }
            }

            eventNames = newEventNames;
            eventListeners = newEventListeners;
        }

        // 触发本地事件
        public void TriggerEvent(string eventName)
        {
            hugf.udondebug.LogUdonMsg(this, $"trg event : {eventName}");

            for (int i = 0; i < eventNames.Length; i++)
            {
                if (eventNames[i] == eventName)
                {
                    foreach (var listener in eventListeners[i])
                    {
                        if (listener != null)
                        {
                            listener.SendCustomEvent(eventName);
                        }
                    }
                    return;
                }
            }
        }

        // 触发带参数的本地事件
        public void TriggerEventWithData(string eventName, object data)
        {
            hugf.udondebug.LogUdonMsg(this, $"trg event dat: {eventName}, {data}");
            for (int i = 0; i < eventNames.Length; i++)
            {
                if (eventNames[i] == eventName)
                {
                    foreach (var listener in eventListeners[i])
                    {
                        if (listener != null)
                        {
                            listener.SetProgramVariable("eventData", data);
                            listener.SendCustomEvent(eventName);
                        }
                    }
                    return;
                }
            }
        }

        // 触发带参数的本地事件
        public void TriggerEventWith2Data(string eventName, object data, object data2)
        {
            hugf.udondebug.LogUdonMsg(this, $"trg event dat: {eventName}, {data}");
            for (int i = 0; i < eventNames.Length; i++)
            {
                if (eventNames[i] == eventName)
                {
                    foreach (var listener in eventListeners[i])
                    {
                        if (listener != null)
                        {
                            listener.SetProgramVariable("eventData", data);
                            listener.SetProgramVariable("eventData1", data);
                            listener.SetProgramVariable("eventData2", data2);
                            listener.SendCustomEvent(eventName);
                        }
                    }
                    return;
                }
            }
        }

        public void TriggerEventWithTf(string eventName, object data)
        {
            hugf.udondebug.LogUdonMsg(this, $"trg event dat: {eventName}, {data}");
            for (int i = 0; i < eventNames.Length; i++)
            {
                if (eventNames[i] == eventName)
                {
                    foreach (var listener in eventListeners[i])
                    {
                        if (listener != null)
                        {
                            listener.SetProgramVariable("eventTfData", data);
                            listener.SendCustomEvent(eventName);
                        }
                    }
                    return;
                }
            }
        }

        public void TriggerEventWithTfInt(string eventName, Transform tf, int data)
        {
            hugf.udondebug.LogUdonMsg(this, $"trg event dat: {eventName}, {data}");
            for (int i = 0; i < eventNames.Length; i++)
            {
                if (eventNames[i] == eventName)
                {
                    foreach (var listener in eventListeners[i])
                    {
                        if (listener != null)
                        {
                            listener.SetProgramVariable("eventData", data);
                            listener.SetProgramVariable("eventTfData", tf);
                            listener.SendCustomEvent(eventName);
                        }
                    }
                    return;
                }
            }
        }

        // 触发网络同步事件
        public void NetworkTriggerEvent(string eventName)
        {
            if (Networking.IsOwner(gameObject))
            {
                syncedEventName = eventName;
                syncedEventData = int.MaxValue;
                syncedEventId = Random.Range(0, int.MaxValue); // 确保事件被触发
                RequestSyn();
            }
        }

        // 触发带参数的网络同步事件
        public void NetworkTriggerEventWithData(string eventName, int data)
        {
            if (Networking.IsOwner(gameObject))
            {
                syncedEventName = eventName;
                syncedEventData = data;
                syncedEventId = Random.Range(0, int.MaxValue); // 确保事件被触发
                RequestSyn();
            }
        }

        void RequestSyn()
        {
#if !UNITY_EDITOR
            if(!Networking.IsOwner(this.gameObject))
            {
                Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
            }
            RequestSerialization();
#else
            OnPreSerialization();
#endif
            ;
        }
        public override void OnPreSerialization()
        {
#if UNITY_EDITOR
            OnDeserialization();
#endif
        }
        // 网络同步回调
        public override void OnDeserialization()
        {
            if (!string.IsNullOrEmpty(syncedEventName))
            {
                if (syncedEventData != int.MaxValue)
                {
                    TriggerEventWithData(syncedEventName, syncedEventData);
                }
                else
                {
                    TriggerEvent(syncedEventName);
                }
            }
        }

        // 清理所有null引用
        public void CleanNullReferences()
        {
            for (int i = 0; i < eventNames.Length; i++)
            {
                int validCount = 0;
                foreach (var listener in eventListeners[i])
                {
                    if (listener != null) validCount++;
                }

                if (validCount < eventListeners[i].Length)
                {
                    UdonSharpBehaviour[] newListeners = new UdonSharpBehaviour[validCount];
                    for (int j = 0, k = 0; j < eventListeners[i].Length; j++)
                    {
                        if (eventListeners[i][j] != null)
                        {
                            newListeners[k++] = eventListeners[i][j];
                        }
                    }
                    eventListeners[i] = newListeners;
                }

                if (validCount == 0)
                {
                    RemoveEvent(i);
                    i--; // 因为数组缩短了
                }
            }
        }
    }
}