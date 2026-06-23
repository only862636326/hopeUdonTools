
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;


namespace HopeTools
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class HopeInteractEvn : UdonSharpBehaviour
    {
        public UdonSharpBehaviour[] udons;
        public string evnName;
        public bool _is_nex = false;
        void Start()
        {
            if (evnName == "")
                evnName = this.gameObject.name;
        }

        public override void Interact()
        {
            foreach (UdonSharpBehaviour udon in udons)
            {
                udon.SetProgramVariable("eventData", evnName);
                var _name = _is_nex ? "nex" : "ex";
                udon.SendCustomEvent(_name);
            }
        }
    }
}











