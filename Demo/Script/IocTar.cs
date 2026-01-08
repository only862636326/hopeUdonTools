
using SGS;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;


namespace HopeTools
{
    public class IocTar : UdonSharpBehaviour
    {
        private HopeUdonFramework hufw;
        void Start()
        {
            hufw = GameObject.Find(SGS_ConstVariable.SGS_HUGF_STRING).GetComponent<HopeUdonFramework>();
            hufw.Init();
            hufw.udonIoc.RegisterSingleton("TestIocTar", this, this.gameObject);
        }
    }
}


