
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;


namespace HopeTools
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class HopePPCamerCtr : UdonSharpBehaviour
    {
        
        [Header("Optional References")]
        public Camera localPlayerCamera;
        public float localPlayerCameraSize = 0.25f;
        public RenderTexture[] renderTextures;
        public HopeUdonFramework hugf;

        public int eventData;

        void Start()
        {
            hugf.Init();
            hugf.udonEvn.RegisterListener("OnTrgPPCapt", this);
        }


        void Update()
        {
            ;
        }
        void CapPP(int vrc_id)
        {
            if (localPlayerCamera != null)
            {                
                var player = VRCPlayerApi.GetPlayerById(vrc_id);
                if (player != null)
                {
                    localPlayerCamera.gameObject.SetActive(true);
                    SendCustomEventDelayedFrames("DisnableLoacalCamera", 2);

                    VRCPlayerApi.TrackingData data = player.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);
#if UNITY_EDITOR
                    var t_p = data.rotation * Vector3.forward * 2;
                    t_p.y = 0;
                    Vector3 cPos = data.position + t_p;
#else
                    Vector3 cPos = data.position + data.rotation * Vector3.forward * 2;
#endif
                    localPlayerCamera.transform.SetPositionAndRotation(cPos, Quaternion.LookRotation(data.position - cPos));
                    localPlayerCamera.orthographicSize = localPlayerCameraSize * GetLocalAvatarHeight();
                }
            }
        }


        public void OnTrgPPCapt()
        {
            // hugf.debug.Log("OnTrgPPCapt");
           
            ChangerTargetT(eventData);
        }


        public void SetTartTextureSrc(RenderTexture texture)
        {
            ;
        }

        public void ChangerTargetT(int x)
        {
            if (x >= renderTextures.Length)
            {
                return;
            }
            localPlayerCamera.targetTexture = renderTextures[x];
            CapPP(Networking.LocalPlayer.playerId);
        }

        public void EnableoacalCamera()
        {
            localPlayerCamera.gameObject.SetActive(true);
        }
        public void DisnableLoacalCamera()
        {
            localPlayerCamera.gameObject.SetActive(false);
        }

        public float GetAvatarHeight(VRCPlayerApi player)
        {
            float height = 0;
            Vector3 postition1 = player.GetBonePosition(HumanBodyBones.Head);
            Vector3 postition2 = player.GetBonePosition(HumanBodyBones.Neck);
            height += (postition1 - postition2).magnitude;
            postition1 = postition2;
            postition2 = player.GetBonePosition(HumanBodyBones.Hips);
            height += (postition1 - postition2).magnitude;
            postition1 = postition2;
            postition2 = player.GetBonePosition(HumanBodyBones.RightLowerLeg);
            height += (postition1 - postition2).magnitude;
            postition1 = postition2;
            postition2 = player.GetBonePosition(HumanBodyBones.RightFoot);
            height += (postition1 - postition2).magnitude;
            return height > 0 ? height : 1;
        }
        private float GetLocalAvatarHeight()
        {
            if (Networking.LocalPlayer == null)
                return 1;
            return GetAvatarHeight(Networking.LocalPlayer);
        }
    }
}