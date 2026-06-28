
using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace HopeTools
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class HopeUdonEvnHelp : UdonSharpBehaviour
    {
        #region evn 存储
        public const int CONFIG_DEFAULT_EVN_LIST_SIZE = 100;
        public string[] evn_list;
        private int _evn_index = 0;

        void Start()
        {

        }
        
        private bool _is_init = false;
        public void Init()
        {
            if (_is_init)
            {
                return;                
            }
            _is_init = true;
            evn_list = new string[CONFIG_DEFAULT_EVN_LIST_SIZE];
            item_list = new Transform[CONFIG_DEFAULT_UI_LIST_SIZE];
            
            item_perferb.SetActive(false);
            ClearEvn();
        }
        public HopeTools.HopeUdonFramework hugf;
        public object eventData;
        public object eventData1;
        public object eventData2;
        public void HugfInitAfter()
        {
            // user code after hugf init here
            hugf.udonEvn.RegisterListener(nameof(this.OnHugfAddEvnCall), this);
        }

        public void HufgIocGet()
        {
            ;
        }

        public void ClearEvn()
        {
            _evn_index = 0;
            ClearAllItemInfo();
        }

        #endregion evn 存储

        #region add evn string

        public void OnHugfAddEvnCall()
        {
            var _name = eventData.ToString();
            AddEvnString(_name);
        }

        /// <summary>
        /// 扩容事件数组
        /// </summary>
        private void ExpandEvnArrays()
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
        }

        public void AddEvnString(string s)
        {
            // 检查是否需要扩容
            if (_evn_index >= evn_list.Length)
            {
                ExpandEvnArrays();
            }
            evn_list[_evn_index] = s;
            ShowItemInfo(_evn_index, s);
            _evn_index++;
            LogMsg("AddEvnString: " + s);
        }

        public void RemoveEvnAt(int idx)
        {
            if (idx < 0 || idx >= _evn_index)
            {
                return;
            }

            // 将后续元素前移
            for (int i = idx; i < _evn_index - 1; i++)
            {
                evn_list[i] = evn_list[i + 1];
                ShowItemInfo(i, evn_list[i]);
            }
            evn_list[_evn_index - 1] = null;
            _evn_index--;

            // 通知UI该项被移除，后续项前移
            SetNextItemDisable(idx - 1);
        }

        // 目前只支持两个数据参数, 并且都要为int
        public void AddEvn(string evn_name)
        {
            AddEvnString(evn_name);
        }

        public void AddEvnWithDat(string evn_name, object data)
        {
            AddEvnString(evn_name + "|" + data.ToString());
        }

        public void AddEvnWith2Dat(string evn_name, object data1, object data2)
        {
            AddEvnString(evn_name + "|" + data1.ToString() + "|" + data2.ToString());
        }
        #endregion add evn string

        #region 事件触发
        /// <summary>
        /// 切换事件运行索引
        /// </summary>
        /// 

        private bool _is_forbig = false;
        public void EnRunEvn()
        {
            _is_forbig = false;
        }

        public void ToggleEvn_RunIdx()
        {
            if(this._is_forbig)
            {
                this.SendCustomEventDelayedSeconds(nameof(EnRunEvn), 0.3f);
                return;
            }
            this._is_forbig = true;
            this.SendCustomEventDelayedSeconds(nameof(EnRunEvn), 0.3f);

            for (int i = 0; i < item_list.Length; i++)
            {
                if(item_list[i] == null)
                {
                    return;
                }
               var tog = item_list[i].Find("ToggleEvn_RunIdx").GetComponent<Toggle>();
               if(tog != null && tog.isOn)
               {
                   HelpTrgCmdEvn(i);
               }
            }


            LogMsg("ToggleEvn_RunIdx");
        }

        public void HelpTrgCmdEvn(int evn_idx)
        {
            //LogMsg(evn_idx.ToString() + "---HelpTrgCmdEvn: " + evn_name);
            if (evn_idx < 0 || evn_idx >= _evn_index)
            {
                LogMsg("HelpTrgCmdEvn: evn_idx 无效");
                return;
            }

            string evn_name = evn_list[evn_idx];
            if (evn_name == null || evn_name.Length == 0)
            {
                LogMsg("HelpTrgCmdEvn: evn_name 为空");
                return;
            }

            string[] ss = evn_name.Split('|');
            string _name = ss[0];

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
        #endregion 事件触发

        #region UI 字段
        public const int CONFIG_DEFAULT_UI_LIST_SIZE = 100;
        public const int CONFIG_MAX_ITEM_COUNT = 100;
        public GameObject item_perferb;
        public Transform[] item_list;

        private void ExpandUIArrays()
        {
            int newSize = Mathf.Max(item_list.Length * 2, CONFIG_DEFAULT_UI_LIST_SIZE);
            Transform[] newItemList = new Transform[newSize];
            for (int i = 0; i < item_list.Length; i++)
            {
                newItemList[i] = item_list[i];
            }
            item_list = newItemList;
        }

        public void ClearAllItemInfo()
        {
            for (int i = 0; i < item_list.Length; i++)
            {
                if (item_list[i] != null)
                {
                    item_list[i].gameObject.SetActive(false);
                }
                else
                {
                    return;
                }
            }
        }

        public void ShowItemInfo(int idx, string _evn)
        {
            if (idx < 0 || idx >= CONFIG_MAX_ITEM_COUNT)
            {
                return;
            }
            if (idx >= item_list.Length)
            {
                ExpandUIArrays();
            }
            if (item_list[idx] == null)
            {
                Transform new_item = Instantiate(item_perferb).transform;
                new_item.SetParent(item_perferb.transform.parent, false);
                item_list[idx] = new_item;
            }
            item_list[idx].GetChild(0).GetChild(0).GetComponent<Text>().text = idx.ToString();
            item_list[idx].GetChild(1).GetComponent<InputField>().text = _evn;
            item_list[idx].gameObject.SetActive(true);
        }

        public void SetNextItemDisable(int idx)
        {
            if (idx < 0 || idx >= CONFIG_MAX_ITEM_COUNT)
            {
                return;
            }
            if (idx >= item_list.Length)
            {
                return;
            }
            for (int i = idx + 1; i < item_list.Length; i++)
            {
                if (item_list[i] == null || !item_list[i].gameObject.activeSelf)
                {
                    break;
                }
                item_list[i].gameObject.SetActive(false);
            }
        }
        #endregion UI 部分

        public void LogMsg(string msg)
        {
            Debug.Log(msg);
        }
    }
}

