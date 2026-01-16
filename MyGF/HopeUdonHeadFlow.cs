
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace HopeTools
{
    public class HopeUdonHeadFlow : UdonSharpBehaviour
    {
        public Transform target;
        public float frontOffset = 2;
        void Start()
        {
            if (target == null)
            {
                target = transform;
            }
        }
        void Update()
        {
            var head_p = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;
            var head_r = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).rotation;

            // 头部前方偏移
            target.position = head_p + head_r * Vector3.forward * frontOffset;
            target.rotation = head_r;

        }
    }
}