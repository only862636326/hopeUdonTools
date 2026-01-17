
using UdonSharp;
using UnityEngine;
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

        private GameObject[] itemgameObjList;
        private Transform[] itemManagerTfList;

        private bool[] _toggle_is_on_name_list;
        private int[] _prt_idx_list;

        public Transform managed_root_list;
        private bool _is_init = false;
        public int item_show_num = 0;

        private Vector3 icon_p_base;
        private Vector3 name_p_base;

        // 初始化
        void Start()
        {
            Init();
        }
        public void Init()
        {
            if (this._is_init)
                return;
            this._is_init = true;

            // 初始化 itemgameObjList
            this.itemgameObjList = new GameObject[max_item_num];
            _toggle_is_on_name_list = new bool[max_item_num];
            _prt_idx_list = new int[max_item_num];
            itemManagerTfList = new Transform[max_item_num];

            var prt = _list_item_pre.transform.parent;


            for (int i = 0; i < itemgameObjList.Length; i++)
            {
                itemgameObjList[i] = Instantiate(_list_item_pre, prt);
                itemgameObjList[i].SetActive(false);
            }

            name_p_base = itemgameObjList[0].transform.Find(CONFIG_STRING_NAME).localPosition;
            icon_p_base = itemgameObjList[0].transform.Find(CONFIG_STRING_ICON).localPosition;

            Destroy(_list_item_pre);

            UpdateItemList();
        }

        void Update()
        {
            ;
        }

        private void SetAcode()
        {
            ;
        }

        public void OnToggleValueChanged_Name()
        {
            // LogMessge("OnToggleValueChanged_Name");
            for (int i = 0; i < item_show_num; i++)
            {
                if (_toggle_is_on_name_list[i] != itemgameObjList[i].transform.Find(CONFIG_STRING_NAME).GetComponent<UnityEngine.UI.Toggle>().isOn)
                {
                    _toggle_is_on_name_list[i] = itemgameObjList[i].transform.Find(CONFIG_STRING_NAME).GetComponent<UnityEngine.UI.Toggle>().isOn;
                    //LogMessge("OnToggleValueChanged_Name " + i + " " + itemgameObjList[i].name);
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

            if (itemgameObjList[idx].transform.Find("ToggleIcom").GetComponent<UnityEngine.UI.Toggle>().isOn == false)
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
                    itemgameObjList[i].SetActive(true);
                    _show_idx++;
                }
                else
                {
                    itemgameObjList[i].SetActive(false);
                }
            }
        }

        public void UpdateItemList()
        {
            this.item_show_num = 0;
            AddChildToManagedList(managed_root_list, 0, -1);
        }

        [RecursiveMethod]
        public void AddChildToManagedList(Transform tf, int depth, int prt_idx)
        {
            if (tf == null || this.item_show_num >= this.itemgameObjList.Length - 1)
                return;

            var s = "  " + tf.name;
            var item = this.itemgameObjList[this.item_show_num];
            
            item.SetActive(true);
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
