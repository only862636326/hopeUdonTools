
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDK3.StringLoading;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;
using static VRC.Core.ApiInfoPushSystem;


namespace HopeTools
{
    public class UrlSynSys : UdonSharpBehaviour
    {
        public const int URL_INDEX_GET_SYN_ID = 1;
        public const int MAX_SYN_DATA_LEN = 80;

        public const int MASK_SYN_ID = 0x07 << 12;
        public const int MASK_SYN_INDEX = 0x3f << 6;
        public const int MASK_SYN_DATA = 0x3f;

        [HideInInspector] public VRCUrl[] urls_list;
        public string ApiBase = "http://127.0.0.1:5000";
        public string info_string;
        public int[] syn_data;
        public int[] loc_data;

        public int _syn_id = 0;
        public bool _is_syn= false;
        public int syn_data_len = 0;
        private bool _is_init = false;
        public long syn_mask_suc = 0;
        public void Init()
        {
            if(!_is_init)
            {
                _is_init = true;
                syn_data = new int[MAX_SYN_DATA_LEN + 2];
                loc_data = new int[MAX_SYN_DATA_LEN + 2];
            }
        }

        void Start()
        {
            ;
        }

        private void OnServerInitialized()
        {
            
        }

        void Update()
        {
            //if(Input.GetKeyDown(KeyCode.P))
            //{
            //    RequestSynId();
            //}
        }


        void StartSynDdata(string base64_data)
        {
            ;
        }


        void SenSynData(int[] x, int l)
        {
            if(!_is_syn) 
                return;
            if(l > MAX_SYN_DATA_LEN)
                return;

            for(int i = 0; i < x.Length; i++)
            {
                loc_data[i + 2] = x[i];
            }
            this._is_syn = true;
            RequestSynId();
        }


        int EncodeData(int id, int index, int data)
        {
            return (id << 12) | (index << 6) | data;
        }

        Vector3Int DecodeData(int data)
        {
            return new Vector3Int(data >> 12, (data >> 6) & 0x3f, data & 0x3f);
        }

        void RequestSynId()
        {
            VRCStringDownloader.LoadUrl(urls_list[EncodeData(0x07, 0x3f, 0x3f)], (IUdonEventReceiver)this);
        }

        public override void OnStringLoadSuccess(IVRCStringDownload result)
        {
            Debug.Log("OnStringLoadSuccess:" + result.Result);
            
            var s = result.Result;
            var s_list = s.Split(',');
            if (s_list[0] == "synid")
            {
                this._syn_id = int.Parse(s_list[1]);
                this.syn_data[0] = EncodeData(0x07, 0x3f, this._syn_id);
                this.syn_data[1] = EncodeData(this._syn_id, 0x3f, this.syn_data_len);
                for (int i = 0; i < this.syn_data_len; i++)
                {
                    this.syn_data[i + 2] = (this._syn_id << 12) | (i << 6) | this.loc_data[i];
                }
                this.syn_mask_suc = 0;
            }


        }

        public override void OnStringLoadError(IVRCStringDownload result)
        {
            Debug.Log("OnStringLoadError:" + result.Error);
        }
    }
}
