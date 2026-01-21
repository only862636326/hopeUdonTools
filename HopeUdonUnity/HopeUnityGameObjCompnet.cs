
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;


namespace HopeTools
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class HopeUnityGameObjCompnet : UdonSharpBehaviour
    {
        public Transform active_obj;

        public UdonBehaviour[] _udon_list;
        void Start()
        {
            _udon_list = GetComponentsInChildren<UdonBehaviour>();
            InitActiveTf(this.active_obj);
        }

        public void InitActiveTf(Transform tf)
        {
            if (tf == null)
                return;

            active_obj = tf;
            foreach (var udon in _udon_list)
            {
                udon.gameObject.SetActive(true);
                udon.SetProgramVariable(nameof(huComponetSimple.active_obj), active_obj);
                udon.SendCustomEvent(nameof(huComponetSimple.UpdataVal));

                //var tfs = (object[]) udon.GetProgramVariable(nameof(huComponetSimple.exter_active_tf));
                //if(tfs != null && tfs.Length == 3) 
                //{
                //    if (tfs[2] == null)
                //    {
                //        ;
                //    }
                //}
            }
        }
    }
}















