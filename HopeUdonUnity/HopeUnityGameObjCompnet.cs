
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
        public Transform __Inspector_size_prt;
        void Start()
        {
            if(__Inspector_size_prt == null)
            {
                __Inspector_size_prt = this.transform.GetChild(0);
            }

            GameStaFunInit();
            GameStaFunTextInit();

            TransformFunInit();
            TransformFunTextInit();

            TextFunInit();
            TextFunTextInit();
        }

        #region GameObjState
        private Transform _game_bj_title;
        private Toggle _gameobj_sta_toggle;
        private InputField _gameobj_name_input;
        private bool _is_fun_gamesta_init = false;
        void GameStaFunInit()
        {
            if(_is_fun_gamesta_init)
            {
                return;
            }
            _is_fun_gamesta_init = true;

            _game_bj_title = __Inspector_size_prt.Find("GameObjectTitle");
            _gameobj_sta_toggle = _game_bj_title.Find("ToggleSta").GetComponent<Toggle>();
            _gameobj_name_input = _game_bj_title.GetComponentInChildren<InputField>();

        }
        void GameStaFunTextInit()
        {
            if (active_obj != null)
            {
                _gameobj_sta_toggle.isOn = active_obj.gameObject.activeSelf;
                _gameobj_name_input.text = active_obj.name;
            }
        }

        public void OnGameObjStateChange()
        {
            GameStaFunInit();

            if(active_obj != null)
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

        #region Transform Fun
        private Transform _item_transform;
        private InputField[] _transform_input;
        private bool _is_fun_transform_init = false;
        private void TransformFunInit()
        {
            if(_is_fun_transform_init)
            {
                return;
            }
            _is_fun_transform_init = true;
            _item_transform = __Inspector_size_prt.Find("TransformTitle");
            _transform_input = __Inspector_size_prt.Find("TransformContent").GetComponentsInChildren<InputField>();
        }

        private void TransformFunTextInit()
        {
            if(active_obj != null)
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
           
            TransformFunInit();
            LogMessge("OnTransformInputSubmit");
            if(active_obj != null)
            {
                if(_transform_input[0].text == "")
                {
                    _transform_input[0].text = active_obj.localPosition.x.ToString();
                }
                if(_transform_input[1].text == "")
                {
                    _transform_input[1].text = active_obj.localPosition.y.ToString();
                }
                if(_transform_input[2].text == "")
                {
                    _transform_input[2].text = active_obj.localPosition.z.ToString();
                }
                if(_transform_input[3].text == "")
                {
                    _transform_input[3].text = active_obj.localRotation.eulerAngles.x.ToString();
                }
                if(_transform_input[4].text == "")
                {
                    _transform_input[4].text = active_obj.localRotation.eulerAngles.y.ToString();
                }
                if(_transform_input[5].text == "")
                {
                    _transform_input[5].text = active_obj.localRotation.eulerAngles.z.ToString();
                }
                if(_transform_input[6].text == "")
                {
                    _transform_input[6].text = active_obj.localScale.x.ToString();
                }
                if(_transform_input[7].text == "")
                {
                    _transform_input[7].text = active_obj.localScale.y.ToString();
                }
                if(_transform_input[8].text == "")
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

        #endregion Transform Fun


        #region Text Fun
        private Transform _item_text;
        private Text _op_text_content;
        private Image _text_color_image;
        private bool _is_fun_text_init = false;
        private InputField _text_input_content;
        private InputField _text_input_font;
        private Slider _slider_text_color_h;
        private Slider _slider_text_color_s;
        private Slider _slider_text_color_v;

        private void TextFunInit()
        {
            if(_is_fun_text_init)            
            {
                return;
            }
            _is_fun_text_init = true;
            _item_text = __Inspector_size_prt.Find("TextTitle");
            var inputs = __Inspector_size_prt.Find("TextContent").GetComponentsInChildren<InputField>();
            _text_input_content = inputs[0];
            _text_input_font = inputs[1];      
            
            _text_color_image = __Inspector_size_prt.Find("TextContent").GetComponentInChildren<Image>();
            var Sliders = __Inspector_size_prt.Find("TextContent").GetComponentsInChildren<Slider>();
            _slider_text_color_h = Sliders[0];
            _slider_text_color_s = Sliders[1];
            _slider_text_color_v = Sliders[2];      

        } 

        private void TextFunTextInit()
        {
            if(active_obj != null)
            {
                _op_text_content = active_obj.GetComponent<Text>();

                _text_input_content.text = _op_text_content.text;
                _text_input_font.text = _op_text_content.fontSize.ToString();

                Color.RGBToHSV(_op_text_content.color, out float h, out float s, out float v);
                _slider_text_color_h.value = h;
                _slider_text_color_s.value = s;
                _slider_text_color_v.value = v;
                _text_color_image.color = _op_text_content.color;
            }
        }

        public void OnTextInputContentSubmit()
        {
            TextFunInit();
            LogMessge("OnTextInputContentSubmit");
            if(active_obj != null)
            {
                _op_text_content.text = _text_input_content.text;
            }
        }

        public void OnTextInputFontSubmit()
        {
            TextFunInit();
            LogMessge("OnTextInputFontSubmit");
            if(active_obj != null)
            {
                _op_text_content.fontSize = int.Parse(_text_input_font.text);
            }
        }

        public void OnTextColorSliderChange()
        {
            TextFunInit();
            LogMessge("OnTextColorSliderChange");
            if(active_obj != null)
            {
                Color new_color = Color.HSVToRGB(
                    _slider_text_color_h.value,
                    _slider_text_color_s.value,
                    _slider_text_color_v.value
                );
                _op_text_content.color = new_color;
                _text_color_image.color = new_color;
            }
        }
        #endregion Text Fun

        #region 

        #endregion 
        public void LogMessge(object messge)
        {
            Debug.Log("                      [HopeUnityGameObjCompnet] " + messge.ToString());
        }   
    }
}
