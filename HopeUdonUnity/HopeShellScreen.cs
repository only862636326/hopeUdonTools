
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;
using System;

namespace HopeTools
{
    public class HopeShellScreen : UdonSharpBehaviour
    {

        
        private bool _is_forbig = false;
        void Start()
        {
            ;
        }

        void Update()
        {
            ForbigCheck();

            if (Input.GetKeyDown(KeyCode.UpArrow) && hope_shell != null)
            {
                hope_shell.OnCmdLineUpArrow();
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) && hope_shell != null)
            {
                hope_shell.OnCmdLineDownArrow();
            }
        }

        public UdonSharpBehaviour extern_forbid_udon;
        void ForbigCheck()
        {
            // ctrl + alt + t
            if (Input.GetKeyDown(KeyCode.T) && Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt))
            {
                _is_forbig = !_is_forbig;
                PrintLine("HopeShell Forbig mode : " + (_is_forbig ? "ON" : "OFF"));

                if (extern_forbid_udon == null)
                {
                    Networking.LocalPlayer.Immobilize(_is_forbig);
                    Networking.LocalPlayer.SetJumpImpulse(!_is_forbig ? 3.0f : 0.0f);
                }
                else
                {
                    string eventName = _is_forbig ? "SuperForbig" : "SuperForbigOff";
                    extern_forbid_udon.SendCustomEvent(eventName);
                }
            }
        }

        public Text _screen_text;
        public HopeShell hope_shell;

        public void PrintLine(string line)
        {
            if(_screen_text == null)
                return;
            _screen_text.text += line + "\n";
        }

        public void ClearScreen()
        {
            if(_screen_text == null)
                return;
            _screen_text.text = "";
        }


        public object eventData;
        public void Evn_PrintLine()
        {
            PrintLine(eventData.ToString());
        }
        public void Evn_ClearScreen()
        {
            ClearScreen();
        }

        public InputField cmd_line;

        public void OnCmdLineSubmit()
        {
            if (hope_shell == null || cmd_line == null || cmd_line.text == "")
                return;
            var cmd = cmd_line.text;
            cmd = cmd.Trim();
            if (cmd.StartsWith(">"))
            {
                cmd = cmd.Substring(1);
            }
            hope_shell.InputCommandText(cmd);
            cmd_line.text = ">";
        }
        
        public void Evn_SetCmdLine()
        {
            if(cmd_line == null)
                return;

            cmd_line.text = (string)eventData;
        }

        public void AddCmdLine(string line)
        {
            if(cmd_line == null)
                return;
            cmd_line.text += line + "\n";
        }

        public void Evn_ClearCmdLine()
        {
            cmd_line.text = "";
        }


    }
}




