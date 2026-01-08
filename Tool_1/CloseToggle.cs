
using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace HopeTools
{
    public class CloseToggle : UdonSharpBehaviour
    {
        public GameObject[] obj_list;
        public bool _reserve = false;
        void Start()
        {
            for (int i = 0; i < obj_list.Length; i++)
            {
                obj_list[i].SetActive(_reserve);               
            }
        }

        public override void OnPlayerTriggerEnter(VRC.SDKBase.VRCPlayerApi player)
        {
            if (player == Networking.LocalPlayer)
            {
                for (int i = 0; i < obj_list.Length; i++)
                {
                    obj_list[i].SetActive(!_reserve);
                }
            }
        }

        public override void OnPlayerTriggerExit(VRC.SDKBase.VRCPlayerApi player)
        {
            if (player == Networking.LocalPlayer)
            {
                for (int i = 0; i < obj_list.Length; i++)
                {
                    obj_list[i].SetActive(_reserve);
                }
            }
        }
    }
}