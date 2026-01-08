using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;


namespace HopeTools
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class HopeUdonPool : UdonSharpBehaviour
    {

        [Header("对象池设置")]
        [Tooltip("要池化的预制体")]
        public string pool_name;
        public GameObject poolPrefab;

        [Tooltip("初始池大小")]
        public int initialPoolSize = 10;

        [Tooltip("是否在需要时自动扩展池")]
        public bool autoExpand = true;

        [Tooltip("最大池大小(0表示无限制)")]
        public int maxPoolSize = 100;

        [Header("调试")]
        public bool showDebugLogs = true;

        // 对象池存储
        private GameObject[] pool;
        public int[] nextIndices; // 用于实现链表结构
        public int firstAvailableIndex = 0;
        private int activeCount = 0;

        private Transform poolContainer;
        //private GameObject poolContainerGM;

        public GameObject null_transform;

        private bool isInitialized = false;

        private void Update()
        {
            ;
        }

        public void InitializePool(GameObject pre)
        {
            if (pre != null) poolPrefab = pre;
            if (isInitialized || poolPrefab == null) return;

            // 创建容器对象用于组织池化对象
            poolContainer = this.transform;
            poolContainer.localPosition = Vector3.zero;

            // 初始化数组
            pool = new GameObject[initialPoolSize];
            nextIndices = new int[initialPoolSize];

            // 预实例化对象
            for (int i = 0; i < initialPoolSize; i++)
            {
                pool[i] = Instantiate(poolPrefab, poolContainer);
                pool[i].SetActive(false);
                nextIndices[i] = i + 1; // 设置链表关系
            }

            // 最后一个元素的nextIndex设为-1表示结束
            nextIndices[initialPoolSize - 1] = -1;
            firstAvailableIndex = 0;
            activeCount = 0;

            isInitialized = true;

            Log($"对象池初始化完成，初始大小: {initialPoolSize}");
        }

        /// <summary>
        /// 从池中获取一个可用对象
        /// </summary>
        public GameObject GetObject()
        {
            if (!isInitialized)
            {
                LogError("对象池未初始化!");
                return null;
            }

            // 如果没有可用对象且允许扩展
            if (firstAvailableIndex == -1)
            {
                if (autoExpand && (maxPoolSize == 0 || pool.Length < maxPoolSize))
                {
                    ExpandPool();
                    // return null;
                }
                else
                {
                    LogWarning("对象池已耗尽，无法获取对象!");
                    return null;
                }
            }

            // 获取第一个可用对象


            int index = firstAvailableIndex;
            GameObject obj = pool[index];

            // 更新链表
            firstAvailableIndex = nextIndices[index];
            nextIndices[index] = -1; // 标记为已使用

            // 激活对象
            obj.SetActive(true);
            activeCount++;

            Log($"从池中获取对象，索引: {index}, 活跃对象数: {activeCount}");

            return obj;
        }

        /// <summary>
        /// only for pool pool
        /// </summary>
        /// <param name="_name"></param>
        /// <returns></returns>
        public HopeUdonPool GetUdonPool(string _name)
        {
            for(int i = 0; i < pool.Length; i++)
            {
                if (pool[i].name == _name && pool[i].activeSelf == true)
                {
                    return pool[i].GetComponent<HopeUdonPool>();
                }
            }
            return null;
        }

        /// <summary>
        /// 将对象返回到池中
        /// </summary>
        public void ReturnObject(GameObject obj)
        {
            if (!isInitialized || obj == null) return;

            // 查找对象在池中的索引
            int index = -1;
            for (int i = 0; i < pool.Length; i++)
            {
                if (pool[i] == obj)
                {
                    index = i;
                    break;
                }
            }

            if (index == -1)
            {
                LogWarning("尝试返回不属于此池的对象: " + obj.name);
                return;
            }

            // 重置对象状态
            obj.SetActive(false);
            obj.transform.SetParent(poolContainer);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;

            // 将对象添加回可用链表
            nextIndices[index] = firstAvailableIndex;
            firstAvailableIndex = index;

            activeCount--;

            Log($"对象返回到池中，索引: {index}, 活跃对象数: {activeCount}");
        }

        /// <summary>
        /// 扩展对象池
        /// </summary>
        private void ExpandPool()
        {
            if (maxPoolSize > 0 && pool.Length >= maxPoolSize)
            {
                LogWarning("已达到最大池大小，无法扩展!");
                return;
            }

            int newSize = Mathf.Max(pool.Length * 2, pool.Length + 10);
            if (maxPoolSize > 0)
            {
                newSize = Mathf.Min(newSize, maxPoolSize);
            }

            Log($"正在扩展对象池从 {pool.Length} 到 {newSize}...");

            // 扩展数组
            // System.Array.Resize(ref pool, newSize);
            // System.Array.Resize(ref nextIndices, newSize);

            var new_pool = new GameObject[newSize];
            var new_next_index = new int[newSize];

            for (int i = 0; i < pool.Length; i++)
            {
                new_pool[i] = pool[i];
                new_next_index[i] = i;
            }

            // 初始化新对象
            for (int i = pool.Length; i < newSize; i++)
            {
                new_pool[i] = Instantiate(poolPrefab, poolContainer);
                new_pool[i].SetActive(false);
                new_next_index[i] = i + 1;
            }

            nextIndices = new_next_index;

            // 连接新旧链表
            if (firstAvailableIndex == -1)
            {
                firstAvailableIndex = pool.Length;
            }
            else
            {
                // 找到链表末尾
                int lastIndex = firstAvailableIndex;
                while (nextIndices[lastIndex] != -1)
                {
                    lastIndex = nextIndices[lastIndex];
                }
                nextIndices[lastIndex] = pool.Length;
            }
            pool = new_pool;


            // 设置新链表的结束标记
            nextIndices[newSize - 1] = -1;

            Log($"对象池已扩展到 {newSize} 个对象");
        }

        /// <summary>
        /// 获取当前活跃对象数量
        /// </summary>
        public int GetActiveCount()
        {
            return activeCount;
        }

        /// <summary>
        /// 获取池的总大小
        /// </summary>
        public int GetPoolSize()
        {
            return pool != null ? pool.Length : 0;
        }

        /// <summary>
        /// 回收所有活跃对象
        /// </summary>
        public void ReturnAllObjects()
        {
            if (!isInitialized) return;

            for (int i = 0; i < pool.Length; i++)
            {
                if (pool[i].activeSelf)
                {
                    ReturnObject(pool[i]);
                }
            }
        }

        private void Log(string message)
        {
            if (showDebugLogs) Debug.Log($"[ObjectPool] {message}", this);
        }

        private void LogWarning(string message)
        {
            Debug.LogWarning($"[ObjectPool] {message}", this);
        }

        private void LogError(string message)
        {
            Debug.LogError($"[ObjectPool] {message}", this);
        }
    }
}

