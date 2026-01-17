
using SGS;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace HopeTools
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class HopeUdonInteractEvn : UdonSharpBehaviour
    {
        /// <summary>
        /// 
        /// </summary>
        [HideInInspector] HopeUdonFramework hufw;
        private string s_evn = "";
        private int evn_data;
        private bool with_dat = false;
        void Start()
        {
            ;
        }

        bool _is_init = false;
        public void Init()
        {
            if (this._is_init)
                return;
            this._is_init = true;

            var ss = this.name.Split('_');
            if (ss[0] == "InteractEvn")
            {
                s_evn = ss[1] + "Call";
                if (ss.Length > 2)
                {
                    evn_data = int.Parse(ss[2]);
                    this.with_dat = true;
                }
            }
        }


        [HideInInspector] public HopeTools.HopeUdonFramework hugf;
        public object eventData;
        public object eventData1; // eventData1 is the same as eventData (eventData1 = eventData)
        public object eventData2;

        public void HugfInitAfter()
        {
            // user code after hugf init here
            //hugf.udonEvn.RegisterListener(nameof(this.DemeFunCall), this);
            //hugf.udonIoc.RegisterSingleton(nameof(this.card_tf_list), this, this.card_tf_list);
            Init();
        }


        public void HufgIocGet()
        {
            //var p = (Transform[])hugf.udonIoc.GetServiceObj(nameof(SDH_FaPaiJi.card_tf_list));
        }

        public override void Interact()
        {
            if (hufw == null || s_evn == "") return;
            if (with_dat) hufw.TriggerEventWithData(s_evn, evn_data);
            else hufw.TriggerEvent(s_evn);
        }
    }
}