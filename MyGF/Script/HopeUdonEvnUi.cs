
using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace HopeTools
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class HopeUdonEvnUi : UdonSharpBehaviour
    {
        public const int CONFIG_DEFAULT_LIST_SIZE = 50;
        public const int CONFIG_MAX_ITEM_COUNT = 1000;
        public GameObject item_perferb;
        private Transform[] item_list;

        void Start()
        {
            item_perferb.SetActive(false);
            item_list = new Transform[CONFIG_DEFAULT_LIST_SIZE];

        }

        public void Update()
        {
            // shift + F5 reset / run   
            if (Input.GetKeyDown(KeyCode.F5))
            {
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                {
                    ToggleEvn_Stop();
                }
                else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                {
                    ToggleEvn_Reset();
                }

                else
                {
                    ToggleEvn_Run();
                }
            }

            if (Input.GetKeyDown(KeyCode.F11))
            {
                ToggleEvn_Next();
            }
        }

        public void ExpandArrays()
        {
            int newSize = Mathf.Max(item_list.Length * 2, CONFIG_DEFAULT_LIST_SIZE);
            Transform[] newItemList = new Transform[newSize];
            for (int i = 0; i < item_list.Length; i++)
            {
                newItemList[i] = item_list[i];
            }
            item_list = newItemList;
        }

        public void UiCmdRev(string cmd, object param)
        {
            switch (cmd)
            {
                case "clear_all":
                case "ClearEvn":
                    ClearAllItemInfo();
                    break;
                case "SetPreEvn":
                    if (param.GetType() == typeof(int))
                    {
                        SetPreEvn((int)param, "");
                    }
                    else if (param.GetType() == typeof(string))
                    {
                        if (int.TryParse((string)param, out int idx))
                        {
                            SetPreEvn(idx, "");
                        }
                    }
                    break;
                default:
                    break;
            }
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
                ExpandArrays();
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

        string[] evn_list;
        public void ShowItemInfoV2(int idx, string _evn)
        {
            ;
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

        [HideInInspector] public HopeTools.HopeUdonFramework hugf;
        public HopeTools.HopeUdonEvnRe udonEvnRe;
        // start method
        public void ToggleEvn_Next()
        {
            //hugf.Log("ToggleEvn_Next");
            udonEvnRe.TrgNextEvn();
        }

        public void ToggleEvn_Run()
        {
            //hugf.Log("ToggleEvn_Run");
            udonEvnRe.AutoTigEvn();
        }
        public void ToggleEvn_Stop()
        {
            //hugf.Log("ToggleEvn_Stop");
            udonEvnRe.StopAutoTrigger();
        }
        public void ToggleEvn_Clear()
        {
            //hugf.Log("ToggleEvn_Clear");
            udonEvnRe.ClearEvn();
        }
        public void ToggleEvn_Reset()
        {
            //hugf.Log("ToggleEvn_Reset");
            udonEvnRe.SetTrg(0);
        }
        public void ToggleEvn_ResetRun()
        {
            //hugf.Log("ToggleEvn_ResetRun"); 
            ToggleEvn_Reset();
            ToggleEvn_Run();
        }

        public InputField evn_all_inputfield;
        public void ToggleEvn_GetAllEvn()
        {
            //hugf.Log("ToggleEvn_GetAllEvn");
            if (evn_all_inputfield == null)
                return;
            var s = udonEvnRe.GetAllEvn();
            evn_all_inputfield.text = s;
        }
        public void ToggleEvn_SetAllEvn()
        {
            if (evn_all_inputfield == null)
                return;
            //hugf.Log("ToggleEvn_SetAllEvn");
            udonEvnRe.SetAllEvn(evn_all_inputfield.text);
        }

        public void ToggleEvn_SetPre()
        {
            //hugf.Log("ToggleEvn_SetPre");
        }
        public void ToggleEvn_SelectIdx()
        {
            //hugf.Log("ToggleEvn_SelectIdx"); 
        }
        public void ToggleEvn_RunIdx()
        {
            //hugf.Log("ToggleEvn_RunIdx"); 
        }
        public void ToggleEvn_SetStop()
        {
            //hugf.Log("ToggleEvn_SetStop"); 
        }

        public void ToggleEvn_StartRe()
        {
            udonEvnRe.EvnReRevCmd("StartReEvn", null);
        }
        public void ToggleEvn_StopRe()
        {
            udonEvnRe.EvnReRevCmd("StopReEvn", null);
        }

        // end method


        public void LogMsg(string msg)
        {
            Debug.Log(msg);
        }

        public void SetPreEvn(int evn_idx, string evn_name)
        {
            var tf = this.item_list[evn_idx];
            if (tf != null)
            {
                tf.Find("ToggleEvn_SetPre").GetComponent<Toggle>().isOn = true;
            }
        }
    }
}





