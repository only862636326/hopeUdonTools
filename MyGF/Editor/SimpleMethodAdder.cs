using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;
using VRC.Udon;
using TMPro;
using UdonSharpEditor;
using System;
using VRC.SDKBase;
using UdonSharp;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor.Events;
using UnityEngine.Events;

namespace HopeTools
{
    [CustomEditor(typeof(SimpleMethodAdder))]
    public class SimpleMethodAdder : EditorWindow
    {
        private const string SESSION_KEY_SRC_PATH = "SingAddScriptPathEditor_ScriptPath";
        //private const string SESSION_KEY_SRC_PATH = "ScriptPathEditor_ScriptPath";

        public string _scr_path;
        public string _method_name;
        public GameObject op_obg;


        SimpleMethodAdder()
        {
            this.titleContent = new GUIContent("快速添加方法");
        }

        [MenuItem("HopeTools/快速添加方法")]
        static void ShowSetToggleEvn()
        {
            EditorWindow.GetWindow(typeof(SimpleMethodAdder));
        }
        private void OnEnable()
        {
            // 从 SessionState 加载路径
            _scr_path = EditorPrefs.GetString(SESSION_KEY_SRC_PATH, "");
        }

        private void OnDisable()
        {
            EditorPrefs.SetString(SESSION_KEY_SRC_PATH, _scr_path);
        }



        private void OnGUI()
        {
            // 脚本路经
            GUILayout.Space(10);
            //EditorPrefs.SetString(SESSION_KEY, _scr_path);

            _scr_path = EditorGUILayout.TextField("脚本路径", _scr_path);
            if (GUILayout.Button("选择脚本"))
            {
                _scr_path = EditorUtility.OpenFilePanel(
                    "选择脚本文件",
                    Application.dataPath,
                    "cs");
            }

            GUILayout.Space(30);

            _method_name = EditorGUILayout.TextField("方法名", _method_name);
            if (GUILayout.Button("添加方法"))
            {
                AddMethodWithoutRoslyn(_scr_path, _method_name);
            }


            GUILayout.Space(30);
            op_obg = (GameObject)EditorGUILayout.ObjectField("操作对象", op_obg, typeof(GameObject), true);
            if (GUILayout.Button("自动绑定子物件事件"))
            {
                if (op_obg != null)
                {
                    for (int i = 0; i < op_obg.transform.childCount; i++)
                    {
                        var g = op_obg.transform.GetChild(i).transform;
                        var tarudon = (UdonSharpBehaviour)GameObject.Find("Hugf").GetComponentInChildren<HopeUdonEvnApi>();

                        var targetToggle = g.GetComponent<Toggle>();
                        SetTogleHufgEvn(targetToggle, tarudon);

                        var tar_s = g.GetComponentsInChildren<Toggle>();
                        foreach (var t in tar_s)
                        {
                            SetTogleHufgEvn(t, tarudon, -1);
                        }
                    }
                }
            }

            if (GUILayout.Button("清除子一层后缀"))
            {
                if (op_obg != null)
                {
                    for (int i = 0; i < op_obg.transform.childCount; i++)
                    {
                        var g = op_obg.transform.GetChild(i).transform;
                        if (g == null)
                            return;

                        var ss = g.name.Split('_');
                        if (int.TryParse(ss[ss.Length - 1], out var idx))
                        {
                            var _n = ss[0];
                            for (int j = 1; j < ss.Length - 1; i++)
                            {
                                _n = _n + "_" + ss[j];
                            }
                            g.name = _n;
                            EditorUtility.SetDirty(g);
                            EditorUtility.DisplayDialog("提示", "完成", "ok");
                        }
                    }
                }
            }

            if (GUILayout.Button("添加子物件方法到脚本, 不带index, 一层"))
            {
                if (op_obg != null)
                {
                    for (int i = 0; i < op_obg.transform.childCount; i++)
                    {
                        var g = op_obg.transform.GetChild(i).transform;
                        var tarudon = (UdonSharpBehaviour)GameObject.Find("Hugf").GetComponentInChildren<HopeUdonEvnApi>();

                        var targetToggle = g.GetComponent<Toggle>();
                        AddMethodWithoutRoslyn(_scr_path, targetToggle.name);
                        var tar_s = g.GetComponentsInChildren<Toggle>();
                        foreach (var t in tar_s)
                        {
                            AddMethodWithoutRoslyn(_scr_path, t.name);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 删除Toggle上绑定的事件
        /// </summary>
        /// <param name="toggle"></param>
        /// <param name="tarudon"></param>
        /// <param name="idx"></param>
        public static string SetTogleHufgEvn(Toggle toggle, UdonSharpBehaviour tarudon, int idx = -1)
        {
            if (toggle == null || tarudon == null)
                return ""; 

            var s = toggle.name.Split("_");
            if (s[0] == "ToggleEvn" && s.Length >= 2)
            {
                var targetToggle = toggle;
                var evn = "ToggleEvn_";
                evn += s[1];

                if (idx != -1)
                {
                    evn = evn + "_" + idx.ToString();
                }

                else if (s.Length == 3 && idx == -1)
                {
                    evn = evn + "_" + s[2];
                }

                targetToggle.transform.name = evn;

                //AddMethodWithoutRoslyn(scr_script, evn);
                UnregisterPersistentListener(targetToggle.onValueChanged);
                //UnityEventTools.AddObjectPersistentListener<Collider>(targetToggle.onValueChanged, tarudon.GetComponent<UdonBehaviour>().OnTriggerEnter, toggle.transform.GetComponent<Collider>());
                UnityEventTools.AddStringPersistentListener(targetToggle.onValueChanged, tarudon.GetComponent<UdonBehaviour>().SendCustomEvent, evn);
                EditorUtility.SetDirty(targetToggle);
                return evn;
            }

            return "";
        }


        // 移除指定目标对象和方法名的持久化监听器
        public static void UnregisterPersistentListener(UnityEventBase unityEvent)
        {
            if (unityEvent == null)
                return;

            // 获取持久化监听器的数量
            int count = unityEvent.GetPersistentEventCount();

            // 遍历所有持久化监听器
            for (int i = count - 1; i >= 0; i--)
            {
                UnityEventTools.RemovePersistentListener(unityEvent, i);
            }
        }

        /// <summary>
        /// 给脚本上自动添加方法
        /// </summary>
        /// <param name="scriptPath"></param>
        /// <param name="newMethod"></param>
        public static void AddMethodWithoutRoslyn(string scriptPath, string newMethod)
        {
            //scriptPath = @"D:/VR_C/VrcDayDark__2505/Assets/HopeTools/MyGF/HopeUdonEvnApi.cs";
            //string newMethod = "asdfsafds";
            //if (scriptPath == null)
            //{
            //    scriptPath = EditorUtility.OpenFilePanel(
            //        "选择脚本文件",
            //        Application.dataPath,
            //        "cs");
            //}

            if (string.IsNullOrEmpty(scriptPath))
                return;

            try
            {
                Debug.Log(scriptPath);
                // 读取原始代码
                string code = File.ReadAllText(scriptPath);
                Debug.Log(code);

                var s_name = $"public void {newMethod}()";
                if (code.Contains(s_name))
                {
                    EditorUtility.DisplayDialog("提示", "方法已存在", "ok");
                    return;
                }
                else
                {
                    var ss = newMethod.Split('_');
                    int idx;
                    // 判断最后一部分是否为整数
                    if (ss.Length >= 2 && int.TryParse(ss[ss.Length - 1], out idx))
                    {
                        var _new = ss[0];
                        for (int i = 1; i < ss.Length - 2; i++)
                        {
                            _new = _new + "_" + ss[i];
                        }

                        // 倒数第二部分是 字母+数字 的情况 eg : ButtonEvn12_34  , add fun ButtonEvn(12, 34)

                        var ss2 = ss[ss.Length - 2];
                        // 找到字母和数字的分界点
                        int splitIndex = ss2.Length;
                        for (int i = ss2.Length - 1; i >= 0; i--)
                        {
                            if (char.IsDigit(ss2[i]))
                            {
                                splitIndex = i;
                            }
                            else
                            {
                                break;
                            }
                        }
                        if (splitIndex < ss2.Length && splitIndex > 0)
                        {
                            var prefix = ss2.Substring(0, splitIndex);
                            var numberPart = ss2.Substring(splitIndex);
                            _new += prefix;

                            s_name = $"public void {newMethod}() {{ {_new}({numberPart}, {idx}); }}\r\n";
                        }
                        else
                        {
                             s_name = $"public void {newMethod}() {{ {_new}({idx}); }}\r\n";  
                        }


                    }

                    else
                    {
                        s_name = $"public void {newMethod}() \r\n        {{         \r\n;         \r\n}}\r\n";
                    }
                    EditorUtility.DisplayDialog("提示", "方法添加完成", "ok");
                }

                // 使用正则表达式找到类的结束花括号
                string pattern = "// end no idx";
                string updatedCode = Regex.Replace(code, pattern, s_name + "\t\t" + pattern);

                //// 保存修改后的代码
                File.WriteAllText(scriptPath, updatedCode);
                AssetDatabase.Refresh();

                Debug.Log("方法已添加到脚本");
            }
            catch (System.Exception e)
            {
                EditorUtility.DisplayDialog("提示", "添加方法失败\r\n请检查脚本", "ok");

                Debug.LogError($"添加方法失败: {e.Message}");
            }
        }
    }
}


