
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;


namespace HopeTools
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]

    public class HopeUdonInit : UdonSharpBehaviour
    {

        public const float HUGF_INIT_DELAY_GET = 0.1f;
        public const float HUGF_INIT_DELAY_AFTER = 0.2f;
        public const float HUGF_INIT_DELAY_IOC = 0.4f;

        public Transform[] managered_tf;

        public HopeTools.HopeUdonFramework hugf;
        void Start()
        {
            hugf = this.GetComponentInParent<HopeTools.HopeUdonFramework>();
            hugf.Init();

            foreach (var tf in managered_tf)
            {
                if (tf != null)
                {
                    var _p = tf.GetComponent<UdonSharpBehaviour>();
                    if (_p != null)
                    {
                        _p.SetProgramVariable("hugf", hugf);

                        _p.SendCustomEvent("Init");
                        _p.SendCustomEventDelayedSeconds("HugfInit", HUGF_INIT_DELAY_GET);
                        _p.SendCustomEventDelayedSeconds("HugfInitAfter", HUGF_INIT_DELAY_AFTER);
                        _p.SendCustomEventDelayedSeconds("HufgIocGet", HUGF_INIT_DELAY_IOC);
                    }
                    
                    var udon = tf.GetComponentsInChildren<UdonSharpBehaviour>();
                    foreach (var item in udon)
                    {
                        item.SetProgramVariable("hugf", hugf);

                        item.SendCustomEvent("Init");
                        item.SendCustomEventDelayedSeconds("HugfInit", HUGF_INIT_DELAY_GET);
                        item.SendCustomEventDelayedSeconds("HugfInitAfter", HUGF_INIT_DELAY_AFTER);
                        item.SendCustomEventDelayedSeconds("HufgIocGet", HUGF_INIT_DELAY_IOC);
                    }
                }
            }
        }
    }
}