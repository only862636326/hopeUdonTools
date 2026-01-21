
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace HopeTools
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class huComponetText : UdonSharpBehaviour
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
            _op_componet.enabled = _component_active_toggle.isOn;
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
            //this.gameObject.SetActive(false);
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
            _op_componet = active_obj.GetComponent<Text>();
            LogMessge("text UpdataVal ");
            SetSelfActive();
            InitSetComponetVal();
        }

        #endregion publicfun

        public Text _op_componet;

        private Image _text_color_image;
        private InputField _text_input_content;
        private InputField _text_input_font;
        private Slider _slider_text_color_h;
        private Slider _slider_text_color_s;
        private Slider _slider_text_color_v;

        private void InitGetComponet()
        {
            var inputs = _content_transform.GetComponentsInChildren<InputField>();
            _text_input_content = inputs[0];
            _text_input_font = inputs[1];
            _text_color_image = _content_transform.GetComponentInChildren<Image>();
            var Sliders = _content_transform.GetComponentsInChildren<Slider>();
            _slider_text_color_h = Sliders[0];
            _slider_text_color_s = Sliders[1];
            _slider_text_color_v = Sliders[2];
        }

        private void InitSetComponetVal()
        {
            if (_op_componet == null)
                return;

            _text_input_content.text = _op_componet.text;
            _text_input_font.text = _op_componet.fontSize.ToString();

            Color.RGBToHSV(_op_componet.color, out float h, out float s, out float v);
            _slider_text_color_h.value = h;
            _slider_text_color_s.value = s;
            _slider_text_color_v.value = v;
            _text_color_image.color = _op_componet.color;
        }

        public void OnTextInputContentSubmit()
        {
            LogMessge("OnTextInputContentSubmit");
            if (active_obj != null)
            {
                _op_componet.text = _text_input_content.text;
            }
        }

        public void OnTextInputFontSubmit()
        {
            LogMessge("OnTextInputFontSubmit");
            if (active_obj != null)
            {
                _op_componet.fontSize = int.Parse(_text_input_font.text);
            }
        }

        public void OnTextColorSliderChange()
        {
            LogMessge("OnTextColorSliderChange");
            if (active_obj != null)
            {
                Color new_color = Color.HSVToRGB(
                    _slider_text_color_h.value,
                    _slider_text_color_s.value,
                    _slider_text_color_v.value
                );
                _op_componet.color = new_color;
                _text_color_image.color = new_color;
            }
        }
    }
}

