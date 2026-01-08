
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace HopeTools
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class Hope_SynComponent_Tile_2 : UdonSharpBehaviour
    {
        public int MAX_SYN_DATA = 10;
        [UdonSynced] public int[] data_int_n;
        [HideInInspector] public Hope_SynComponent_TileManager udon_syn_manager;
        private int owner_id = 0;

        private void Start()
        {
            this.data_int_n = new int[MAX_SYN_DATA];
        }

        public void BecomeOwner()
        {
#if !UNITY_EDITOR
        if (!Networking.IsOwner(this.gameObject))
            Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
#endif
        }

        public void SetownerId(int id)
        {
            this.owner_id = id;
        }

        public void SendData(int[] data, int n)
        {
            n = n > MAX_SYN_DATA ? MAX_SYN_DATA : n;
            for(int i = 0; i < n; i++)
            {
                this.data_int_n[i] = data[i];
            }
            RequestSerialization();
        }

        public override void OnPreSerialization()
        {
            //base.OnPreSerialization();    
            //udon_syn_manager.SetProgramVariable(data_int_n, MAX_SYN_DATA, this.owner_id);
        }

        public override void OnDeserialization()
        {
            //udon_syn_manager.OnReceiveSynDataInt_N(data_int_n, MAX_SYN_DATA, this.owner_id);
        }
    }
}