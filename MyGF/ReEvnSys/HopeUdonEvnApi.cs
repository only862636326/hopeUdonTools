
using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.UIElements;
using VRC.SDK3.UdonNetworkCalling;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

namespace HopeTools
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class HopeUdonEvnApi : UdonSharpBehaviour
    {
        [HideInInspector] public HopeTools.HopeUdonFramework hugf;
        public object eventData;
        public object eventData1;
        public object eventData2;


        public void Update()
        {
            //if (Input.GetKeyDown(KeyCode.O))
            //{
            //    hugf.TriggerReEvent(nameof(SDH_OutCartFsm.StartChuPaiCall));
            //}
            //if (Input.GetKeyDown(KeyCode.P))
            //{
            //    //  eventData = 随机种子 
            //    // 使用当前时间作为随机种子
            //    int seed = DateTime.Now.Ticks.GetHashCode();
            //    this.eventData = seed;
            //    hugf.TriggerReEventWithData(nameof(SDH_FaPaiJi.StartShuffleCall), this.eventData);
            //}
            //if (Input.GetKeyUp(KeyCode.P))
            //{
            //    hugf.TriggerReEvent(nameof(SDH_JiaoZhuang.StartJiaoCall));
            //}
            //if (Input.GetKeyUp(KeyCode.P))
            //if (Input.GetKeyUp(KeyCode.L))
            //{
            //    this.TriggerReEvent(nameof(SDH_GameManager.SDH_GameResetCall));
            //}
        }

        public HopeTools.HopeUdonEvnRe udonEvnRe;

        public void HugfInitAfter()
        {
            // user code after hugf init here
            //hugf.udonEvn.RegisterListener(nameof(this.DemeFunCall), this);
            hugf.udonIoc.RegisterSingleton(nameof(this.name), this, this);
            if (udonEvnRe != null)
            {
                udonEvnRe.Init();
                udonEvnRe.hugf = this.hugf;
            }
        }

#region 事件网络接口
        [NetworkCallable]
        public void NetWordEvn(string eventName)
        {
            hugf.TriggerReEvent(eventName);
            if(udonEvnRe != null)
                udonEvnRe.AddEvn(eventName);
        }

        [NetworkCallable]
        public void NetWorkEvnDat(string eventName, int data)
        {
            hugf.TriggerEventWithData(eventName, data);
            if(udonEvnRe != null)
                udonEvnRe.AddEvnWithDat(eventName, data);
        }

        [NetworkCallable]
        public void NetWorkEvn2Dat(string eventName, int data1, int data2)
        {
            hugf.TriggerEventWith2Data(eventName, data1, data2);
            if (udonEvnRe != null)
                udonEvnRe.AddEvnWith2Dat(eventName, data1, data2);
        }

        private void EnsureOwnership()
        {
#if !UNITY_EDITOR
            if (!Networking.IsOwner(this.gameObject))
            {
                Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
            }
#endif
        }

        public void TriggerReEvent(string eventName)
        {
            EnsureOwnership();
            this.SendCustomNetworkEvent(NetworkEventTarget.All, nameof(NetWordEvn), eventName);
        }

        public void TriggerReEventWithData(string eventName, int data)
        {
            EnsureOwnership();
            this.SendCustomNetworkEvent(NetworkEventTarget.All, nameof(NetWorkEvnDat), eventName, data);
        }

        public void TriggerReEventWith2Data(string eventName, int data1, int data2)
        {
            EnsureOwnership();
            this.SendCustomNetworkEvent(NetworkEventTarget.All, nameof(NetWorkEvn2Dat), eventName, data1, data2);
        }

        #endregion 事件网络接口
        
        // start method

		// end method
    }
}




