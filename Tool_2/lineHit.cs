
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

public class lineHit : UdonSharpBehaviour
{
    private LineRenderer line;
    public Vector3[] line_point;
    public int index = 0;
    public float MIN_MOVE_DIS = 0.01f;
    public bool is_in_draw;
    public Vector3 last_point;

    public GameObject hit_obg;

    public Transform look_obj_tf;
    //public Transform senser_obj_tf;
    public Text show_text;
    public float angle_sum = 0;

    void Start()
    {
        this.line_point = new Vector3[1000];
        this.is_in_draw = false;
        this.line = this.GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            var hit_p = get_hit_point();
            if (hit_p == Vector3.zero)
            {
                this.is_in_draw = false;
                this.angle_sum = 0;
                return;
            }

            show_in_page(hit_p);
            if (!this.is_in_draw)
            {
                this.index = 0;
                this.line_point[this.index++] = hit_p;
                this.last_point = hit_p;

                this.is_in_draw = true;
            }
            else
            {
                if (Vector3.Distance(this.last_point, hit_p) > MIN_MOVE_DIS)
                {
                    this.last_point = hit_p;
                    this.line_point[this.index++] = hit_p;

                    if (this.index > 1000)
                    {
                        this.index = 0;
                    }
                }
            }
            this.line.positionCount = this.index;
            this.line.SetPosition(this.index - 1, hit_p);
        }

        else
        {
            this.angle_sum = 0;
            this.is_in_draw = false;
        }
    }

    float last_angle = 0;

    void show_in_page(Vector3 point)
    {
        var angle = cal_angle(point, this.look_obj_tf.position);

        var angle_tdl = angle - this.last_angle;
        this.last_angle = angle;
        if (angle_tdl > 180)
            angle_tdl -= 360;
        if (angle_tdl < -180)
            angle_tdl += 360;

        this.angle_sum += angle_tdl;
        var n = Mathf.Floor(this.angle_sum / 360);
        n = n < 0 ? n + 1 : n;
        this.show_text.text = n.ToString();
    }


    /// <summary>
    /// 计算三维点与目标点的二维映射角度
    /// </summary>
    /// <param name="hit_p"></param>
    /// <param name="target_p"></param>
    /// <returns></returns>
    float cal_angle(Vector3 hit_p, Vector3 target_p)
    {
        var head_p = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;
        var look_vec = hit_p - head_p;                                                               // 头部坐标系的点位置
        var relative_r = Quaternion.FromToRotation((head_p - target_p).normalized, Vector3.forward); // 到target坐标系的旋转
        look_vec = relative_r * look_vec;                                                            // 把点旋转到target坐标系
        var angle = Mathf.Atan2(look_vec.y, look_vec.x) * 180 / 3.14f;                               // 角度计算

        return angle;
    }


    /// <summary>
    /// 获取头部向前打的点,
    /// </summary>
    /// <returns>(0,0,0) 表示无目标点被击中</returns>
    Vector3 get_hit_point()
    {
        RaycastHit raycastHit;
#if !UNITY_EDITOR
        var head_p = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;
        var head_r = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).rotation;
#else
        var head_p = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;
        var head_r = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).rotation;
#endif
        // 向头部前方打射线
        if (Physics.Raycast(head_p, head_r * Vector3.forward, out raycastHit, 20, 1 << 11))
        {
            var p = raycastHit.point + raycastHit.normal.normalized * 0.0f; // 向法线方向上移一点,防止和表面重合
            this.hit_obg.transform.position = p;
            return p;
        }

        return Vector3.zero;
    }

}



