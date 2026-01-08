
#define HOPE_SYN_LOG
#define SYN_DATA_INT_ONE
//#define SYN_DATA_INT_N
//#define SYN_DATA_STRING


using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace HopeTools
{
    public class Hope_SynComponent_TileManager : UdonSharpBehaviour
    {
        public const int MAX_PLAY = 50;
#if SYN_DATA_INT_ONE
        private Hope_SynComponent_Tile_1[] hope_SynComponent_Tile_1_s;
#endif

#if SYN_DATA_INT_N
        private Hope_SynComponent_Tile_2[] hope_SynComponent_Tile_2_s;
#endif

#if SYN_DATA_STRING
        private Hope_SynComponent_Tile_3[] hope_SynComponent_Tile_3_s;
#endif
        private int local_play_id = 0;
        private int send_temp = 0;
        private bool _is_init = false;
        private Transform syn_tile_parent;
        public void Init()
        {
            if(!this._is_init)
            {
                this._is_init = true;
                GameObject syn_tile = this.transform.GetChild(0).gameObject;
                this.syn_tile_parent = this.transform.GetChild(1).gameObject.transform;

                for (int i = 0; i < MAX_PLAY; i++)
                {
                    GameObject syn_tile_clone = Instantiate(syn_tile);
                    syn_tile_clone.name = "syn_tile_" + i;
                    syn_tile_clone.gameObject.SetActive(false);
                    syn_tile_clone.transform.parent = syn_tile_parent.transform;
                }   

#if SYN_DATA_INT_ONE
                this.hope_SynComponent_Tile_1_s = new Hope_SynComponent_Tile_1[MAX_PLAY];
#endif
#if SYN_DATA_INT_N

                this.hope_SynComponent_Tile_2_s = new Hope_SynComponent_Tile_2[MAX_PLAY];
#endif

#if SYN_DATA_STRING

                this.hope_SynComponent_Tile_3_s = new Hope_SynComponent_Tile_3[MAX_PLAY];
#endif

                for (int i = 0; i < this.syn_tile_parent.childCount; i++)
                {
#if SYN_DATA_INT_ONE
                    this.hope_SynComponent_Tile_1_s[i] = this.syn_tile_parent.GetChild(i).GetComponent<Hope_SynComponent_Tile_1>();
                    this.hope_SynComponent_Tile_1_s[i].udon_syn_manager = this;
                    this.hope_SynComponent_Tile_1_s[i].SetownerId(i);
#endif

#if SYN_DATA_INT_N

                    this.hope_SynComponent_Tile_2_s[i] = this.syn_tile_parent.GetChild(i).GetComponent<Hope_SynComponent_Tile_2>();
                    this.hope_SynComponent_Tile_2_s[i].udon_syn_manager = this;
                    this.hope_SynComponent_Tile_2_s[i].SetownerId(i);
#endif

#if SYN_DATA_STRING
                    this.hope_SynComponent_Tile_3_s[i] = this.syn_tile_parent.GetChild(i).GetComponent<Hope_SynComponent_Tile_3>();
                    this.hope_SynComponent_Tile_3_s[i].udon_syn_manager = this;
                    this.hope_SynComponent_Tile_3_s[i].SetownerId(i);
#endif
                }
            }
        }

        void Start()
        {
            Init();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                SendSynDataOne(this.send_temp++);
                //SendSynDataString( "syn_string : " + this.send_temp.ToString());
            }
        }

        public override void OnPlayerLeft(VRCPlayerApi player)
        {
            //base.OnPlayerLeft(player);
            this.syn_tile_parent.GetChild(player.playerId).gameObject.SetActive(false);
            //this.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ManualSynComponent");
        }

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            //base.OnPlayerJoined(player);
            this.syn_tile_parent.GetChild(player.playerId).gameObject.SetActive(true);
            //this.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ManualSynComponent");
        }

        public void ManualSynComponent()
        {
            VRCPlayerApi[] palyer_s = new VRCPlayerApi[MAX_PLAY];
            VRCPlayerApi.GetPlayers(palyer_s);

            for(int i = 0;i < MAX_PLAY;i++)
            {
                if (palyer_s[i] != null && palyer_s[i].playerId != 0)
                {
                    this.syn_tile_parent.GetChild(i).gameObject.SetActive(true);
                }
                else
                {
                    this.syn_tile_parent.GetChild(i).gameObject.SetActive(false);
                }
            }
        }

#if SYN_DATA_INT_ONE
        public void SendSynDataOne(int data)
        {
            if (this.local_play_id == 0)
            {
                this.local_play_id = Networking.LocalPlayer.playerId;
                this.syn_tile_parent.GetChild(this.local_play_id).gameObject.SetActive(true);
                this.hope_SynComponent_Tile_1_s[this.local_play_id].BecomeOwner();
            }
            if (VRCPlayerApi.GetPlayerCount() > 1)
                this.hope_SynComponent_Tile_1_s[this.local_play_id].SendData(data);
            else
                OnPreSendSynDataOne(data, this.local_play_id);
        }

        public void OnPreSendSynDataOne(int data, int sender)
        {
#if HOPE_SYN_LOG
            Debug.Log($"OnPreSendSynDataOne : {data}, {sender}");
#endif
        }

        public void OnReceiveSynDataOne(int data, int sender)
        {
#if HOPE_SYN_LOG
            Debug.Log($"OnReceiveSynDataOne : {data}, {sender}");
#endif
        }
#endif


#if SYN_DATA_INT_N
        public void SendSynDataInt_N(int[] data, int n)
        {
            if (this.local_play_id == 0)
            {
                this.local_play_id = Networking.LocalPlayer.playerId;
                this.syn_tile_parent.GetChild(this.local_play_id).gameObject.SetActive(true);
                this.hope_SynComponent_Tile_1_s[this.local_play_id].BecomeOwner();
            }
            if (VRCPlayerApi.GetPlayerCount() > 1)
                this.hope_SynComponent_Tile_2_s[this.local_play_id].SendData(data, n);
            else
                OnPreSendSynDataInt_N(data, n, this.local_play_id);
        }

        public void OnPreSendSynDataInt_N(int[] data,int n, int sender)
        {
#if HOPE_SYN_LOG
            Debug.Log($"OnPreSendSynDataInt_N : {data}, {n} ,{sender}");
#endif
        }

        public void OnReceiveSynDataInt_N(int[] data,int n, int sender)
        {
#if HOPE_SYN_LOG
            Debug.Log($"OnReceiveSynDataInt_N : {data}, {n} ,{sender}");
#endif
        }
#endif


#if SYN_DATA_STRING
        public void SendSynDataString(string data)
        {
            if (this.local_play_id == 0)
            {
                this.local_play_id = Networking.LocalPlayer.playerId;
                this.syn_tile_parent.GetChild(this.local_play_id).gameObject.SetActive(true);
                this.hope_SynComponent_Tile_3_s[this.local_play_id].BecomeOwner();
            }
            if (VRCPlayerApi.GetPlayerCount() > 1)
                this.hope_SynComponent_Tile_3_s[this.local_play_id].SendData(data);
            else
                OnPreSendSynDataString(data, this.local_play_id);
        }

        public void OnPreSendSynDataString(string data, int sender)
        {
            Debug.Log($"OnPreSendSynDataString : {data}, {sender}");
        }

        public void OnReceiveSynDataString(string data, int sender)
        {
            Debug.Log($"OnReceiveSynDataString : {data}, {sender}");
        }
#endif

    }
}