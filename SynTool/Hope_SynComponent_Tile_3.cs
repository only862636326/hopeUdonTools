
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace HopeTools
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class Hope_SynComponent_Tile_3 : UdonSharpBehaviour
    {
        [UdonSynced] public string data_string;
        [HideInInspector] public Hope_SynComponent_TileManager udon_syn_manager;
        private int owner_id = 0;

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

        public void SendData(string s)
        {
            this.data_string = s;
            RequestSerialization();
        }

        public override void OnPreSerialization()
        {
            //base.OnPreSerialization();    
            //udon_syn_manager.OnPreSendSynDataString(data_string, this.owner_id);
        }

        public override void OnDeserialization()
        {
            //udon_syn_manager.OnReceiveSynDataString(data_string, this.owner_id);
        }
    }
}   
