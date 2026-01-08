
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace HopeTools
{
    public class HopeUdonFsm : UdonSharpBehaviour
    {
        UdonSharpBehaviour[] fsm_sta_list;
        UdonSharpBehaviour _current_fsm;
        string _current_state;

        public void Init()
        {
            fsm_sta_list = GetComponentsInChildren<UdonSharpBehaviour>();
            foreach (var fsm in fsm_sta_list)
            {
                fsm.SendCustomEvent("Init");
            }
        }

        public int FsmStateCount()
        {
            var i = 0;
            foreach (var fsm in fsm_sta_list)
            {
                if(fsm != null)
                    i++;
            }
            return i;
        }

        public void ChangeFsmSta(string s)
        {
            if(s == null)
            {
                return;
            }

            if (s == "")
            {
                _current_fsm.SendCustomEvent("OnExit");
                _current_fsm = null;
                _current_state = "";
            }

            foreach (var fsm in fsm_sta_list)
            {
                var _name = fsm.GetProgramVariable("fsm_name").ToString();
                if (s == _name)
                {
                    _current_state = s;
                    _current_fsm.SendCustomEvent("OnExit");
                    fsm.SendCustomEvent("OnEnter");
                    _current_fsm = fsm;
                    break;
                }
            }
        }

        public override void OnPlayerCollisionStay(VRCPlayerApi player)
        {
            ;
        }


        public string GetCrutSat()
        {
            return _current_state;
        }
    }
}
