
#define DELAY_SYN_FUNCITON

using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace HopeTools
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class Hope_SynComponent : UdonSharpBehaviour
    {
        public const int SYN_LIST_LEN = 40;
        public const int INDEX_SYN_ID = 39;
        public const int INDEX_PLAYER_ID = 38;

        [UdonSynced] public int[] syn_list;
        public int[] loc_list;
        public UdonBehaviour[] udon_src;

        private float manusyn_delay_second = -1.0f;
        private bool _is_init = false;

        public void Init()
        {
            if (!this._is_init)
            {
                this._is_init = true;
                this.syn_list = new int[SYN_LIST_LEN];
                this.loc_list = new int[SYN_LIST_LEN];
            }
        }

        void Start()
        {
            Init();
        }

        public void ClearSynList()
        {
            for (int i = 0; i < SYN_LIST_LEN; i++)
            {
                this.syn_list[i] = 0;
            }
        }
#if DELAY_SYN_FUNCITON
        void Update()
        {
            if (this.manusyn_delay_second > 0)
            {
                this.manusyn_delay_second -= Time.deltaTime;
                if (this.manusyn_delay_second <= 0)
                {
#if !UNITY_EDITOR
                        if(!Networking.IsOwner(this.gameObject))
                            Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
#endif
                        SendSynLocList();
                }
            }
        }
#endif

        #region 同步
        public override void OnPreSerialization()
        {
            OnDeserialization();
        }

        public void SendSynLocList()
        {
#if !UNITY_EDITOR
        if (Networking.IsOwner(this.gameObject))
#endif
            {
                for (int i = 0; i < SYN_LIST_LEN; i++)
                {
                    this.syn_list[i] = this.loc_list[i];
                }

                this.syn_list[INDEX_SYN_ID]++;
                this.syn_list[INDEX_PLAYER_ID] = Networking.LocalPlayer.playerId;
#if !UNITY_EDITOR
                RequestSerialization();
#else
                OnDeserialization();
#endif
            }
        }

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            //base.OnPlayerJoined(player);
            SendSynLocList();
        }

        public void ManualSynDelayFrames(float f)
        {
            this.manusyn_delay_second = f;
        }

        public override void OnDeserialization()
        {
            //Debug.Log($"self log : OnDeserialization : {Networking.LocalPlayer.playerId}");

            for (int i = 0; i < SYN_LIST_LEN; i++)
            {
                this.loc_list[i] = syn_list[i];
            }
            for (int i = 0; i < this.udon_src.Length; i++)
            {
                if (this.udon_src[i] != null)
                {
                    this.udon_src[i].SendCustomEvent("SynCall");
                }
            }
            // LogLocList();
        }

        public void BecomeOwner()
        {
#if !UNITY_EDITOR
        if (!Networking.IsOwner(this.gameObject))
            Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
#endif
        }

        public void LogLocList()
        {
            //return;
            var s = "self log  loc list: ";
            for (int i = 20; i < SYN_LIST_LEN; i++)
            {
                if(i %5 == 0)
                    s += "--";
                s += this.loc_list[i].ToString() + " ";
            }
            Debug.Log(s);
        }
        #endregion 同步
    }
}