using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;


namespace HopeTools
{
    public class AdvancedEventManager : UdonSharpBehaviour
    {
        #region 基础变量
        [Header("事件配置")]
        [Tooltip("所有事件名称")]
        public string[] eventNames;

        [Tooltip("每个事件的监听者数组")]
        public UdonBehaviour[][] eventListeners;

        [Tooltip("每个监听者的优先级")]
        public int[][] listenerPriorities;

        [Tooltip("是否一次性监听")]
        public bool[][] listenerIsOnce;

        [Tooltip("是否拦截事件")]
        public bool[][] listenerIntercept;

        [Header("网络同步")]
        [UdonSynced]
        private string syncedEventName;
        [UdonSynced]
        private int syncedEventData;
        [UdonSynced]
        private int syncedEventId;
        #endregion

        #region 初始化
        void Start()
        {
            if (eventNames == null)
            {
                eventNames = new string[0];
                eventListeners = new UdonBehaviour[0][];
                listenerPriorities = new int[0][];
                listenerIsOnce = new bool[0][];
                listenerIntercept = new bool[0][];
            }
        }
        #endregion

        #region 监听者管理
        public void RegisterListener(string eventName, UdonBehaviour listener)
        {
            RegisterListenerAdvanced(eventName, listener, 0, false, false);
        }

        public void RegisterListenerOnce(string eventName, UdonBehaviour listener)
        {
            RegisterListenerAdvanced(eventName, listener, 0, true, false);
        }

        public void RegisterListenerAdvanced(string eventName, UdonBehaviour listener, int priority, bool isOnce, bool intercept)
        {
            int eventIndex = GetOrCreateEventIndex(eventName);

            // 检查是否已存在
            for (int i = 0; i < eventListeners[eventIndex].Length; i++)
            {
                if (eventListeners[eventIndex][i] == listener)
                {
                    // 更新现有监听者属性
                    listenerPriorities[eventIndex][i] = priority;
                    listenerIsOnce[eventIndex][i] = isOnce;
                    listenerIntercept[eventIndex][i] = intercept;
                    return;
                }
            }

            // 扩展数组
            int newLength = eventListeners[eventIndex].Length + 1;

            UdonBehaviour[] newListeners = new UdonBehaviour[newLength];
            int[] newPriorities = new int[newLength];
            bool[] newIsOnce = new bool[newLength];
            bool[] newIntercept = new bool[newLength];

            // 复制旧数据
            for (int i = 0; i < eventListeners[eventIndex].Length; i++)
            {
                newListeners[i] = eventListeners[eventIndex][i];
                newPriorities[i] = listenerPriorities[eventIndex][i];
                newIsOnce[i] = listenerIsOnce[eventIndex][i];
                newIntercept[i] = listenerIntercept[eventIndex][i];
            }

            // 添加新监听者
            int insertIndex = FindInsertIndex(eventIndex, priority);

            // 移动元素腾出插入位置
            for (int i = newLength - 1; i > insertIndex; i--)
            {
                newListeners[i] = newListeners[i - 1];
                newPriorities[i] = newPriorities[i - 1];
                newIsOnce[i] = newIsOnce[i - 1];
                newIntercept[i] = newIntercept[i - 1];
            }

            // 插入新元素
            newListeners[insertIndex] = listener;
            newPriorities[insertIndex] = priority;
            newIsOnce[insertIndex] = isOnce;
            newIntercept[insertIndex] = intercept;

            // 更新数组
            eventListeners[eventIndex] = newListeners;
            listenerPriorities[eventIndex] = newPriorities;
            listenerIsOnce[eventIndex] = newIsOnce;
            listenerIntercept[eventIndex] = newIntercept;
        }

        private int GetOrCreateEventIndex(string eventName)
        {
            for (int i = 0; i < eventNames.Length; i++)
            {
                if (eventNames[i] == eventName) return i;
            }

            // 创建新事件
            int newLength = eventNames.Length + 1;

            string[] newEventNames = new string[newLength];
            UdonBehaviour[][] newEventListeners = new UdonBehaviour[newLength][];
            int[][] newListenerPriorities = new int[newLength][];
            bool[][] newListenerIsOnce = new bool[newLength][];
            bool[][] newListenerIntercept = new bool[newLength][];

            // 复制旧数据
            for (int i = 0; i < eventNames.Length; i++)
            {
                newEventNames[i] = eventNames[i];
                newEventListeners[i] = eventListeners[i];
                newListenerPriorities[i] = listenerPriorities[i];
                newListenerIsOnce[i] = listenerIsOnce[i];
                newListenerIntercept[i] = listenerIntercept[i];
            }

            // 添加新事件
            newEventNames[eventNames.Length] = eventName;
            newEventListeners[eventNames.Length] = new UdonBehaviour[0];
            newListenerPriorities[eventNames.Length] = new int[0];
            newListenerIsOnce[eventNames.Length] = new bool[0];
            newListenerIntercept[eventNames.Length] = new bool[0];

            // 更新数组
            eventNames = newEventNames;
            eventListeners = newEventListeners;
            listenerPriorities = newListenerPriorities;
            listenerIsOnce = newListenerIsOnce;
            listenerIntercept = newListenerIntercept;

            return newLength - 1;
        }

        private int FindInsertIndex(int eventIndex, int priority)
        {
            for (int i = 0; i < listenerPriorities[eventIndex].Length; i++)
            {
                if (priority > listenerPriorities[eventIndex][i])
                {
                    return i;
                }
            }
            return listenerPriorities[eventIndex].Length;
        }

        public void UnregisterListener(string eventName, UdonBehaviour listener)
        {
            for (int eventIndex = 0; eventIndex < eventNames.Length; eventIndex++)
            {
                if (eventNames[eventIndex] == eventName)
                {
                    for (int listenerIndex = 0; listenerIndex < eventListeners[eventIndex].Length; listenerIndex++)
                    {
                        if (eventListeners[eventIndex][listenerIndex] == listener)
                        {
                            RemoveListener(eventIndex, listenerIndex);
                            return;
                        }
                    }
                }
            }
        }

        private void RemoveListener(int eventIndex, int listenerIndex)
        {
            int newLength = eventListeners[eventIndex].Length - 1;

            UdonBehaviour[] newListeners = new UdonBehaviour[newLength];
            int[] newPriorities = new int[newLength];
            bool[] newIsOnce = new bool[newLength];
            bool[] newIntercept = new bool[newLength];

            for (int i = 0, j = 0; i < eventListeners[eventIndex].Length; i++)
            {
                if (i != listenerIndex)
                {
                    newListeners[j] = eventListeners[eventIndex][i];
                    newPriorities[j] = listenerPriorities[eventIndex][i];
                    newIsOnce[j] = listenerIsOnce[eventIndex][i];
                    newIntercept[j] = listenerIntercept[eventIndex][i];
                    j++;
                }
            }

            eventListeners[eventIndex] = newListeners;
            listenerPriorities[eventIndex] = newPriorities;
            listenerIsOnce[eventIndex] = newIsOnce;
            listenerIntercept[eventIndex] = newIntercept;

            if (newLength == 0)
            {
                RemoveEvent(eventIndex);
            }
        }

        private void RemoveEvent(int eventIndex)
        {
            int newLength = eventNames.Length - 1;

            string[] newEventNames = new string[newLength];
            UdonBehaviour[][] newEventListeners = new UdonBehaviour[newLength][];
            int[][] newListenerPriorities = new int[newLength][];
            bool[][] newListenerIsOnce = new bool[newLength][];
            bool[][] newListenerIntercept = new bool[newLength][];

            for (int i = 0, j = 0; i < eventNames.Length; i++)
            {
                if (i != eventIndex)
                {
                    newEventNames[j] = eventNames[i];
                    newEventListeners[j] = eventListeners[i];
                    newListenerPriorities[j] = listenerPriorities[i];
                    newListenerIsOnce[j] = listenerIsOnce[i];
                    newListenerIntercept[j] = listenerIntercept[i];
                    j++;
                }
            }

            eventNames = newEventNames;
            eventListeners = newEventListeners;
            listenerPriorities = newListenerPriorities;
            listenerIsOnce = newListenerIsOnce;
            listenerIntercept = newListenerIntercept;
        }
        #endregion

        #region 事件触发
        public void TriggerEvent(string eventName)
        {
            for (int eventIndex = 0; eventIndex < eventNames.Length; eventIndex++)
            {
                if (eventNames[eventIndex] == eventName)
                {
                    TriggerEventAtIndex(eventIndex, null);
                    return;
                }
            }
        }

        public void TriggerEventWithData(string eventName, object data)
        {
            for (int eventIndex = 0; eventIndex < eventNames.Length; eventIndex++)
            {
                if (eventNames[eventIndex] == eventName)
                {
                    TriggerEventAtIndex(eventIndex, data);
                    return;
                }
            }
        }

        private void TriggerEventAtIndex(int eventIndex, object data)
        {
            bool shouldIntercept = false;
            int[] toRemove = new int[eventListeners[eventIndex].Length];
            int removeCount = 0;

            for (int i = 0; i < eventListeners[eventIndex].Length; i++)
            {
                UdonBehaviour listener = eventListeners[eventIndex][i];
                if (listener != null)
                {
                    if (data != null)
                    {
                        listener.SetProgramVariable("eventData", data);
                    }

                    listener.SendCustomEvent(eventNames[eventIndex]);

                    if (listenerIntercept[eventIndex][i])
                    {
                        shouldIntercept = true;
                    }

                    if (listenerIsOnce[eventIndex][i])
                    {
                        toRemove[removeCount++] = i;
                    }
                }
            }

            // 从后往前移除
            for (int i = removeCount - 1; i >= 0; i--)
            {
                RemoveListener(eventIndex, toRemove[i]);
            }

            if (shouldIntercept)
            {
                Debug.Log($"Event {eventNames[eventIndex]} was intercepted");
            }
        }
        #endregion

        #region 网络事件
        public void NetworkTriggerEvent(string eventName)
        {
            if (Networking.IsOwner(gameObject))
            {
                syncedEventName = eventName;
                syncedEventData = 0;
                syncedEventId = Random.Range(0, int.MaxValue);
                RequestSerialization();
            }
        }

        public void NetworkTriggerEventWithData(string eventName, int data)
        {
            if (Networking.IsOwner(gameObject))
            {
                syncedEventName = eventName;
                syncedEventData = data;
                syncedEventId = Random.Range(0, int.MaxValue);
                RequestSerialization();
            }
        }

        public override void OnDeserialization()
        {
            if (!string.IsNullOrEmpty(syncedEventName))
            {
                if (syncedEventData != 0)
                {
                    TriggerEventWithData(syncedEventName, syncedEventData);
                }
                else
                {
                    TriggerEvent(syncedEventName);
                }
            }
        }
        #endregion

        #region 维护和调试
        public void CleanNullReferences()
        {
            for (int eventIndex = 0; eventIndex < eventNames.Length; eventIndex++)
            {
                int validCount = 0;
                for (int i = 0; i < eventListeners[eventIndex].Length; i++)
                {
                    if (eventListeners[eventIndex][i] != null) validCount++;
                }

                if (validCount < eventListeners[eventIndex].Length)
                {
                    UdonBehaviour[] newListeners = new UdonBehaviour[validCount];
                    int[] newPriorities = new int[validCount];
                    bool[] newIsOnce = new bool[validCount];
                    bool[] newIntercept = new bool[validCount];

                    for (int i = 0, j = 0; i < eventListeners[eventIndex].Length; i++)
                    {
                        if (eventListeners[eventIndex][i] != null)
                        {
                            newListeners[j] = eventListeners[eventIndex][i];
                            newPriorities[j] = listenerPriorities[eventIndex][i];
                            newIsOnce[j] = listenerIsOnce[eventIndex][i];
                            newIntercept[j] = listenerIntercept[eventIndex][i];
                            j++;
                        }
                    }

                    eventListeners[eventIndex] = newListeners;
                    listenerPriorities[eventIndex] = newPriorities;
                    listenerIsOnce[eventIndex] = newIsOnce;
                    listenerIntercept[eventIndex] = newIntercept;
                }

                if (validCount == 0)
                {
                    RemoveEvent(eventIndex);
                    eventIndex--;
                }
            }
        }

        public void PrintEventLog()
        {
            string log = "=== Event Manager Log ===\n";
            log += $"Total Events: {eventNames.Length}\n";

            for (int eventIndex = 0; eventIndex < eventNames.Length; eventIndex++)
            {
                log += $"\nEvent: {eventNames[eventIndex]}\n";
                log += $"Listeners: {eventListeners[eventIndex].Length}\n";

                for (int i = 0; i < eventListeners[eventIndex].Length; i++)
                {
                    string behaviourName = eventListeners[eventIndex][i] != null ?
                        eventListeners[eventIndex][i].name : "null";

                    log += $"- {behaviourName} (Prio: {listenerPriorities[eventIndex][i]}, " +
                          $"Once: {listenerIsOnce[eventIndex][i]}, Intercept: {listenerIntercept[eventIndex][i]})\n";
                }
            }

            Debug.Log(log);
        }
        #endregion

        #region 类型安全事件
        public const string PlayerJoined = "PlayerJoined";
        public const string PlayerLeft = "PlayerLeft";
        public const string ScoreUpdated = "ScoreUpdated";
        public const string GameStarted = "GameStarted";
        public const string GameEnded = "GameEnded";
        public const string ItemCollected = "ItemCollected";

        public void TriggerEventByType(string eventType)
        {
            TriggerEvent(eventType);
        }

        public void TriggerEventWithDataByType(string eventType, object data)
        {
            TriggerEventWithData(eventType, data);
        }

        public void NetworkTriggerEventByType(string eventType)
        {
            NetworkTriggerEvent(eventType);
        }

        public void NetworkTriggerEventWithDataByType(string eventType, int data)
        {
            NetworkTriggerEventWithData(eventType, data);
        }
        #endregion
    }
}