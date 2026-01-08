
using SGS;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace HopeTools
{
    public class IocDemo : UdonSharpBehaviour
    {
        private HopeUdonFramework hufw;
        void Start()
        {
            hufw = GameObject.Find(SGS_ConstVariable.SGS_HUGF_STRING).GetComponent<HopeUdonFramework>();
            hufw.Init();
            hufw.udonEvn.RegisterListener(nameof(this.ExternITCall), this);
        }

        private GameObject _ctr_obj; // 私有字段

        GameObject ctr_obj
        {
            get
            {
                if (_ctr_obj == null)
                {
                    _ctr_obj = (GameObject)hufw.udonIoc.GetServiceObj("TestIocTar");
                }
                return _ctr_obj;
            }
        }
        public void ExternITCall()
        {
            ctr_obj.SetActive(!ctr_obj.activeSelf);
        }
    }
}
