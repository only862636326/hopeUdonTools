
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace HopeTools
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]

    public class HopeUdonFramework : UdonSharpBehaviour
    {
        public const string HopeUdonFrameworkString = "HopeUdonFrameworkString";
        public GameObject empty_obj;
        public HopeUdonEvn udonEvn;
        public HopeUdonDebug udondebug;
        public HopeUdonPoolManager udonPool;
        public HopeUdonIoc udonIoc;
        public int get_test_val;
        private bool _is_init = false;

        //[UdonSynced]
        public int FrameworkVersion = 20251113;

        public Vector3 vrc_head_p;
        public Quaternion vrc_head_r;
        public VRCPlayerApi vrc_local_player;

        /// <summary>
        /// 框架初始化方法（网络同步）
        /// </summary>
        public void Init()
        {
            if (!_is_init)
            {
                InitSubsystems();
                vrc_local_player = Networking.LocalPlayer;
                _is_init = true;
            }
        }


        private void InitSubsystems()
        {
            Debug.Log("------------------hugf_init------------------");
            if (udondebug == null) udondebug = GetComponentInChildren<HopeUdonDebug>();
            if (udonEvn == null) udonEvn = GetComponentInChildren<HopeUdonEvn>();
            if (udonPool == null) udonPool = GetComponentInChildren<HopeUdonPoolManager>();
            if (udonIoc == null) udonIoc = GetComponentInChildren<HopeUdonIoc>();

            udondebug.Init();
            udonEvn.Init();
            udonPool.Init();
            udonIoc.Init();

            // 初始化事件系统
            if (udonEvn != null)
            {
                // udonEvn.Initialize();
            }

            // 启动调试系统
            if (udondebug != null)
            {
                // debug.EnableLogging(true);
            }
        }

        /// <summary>
        /// 跨模块事件触发接口
        /// </summary>
        public void TriggerEvent(string eventName)
        {
            if (udonEvn != null)
            {
                udonEvn.TriggerEvent(eventName);
            }
        }
        public void TriggerEventWithData(string eventName, object dat)
        {
            if (udonEvn != null)
            {
                udonEvn.TriggerEventWithData(eventName, dat);
            }
        }

        public void TriggerEventWith2Data(string eventName, object dat, object dat2)
        {
            if (udonEvn != null)
            {
                udonEvn.TriggerEventWith2Data(eventName, dat, dat2);
            }
        }

        public void Log(string message)
        {
            if(udondebug != null)
            {
                udondebug.Log(message);
            }
        }


        // 新增对象池管理接口
        public GameObject GetPooledObject()
        {
            //if (HopeUdonPool.Instance != null)
            //{
            //    return HopeUdonPool.Instance.GetPooledObject();
            //}
            return null;
        }

        public GameObject GetEmptyObj()
        {
            return empty_obj;
        }

        void Start()
        {
            Init();
        }

        void Update()
        {
            //vrc_head_p = vrc_local_player.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;
            //vrc_head_r = vrc_local_player.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).rotation;            
            //udondebug.DrawDayHit(vrc_head_p + Vector3.up * 0.1f, vrc_head_r, 20);
        }
    }
}
