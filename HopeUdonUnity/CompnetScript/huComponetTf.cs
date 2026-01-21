
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;


namespace HopeTools
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class huComponetTf : UdonSharpBehaviour
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

        //public void ToggleActive()
        //{
        //    if (active_obj == null)
        //        return;
        //    if (_op_componet == null)
        //        return;
        //    _op_componet.enabled = _component_active_toggle.isOn;
        //}

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
                _is_init = true;
            }
            if (active_obj == null)
                return;
            //_op_componet = active_obj;
            //SetSelfActive();
            InitSetComponetVal();
        }

        #endregion publicfun


        private Transform _op_componet;

        private InputField[] _transform_input;

        private void InitGetComponet()
        {
            _title_transform = this.transform;
            _transform_input = _content_transform.GetComponentsInChildren<InputField>();
        }

        private void InitSetComponetVal()
        {
            if (active_obj != null)
            {
                _transform_input[0].text = active_obj.localPosition.x.ToString();
                _transform_input[1].text = active_obj.localPosition.y.ToString();
                _transform_input[2].text = active_obj.localPosition.z.ToString();
                _transform_input[3].text = active_obj.localRotation.eulerAngles.x.ToString();
                _transform_input[4].text = active_obj.localRotation.eulerAngles.y.ToString();
                _transform_input[5].text = active_obj.localRotation.eulerAngles.z.ToString();
                _transform_input[6].text = active_obj.localScale.x.ToString();
                _transform_input[7].text = active_obj.localScale.y.ToString();
                _transform_input[8].text = active_obj.localScale.z.ToString();
            }
        }

        public void OnTransformInputSubmit()
        {
            if (active_obj != null)
            {
                if (_transform_input[0].text == "")
                {
                    _transform_input[0].text = active_obj.localPosition.x.ToString();
                }
                if (_transform_input[1].text == "")
                {
                    _transform_input[1].text = active_obj.localPosition.y.ToString();
                }
                if (_transform_input[2].text == "")
                {
                    _transform_input[2].text = active_obj.localPosition.z.ToString();
                }
                if (_transform_input[3].text == "")
                {
                    _transform_input[3].text = active_obj.localRotation.eulerAngles.x.ToString();
                }
                if (_transform_input[4].text == "")
                {
                    _transform_input[4].text = active_obj.localRotation.eulerAngles.y.ToString();
                }
                if (_transform_input[5].text == "")
                {
                    _transform_input[5].text = active_obj.localRotation.eulerAngles.z.ToString();
                }
                if (_transform_input[6].text == "")
                {
                    _transform_input[6].text = active_obj.localScale.x.ToString();
                }
                if (_transform_input[7].text == "")
                {
                    _transform_input[7].text = active_obj.localScale.y.ToString();
                }
                if (_transform_input[8].text == "")
                {
                    _transform_input[8].text = active_obj.localScale.z.ToString();
                }

                active_obj.localPosition = new Vector3(
                    float.Parse(_transform_input[0].text),
                    float.Parse(_transform_input[1].text),
                    float.Parse(_transform_input[2].text)
                );
                active_obj.localRotation = Quaternion.Euler(
                    float.Parse(_transform_input[3].text),
                    float.Parse(_transform_input[4].text),
                    float.Parse(_transform_input[5].text)
                );
                active_obj.localScale = new Vector3(
                    float.Parse(_transform_input[6].text),
                    float.Parse(_transform_input[7].text),
                    float.Parse(_transform_input[8].text)
                );
            }
        }
    }
}