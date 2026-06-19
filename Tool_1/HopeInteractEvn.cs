
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
        public override void Interact()
        {
            if(evnName == "")
                evnName = this.gameObject.name;
            foreach (UdonSharpBehaviour udon in udons)
            {
                udon.SendCustomEvent(this.gameObject.name);
            }
        }
    }
}









