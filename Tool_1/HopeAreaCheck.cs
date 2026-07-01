using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace HopeTools
{
    public class HopeAreaCheck : UdonSharpBehaviour
    {
        public GameObject target;
        public bool isTargetInside;

        void Update()
        {
            if (target == null)
            {
                isTargetInside = false;
                return;
            }

            isTargetInside = CheckBinA(transform, target.transform);
        }

        public bool CheckBinA(Transform a, Transform b)
        {
            Vector3 localPos = a.InverseTransformPoint(b.position);
            Vector3 halfScale = a.localScale * 0.5f;

            return
                Mathf.Abs(localPos.x) <= Mathf.Abs(halfScale.x) &&
                Mathf.Abs(localPos.y) <= Mathf.Abs(halfScale.y) &&
                Mathf.Abs(localPos.z) <= Mathf.Abs(halfScale.z);
        }

        void OnDrawGizmos()
        {
            Color c = isTargetInside ? Color.green : Color.white;
            Gizmos.color = c;
            Matrix4x4 baseMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);

            // for (int x = -1; x <= 1; x++)
            // {
            //     for (int y = -1; y <= 1; y++)
            //     {
            //         for (int z = -1; z <= 1; z++)
            //         {
            //             if (x == 0 && y == 0 && z == 0) continue;
            //             Gizmos.matrix = baseMatrix * Matrix4x4.TRS(new Vector3(x, y, z) * 0.002f, Quaternion.identity, Vector3.one);
            //             Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
            //         }
            //     }
            // }

            Gizmos.matrix = baseMatrix;
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        }
    }
}
