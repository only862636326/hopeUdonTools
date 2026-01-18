using System.Collections;
using System.Collections.Generic;
using UdonSharp;
using UnityEditor;
using UnityEngine;
using VRC.SDKBase;

namespace HopeTools
{
    public class FixAdd : EditorWindow
    {
        // Start is called before the first frame update

        public GameObject op_obg;
        public string prefix = "";
        public string postfix = "";

        public int x, y;

        FixAdd()
        {
            this.titleContent = new GUIContent("排列、重命名");
        }

        [MenuItem("HopeTools/排列、重命名")]
        static void ShowFixAdd()
        {
            EditorWindow.GetWindow(typeof(FixAdd));
        }

        private void OnGUI()
        {
            GUILayout.Space(10);
            GUI.skin.label.fontSize = 20;
            GUILayout.Label("Rrl");

            op_obg = (GameObject)EditorGUILayout.ObjectField("操作对象", op_obg, typeof(GameObject), true);
            prefix = EditorGUILayout.TextField("前缀", prefix);
            postfix = EditorGUILayout.TextField("后缀", postfix);

            GUILayout.Space(10);

            if (GUILayout.Button("子级保持原名"))
            {
                if (op_obg != null)
                {
                    for(int i = 0; i < op_obg.transform.childCount; i++)
                    {
                        var child = op_obg.transform.GetChild(i);
                        child.name = $"{prefix}_{child.name}_{postfix}";
                    }
                }
            }

            if (GUILayout.Button("子级新数字"))
            {
                if (op_obg != null)
                {
                    for (int i = 0; i < op_obg.transform.childCount; i++)
                    {
                        var child = op_obg.transform.GetChild(i);
                        child.name = $"{prefix}{i}{postfix}";
                    }
                }
            }

            if (GUILayout.Button("递归保持原名"))
            {
                if (op_obg != null)
                {
                    var g = op_obg.GetComponentsInChildren<Transform>();
                    for (int i = 0; i < g.Length; i++)
                    {
                        g[i].name = $"{prefix}_{g[i].name}_{postfix}";
                    }
                }
            }

            GUILayout.Space(40);

            op_obg = (GameObject)EditorGUILayout.ObjectField("操作对象", op_obg, typeof(GameObject), true);
            x = EditorGUILayout.IntField("行", x);
            y = EditorGUILayout.IntField("列", y);

            if (GUILayout.Button("排序x, y"))
            {
                if (op_obg != null)
                {
                    var n = op_obg.transform.childCount;
                    var g = op_obg.transform;
                    if (n == x * y)
                    {
                        var g1 = g.GetChild(0).transform;
                        var g2 = g.GetChild(g.transform.childCount - 1).transform;

                        var offset_x = (g2.position.x - g1.position.x) / (x - 1);
                        var offset_y = (g2.position.y - g1.position.y) / (y - 1);

                        //for (int i = 0; i < x; i++)
                        //{
                        //    for (int j = 0; j < y; j++)
                        //    {
                        //        g.GetChild(i * x + j).position = g1.position + new Vector3(offset_x * i, offset_y * j, 0);
                        //    }
                        //}

                        for (int i = 0; i < n; i++)
                        {
                            var _x = i / y;
                            var _y = i % y;
                            g.GetChild(i).position = g1.position + new Vector3(offset_x * _x, offset_y * _y, 0);
                        }
                    }
                }
            }

            if (GUILayout.Button("排序y, x"))
            {
                if (op_obg != null)
                {
                    var n = op_obg.transform.childCount;
                    var g = op_obg.transform;
                    if (n == x * y)
                    {
                        var g1 = g.GetChild(0).transform;
                        var g2 = g.GetChild(g.transform.childCount - 1).transform;

                        var offset_x = (g2.position.x - g1.position.x) / (x - 1);
                        var offset_y = (g2.position.y - g1.position.y) / (y - 1);

                        //for (int i = 0; i < x; i++)
                        //{
                        //    for (int j = 0; j < y; j++)
                        //    {
                        //        g.GetChild(i * x + j).position = g1.position + new Vector3(offset_x * i, offset_y * j, 0);
                        //    }
                        //}

                        for (int i = 0; i < n; i++)
                        {
                            var _y = i / x;
                            var _x = i % x;
                            g.GetChild(i).position = g1.position + new Vector3(offset_x * _x, offset_y * _y, 0);
                        }
                    }
                }
            }

            if (GUILayout.Button("排序x, z"))
            {
                var n = op_obg.transform.childCount;
                var g = op_obg.transform;
                if (n == x * y)
                {
                    var g1 = g.GetChild(0).transform;
                    var g2 = g.GetChild(g.transform.childCount - 1).transform;

                    var offset_x = (g2.position.x - g1.position.x) / (x - 1);
                    var offset_z = (g2.position.z - g1.position.z) / (y - 1);

                    //for (int i = 0; i < x; i++)
                    //{
                    //    for (int j = 0; j < y; j++)
                    //    {
                    //        g.GetChild(i * x + j).position = g1.position + new Vector3(offset_x * i, offset_y * j, 0);
                    //    }
                    //}

                    for (int i = 0; i < n; i++)
                    {
                        var _x = i / y;
                        var _y = i % y;
                        g.GetChild(i).position = g1.position + new Vector3(offset_x * _x, 0, offset_z * _y);
                    }
                }
            }

            if (GUILayout.Button("排序z, x"))
            {
                var n = op_obg.transform.childCount;
                var g = op_obg.transform;
                if (n == x * y)
                {
                    var g1 = g.GetChild(0).transform;
                    var g2 = g.GetChild(g.transform.childCount - 1).transform;
                    var offset_x = (g2.position.x - g1.position.x) / (x - 1);
                    var offset_z = (g2.position.z - g1.position.z) / (y - 1);
                    //for (int i = 0; i < x; i++)
                    //{
                    //    for (int j = 0; j < y; j++)
                    //    {
                    //        g.GetChild(i * x + j).position = g1.position + new Vector3(offset_x * i, offset_y * j, 0);
                    //    }
                    //}
                    for (int i = 0; i < n; i++)
                    {
                        var _y = i / x;
                        var _x = i % x;
                        g.GetChild(i).position = g1.position + new Vector3(offset_x * _x, 0, offset_z * _y);
                    }
                }
            }
        }
    }
}