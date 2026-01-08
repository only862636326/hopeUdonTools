
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace HopeTools
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class Hope_SynComponent_Tile_1 : UdonSharpBehaviour
    {
        [UdonSynced] public int data_int_1;
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

        public void SendData(int data)
        {
            this.data_int_1 = data;
            RequestSerialization();
        }

        public override void OnPreSerialization()
        {
            //base.OnPreSerialization();
            udon_syn_manager.OnPreSendSynDataOne(data_int_1, this.owner_id);
        }

        public override void OnDeserialization()
        {
            udon_syn_manager.OnPreSendSynDataOne(data_int_1, this.owner_id);
        }
    }
}