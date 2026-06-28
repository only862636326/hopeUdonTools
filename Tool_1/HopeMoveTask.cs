using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace HopeTools
{
    public class HopeMoveTask : UdonSharpBehaviour
    {
        public const int MAX_TASK_NUM = 200;
        private const float PROGRESS_MIN = 0.0f;
        private const float PROGRESS_MAX = 1.0f;
        private const float BEZIER_MIDPOINT = 0.5f;
        private const float BEZIER_CONTROL_COEFFICIENT = 2.0f;
        private const int NO_SLOT = -1;
        public float moveDuration = 1.0f;

        public Transform[] task_tar_tf_list = new Transform[MAX_TASK_NUM];
        public Transform[] task_obj_tf_list = new Transform[MAX_TASK_NUM];

        private Vector3[] _taskOrgPos;
        private Quaternion[] _taskOrgRot;
        private Vector3[] _taskBezierOffset;

        private float[] _taskProgress;
        private bool[] _taskActive;

        void Start()
        {
            task_tar_tf_list = new Transform[MAX_TASK_NUM];
            task_obj_tf_list = new Transform[MAX_TASK_NUM];
            _taskProgress = new float[MAX_TASK_NUM];
            _taskActive = new bool[MAX_TASK_NUM];
            _taskOrgPos = new Vector3[MAX_TASK_NUM];
            _taskOrgRot = new Quaternion[MAX_TASK_NUM];
            _taskBezierOffset = new Vector3[MAX_TASK_NUM];
        }

        void Update()
        {
            if (task_tar_tf_list == null || task_obj_tf_list == null)
                return;

            int count = Mathf.Min(task_tar_tf_list.Length, task_obj_tf_list.Length, MAX_TASK_NUM);

            for (int i = 0; i < count; i++)
            {
                if (!_taskActive[i]) continue;
                if (task_tar_tf_list[i] == null || task_obj_tf_list[i] == null) continue;

                _taskProgress[i] += Time.deltaTime / moveDuration;

                if (_taskProgress[i] >= PROGRESS_MAX)
                {
                    _taskProgress[i] = PROGRESS_MAX;
                    task_obj_tf_list[i].position = task_tar_tf_list[i].position;
                    task_obj_tf_list[i].rotation = task_tar_tf_list[i].rotation;
                    _taskActive[i] = false;
                    task_obj_tf_list[i] = null;
                    task_tar_tf_list[i] = null;
                    continue;
                }

                float t = _taskProgress[i];
                Vector3 p0 = _taskOrgPos[i];
                Vector3 p2 = task_tar_tf_list[i].position;
                Vector3 p1 = (p0 + p2) * BEZIER_MIDPOINT + _taskBezierOffset[i];

                // 二次贝塞尔 B(t) = (1-t)^2*P0 + 2(1-t)t*P1 + t^2*P2
                float u = PROGRESS_MAX - t;
                Vector3 bezierPos = u * u * p0 + BEZIER_CONTROL_COEFFICIENT * u * t * p1 + t * t * p2;

                task_obj_tf_list[i].position = bezierPos;
                task_obj_tf_list[i].rotation = Quaternion.Slerp(_taskOrgRot[i], task_tar_tf_list[i].rotation, t);
            }
        }

        /// <summary>自动找空位启动一个移动任务，返回索引，没空位返回-1</summary>
        public int AssignTask(Transform obj, Transform org, Transform tar, Vector3 bezierOffset)
        {
            if (task_tar_tf_list == null || task_obj_tf_list == null)
                return NO_SLOT;
            //return NO_SLOT;
            int count = Mathf.Min(task_tar_tf_list.Length, task_obj_tf_list.Length, MAX_TASK_NUM);

            for (int i = 0; i < count; i++)
            {
                if (!_taskActive[i] && task_obj_tf_list[i] == null)
                {
                    _taskOrgPos[i] = org.position;
                    _taskOrgRot[i] = org.rotation;
                    task_tar_tf_list[i] = tar;
                    task_obj_tf_list[i] = obj;
                    _taskBezierOffset[i] = bezierOffset;
                    _taskProgress[i] = PROGRESS_MIN;
                    _taskActive[i] = true;
                    return i;
                }
            }

            return NO_SLOT;
        }

        public void StartAllTasks()
        {
            if (task_obj_tf_list == null) return;

            int count = Mathf.Min(task_obj_tf_list.Length, MAX_TASK_NUM);
            for (int i = 0; i < count; i++)
            {
                if (task_obj_tf_list[i] != null && task_tar_tf_list[i] != null)
                {
                    _taskProgress[i] = PROGRESS_MIN;
                    _taskActive[i] = true;
                }
            }
        }

        public void StopAllTasks()
        {
            for (int i = 0; i < MAX_TASK_NUM; i++)
            {
                _taskActive[i] = false;
                task_obj_tf_list[i] = null;
                task_tar_tf_list[i] = null;
            }
        }
        
        public void FinishAllTask()
        {
            if (task_obj_tf_list == null || task_tar_tf_list == null) return;

            int count = Mathf.Min(task_obj_tf_list.Length, task_tar_tf_list.Length, MAX_TASK_NUM);
            for (int i = 0; i < count; i++)
            {
                if (_taskActive[i] && task_obj_tf_list[i] != null && task_tar_tf_list[i] != null)
                {
                    task_obj_tf_list[i].position = task_tar_tf_list[i].position;
                    task_obj_tf_list[i].rotation = task_tar_tf_list[i].rotation;
                    _taskProgress[i] = PROGRESS_MAX;
                    _taskActive[i] = false;
                    task_obj_tf_list[i] = null;
                    task_tar_tf_list[i] = null;
                }
            }
        }
    }
}


