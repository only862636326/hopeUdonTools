
using HopeMahjong;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace HuPad
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class HuPadCtr : UdonSharpBehaviour
    {
        BoxCollider[] all_box_list;
        #region init code

        private void Start()
        {
            InitAppCtr();
            Init();
        }
        private bool _is_init = false;
        public void Init()
        {
            if (this._is_init)
                return;
            this._is_init = true;

            all_box_list = GetComponentsInChildren<BoxCollider>();
            
            foreach (var item in all_box_list)
            {
                //item.enabled = false;
            }
            this.GetComponent<BoxCollider>().enabled = true;
        }

#if false
        public HopeTools.HopeUdonFramework hugf;
        public object eventData;
        public object eventData1;
        public object eventData2;

        public void HugfInitAfter()
        {
            // user code after hugf init here
            //hugf.udonIoc.RegisterSingleton("IocDemoTar", this, this); // IOC
            //hugf.udonIoc.RegisterSingleton(nameof(HuPadCtr), this, this); // IOC
            //hugf.udonEvn.RegisterListener(nameof(this.DemoFunCall), this); // EVN
            //hugf.udonEvn.RegisterListener(nameof(this.DemoEvnHelp), this); // EVN
        }

        HopeMahjongInput _input;
        public void HufgIocGet()
        {
            //var ioc = (HuPadCtr)hugf.udonIoc.GetServiceUdon(nameof(HuPadCtr));
            //hugf.TriggerEventWithData(nameof(HopeTools.HopeUdonEvnHelp.OnHugfAddEvnCall), nameof(this.DemoEvnHelp));

           
        }
#endif

#endregion end init code
        private void Update()
        {
            _UpdatePingTime();
            _UpdateAppAnim();
        }

        private void _UpdateAppAnim()
        {
            if (_anim_dir == 0 || _anim_app_idx < 0)
                return;
            if (_anim_app_idx >= appPrt.childCount)
                return;

            Transform appTf = appPrt.GetChild(_anim_app_idx);

            if (_anim_dir == 1) // 打开
            {
                _anim_t += Time.deltaTime * _anim_speed;
                if (_anim_t >= 1f)
                {
                    _anim_t = 1f;
                    // 动画结束，设置最终状态
                    appTf.position = _app_orig_world_pos[_anim_app_idx];
                    appTf.localScale = _app_orig_local_scale[_anim_app_idx];
                    _anim_dir = 0;
                    return;
                }
                float t = Mathf.SmoothStep(0f, 1f, _anim_t);
                appTf.position = Vector3.Lerp(
                    _anim_icon_world_pos,
                    _app_orig_world_pos[_anim_app_idx], t);
                appTf.localScale = Vector3.Lerp(Vector3.zero, _app_orig_local_scale[_anim_app_idx], t);
            }

            else if (_anim_dir == -1) // 关闭
            {
                _anim_t -= Time.deltaTime * _anim_speed;
                if (_anim_t <= 0f)
                {
                    _anim_t = 0f;
                    // 动画结束，隐藏
                    appTf.position = _anim_icon_world_pos;
                    appTf.localScale = Vector3.zero;
                    appTf.gameObject.SetActive(false);
                    _anim_dir = 0;
                    return;
                }
                float t = Mathf.SmoothStep(0f, 1f, _anim_t);
                appTf.position = Vector3.Lerp(
                    _anim_icon_world_pos,
                    _app_orig_world_pos[_anim_app_idx], t);
                appTf.localScale = Vector3.Lerp(Vector3.zero, _app_orig_local_scale[_anim_app_idx], t);
            }
        }
        public void OpenClosePad()
        {
            var tf = this.transform.Find("AppContrl");
            for (int i = 0; i < tf.childCount; i++)
            {
                tf.GetChild(i).gameObject.SetActive(false);
            }
        }

        private Transform appIconPrt;
        private Transform appPrt;

        private bool _is_app_init = false;
        private bool[] _toggle_list;
        private Toggle[] _toggle_btn_list;

        // 动画相关
        private Vector3[] _app_orig_world_pos;
        private Vector3[] _app_orig_local_scale;
        private Vector3[] _app_icon_world_pos;
        private int _anim_app_idx = -1;
        private Vector3 _anim_icon_world_pos;
        private float _anim_t = 0f;
        private int _anim_dir = 0; // 0=none, 1=打开, -1=关闭
        [SerializeField] private float _anim_speed = 2f;

        private void InitAppCtr()
        {
            if(!this._is_app_init)
            {
                this._is_app_init = true;
                appIconPrt = this.transform.Find("AppIconPrt");
                appPrt = this.transform.Find("AppPrt");
                if(appIconPrt != null && appPrt != null)
                {
                    int iconCount = appIconPrt.childCount;
                    int appCount = appPrt.childCount;

                    _toggle_list = new bool[iconCount];
                    _toggle_btn_list = new Toggle[iconCount];

                    // 记录动画数据
                    _app_orig_world_pos = new Vector3[appCount];
                    _app_orig_local_scale = new Vector3[appCount];
                    _app_icon_world_pos = new Vector3[iconCount];

                    // 初始化应用图标
                    for (int i = 0; i < iconCount; i++)
                    {
                        _toggle_btn_list[i] = appIconPrt.GetChild(i).GetComponentInChildren<Toggle>();
                        _toggle_list[i] = _toggle_btn_list[i].isOn;
                        _app_icon_world_pos[i] = appIconPrt.GetChild(i).position;
                    }

                    for(int i = 0; i < appCount; i++)
                    {
                        Transform appTf = appPrt.GetChild(i);
                        _app_orig_world_pos[i] = appTf.position;
                        _app_orig_local_scale[i] = appTf.localScale;
                        appTf.gameObject.SetActive(false);
                    }
                }
            }
        }
        public void ToggleEvn_HuPadAppClick()
        {
            if(!_is_app_init)
                return;

            if(_toggle_btn_list == null)
                return;

            for (int i = 1; i < appIconPrt.childCount; i++)
            {
                if (_toggle_list[i] != _toggle_btn_list[i].isOn)
                {
                    _toggle_list[i] = _toggle_btn_list[i].isOn;
                    _SnapCloseCurrentAnimApp();
                    _StartOpenAnim(i);
                    break;
                }
            }
        }

        // 根据icon索引找到对应app索引（按名称匹配）
        private int _FindAppIdxByIconIdx(int iconIdx)
        {
            if (iconIdx < 0 || iconIdx >= appIconPrt.childCount)
                return -1;
            string appName = appIconPrt.GetChild(iconIdx).name;
            for (int i = 0; i < appPrt.childCount; i++)
            {
                if (appPrt.GetChild(i).name == appName)
                    return i;
            }
            return -1;
        }

        // 根据app索引找到对应icon索引（按名称匹配）
        private int _FindIconIdxByAppIdx(int appIdx)
        {
            if (appIdx < 0 || appIdx >= appPrt.childCount)
                return -1;
            string appName = appPrt.GetChild(appIdx).name;
            for (int i = 0; i < appIconPrt.childCount; i++)
            {
                if (appIconPrt.GetChild(i).name == appName)
                    return i;
            }
            return -1;
        }

        // 立即关闭当前正在动画的app（不播放动画）
        private void _SnapCloseCurrentAnimApp()
        {
            if (_anim_dir != 0 && _anim_app_idx >= 0 && _anim_app_idx < appPrt.childCount)
            {
                Transform appTf = appPrt.GetChild(_anim_app_idx);
                appTf.position = _anim_icon_world_pos;
                appTf.localScale = Vector3.zero;
                appTf.gameObject.SetActive(false);
            }
            _anim_dir = 0;
            _anim_t = 0f;
            _anim_app_idx = -1;

            // 同时关闭其他可能还active的app
            if (appPrt != null)
            {
                for (int i = 0; i < appPrt.childCount; i++)
                {
                    if (appPrt.GetChild(i).gameObject.activeSelf)
                    {
                        appPrt.GetChild(i).gameObject.SetActive(false);
                    }
                }
            }
        }

        // 开始打开动画
        private void _StartOpenAnim(int iconIdx)
        {
            int appIdx = _FindAppIdxByIconIdx(iconIdx);
            if (appIdx < 0 || appIdx >= appPrt.childCount)
                return;

            Transform appTf = appPrt.GetChild(appIdx);
            appTf.gameObject.SetActive(true);
            _anim_icon_world_pos = _app_icon_world_pos[iconIdx];
            appTf.position = _anim_icon_world_pos;
            appTf.localScale = Vector3.zero;

            _anim_app_idx = appIdx;
            _anim_t = 0f;
            _anim_dir = 1;
        }

        // 开始关闭动画（通过icon索引）
        private void _StartCloseAnimByIconIdx(int iconIdx)
        {
            int appIdx = _FindAppIdxByIconIdx(iconIdx);
            if (appIdx < 0 || appIdx >= appPrt.childCount)
                return;

            Transform appTf = appPrt.GetChild(appIdx);
            if (!appTf.gameObject.activeSelf)
                return;

            _anim_icon_world_pos = _app_icon_world_pos[iconIdx];
            _anim_app_idx = appIdx;
            _anim_dir = -1;
            // _anim_t 保持当前值，从当前位置反向动画
        }
 
        public void  ToggleEvn_HuPadBottomClick()
        {
            if(!_is_app_init)
                return;
            if(appPrt == null)
                return;

            // 如果正在动画中，启动关闭动画
            if (_anim_dir != 0 && _anim_app_idx >= 0 && _anim_app_idx < appPrt.childCount)
            {
                _anim_dir = -1;
                return;
            }

            // 查找当前active的app并关闭
            for(int i = 0; i < appPrt.childCount; i++)
            {
                if(appPrt.GetChild(i).gameObject.activeSelf)
                {
                    int iconIdx = _FindIconIdxByAppIdx(i);
                    if (iconIdx >= 0)
                        _anim_icon_world_pos = _app_icon_world_pos[iconIdx];
                    _anim_app_idx = i;
                    _anim_t = 1f;
                    _anim_dir = -1;
                    return;
                }
            }
        }

        #region 屏保时间显示　
        public Text _ping_time;
        public Text _pong_date;
        
        private void _UpdatePingTime()
        {
            if(!_ping_time.gameObject.activeSelf)
                return;

            if (_ping_time == null || _pong_date == null)
                return;
            // 更新时间
            _ping_time.text = System.DateTime.Now.ToString("HH:mm:ss");
            // 更新日期 星期
            _pong_date.text = System.DateTime.Now.ToString("yyyy-MM-dd dddd");
        }
        #endregion　屏保时间显示　
    }
}





