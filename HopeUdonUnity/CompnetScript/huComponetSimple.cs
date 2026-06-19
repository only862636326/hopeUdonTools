
using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;


namespace HopeTools
{
    public enum TargetType
    {
        Auto = -1,
        None = 0,
        Toggle,
        Button,
        Slider,
        Mesh,
        Audio,
        Box3D,
        Box2D,
        Udon,
        Animator,
        Image,
        RawImage,
            
    }

    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class huComponetSimple : UdonSharpBehaviour
    {
        public TargetType targetType = TargetType.Auto;
        private TargetType turelType = TargetType.None;
        public bool _has_content = false;
        public Type _op_type;
        public Transform active_obj;
        #region public fun
        private Transform _title_transform;
        private Transform _content_transform;
        private Toggle _component_active_toggle;

        private void PublicInitGetComponet()
        {
            _title_transform = this.transform;
            var prt = transform.parent;
            if (_has_content)
            {
                var idx = this.transform.GetSiblingIndex() + 1;
                _content_transform = prt.GetChild(idx);
            }
            _component_active_toggle = this.transform.Find("ToggleCopmActive").GetComponent<Toggle>();
        }

        public void ToggleCopmActive()
        {
            if (active_obj == null)
                return;
            if (_op_componet == null)
                return;
            SetCompActive();
        }

        public void SetSelfActive()
        {
            if (_content_transform != null)
                _content_transform.gameObject.SetActive(_op_componet != null);
            if (_title_transform != null)
                _title_transform.gameObject.SetActive(_op_componet != null);

            if (_op_componet != null && turelType != TargetType.None)
            {
                _op_componet.gameObject.SetActive(_component_active_toggle.isOn);
            }
        }

        public void LogMessge(object messge)
        {
            Debug.Log("                      [HopeUnityGameObjCompnet] " + messge.ToString());
        }

        void Start()
        {
            PublicInitGetComponet();
            InitGetComponet();
        }

        private bool _is_init = false;
        public void UpdataVal()
        {
            if (!this._is_init)
            {
                PublicInitGetComponet();
                InitGetComponet();
                _is_init = true;
            }
            if (active_obj == null)
                return;

            _op_type = GetOpType(targetType);
            if (_op_type != null)
            {
                _op_componet = active_obj.GetComponent(_op_type);
            }
            if (_op_componet == null)
            {
                LogMessge("_op_componet is null");
            }
            SetSelfActive();
            InitSetComponetVal();
        }

        private Type GetOpType(TargetType targetType)
        {
            switch (targetType)
            {
                case TargetType.Auto:
                    return GetTypeByName(_title_transform.name);
                case TargetType.Toggle:
                    turelType = TargetType.Toggle;
                    return typeof(Toggle);
                case TargetType.Button:
                    turelType = TargetType.Button;
                    return typeof(Button);
                case TargetType.Slider:
                    turelType = TargetType.Slider;
                    return typeof(Slider);
                case TargetType.Mesh:
                    turelType = TargetType.Mesh;
                    return typeof(MeshRenderer);
                case TargetType.Audio:
                    turelType = TargetType.Audio;
                    return typeof(AudioSource);
                case TargetType.Box3D:
                    turelType = TargetType.Box3D;
                    return typeof(BoxCollider);
                case TargetType.Box2D:
                    turelType = TargetType.Box2D;
                    return typeof(BoxCollider2D);
                case TargetType.Udon:
                    turelType = TargetType.Udon;
                    return typeof(UdonSharpBehaviour);
                case TargetType.Animator:
                    turelType = TargetType.Animator;
                    return typeof(Animator);
                case TargetType.RawImage:
                    turelType = TargetType.RawImage;
                    return typeof(RawImage);
                case TargetType.Image:
                    turelType = TargetType.Image;
                    return typeof(Image);
                case TargetType.None:
                    return null;
            }
            return null;
        }

        private Type GetTypeByName(string typeName)
        {
            typeName = typeName.ToLower();
            switch (typeName)
            {
                case "box3dtitle":
                case "boxtitle":
                    turelType = TargetType.Box3D;
                    return typeof(BoxCollider);
                case "box2dtitle":
                    turelType = TargetType.Box2D;
                    return typeof(BoxCollider2D);
                case "slidertitle":
                    turelType = TargetType.Slider;
                    return typeof(Slider);
                case "meshtitle":
                    turelType = TargetType.Mesh;
                    return typeof(MeshRenderer);
                case "audiotitle":
                    turelType = TargetType.Audio;
                    return typeof(AudioSource);
                case "udontitle":
                    turelType = TargetType.Udon;
                    return typeof(UdonSharpBehaviour);
                case "toggletitle":
                    turelType = TargetType.Toggle;
                    return typeof(Toggle);
                case "buttontitle":
                    turelType = TargetType.Button;
                    return typeof(Button);
                case "animtitle":
                case "animatitle":
                    turelType = TargetType.Animator;
                    return typeof(Animator);
                case "rawimagetitle":
                    turelType = TargetType.RawImage;
                    return typeof(RawImage);
                case "imagetitle":
                    turelType = TargetType.Image;
                    return typeof(Image);

                default:
                    return null;
            }
            return null;
        }

        private void SetCompActive()
        {
            if (_op_componet == null || turelType == TargetType.None)
            {
                return;
            }
            switch (turelType)
            {
                case TargetType.Toggle:
                    var toggle = (Toggle)_op_componet;
                    toggle.enabled = _component_active_toggle.isOn;
                    break;
                case TargetType.Button:
                    var button = (Button)_op_componet;
                    button.interactable = _component_active_toggle.isOn;
                    break;
                case TargetType.Slider:
                    var slider = (Slider)_op_componet;
                    slider.interactable = _component_active_toggle.isOn;
                    break;
                case TargetType.Mesh:
                    var mesh = (MeshRenderer)_op_componet;
                    mesh.enabled = _component_active_toggle.isOn;
                    break;
                case TargetType.Audio:
                    var audio = (AudioSource)_op_componet;
                    audio.enabled = _component_active_toggle.isOn;
                    break;
                case TargetType.Box3D:
                    var box3d = (BoxCollider)_op_componet;
                    box3d.enabled = _component_active_toggle.isOn;
                    break;
                case TargetType.Box2D:
                    var box2d = (BoxCollider2D)_op_componet;
                    box2d.enabled = _component_active_toggle.isOn;
                    break;
                case TargetType.Udon:
                    var udon = (UdonSharpBehaviour)_op_componet;
                    udon.enabled = _component_active_toggle.isOn;
                    break;
                case TargetType.Animator:
                    var anim = (Animator)_op_componet;
                    anim.enabled = _component_active_toggle.isOn;
                    break;
                case TargetType.RawImage:
                    var rawimage = (RawImage)_op_componet;
                    rawimage.enabled = _component_active_toggle.isOn;
                    break;
                case TargetType.Image:
                    var image = (Image)_op_componet;
                    image.enabled = _component_active_toggle.isOn;
                    break;

                default:
                    break;
            }
        }

        #endregion publicfun

        public Component _op_componet;

        public void InitGetComponet()
        {
            ;
        }
        public void InitSetComponetVal()
        {
            ;
        }
    }
}