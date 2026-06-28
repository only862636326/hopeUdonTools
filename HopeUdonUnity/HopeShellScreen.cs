
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

namespace HopeTools
{
    public class HopeShellScreen : UdonSharpBehaviour
    {
        public InputField _screen_text;
        public Scrollbar _scrobal;
        public int _line_count = 0;

        private bool _is_forbig = false;
        public UdonSharpBehaviour _udon;

        void Start()
        {
            ;
        }

        void Update()
        {
            ForbigCheck();
        }

        void ForbigCheck()
        {
            // ctrl + alt + t
            if (Input.GetKeyDown(KeyCode.T) && Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt))
            {
                _is_forbig = !_is_forbig;
                PrintLine("HopeShell Forbig mode : " + (_is_forbig ? "ON" : "OFF"));

                if (_udon == null)
                {
                    Networking.LocalPlayer.Immobilize(_is_forbig);
                    Networking.LocalPlayer.SetJumpImpulse(!_is_forbig ? 3.0f : 0.0f);
                }
                else
                {
                    string eventName = _is_forbig ? "SuperForbig" : "SuperForbigOff";
                    _udon.SendCustomEvent(eventName);
                }
            }
        }

        public void PrintLine(string line)
        {
            _line_count += 1;
            _screen_text.text += "    " + line + "\r\n";

            AutoHeight();
            SendCustomEventDelayedSeconds(nameof(ScrollToBottom), 0.05f);
        }

        public void ScrollToBottom()
        {
            _scrobal.value = 0.0f;
        }

        public void ClearScreen()
        {
            _line_count = 0;
            _screen_text.text = "";
            AutoHeight();
        }

        private float _org_height = 0;
        private void AutoHeight()
        {
            if(_org_height < 10.0f)
                _org_height = _screen_text.GetComponent<RectTransform>().sizeDelta.y;

            var h = _line_count * 18.0f;
            h = Mathf.Max(h, _org_height);
            var org_wh = _screen_text.GetComponent<RectTransform>().sizeDelta;
            org_wh.y = h;
            _screen_text.GetComponent<RectTransform>().sizeDelta = org_wh;
        }
    }
}