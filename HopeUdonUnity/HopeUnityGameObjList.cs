
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace HopeTools
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]

    public class HopeUnityGameObjList : UdonSharpBehaviour
    {
        public const int max_item_num = 50;
        public GameObject _list_item_pre; // perfab for list item
        public GameObject _list_item_offeset; // perfab for list item

        private Vector3 item_p_offset;
        private Vector3 item_p_base;


        private GameObject[] itemgameObjList;
        private Transform[] itemManagerTfList;

        private bool[] _toggle_is_on_name_list;
        private int[] _prt_idx_list;

        public Transform managed_root_list;
        private bool _is_init = false;
        public int item_show_num = 0;
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


            if(_list_item_offeset == null)
            {
                _list_item_offeset = _list_item_pre.transform.parent.GetChild(1).gameObject;
            }

            var offset = _list_item_offeset.transform.localPosition -_list_item_pre.transform.localPosition;
            var prt = _list_item_pre.transform.parent;
            item_p_offset = offset;
            item_p_base = _list_item_pre.transform.localPosition;

            for (int i = 0; i < itemgameObjList.Length; i++)
            {
                itemgameObjList[i] = Instantiate(_list_item_pre, prt);
                itemgameObjList[i].SetActive(false);
                itemgameObjList[i].transform.localPosition = item_p_base + item_p_offset * i;
            }

            Destroy(_list_item_offeset);
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
                if (_toggle_is_on_name_list[i] != itemgameObjList[i].transform.GetChild(1).GetComponent<UnityEngine.UI.Toggle>().isOn)
                {
                    _toggle_is_on_name_list[i] = itemgameObjList[i].transform.GetChild(1).GetComponent<UnityEngine.UI.Toggle>().isOn;
                    LogMessge("OnToggleValueChanged_Name " + i + " " + itemgameObjList[i].name);
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

            if (itemgameObjList[idx].transform.GetChild(0).GetComponent<UnityEngine.UI.Toggle>().isOn == false)
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
                    itemgameObjList[i].transform.localPosition = item_p_base + item_p_offset * _show_idx;
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

            var s = new string(' ', depth * 4) + tf.name;
            this.itemgameObjList[this.item_show_num].SetActive(true);
            this.itemgameObjList[this.item_show_num].name = s;
            this.itemgameObjList[this.item_show_num].transform.GetChild(1).GetChild(0).GetComponent<UnityEngine.UI.Text>().text = s;
            this._prt_idx_list[this.item_show_num] = prt_idx;
            this.itemManagerTfList[this.item_show_num] = tf;

            prt_idx = this.item_show_num;
            this.item_show_num++;

            if (tf.childCount == 0)
            {
                this.itemgameObjList[prt_idx].transform.GetChild(0).gameObject.SetActive(false);
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
