
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace HopeTools
{
    public class HopeUdonPoolManager : UdonSharpBehaviour
    {
        public HopeUdonFramework hugf;
        public GameObject pool_pool; // 对象池的对象池
        private HopeUdonPool hopeUdonPool;
        private bool isInitialized = false;

        [Header("默认池设置")]
        public int defaultInitialSize = 10;
        public bool defaultAutoExpand = true;
        public int defaultMaxSize = 100;

        public int test_pool_indext = 0;

        void Start()
        {
            Init();

            // use demo
            //CreatePool("pool_empty_obj", hugf.empty_obj);
            //var o = GetObject("pool_empty_obj");
            //ReturnObject(o);
        }
        public void Init()
        {
            if (!this.isInitialized)
            {
                hugf = GetComponentInParent<HopeUdonFramework>();
                hopeUdonPool = pool_pool.GetComponent<HopeUdonPool>();
                hopeUdonPool.InitializePool(null);
                this.isInitialized = true;
            }
        }

        void Update()
        {
            ;
        }

        /// <summary>
        /// 创建新的对象池
        /// </summary>
        /// <param name="poolName">池名称</param>
        /// <param name="prefab">预制体</param>
        /// <param name="initialSize">初始大小</param>
        /// <param name="autoExpand">是否自动扩展</param>
        /// <param name="maxSize">最大大小</param>
        public HopeUdonPool CreatePool(string poolName, GameObject prefab)
        {
            if (hopeUdonPool.GetUdonPool(poolName) == null)
            {
                GameObject poolObj = hopeUdonPool.GetObject();
                HopeUdonPool pool = poolObj.GetComponent<HopeUdonPool>();
                pool.pool_name = poolName;
                pool.name = poolName;
                pool.poolPrefab = prefab;
                pool.autoExpand = defaultAutoExpand;
                pool.maxPoolSize = defaultMaxSize;
                pool.initialPoolSize = defaultInitialSize;

                pool.InitializePool(prefab);
                hugf.udondebug.Log($"创建对象池<<{poolName}>>成功");
                return pool;
            }
            else
            {
                hugf.udondebug.Log($"对象池<<{poolName}>>已存在");
                return null;
            }
        }

        public GameObject GetObject(string poolName)
        {
            hopeUdonPool = hopeUdonPool.GetUdonPool(poolName);
            if (hopeUdonPool != null)
            {
                return hopeUdonPool.GetObject();
            }
            return null;
        }

        public void ReturnObject(GameObject obj)
        {
            var ud = obj.transform.parent.GetComponent<HopeUdonPool>();
            if (ud != null)
            {
                ud.ReturnObject(obj);
            }
        }
    }
}
