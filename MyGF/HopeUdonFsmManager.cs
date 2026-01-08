
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace HopeTools
{
    public class HopeUdonFsmManager : UdonSharpBehaviour
    {
        UdonSharpBehaviour[] fsm_list;
        string _current_state;
        private int updata_delay_frame = 0;
        void Start()
        {
            ;
        }

        void Update()
        {
            ;
        }

        public void AddFsm(UdonSharpBehaviour fsm)
        {
            var new_fsm_list = new UdonSharpBehaviour[fsm_list.Length + 1];

            for (int i = 0; i < fsm_list.Length; i++)
            {
                new_fsm_list[i] = fsm_list[i];
            }
            new_fsm_list[new_fsm_list.Length - 1] = fsm;
            fsm_list = new_fsm_list;
        }
    }
}
