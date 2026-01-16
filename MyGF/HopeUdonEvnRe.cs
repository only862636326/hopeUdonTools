
using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
using UnityEditor.UI;

namespace HopeTools
{
    public class HopeUdonEvnRe : UdonSharpBehaviour
    {
        public string evn_list;
        public int[] pause_list;
        public Text show_ui;

        void Start()
        {
            ;
        }

        public void Init()
        {
            ;
        }

        public void ClearEvn()
        {
            evn_list = "";
        }

        public void AddEvn(string evn_name)
        {
            evn_list += evn_name + "\n";
        }

        public void AddEvnWithDat(string evn_name, object data)
        {
            evn_list += evn_name + " : " + data.ToString() + "\n";
        }

        public void AddEvnWith2Dat(string evn_name, object data1, object data2)
        {
            evn_list += evn_name + " : " + data1.ToString() + " , " + data2.ToString() + "\n";
        }

        public void TrgNextEvn()
        {
            ;
        }
        public void AutoTigEvn()
        {
            ;
        }
        public void PauseEvn()
        {
            ;
        }
    }
}