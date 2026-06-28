
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace HopeUdonTools
{
    public class HopeChair : UdonSharpBehaviour
    {
        public void SuperForbig()
        {
            Debug.Log("SuperForbig");

            VRCPlayerApi player = Networking.LocalPlayer;
            if (player == null)
            {
                Debug.LogWarning("SuperForbig: LocalPlayer is null.");
                return;
            }

            // 将椅子移动到玩家位置
            transform.position = player.GetPosition();
            transform.rotation = player.GetRotation();

            // 让玩家坐上去
            var station = GetComponent<VRC.SDKBase.VRCStation>();
            if (station != null)
            {
                station.UseStation(player);
            }
            else
            {
                Debug.LogWarning("SuperForbig: VRCStation component not found on this object.");
            }
        }

        public void SuperForbigOff()
        {
            Debug.Log("SuperForbigOff");

            VRCPlayerApi player = Networking.LocalPlayer;
            if (player == null)
            {
                Debug.LogWarning("SuperForbigOff: LocalPlayer is null.");
                return;
            }

            // 优先使用椅子上的 ExitStation API 退出座位
            var station = GetComponent<VRC.SDKBase.VRCStation>();
            if (station != null)
            {
                station.ExitStation(player);
            }
            else
            {
                Debug.LogWarning("SuperForbigOff: VRCStation component not found on this object; falling back to UseAttachedStation().");
                player.UseAttachedStation();
            }
        }

        // public override void Interact()
        // {
        //     Debug.Log("Interact");
        // }
        
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

