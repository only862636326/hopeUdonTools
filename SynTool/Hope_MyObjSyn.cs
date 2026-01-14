
using SGS;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

namespace SGS
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class Hope_MyObjSyn : UdonSharpBehaviour
    {
        [UdonSynced] public Vector3 syn_p;
        [UdonSynced] public Quaternion syn_r;
        public int card_idx = 0;
        HopeTools.HopeUdonFramework hugf;
        public int _card_id = 0;        
        private bool _is_pick_up;
        
        void Start()
        {
            //hugf = GameObject.Find("Hugf").GetComponent<HopeTools.HopeUdonFramework>();   
        }
        float delay_tim = 0.1f;
        void Update()
        {
            delay_tim -= Time.deltaTime;
            if(delay_tim > 0) { return; }
            delay_tim = 0.1f;

            {
                if (Networking.IsOwner(this.gameObject))
                {
                    if(Vector3.Distance(syn_p, this.transform.position) > 0.01f)
                    {
                        syn_p = this.transform.position;
                        syn_r = this.transform.rotation;
                        RequestSerialization();
                    }
                }
                else
                {
                    this.transform.position = syn_p;
                    this.transform.rotation = syn_r;
                }
            }
        }

        private VRC_Pickup _vrcPickup;
        public void DropAndMoveToTf(Transform tf)
        {
            if (_vrcPickup == null)
            {
                _vrcPickup = ((VRC_Pickup)this.transform.GetComponent(typeof(VRC_Pickup)));
            }

            _vrcPickup.Drop();
            this.transform.position = tf.position + Vector3.up * 0.01f;
            this.transform.rotation = tf.rotation;
        }

        public override void OnDeserialization()
        {
            ;
        }


        public override void OnPickup()
        {
            base.OnPickup();
            BecomeOwner();

            var t = this._card_id << 8;
            t |= this.card_idx;
            //hugf.TriggerEventWithData(nameof(SGS_Input.UnChooseGenerlCall), t);
            _is_pick_up = true;
        }

        public override void OnDrop()
        {
            base.OnDrop();
            _is_pick_up = false;
        }
        public override void OnPickupUseUp()
        {
            base.OnPickupUseUp();
            if (_is_pick_up)
            {
                var t = this._card_id << 8;
                t |= this.card_idx;
                //hugf.TriggerEventWithData(nameof(SGS_Input.ChooseGeneralCall), t);
            }
        }

        public void SetCardIdx(int idx)
        {
            this.card_idx = idx;
        }

        public void BecomeOwner()
        {
#if !UNITY_EDITOR
            if(!Networking.IsOwner(this.gameObject))
            {
                Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
            }
#endif
        }
    }
}