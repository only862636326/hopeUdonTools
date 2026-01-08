
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace HopeTools
{
    public class LineSwitch : UdonSharpBehaviour
    {        
        [Header("拉线")]
        public LineRenderer lineRenderer;
        [Header("开关的上部分")]
        public Transform switch_up_obg_tf;
        [Header("开关的下部分")]
        public Transform switch_down_obg_tf;
        [Header("控制的物品列表")]
        public GameObject[] control_obg_list;
        [Header("AudioSource, 可为空")]
        public AudioSource switch_audio;
        [Header("打开音效")]
        public AudioClip switch_audio_clip__on;
        [Header("关闭音效，当为空时使用和打开时一样的音效")]
        public AudioClip switch_audio_clip_off;
        [Header("默认开关音效是参考是列表中的第一个物化状态，\r\n当开音效与第一个物体acticve状态相反时，设为true")]
        public bool reserved = false;
        [Header("向下移动多少距离触发")]
        public float activ_distence = 0.05f;
        [Header("松开手时多久回位")]
        public float move_to_org_time = 1.0f;


        private bool is_pick_up = false;
        private Vector3 org_collider_position;
        private Vector3 org_switch_down_position;
        private bool is_active = false;
        private bool is_in_org_position = false;
        float time_tdl = 0;
        private bool is_enable = false;
        //[System.Obsolete]
        void Start()
        {
            //lineRenderer = GetComponent<LineRenderer>();
            this.org_collider_position = this.transform.position;
            this.org_switch_down_position = this.switch_down_obg_tf.position;
            this.lineRenderer.positionCount = 2;
            this.lineRenderer.SetPosition(0, this.switch_up_obg_tf.position);
            this.lineRenderer.SetPosition(1, this.switch_down_obg_tf.position);
            this.is_in_org_position = true;
            this.is_enable = this.control_obg_list.Length > 0;
            if (!this.switch_audio_clip_off) this.switch_audio_clip_off = this.switch_audio_clip__on;
        }

        private void Update()
        {
            if (!this.is_in_org_position && this.is_enable)
            {
                if (this.is_pick_up)
                {
                    var p = this.transform.position;
                    var p1 = this.switch_down_obg_tf.position;
                    // 下部分位置不能低于最开始的位置
                    if (p.y > this.org_collider_position.y)
                    {
                        p.y = this.org_collider_position.y;
                    }
                    p1.y = p.y;

                    this.switch_down_obg_tf.position = p1;
                    // 设置接线的位置
                    this.lineRenderer.SetPosition(1, this.switch_down_obg_tf.position);

                    if (this.is_active)
                    {
                        if (this.org_collider_position.y - p.y < this.activ_distence * 0.5)
                        {
                            this.is_active = false;
                        }
                    }
                    else
                    {
                        if (this.org_collider_position.y - p.y > this.activ_distence)
                        {
                            this.is_active = true;
                            
                            foreach (var control_obg in this.control_obg_list)
                            {
                                if (control_obg) control_obg.SetActive(!control_obg.activeSelf);
                            }

                            var swtich_sta = this.control_obg_list[0].activeSelf;
                            if (this.reserved) swtich_sta = !swtich_sta;
                            if (this.switch_audio && this.switch_audio_clip__on)
                            {
                                if (swtich_sta) this.switch_audio.PlayOneShot(this.switch_audio_clip__on);
                                else this.switch_audio.PlayOneShot(this.switch_audio_clip_off);
                            }
                        }
                    }
                }
                else
                {
                    // 松手后，一定时间后回到开始位置
                    var p = this.transform.position;
                    var p1 = this.switch_down_obg_tf.position;
                    this.time_tdl -= Time.deltaTime / this.move_to_org_time;
                    this.lineRenderer.SetPosition(1, this.switch_down_obg_tf.position);
                    p = Vector3.Lerp(this.org_collider_position,p, this.time_tdl);
                    if (p.y > this.org_collider_position.y)
                    {
                        p.y = this.org_collider_position.y;
                    }
                    p1.y = p.y;
                    this.switch_down_obg_tf.position = p1;
                    this.transform.position = p;
                    if(this.time_tdl <= 0)
                        this.is_in_org_position = true;
                }
            }
        }

        public override void OnPickup()
        {
            //base.OnPickup();
            this.is_pick_up = true;
            this.is_in_org_position = false;
        }

        public override void OnDrop()
        {
            //base.OnDrop();
            this.is_pick_up = false;
            this.time_tdl = 1;
        }
    }
}
