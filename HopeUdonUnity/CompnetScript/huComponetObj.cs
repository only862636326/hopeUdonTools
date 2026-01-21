
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace HopeTools
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class huComponetObj : UdonSharpBehaviour
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

            //    _op_componet.enabled = _component_active_toggle.isOn;
            _op_componet.SetActive(_component_active_toggle.isOn);
        }

        //public void SetSelfActive()
        //{
        //    this.content.gameObject.SetActive(_op_componet == null);
        //    title.gameObject.SetActive(_op_componet == null);
        //}

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
            _op_componet = active_obj.gameObject;
            //SetSelfActive();
            InitSetComponetVal();
        }

        #endregion publicfun
        private GameObject _op_componet;

        #region GameObj fun
        private Toggle _gameobj_sta_toggle;
        private InputField _gameobj_name_input;

        void InitGetComponet()
        {
            _gameobj_sta_toggle = transform.Find("ToggleActive").GetComponent<Toggle>();
            _gameobj_name_input = transform.GetComponentInChildren<InputField>();
        }

        void InitSetComponetVal()
        {
            if (active_obj != null)
            {
                _gameobj_sta_toggle.isOn = active_obj.gameObject.activeSelf;
                _gameobj_name_input.text = active_obj.name;
            }
        }

        public void OnGameObjStateChange()
        {
            if (active_obj != null)
            {
                active_obj.gameObject.SetActive(_gameobj_sta_toggle.isOn);
            }
        }
        public void OnInputFieldSubmit()
        {
            if (active_obj != null)
            {
                active_obj.name = _gameobj_name_input.text;
            }
        }
        #endregion GameObjState
    }
}


