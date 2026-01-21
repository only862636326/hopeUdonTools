
using System.Linq;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace HopeTools
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]

    public class HopeUnityGameObjList : UdonSharpBehaviour
    {
        public const int CONFIG_TILE_TABLE_SIZE = 20;
        public const string CONFIG_STRING_ICON = "ToggleIcom";
        public const string CONFIG_STRING_NAME = "ToggleName";

        public const int max_item_num = 50;
        public GameObject _list_item_pre; // perfab for list item

        private Transform[] itemgameObjList;
        [SerializeField] private Transform[] itemManagerTfList;
        
        private Toggle[] _toggle_name_list;
        private Toggle[] _toggle_icon_list;
        private bool[] _toggle_is_on_name_list;
        private int[] _prt_idx_list;

        public Transform managed_root;
        public Transform active_tf;
        private bool _is_init = false;
        public int item_show_num = 0;

        private Vector3 icon_p_base;
        private Vector3 name_p_base;

        public HopeUnityGameObjCompnet hopeUnityGameObj;

        // 初始化
        void Start()
        {
            this.SendCustomEventDelayedFrames(nameof(this.Init), 1);
        }
        public void Init()
        {
            if (this._is_init)
                return;
            this._is_init = true;

            // 初始化 itemgameObjList
            this.itemgameObjList = new Transform[max_item_num];
            _toggle_is_on_name_list = new bool[max_item_num];
            _prt_idx_list = new int[max_item_num];
            itemManagerTfList = new Transform[max_item_num];
            _toggle_name_list = new Toggle[max_item_num];
            _toggle_icon_list = new Toggle[max_item_num];

            var prt = _list_item_pre.transform.parent;

            for (int i = 0; i < itemgameObjList.Length; i++)
            {
                itemgameObjList[i] = Instantiate(_list_item_pre, prt).transform;
                itemgameObjList[i].gameObject.SetActive(false);
                //itemManagerTfList[i] = itemgameObjList[i].transform;
                _toggle_name_list[i] = itemgameObjList[i].transform.Find(CONFIG_STRING_NAME).GetComponent<UnityEngine.UI.Toggle>();
                _toggle_icon_list[i] = itemgameObjList[i].transform.Find(CONFIG_STRING_ICON).GetComponent<UnityEngine.UI.Toggle>();
                _toggle_name_list[i].isOn = false;
            }

            name_p_base = itemgameObjList[0].transform.Find(CONFIG_STRING_NAME).localPosition;
            icon_p_base = itemgameObjList[0].transform.Find(CONFIG_STRING_ICON).localPosition;

            Destroy(_list_item_pre);
            UpdateItemList();

            _toggle_name_list[0].isOn = true;
            _toggle_is_on_name_list[0] = true;
            itemgameObjList[0].GetChild(1).gameObject.SetActive(true);
        }

        void Update()
        {
            ;
        }

        private void SetAcode()
        {
            ;
        }

        private int _pre_active_idx = 0;
        public void OnToggleValueChanged_Name()
        {
            if (_toggle_is_on_name_list[_pre_active_idx] != _toggle_name_list[_pre_active_idx].isOn)
            {
                _toggle_is_on_name_list[_pre_active_idx] = _toggle_name_list[_pre_active_idx].isOn;
                itemgameObjList[_pre_active_idx].GetChild(1).gameObject.SetActive(true);
                return;
            }
            for (int i = 0; i < item_show_num; i++)
            {
                if (_toggle_name_list[i].isOn != _toggle_is_on_name_list[i])
                {
                    itemgameObjList[_pre_active_idx].GetChild(1).gameObject.SetActive(false);
                    _pre_active_idx = i;
                    itemgameObjList[_pre_active_idx].GetChild(1).gameObject.SetActive(true);

                    _toggle_name_list[_pre_active_idx].isOn = false;
                    this.active_tf = itemManagerTfList[_pre_active_idx];
                    if (hopeUnityGameObj != null)
                        hopeUnityGameObj.InitActiveTf(this.active_tf);
                    LogMessge("OnToggleValueChanged_Name " + _pre_active_idx);
                    break;
                }
            }
        }

        [RecursiveMethod]
        public bool CheckIsActive(int idx)
        {
            if (idx == -1)
            {
                return true;
            }

            if (_toggle_icon_list[idx].isOn == false)
            {
                return false;
            }
            else
            {
                return CheckIsActive(_prt_idx_list[idx]);
            }
        }

        public void OnToggleValueChanged_Icon()
        {
            LogMessge("OnToggleValueChanged_Icon ");
            var _show_idx = 0;
            for(int i = 0; i < item_show_num; i++)
            {
                if (CheckIsActive(_prt_idx_list[i]))
                {
                    itemgameObjList[i].gameObject.SetActive(true);
                    _show_idx++;
                }
                else
                {
                    itemgameObjList[i].gameObject.SetActive(false);
                }
            }
        }

        public void UpdateItemList()
        {
            this.item_show_num = 0;
            AddChildToManagedList(managed_root, 0, -1);
        }

        [RecursiveMethod]
        public void AddChildToManagedList(Transform tf, int depth, int prt_idx)
        {
            if (tf == null || this.item_show_num >= this.itemgameObjList.Length - 1)
                return;

            var s = "  " + tf.name;
            var item = this.itemgameObjList[this.item_show_num];
            
            item.gameObject.SetActive(true);
            item.name = s;
            var text = item.transform.Find(CONFIG_STRING_NAME).GetChild(0).GetComponent<UnityEngine.UI.Text>();
            text.text = s;
            if (tf.gameObject.activeInHierarchy == false)
            {
                text.color = new Color(0.6f, 0.6f, 0.6f, 1.0f);
            }
            else
            {
                text.color = Color.black;
            }
            
            this._prt_idx_list[this.item_show_num] = prt_idx;
            this.itemManagerTfList[this.item_show_num] = tf;

            var toggle_icon = item.transform.Find(CONFIG_STRING_ICON);
            toggle_icon.localPosition = icon_p_base + new Vector3(depth * CONFIG_TILE_TABLE_SIZE, 0, 0);

            var toggle_name = item.transform.Find(CONFIG_STRING_NAME);
            toggle_name.localPosition = name_p_base + new Vector3(depth * CONFIG_TILE_TABLE_SIZE, 0, 0);

            prt_idx = this.item_show_num;
            this.item_show_num++;

            if (tf.childCount == 0)
            {
                item.transform.Find(CONFIG_STRING_ICON).gameObject.SetActive(false);
            }
            else
            {
                for (int i = 0; i < tf.childCount; i++)
                {
                    AddChildToManagedList(tf.GetChild(i), depth + 1, prt_idx);
                }
            }
        }

        public void LogMessge(object messge)
        {
            
            Debug.Log("                      [HopeUnityGameObjList] " + messge.ToString());
        }

    }
}
