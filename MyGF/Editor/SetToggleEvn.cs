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
using UnityEditor.Events;
using UnityEngine.Events;
using System.ComponentModel;
using HopeTools;
using System.Text.RegularExpressions;


namespace HopeTools
{
    public class SetToggleEvn : EditorWindow    
    {
        // Start is called before the first frame update

        public GameObject op_obg;
        public GameObject tar_udon_obj;
        static public string scr_script;
        public string _scr_path;
        private const string SESSION_KEY = "ToggleEvnScriptPathEditor_ScriptPath";


        SetToggleEvn()
        {
            this.titleContent = new GUIContent("设置ToggleUdon事件");
        }

        [MenuItem("HopeTools/设置ToggleUdon事件")]
        static void ShowSetToggleEvn()
        {
            EditorWindow.GetWindow(typeof(SetToggleEvn));
        }

        private void OnEnable()
        {
            // 从 SessionState 加载路径
            _scr_path = EditorPrefs.GetString(SESSION_KEY, "");
        }

        private void OnDisable()
        {
            EditorPrefs.SetString(SESSION_KEY, _scr_path);
        }


        private void OnGUI()
        {
            GUILayout.Space(10);


            SessionState.SetString(SESSION_KEY, _scr_path);


            op_obg = (GameObject)EditorGUILayout.ObjectField("操作对象", op_obg, typeof(GameObject), true);
            tar_udon_obj = (GameObject)EditorGUILayout.ObjectField("目标Udon", tar_udon_obj, typeof(GameObject), true);
            // 脚本路经
            GUILayout.Space(10);
            SessionState.SetString(SESSION_KEY, _scr_path);
            _scr_path = EditorGUILayout.TextField("脚本路径", _scr_path);
            if (GUILayout.Button("选择脚本"))
            {
                _scr_path = EditorUtility.OpenFilePanel(
                    "选择脚本文件",
                    Application.dataPath,
                    "cs");
                SessionState.SetString(SESSION_KEY, _scr_path);
            }

            GUILayout.Space(10);
            if (GUILayout.Button("设置事子物体udon Evn"))
            {
                if (op_obg != null)
                {
                    SessionState.SetString(SESSION_KEY, _scr_path);
                    scr_script = _scr_path;
                    for (int i = 0; i < op_obg.transform.childCount; i++)
                    {
                        var g = op_obg.transform.GetChild(i).transform;                        
                        var tarudon = tar_udon_obj.GetComponent<UdonSharpBehaviour>();

                        var targetToggle = g.GetComponent<Toggle>();
                        SetTogleHufgEvn(targetToggle, tarudon);

                        var tar_s = g.GetComponentsInChildren<Toggle>();
                        foreach (var t in tar_s)
                        {
                            SetTogleHufgEvn(t, tarudon, i);
                        }
                    }
                }
            }

            GUILayout.Space(10);
            if (GUILayout.Button("设置udon Evn, 不带后缀"))
            {
                if (op_obg != null)
                {
                    SessionState.SetString(SESSION_KEY, _scr_path);
                    scr_script = _scr_path;
                    for (int i = 0; i < op_obg.transform.childCount; i++)
                    {
                        var g = op_obg.transform.GetChild(i).transform;
                        var tarudon = (UdonSharpBehaviour)GameObject.Find("Hugf").GetComponentInChildren<HopeUdonEvnApi>();

                        var targetToggle = g.GetComponent<Toggle>();
                        SetTogleHufgEvn(targetToggle, tarudon);

                        var tar_s = g.GetComponentsInChildren<Toggle>();
                        foreach (var t in tar_s)
                        {
                            SetTogleHufgEvn(t, tarudon);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// SetTogleHufgEvn
        /// </summary>
        /// <param name="toggle"></param>
        /// <param name="tarudon"></param>
        /// <param name="idx"></param>
        public static void SetTogleHufgEvn(Toggle toggle, UdonSharpBehaviour tarudon, int idx = -1)
        {
            if (toggle == null || tarudon == null)
                return;

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

                AddMethodWithoutRoslyn(scr_script, evn);
                UnregisterPersistentListener(targetToggle.onValueChanged);                
                //UnityEventTools.AddObjectPersistentListener<Collider>(targetToggle.onValueChanged, tarudon.GetComponent<UdonBehaviour>().OnTriggerEnter, toggle.transform.GetComponent<Collider>());
                UnityEventTools.AddStringPersistentListener(targetToggle.onValueChanged, tarudon.GetComponent<UdonBehaviour>().SendCustomEvent, evn);
                EditorUtility.SetDirty(targetToggle);
            }
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
                    return;
                }
                else
                {
                    var ss = newMethod.Split('_');
                    int idx;
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

                        var prefix = ss2.Substring(0, splitIndex);
                        var numberPart = ss2.Substring(splitIndex);
                        _new += "_" + prefix;

                        if (numberPart != "")
                        {
                            s_name = $"public void {newMethod}() {{ {_new}({numberPart}, {idx}); }}\r\n";
                        }
                        else
                        {
                            s_name = $"public void {newMethod}() {{ {_new}({idx}); }}\r\n";
                        }
                    }
                    

                    else
                    {
                        s_name = $"public void {newMethod}() \r\n{{ \r\n; \r\n}}\r\n";
                    }
                }

                // 使用正则表达式找到类的结束花括号
                string pattern = "// end method";
                string updatedCode = Regex.Replace(code, pattern, s_name +"\t\t"+ pattern);

                //// 保存修改后的代码
                File.WriteAllText(scriptPath, updatedCode);
                AssetDatabase.Refresh();

                Debug.Log("方法已添加到脚本");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"添加方法失败: {e.Message}");
            }
        }
    }
}