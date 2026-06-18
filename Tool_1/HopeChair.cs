
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace HopeUdonTools
{
    public class HopeChair : UdonSharpBehaviour
    {
        void Start()
        {
            ;
        }

        void Update()
        {
            ;
        }

        public void SuperForbig()
        {
            Debug.Log("SuperForbig");
            VRCPlayerApi p = Networking.LocalPlayer;
            p.UseAttachedStation();
        }

        public override void Interact()
        {
            Debug.Log("Interact");
        }
        
        public override void OnStationEntered(VRCPlayerApi player)
        {
            Debug.Log("OnStationEntered");
        }

        public override void OnStationExited(VRCPlayerApi player)
        {
            Debug.Log("OnStationExited");
        }
    }
}