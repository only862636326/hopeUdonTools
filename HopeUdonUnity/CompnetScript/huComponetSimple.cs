
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;


namespace HopeTools
{

    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class huComponetSimple : UdonSharpBehaviour
    {
        public Transform active_obj;
        #region public fun
        private Transform _title_transform;
        private Transform _content_transform;
        private Toggle _component_active_toggle;

        private void PublicInitGetComponet()
        {
            _title_transform = this.transform;
            var prt = transform.parent;
            var idx = this.transform.GetSiblingIndex() + 1;
            _content_transform = prt.GetChild(idx);
            _component_active_toggle = this.transform.Find("ToggleActive").GetComponent<Toggle>();
        }

        public void ToggleActive()
        {
            if (active_obj == null)
                return;
            if (_op_componet == null)
                return;
            //_op_componet.enabled = _component_active_toggle.isOn;
        }

        public void SetSelfActive()
        {
            _content_transform.gameObject.SetActive(_op_componet != null);
            _title_transform.gameObject.SetActive(_op_componet != null);
        }

        public void LogMessge(object messge)
        {
            Debug.Log("                      [HopeUnityGameObjCompnet] " + messge.ToString());
        }

        void Start()
        {
            PublicInitGetComponet();
            InitGetComponet();
        }

        private bool _is_init = false;
        public void UpdataVal()
        {
            if (!this._is_init)
            {
                PublicInitGetComponet();
                InitGetComponet();
            }
            if (active_obj == null)
                return;
            //_op_componet = active_obj.gameObject;
            SetSelfActive();
            InitSetComponetVal();
        }
        #endregion publicfun

        private object _op_componet;

        public void InitGetComponet()
        {
            ;
        }
        public void InitSetComponetVal()
        {
            ;
        }
    }
}