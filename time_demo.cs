
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class time_demo : UdonSharpBehaviour
{
    public const long BASE_UTC_TIME_2020 = 63713433600;
    public long start_time_utc_ticks;

    void Start()
    {
        this.start_time_utc_ticks = DateTime.UtcNow.Ticks;
    }
}
