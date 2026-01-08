
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;
using System;
using System.Globalization;
using VRC.SDK3.Components;


public class DoubleClickHighSpeed : UdonSharpBehaviour
{
    private const bool DIR_UP = true;
    private const bool DIR_DOWN = false;

    private const float TRIGGER_MIN = 0.3f;
    private const float TRIGGER_MAX = 0.7f;

    public float run_speed = 12;
    public Text debug_text;
    private float org_speed_walk = 2;
    private float org_speed__run = 4;

    private int clicked_num = 0;
    private bool clicked_flag = false;
    private float time_tdl = 0.0f;
    private float time_tdl_2 = 0.0f;

    private bool asix_dir = DIR_DOWN;
    private bool asix_dir_his = DIR_DOWN;

    private float asix_v = -0.1f;
    private float asix_v_his = 0.0f;

    private float asix_h = 0.0f;

    private bool judge2run = false;
    //public float asix_min = 0.0f;

    private bool user_is_run = false;

    void Start()
    {
        // get user org speed 
        this.org_speed_walk = Networking.LocalPlayer.GetWalkSpeed();
        this.org_speed__run = Networking.LocalPlayer.GetRunSpeed();

        //Networking.LocalPlayer.SetVoiceDistanceFar(200.0f);
        //Networking.LocalPlayer.SetAvatarAudioFarRadius(200.0f);

        this.asix_dir = DIR_UP;
    }

    void Update()
    {
        if (debug_text) this.debug_text.text = $"num : {clicked_num}, aixs : {this.asix_v} , SPEED:{Networking.LocalPlayer.GetVelocity()}";

        if (this.time_tdl > 0)
        {
            this.time_tdl -= Time.deltaTime;
            if (this.time_tdl < 0)
            {
                // if after time_tdl end, key is not be click, reset click num
                this.clicked_num = 0;
            }
        }

        if (this.user_is_run)
        {
            this.time_tdl -= Time.deltaTime;
            if (this.asix_v * this.asix_v + this.asix_h * this.asix_h < 0.1f)
            {
                if (this.time_tdl <= 0.0f)
                {
                    this.user_is_run = false;
                    Networking.LocalPlayer.SetWalkSpeed(this.org_speed_walk);
                    Networking.LocalPlayer.SetRunSpeed(this.org_speed__run);
                    this.clicked_num = 0;
                    this.asix_dir = DIR_UP;
                }                
            }
            else
            {
                this.time_tdl = 0.3f;
            }
        }

        // delay 1s, if in this 1s, input walk forward, change speed
        if (this.judge2run && !this.user_is_run)
        {
            if (this.asix_v > TRIGGER_MAX)
            {
                var t = Time.time - this.time_tdl_2;
                //Debug.Log(t);
                if (t > 0.3f)
                {
                    Networking.LocalPlayer.SetWalkSpeed(this.run_speed);
                    Networking.LocalPlayer.SetRunSpeed(this.run_speed);
                    this.user_is_run = true;
                    this.judge2run = false;
                    this.time_tdl = 0.5f;
                }
            }
            else
            {
                judge2run = false;
            }
        }
    }

    public override void InputMoveVertical(float value, VRC.Udon.Common.UdonInputEventArgs args)
    {
        // this function will be call when uer inputMoveVertical is more than 0.1f;
        var his_asix_changed = false;
        //Debug.Log(Time.time);
        if (Mathf.Abs(this.asix_v - value) > 0.1f)
        {
            //this.asix_v_his = this.asix_v;
            this.asix_v = value;
            his_asix_changed = true;
        }

        if (his_asix_changed && !user_is_run)
        {
            if (this.asix_v > TRIGGER_MAX && this.asix_dir == DIR_UP)
            {
                this.clicked_num++;
                this.time_tdl = 0.3f;
                this.clicked_flag = true;
                this.asix_dir = DIR_DOWN;
            }

            if (this.asix_dir == DIR_DOWN && this.asix_v < TRIGGER_MIN && this.asix_v >= -TRIGGER_MIN)
            {
                this.asix_dir = DIR_UP;
            }

            if (this.asix_v < -0.7f)
            {
                this.asix_dir = DIR_UP;
                this.time_tdl = -1.0f;
                this.clicked_num = 0;
            }

            if (this.clicked_flag)
            {
                this.clicked_flag = false;
                if (this.clicked_num == 2 && !this.user_is_run)
                {
                    this.time_tdl_2 = Time.time;
                    this.judge2run = true;                    
                    this.clicked_num = 0;
                }
            }
        }
    }

    public override void InputMoveHorizontal(float value, VRC.Udon.Common.UdonInputEventArgs args)
    {
        this.asix_h = value;
    }
}
