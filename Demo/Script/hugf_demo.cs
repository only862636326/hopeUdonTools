
using HopeMahjong;
using HuPad;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace HopeTools
{
    public class hugf_demo : UdonSharpBehaviour
    {
        #region init code
        private bool _is_init = false;
        public void Init()
        {
            if (this._is_init)
                return;
            this._is_init = true;

        }

        public HopeTools.HopeUdonFramework hugf;
        public object eventData;
        public object eventData1;
        public object eventData2;

        public void HugfInitAfter()
        {
            // user code after hugf init here
            //hugf.udonIoc.RegisterSingleton("IocDemoTar", this, this); // IOC
            //hugf.udonIoc.RegisterSingleton(nameof(HuPadCtr), this, this); // IOC
            //hugf.udonEvn.RegisterListener(nameof(this.DemoFunCall), this); // EVN
            //hugf.udonEvn.RegisterListener(nameof(this.DemoEvnHelp), this); // EVN
        }

        public void HufgIocGet()
        {
            //var ioc = (HuPadCtr)hugf.udonIoc.GetServiceUdon(nameof(HuPadCtr));

            //hugf.TriggerEventWithData(nameof(HopeTools.HopeUdonEvnHelp.OnHugfAddEvnCall), nameof(this.DemoEvnHelp));
        }


        public void DemoFunCall()
        {
            ;
        }

        public void DemoEvnHelp()
        {
            ;
        }

        #endregion end init code
    }
}