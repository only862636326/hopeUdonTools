using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;
using VRC.Udon.Common;

namespace HopeTools
{
    /// <summary>
    /// VR模式辅助: 手部跟随 + 射线检测牌(Toggle开关) + Use触发
    /// </summary>
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class HopeVrHelp : UdonSharpBehaviour
    {
        // 常量 — 事件名/前缀
        private const string EVN_VR_INPUT_ENTER = "VrInputEnter";
        private const string EVN_VR_INPUT_EXIT = "VrInputExit";
        private const string PREFIX_HIT_BOX = "HitBox";
        private const string PREFIX_TOGGLE_EVN = "ToggleEvn_";
        private const string EVN_VR_INPUT_TRG = "VrInputTrg";

        // 射线检测
        public float _front_len = 0.5f;
        LineRenderer _line_renderer;
        public int HIT_BOX_LAYER = 0;             // 牌HitBox所在Layer
        public int HIT_TOGGLE_LAYER = 17;          // Toggle按钮所在Layer

        // 配置
        public bool _is_right_hand = false;        // 当前跟随右手还是左手
        public bool _set_hit_box = false;          // 是否启用牌HitBox检测
        public bool _set_hit_toggle = false;       // 是否启用Toggle按钮检测
        public Transform _hit_icon_tf;             // 命中图标(准星)
        public BoxCollider[] _tar_canvas_box;       // 关联Canvas碰撞体列表, 跟随模式时禁用避免误触

        void Start()
        {
            // PC非VR用户隐藏该组件
#if !UNITY_EDITOR
            if (!Networking.LocalPlayer.IsUserInVR())
            {
                this.gameObject.SetActive(false);
            }
#endif
            _line_renderer = (LineRenderer)this.GetComponentInChildren(typeof(LineRenderer));
        }

        void Update()
        {
            // 自动跟随模式下, 每帧更新位置到手上
            if (_is_auto_flow)
            {
                GetHandData(_is_right_hand, out Vector3 p, out Quaternion r);
                this.transform.position = p + r * _position_offset;
                this.transform.rotation = r * _rotation_offset;
            }

            // 每帧统一射线检测
            HitCheckAll();
        }

        // ---------- 射线检测 ----------

        private bool _is_hit_box = false;         // 当前帧是否命中牌HitBox
        private bool _is_hit_toggle = false;       // 当前帧是否命中Toggle按钮
        private Transform _hit_history_tf;          // 上一次命中的HitBox, 用于切换时发Exit事件
        private UdonSharpBehaviour _udonBehaviour; // 命中牌HitBox上的U#脚本

        /// <summary>每帧分别检测牌HitBox和Toggle按钮, 互斥: HitBox优先</summary>
        private void HitCheckAll()
        {
            Vector3 head_p = this.transform.position;
            Vector3 forward = this.transform.rotation * Vector3.forward;
            _line_renderer.SetPosition(0, head_p);

            bool hitBox = false;
            bool hitToggle = false;

            // 检测牌HitBox
            if (_set_hit_box)
            {
                if (Physics.Raycast(head_p, forward, out RaycastHit hbHit, _front_len, 1 << HIT_BOX_LAYER)
                    && hbHit.transform.name.StartsWith(PREFIX_HIT_BOX))
                {
                    _hit_icon_tf.position = hbHit.point;
                    _hit_icon_tf.gameObject.SetActive(true);
                    _line_renderer.SetPosition(1, hbHit.point);
                    ProcessHitBox(hbHit);
                    hitBox = true;
                }
            }

            // 仅HitBox未命中时才检测Toggle
            if (!hitBox)
            {
                if (_set_hit_toggle)
                {
                    if (Physics.Raycast(head_p, forward, out RaycastHit tgHit, _front_len, 1 << HIT_TOGGLE_LAYER))
                    {
                        if (tgHit.transform.name.StartsWith(PREFIX_TOGGLE_EVN))
                        {
                            _hit_icon_tf.position = tgHit.point;
                            _line_renderer.SetPosition(1, tgHit.point);
                            _hit_icon_tf.gameObject.SetActive(true);
                            ProcessHitToggle(tgHit);
                            hitToggle = true;
                        }
                    }
                }

                // HitBox未命中时清除其状态(包含从HitBox→Toggle过渡)
                if (!hitBox)
                    ClearHitBox();
            }

            // Toggle未命中时清除其状态
            if (!hitToggle)
                _is_hit_toggle = false;

            // 未命中任何目标, 画线到头 + 隐藏准星
            if (!hitBox && !hitToggle)
            {
                _line_renderer.SetPosition(1, head_p + forward * _front_len);
                _hit_icon_tf.gameObject.SetActive(false);
            }
        }

        /// <summary>命中牌HitBox: 切换目标时发Exit/Enter事件</summary>
        private void ProcessHitBox(RaycastHit hit)
        {
            if (_hit_history_tf != hit.transform)
            {
                if (_udonBehaviour != null)
                    _udonBehaviour.SendCustomEvent(EVN_VR_INPUT_EXIT);
                _hit_history_tf = hit.transform;
                _udonBehaviour = (UdonSharpBehaviour)hit.transform.GetComponent(typeof(UdonSharpBehaviour));
                if (_udonBehaviour != null)
                    _udonBehaviour.SendCustomEvent(EVN_VR_INPUT_ENTER);
            }
            _is_hit_box = true;
            _is_hit_toggle = false;

        }

        /// <summary>退出牌HitBox命中状态, 发Exit事件</summary>
        private void ClearHitBox()
        {
            _is_hit_box = false;
            _hit_history_tf = null;
            if (_udonBehaviour != null)
            {
                _udonBehaviour.SendCustomEvent(EVN_VR_INPUT_EXIT);
                _udonBehaviour = null;
            }
        }

        /// <summary>命中Toggle按钮</summary>
        private Transform _toggle_hit_history_tf = null;
        private void ProcessHitToggle(RaycastHit hit)
        {
            _toggle_hit_history_tf = hit.transform;
            _is_hit_toggle = true;
            _is_hit_box = false;
        }

        #region 自动跟随模式
        // ---------- 自动跟随模式 ----------

        private bool _is_auto_flow = false;        // 是否处于自动跟随状态
        private bool _is_use_check_task = false;   // 是否有延迟检测任务进行中
        private int _interact_times;               // 连续Use次数计数
        private Vector3 _position_offset;          // 物体相对手的本地位置偏移
        private Quaternion _rotation_offset;       // 物体相对手的本地旋转偏移

        /// <summary>获取手部追踪数据, 编辑器下用头部模拟</summary>
        private void GetHandData(bool isRight, out Vector3 pos, out Quaternion rot)
        {
#if UNITY_EDITOR
            var data = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);
#else
            var type = isRight ? VRCPlayerApi.TrackingDataType.RightHand : VRCPlayerApi.TrackingDataType.LeftHand;
            var data = Networking.LocalPlayer.GetTrackingData(type);
#endif
            pos = data.position;
            rot = data.rotation;
        }

        /// <summary>进入自动跟随: 计算偏移, 放下物体, 禁拾取, 关Canvas碰撞</summary>
        private void EnterAutoFollow()
        {
            var _pick = (VRCPickup)this.GetComponent(typeof(VRCPickup));
            if (_pick == null) return;

            // 记录当前手持手
            _is_right_hand = (_pick.currentHand == VRCPickup.PickupHand.Right);
            GetHandData(_is_right_hand, out Vector3 p, out Quaternion r);

            // 计算物体相对于手的本地位置/旋转偏移
            _position_offset = Quaternion.Inverse(r) * (this.transform.position - p);
            _rotation_offset = Quaternion.Inverse(r) * this.transform.rotation;

            _pick.Drop();
            _pick.pickupable = false;

            SetCanvasBoxEnabled(false);

            _is_auto_flow = true;
        }

        /// <summary>延迟回调: 0.5秒内连按>=3次Use则进入自动跟随</summary>
        public void DecTimeUser()
        {
            if (_interact_times >= 3)
                EnterAutoFollow();

            _interact_times = 0;
            _is_use_check_task = false;
        }

        /// <summary>按下Use时累计次数, 首次启动0.5秒倒计时</summary>
        public override void OnPickupUseUp()
        {
            _interact_times += 1;
            if (!_is_use_check_task)
            {
                _is_use_check_task = true;
                this.SendCustomEventDelayedSeconds(nameof(DecTimeUser), 0.5f);
            }
        }

        #endregion 自动跟随模式

        /// <summary>Use触发: 命中牌HitBox时发VrInputTrg, 命中Toggle时翻转Toggle状态</summary>
        public override void InputUse(bool value, VRC.Udon.Common.UdonInputEventArgs args)
        {
            if (!value) return;

            // 只在当前手持手触发
            if (this._is_right_hand && (args.handType == HandType.LEFT)) return;
            if (!this._is_right_hand && (args.handType == HandType.RIGHT)) return;

            // 命中牌 → 触发牌上的VrInputTrg
            if (_udonBehaviour != null)
            {
                _udonBehaviour.SendCustomEvent(EVN_VR_INPUT_TRG);
            }

            // 命中Toggle → 翻转开关
            if (this._is_hit_toggle)
            {
                var toggle = (Toggle)this._toggle_hit_history_tf.GetComponent(typeof(Toggle));
                if (toggle != null)
                {
                    toggle.isOn = !toggle.isOn;
                }
            }
        }

        #region 退出自动跟随
        // ---------- 退出自动跟随 ----------

        private int _grab_count = 0;
        private bool _is_grab_check_task = false;

        /// <summary>延迟回调: 0.5秒内连按>3次Grab则退出跟随, 恢复拾取和Canvas</summary>
        public void DecTimeGrab()
        {
            if (_grab_count > 3)
            {
                ((VRCPickup)this.GetComponent(typeof(VRCPickup))).pickupable = true;
                this._is_auto_flow = false;
                SetCanvasBoxEnabled(true);
            }
            _grab_count = 0;
            _is_grab_check_task = false;
        }

        /// <summary>跟随模式下按Grab累计次数, 首次启动0.5秒倒计时</summary>
        public override void InputGrab(bool value, UdonInputEventArgs args)
        {
            if (this._is_auto_flow)
            {
                _grab_count++;
                if (!_is_grab_check_task)
                    this.SendCustomEventDelayedSeconds(nameof(DecTimeGrab), 0.5f);
            }
        }

        /// <summary>批量设置Canvas碰撞体的enabled</summary>
        private void SetCanvasBoxEnabled(bool en)
        {
            if (_tar_canvas_box == null) return;
            for (int i = 0; i < _tar_canvas_box.Length; i++)
            {
                if (_tar_canvas_box[i] != null)
                    _tar_canvas_box[i].enabled = en;
            }
        }
        #endregion 退出自动跟随
    }
}
