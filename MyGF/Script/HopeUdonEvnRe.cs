
using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
using UnityEditor.UI;
using System.Collections.Generic;

namespace HopeTools
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class HopeUdonEvnRe : UdonSharpBehaviour
    {
        public const int CONFIG_DEFAULT_EVN_LIST_SIZE = 100;
        public string[] evn_list;
        private int _evn_index = 0;
        private int _pre_run_index = 0;

        public bool[] pause_list;

        public float auto_trigger_interval = 1.0f; // 自动触发间隔时间（秒）
        private bool is_auto_triggering = false; // 是否正在自动触发中

        void Start()
        {
            ;
        }

        public void Init()
        {
            evn_list = new string[CONFIG_DEFAULT_EVN_LIST_SIZE];
            pause_list = new bool[CONFIG_DEFAULT_EVN_LIST_SIZE];
        }

        public void ClearEvn()
        {
            // 重置索引，清空数组内容
            // for (int i = 0; i < _evn_index; i++)
            // {
            //     evn_list[i] = null;
            // }
            _evn_index = 0;
            ApiSendUiCmd("ClearEvn", null);
        }

        /// <summary>
        /// 扩容事件数组
        /// </summary>
        private void ExpandArrays()
        {
            // 扩容为原来的2倍
            int newSize = evn_list.Length * 2;
            string[] newArray = new string[newSize];

            // 复制原数组数据
            for (int i = 0; i < evn_list.Length; i++)
            {
                newArray[i] = evn_list[i];
            }

            evn_list = newArray;

            // 同步扩容pause_list
            bool[] newPauseList = new bool[newSize];
            for (int i = 0; i < pause_list.Length; i++)
            {
                newPauseList[i] = pause_list[i];
            }
            pause_list = newPauseList;
        }

        public void AddEvnString(string s)
        {
            if (!_is_recording)
            {
                return;
            }
            // 检查是否需要扩容
            if (_evn_index >= evn_list.Length)
            {
                ExpandArrays();
            }
            evn_list[_evn_index] = s;
            ApiUIItemShowCmd(_evn_index, s);
            _evn_index++;
        }

        // 目前只支持两个数据参数, 并且都要为int
        public void AddEvn(string evn_name)
        {
            AddEvnString(evn_name);
        }
        private bool _is_recording = true;
        public void StopReEvn()
        {
            _is_recording = false;
        }

        public void StartReEvn()
        {
            _is_recording = true;
        }

        public void AddEvnWithDat(string evn_name, object data)
        {
            
            AddEvnString(evn_name + "|" + data.ToString());
        }

        public void AddEvnWith2Dat(string evn_name, object data1, object data2)
        {
            AddEvnString(evn_name + "|" + data1.ToString() + "|" + data2.ToString());
        }

        public void TrgNextEvn()
        {
            if (_pre_run_index < _evn_index)
            {
                // 检查当前事件是否被暂停
                if (pause_list[_pre_run_index])
                {
                    return; // 暂停，不触发事件
                }
                // 触发当前事件

                // 移动到下一个事件
                ApiTrgCmdEvn(_pre_run_index, evn_list[_pre_run_index]);
                _pre_run_index++;
                ApiSendUiCmd("SetPreEvn", _pre_run_index.ToString());
            }
            else
            {
                // 所有事件触发完成
            }
        }

        public void SetTrg(int idx)
        {
            if (idx >= 0 && idx < _evn_index)
            {
                _pre_run_index = idx;
                this.StopAutoTrigger();
                ApiSendUiCmd("SetPreEvn", _pre_run_index.ToString());
            }
        }

        public void AutoTigEvn()
        {
            AutoTigEvnWithInterval(auto_trigger_interval);
        }

        public void AutoTigEvnWithInterval(float interval)
        {
            // 如果已经在自动触发中，先停止
            if (is_auto_triggering)
            {
                StopAutoTrigger();
                return;
            }

            // 开始新的自动触发序列
            is_auto_triggering = true;
            auto_trigger_interval = interval;
            ContinueAutoTrigger();
        }

        public void ContinueAutoTrigger()
        {
            if (!is_auto_triggering || _pre_run_index >= _evn_index)
            {
                StopAutoTrigger();
                return;
            }

            if (!pause_list[_pre_run_index])
            {
                TrgNextEvn();

                // 如果还有事件未触发，继续延迟触发
                if (_pre_run_index < _evn_index && is_auto_triggering)
                {
                    SendCustomEventDelayedSeconds(nameof(ContinueAutoTrigger), auto_trigger_interval);
                }
                else
                {
                    StopAutoTrigger();
                }
            }
            else
            {
                StopAutoTrigger();
            }
        }

        public void StopAutoTrigger()
        {
            is_auto_triggering = false;
        }

        public void PauseEvn()
        {
            if (_pre_run_index < _evn_index)
            {
                pause_list[_pre_run_index] = true;
            }
        }

        public void ResumeEvn()
        {
            if (_pre_run_index < _evn_index)
            {
                pause_list[_pre_run_index] = false;
            }
        }

        public void ResetEvn()
        {
            _pre_run_index = 0;
        }

        #region UI 部分
        public HopeUdonEvnUi hopeUdonEvnUi;
        /// <summary>
        /// 带实现
        /// </summary>
        /// <param name="evn_name"></param>
        public void ApiUIItemShowCmd(int idx, string s)
        {
            //LogMsg("ApiUIItemShowCmd: " + idx + " " + s);
            if (hopeUdonEvnUi != null)
            {
                hopeUdonEvnUi.ShowItemInfo(idx, s);
            }
        }

        public void ApiShowUiInfo(string s)
        {
            ;
        }

        public void ApiSendUiCmd(string s, object dat)
        {
            hopeUdonEvnUi.UiCmdRev(s, dat);
        }
        #endregion UI 部分
        // 触发接口
        public HopeTools.HopeUdonFramework hugf;
        public void ApiTrgCmdEvn(int evn_idx, string evn_name)
        {
            //LogMsg(evn_idx.ToString() + "---ApiTrgCmdEvn: " + evn_name);
            if (evn_name == null || evn_name.Length == 0)
            {
                LogMsg("ApiTrgCmdEvn: evn_name 为空");
                return;
            }

            var ss = evn_name.Split('|');
            var _name = ss[0];
            if (ss.Length == 1)
            {
                hugf.TriggerEvent(_name);
            }
            else if (ss.Length == 2)
            {
                var _dat1 = int.Parse(ss[1]);
                hugf.TriggerEventWithData(_name, _dat1);
            }
            else if (ss.Length == 3)
            {
                var _dat1 = int.Parse(ss[1]);
                var _dat2 = int.Parse(ss[2]);
                hugf.TriggerEventWith2Data(_name, _dat1, _dat2);
            }
            else
            {
                LogMsg("ApiTrgCmdEvn: " + evn_name + " 格式错误");
            }
        }

        public void EvnReRevCmd(string cmd , object param)
        {
            if (cmd == "TrgNextEvn")
            {
                TrgNextEvn();
            }
            else if (cmd == "ResetEvn")
            {
                ResetEvn();
            }
            else if (cmd == "PauseEvn")
            {
                PauseEvn();
            }
            else if (cmd == "ResumeEvn")
            {
                ResumeEvn();
            }
            else if (cmd == "AutoTigEvn")
            {
                AutoTigEvn();
            }
            else if (cmd == "StopAutoTrigger")
            {
                StopAutoTrigger();
            }
            else if (cmd == "StopReEvn")
            {
                StopReEvn();
            }
            else if (cmd == "StartReEvn")
            {
                StartReEvn();
            }
        }

        public void LogMsg(string msg)
        {
            Debug.Log(msg);
        }

        public string GetAllEvn()
        {
            var s = "";
            for (int i = 0; i < _evn_index; i++)
            {
                s += evn_list[i] + "\n";
            }
            return s;
        }

        private string[] _all_evn_ss;
        private int _all_evn_idx;
        private float _last_exit_time;
        private int _line_count = 2;
        public void AddLoop()
        {
            var _add_limit = _line_count;

            while (_all_evn_idx < _all_evn_ss.Length)
            {
                if (_line_count > 0)
                {
                    var evn = _all_evn_ss[_all_evn_idx].Trim();
                    if (evn.Length > 0)
                        AddEvnString(evn);
                    _all_evn_idx++;
                    _add_limit--;
                }
                if (_add_limit <= 0)
                {
                    // 33 frames per second, 1 frame 大约 0.03 秒
                    if (Time.time - _last_exit_time > 0.03f)
                    {
                        _line_count--;
                    }
                    else
                    {
                        _line_count++;
                    }
                    _last_exit_time = Time.time;
                    this.SendCustomEventDelayedFrames(nameof(AddLoop), 1);
                    return;
                }
            }
        }

        public void SetAllEvn(string all_evn_str)
        {
            ClearEvn();
            _all_evn_ss = all_evn_str.Split('\n');
            _all_evn_idx = 0;
            AddLoop();
        }
    }
}







