
using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.UdonNetworkCalling;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

namespace HopeTools
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class HopeShell : UdonSharpBehaviour
    {
        // Udon compatible arrays instead of Lists and Dictionaries

        // Variables for triple Enter detection 
        private float tripleEnterTimeWindow = 1.0f; // 1 second window for triple press

        // Simple variable storage using arrays`
        private string[] variableNames;
        private string[] variableValues;
        private string[] variableType;
        private int variableCount = 0;
        [SerializeField] private int maxVariables = 20;

        public Transform active_transform;
        public Transform[] _managered_transform;

        private bool _compar_result = false;

        void Start()    
        {
            // Initialize arrays
            variableNames = new string[maxVariables];
            variableValues = new string[maxVariables];
            variableType = new string[maxVariables];

            // Initialize variables
            SetVariable("__VERSION__", "1.0.0");

            GenerPwdVar(active_transform);
            // Display welcome message
            PrintLine("HopeShell v1.0.0 - Type 'help' for available commands");
        }

        void Update()
        {
            ;
        }
       
        #region Command Processing
        
        #region Command Processing core
        private void ProcessCommand(string command)
        {
            if (string.IsNullOrWhiteSpace(command)) return;
            var _pre_cmd = "    ";

            // Check if this is a member variable access (e.g., ".variableName")
            if (command.StartsWith(".") && command.Length > 1)
            {
                var ret = MemberVariableAccess(active_transform, command);
                PrintLine(_pre_cmd + ret);
                return;
            }

            // Process variable access and assignment with $ symbol
            if (command.StartsWith("$") && command.Length > 1)
            {
                var ret = ProcessVariableAccess(command);
                PrintLine(_pre_cmd + ret);
                return;
            }

            var sp = command.Split(' ', 2);
            var cmd = sp[0].ToLower();
            var par = sp.Length > 1 ? sp[1] : "";
            // Process command
            if (cmd == "help")
            {
                ShowHelp();
            }
            else if (cmd == "clear" || cmd == "cls")
            {
                ClearCommand();
            }
            else if (cmd == "history")
            {
                PrintLine(_pre_cmd + ShowHistory());
            }
            else if (cmd == "time")
            {
                PrintLine(_pre_cmd + GetCurrentTime());
            }
            else if (cmd == "ls")
            {
                PrintLine(_pre_cmd + ListChildren());
            }
            else if (cmd == "cd")
            {
                PrintLine(ChangeDirectory(par));				   
            }
            else if (cmd == "addroot")
            {
                PrintLine(_pre_cmd + FindAndAddTfToManager(par));
            }

            else
            {
                PrintLine(_pre_cmd + "Command not found: " + cmd + ". Type 'help' for available commands.");
            }
        }

         /// <summary>
         /// 处理成员变量访问和赋值
         /// 中文: 处理成员变量访问和赋值，使用.符号
         /// </summary>
         /// <param name="tf">要操作组件的Transform</param>
         /// <param name="assignmentExpression">赋值表达式，例如".position = (1, 2, 3)"</param>
         /// <returns>处理结果</returns>
        private string MemberVariableAccess(Transform tf, string assignmentExpression)
        {
            if (tf == null)
            {
                return "[err] :No active Transform selected";
            }
            // start with '.' and morethan 1 char
            if (!assignmentExpression.StartsWith(".") || assignmentExpression.Length <= 1)
            {
                return "[err] : cmd err [" + assignmentExpression + "]";
            }
            assignmentExpression = assignmentExpression.Substring(1).Trim(); // remove leading '.'

            // Parse the assignment expression (e.g., "transform.position = (1, 2, 3)")
            string[] parts = assignmentExpression.Split("=", 2); // Split into 2 parts only      

            string memberPath = parts[0].Trim();

            if (parts.Length == 1)
            {
                return MemberVariableEq(tf, memberPath, "");
            }

            if (parts.Length == 2)
            {
                return MemberVariableEq(tf, memberPath, parts[1].Trim());
            }

            return "[err] : cmd err [" + assignmentExpression + "]";
        }

        
        /// <summary>
        /// Process variable access and assignment with $ symbol
        /// 中文: 处理变量访问和赋值，使用$符号
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>

        private string ProcessVariableAccess(string command)
        {
            var return_value = "";
            // Check if this is a variable access with $ symbol (e.g., "$variableName")
            if (command.StartsWith("$") && command.Length > 1)
            {
                int equalPos = command.IndexOf('=');
                if (equalPos == -1)
                {
                    // Variable access: $var
                    string varName = command.Substring(1);

                    // Special handling for $pwd - return current directory path
                    if (varName == "pwd")
                    {
                        return_value = "pwd = " + GetVariable("pwd");
                        return return_value;
                    }

                    string value = GetVariable(varName);

                    if (value != null)
                    {
                        return_value = varName + " = " + value;
                        return return_value;
                    }
                    else
                    {
                        return_value = "[err] : Variable " + varName + " not found";
                        return return_value;
                    }
                }

                else
                {
                    // Variable assignment: $var = value
                    // Find the position of '=' in the command
                    equalPos = command.IndexOf('=');
                    // Extract variable name
                    string varName = command.Substring(0, equalPos).Trim();

                    // Handle variable name with $ symbol
                    if (varName.StartsWith("$") && varName.Length > 1)
                    {
                        varName = varName.Substring(1);
                    }

                    // Prevent direct assignment to $pwd - it's managed by cd command
                    if (varName == "pwd")
                    {
                        return_value = "[err] : $pwd is a read-only system variable managed by cd command";
                        return return_value;
                    }

                    // Extract value (everything after '=')
                    string value = command.Substring(equalPos + 1).Trim();

                    // Handle quoted values
                    if (value.StartsWith("\"") && value.EndsWith("\""))
                    {
                        // Remove the quotes
                        value = value.Substring(1, value.Length - 2);
                    }

                    else if (value.StartsWith("\'") && value.EndsWith("\'"))
                    {
                        value = value.Substring(1, value.Length - 2);
                    }

                    // Handle variable references with $ symbol
                    else if (value.StartsWith("$") && value.Length > 1)
                    {
                        string refVarName = value.Substring(1).Trim();

                        // Special handling for $pwd reference
                        if (refVarName == "pwd")
                        {
                            value = GetVariable("pwd");
                        }
                        else
                        {
                            string refValue = GetVariable(refVarName);
                            if (refValue != null)
                            {
                                value = refValue;
                            }
                            else
                            {
                                return_value = "[err] : Variable " + refVarName + " not found";
                                return return_value;
                            }
                        }
                    }
                    // Set the variable
                    SetVariable(varName, value);
                    return_value = varName + " = " + value;
                    return return_value;
                }
            }
            return "[err] : err cmd : [" + command + "]";
        }

        private string MemberVariableEq(Transform tf, string memberPath, string value)
        {
            if (memberPath.StartsWith("."))
            {
                memberPath = memberPath.Substring(1);
            }

            //memberPath = memberPath.ToLower();

            var _mem1 = "";
            var _mem2 = "";
            var _mem3 = "";
            var sp = memberPath.Split('.');

            if (sp.Length >= 1)
            {
                _mem1 = sp[0];
            }
            if (sp.Length >= 2)
            {
                _mem2 = sp[1];
            }
            if (sp.Length >= 3)
            {
                _mem3 = sp[2];
            }

            switch (_mem1.ToLower())
            {
                case "transform":
                case "tf":
                    GetSetTransformPropertyValue(tf, _mem2, value, out object val, out string typ);
                    return val.ToString();
                    break;
                case "active":
                case "enable":
                case "isenabled":
                case "en":
                    return GetSetActiveState(tf, value);
                    break;
                case "text":
                case "t":
                    return GetSetText(tf, _mem2, value);
                    break;
                case "toggle":
                case "tog":
                case "tg":
                    return GetSetToggle(tf, _mem2, value);
                    break;
                case "udon":
                case "udonsharp":
                case "u":
                    return GetSetUdon(tf, _mem2, value);
                    break;
                case "uevn":
                case "uevent":
                case "ut":
                case "udonevent":
                    return GetSetUdonEvn(tf, _mem2, value);
                    break;
                case "img":
                case "image":
                case "rawimg":
                case "rawimage":
                case "ri":
                    return GetSetImage(tf, _mem2, value);
                    break;
                case "slider":
                case "sl":
                    return GetSetSlider(tf, _mem2, value);
                    break;
                case "audio":
                case "aud":
                case "audiosource":
                case "as":
                    return GetSetAudioSource(tf, _mem2, value);
                    break;
                case "boxcollider":
                case "box":
                case "bc":
                    return GetSetBoxCollider(tf, _mem2, value);
                    break;
                default:
                    ;
                    break;
            }
            return "[err] illege";
        }

        #endregion Command Processing core

        #region other comd
        private void ShowHelp()
        {
            ;
        }

        // Helper method to process variable substitution in strings

        // Helper method to set a variable
        private void SetVariable(string name, string value)
        {
            // Check if variable already exists
            for (int i = 0; i < variableCount; i++)
            {
                if (variableNames[i] == name)
                {
                    variableValues[i] = value;
                    return;
                }
            }

            // Add new variable if there's space
            if (variableCount < maxVariables)
            {
                variableNames[variableCount] = name;
                variableValues[variableCount] = value;
                variableCount++;
            }
        }

        // Helper method to get a variable value
        private string GetVariable(string name)
        {
            for (int i = 0; i < variableCount; i++)
            {
                if (variableNames[i] == name)
                {
                    return variableValues[i];
                }
            }
            return null;
        }

        private string ListVariables()
        {
            var s = "";
            for (int i = 0; i < variableCount; i++)
            {
                s += "  " + variableNames[i] + " = " + variableValues[i] + "\n";
            }
            return s;
        }

        private string GetCurrentTime()
        {
            // Udon compatible time formatting
            System.DateTime now = System.DateTime.Now;
            string year = now.Year.ToString();
            string month = now.Month.ToString().PadLeft(2, '0');
            string day = now.Day.ToString().PadLeft(2, '0');
            string hour = now.Hour.ToString().PadLeft(2, '0');
            string minute = now.Minute.ToString().PadLeft(2, '0');
            string second = now.Second.ToString().PadLeft(2, '0');

            string currentTime = year + "-" + month + "-" + day + " " + hour + ":" + minute + ":" + second;
            return "Current time: " + currentTime;
        }

        private string ListChildren()
        {
            var s = "";
            if (active_transform == null)
            {
                for (int i = 0; i < _managered_transform.Length; i++)
                {
                    if (_managered_transform[i] == null) continue;
                    s += _managered_transform[i].name + ",";
                }
                return s;
            }
            else
            {
                for (int i = 0; i < active_transform.childCount; i++)
                {
                    s += active_transform.GetChild(i).name + ",";
                }
                return s;
            }
			
        }
        

        private string FindAndAddTfToManager(string path)
        {
            path = path.Trim();
            if (string.IsNullOrEmpty(path))
            {
               return "Path is empty.";
            }

            if (path[0] == '/')
            {
                path = path.Substring(1);
            }

            // 处理单引号路径
            if (path.StartsWith("'") && path.EndsWith("'"))
            {
                path = path.Substring(1, path.Length - 2);
            }
            else if (path.StartsWith("\"") && path.EndsWith("\""))
            {
                path = path.Substring(1, path.Length - 2);
            }

            GameObject obj = GameObject.Find(path);
            Transform tf = obj != null ? obj.transform : null;

            if (tf == null)
            {
                return "No such GameObject: " + path;
            }
            for(int i = 0; i < _managered_transform.Length; i++)
								  
            {
                if (_managered_transform[i] == tf)
                {
                    return "TF already in manager.";
                }
            }

            for (int i = 0; i < _managered_transform.Length; i++)
            {
                if (_managered_transform[i] == null)
                {
                    _managered_transform[i] = tf;
                    return "Add " + path + " to manager.";
                }
            }
            return "_managered_transform is Full";
        }

        private string ChangeDirectory(string path)
        {
            path = path.Trim();
            if (string.IsNullOrEmpty(path))
            {
                return "[err] dir is null !!!";
            }

            if (path.StartsWith("$"))
            {
                path = path.Substring(1);
                path = GetVariable(path);
            }

            // 处理引号包围的整个路径
            if (path.StartsWith("'") && path.EndsWith("'"))
            {
                path = path.Substring(1, path.Length - 2);
            }
            else if (path.StartsWith("\"") && path.EndsWith("\""))
            {
                path = path.Substring(1, path.Length - 2);
            }

            // 确定起点：绝对路径从 null 开始（FindChildTransform 会查 _managered_transform / GameObject.Find）
            Transform current = active_transform;
            bool isAbsolute = path.StartsWith("/");

            if (isAbsolute)
            {
                path = path.Substring(1);
                current = null;
            }
            
            if (string.IsNullOrEmpty(path))
            {
                if (current != null)
                {
                    active_transform = current;
                }
                return GenerPwdVar(active_transform);
            }

            // 按 / 分割，逐段查找
            string[] segments = path.Split('/');
            for (int j = 0; j < segments.Length; j++)
            {
                string seg = segments[j];
                if (string.IsNullOrEmpty(seg))
                    continue;

                if (seg == "..")
                {
                    if (current != null && current.parent != null)
                    {
                        current = current.parent;
                    }
                    else
                    {
                        return "[err] 已在根目录";
                    }
                }
                else if (seg == ".")
                {
                    continue;
                }
                else
                {
                    Transform child = FindChildTransform(current, seg);
                    if (child == null)
                    {
                        return "找不到子物体: " + seg;
                    }
                    current = child;
                }
            }

            if (current == null)
            {
                return "[err] No such GameObject: " + path;
            }

            active_transform = current;
            return GenerPwdVar(current);
        }

        private string GenerPwdVar(Transform tf)
        {
            var currentTf = tf;
            _pre_cmd = "";
            var _t = currentTf;
            while (_t != null)
            {
                _pre_cmd = "/" + _t.name + _pre_cmd;
                _t = _t.parent;
            }
            SetVariable("pwd", _pre_cmd);
            _pre_cmd += ">>>";
            return _pre_cmd;
        }

        // 查找子物体的辅助方法
        private Transform FindChildTransform(Transform parent, string childName)
        {
            // 如果parent为null，尝试在管理器中查找根物体
            if (parent == null)
            {
                for (int i = 0; i < _managered_transform.Length; i++)
                {
                    if (_managered_transform[i] != null && _managered_transform[i].name == childName)
                    {
                        return _managered_transform[i];
                    }
                }
                // 如果管理器中没有，尝试在场景中查找
                GameObject rootObj = GameObject.Find(childName);
                if (rootObj != null)
                {
                    // 自动加入管理器
                    for (int j = 0; j < _managered_transform.Length; j++)
                    {
                        if (_managered_transform[j] == null)
                        {
                            _managered_transform[j] = rootObj.transform;
                            break;
                        }
                    }
                    return rootObj.transform;
                }
                return null;
            }

            // 在父物体的子物体中查找
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);
                if (child.name == childName)
                {
                    return child;
                }
            }

            return null;
        }
        
        #endregion

        #region udon 
        private string GetSetUdonEvn(Transform tf, string _mem2, string valset)
        {
            if (tf == null)
            {
                return "[err] :No active Transform selected";
            }
            var udon_comps = tf.GetComponent<UdonSharpBehaviour>();
            if (udon_comps == null)
            {
                return "[err] : No UdonBehaviour component found on the selected Transform";
            }
            if (string.IsNullOrEmpty(_mem2))
            {
                return udon_comps.enabled.ToString();
            }
            var dat = "";
            if (!string.IsNullOrEmpty(valset))
            {
                if (valset.IndexOf(',') >= 0)
                {
                    // 双参数单事件：用 , 分割，空字符串不算
                    var parts = valset.Split(',');
                    var p0 = parts.Length > 0 ? parts[0].Trim() : "";
                    var p1 = parts.Length > 1 ? parts[1].Trim() : "";
                    if (!string.IsNullOrEmpty(p0))
                    {
                        MyParseValue(p0, out object b1, out string typ1);
                        udon_comps.SetProgramVariable("eventData", b1);
                        dat = "eventData=" + b1;
                    }
                    if (!string.IsNullOrEmpty(p1))
                    {
                        MyParseValue(p1, out object b2, out string typ2);
                        udon_comps.SetProgramVariable("eventData2", b2);
                        dat = (string.IsNullOrEmpty(dat) ? "" : dat + " ") + "eventData=" + b2;
                    }
                    if (!string.IsNullOrEmpty(dat)) dat = "set " + _mem2 + " " + dat;
                    udon_comps.SendCustomEvent(_mem2);
                }
                // else if (valset.IndexOf(',') >= 0)
                // {
                //     // 多参数单事件：用 , 分割后直接传给 SendCustomNetworkEvent
                //     var parts = valset.Split(',');
                //     var valid = new object[10];
                //     var count = 0;
                //     for (int k = 0; k < parts.Length && count < 10; k++)
                //     {
                //         var p = parts[k].Trim();
                //         if (string.IsNullOrEmpty(p)) continue;
                //         MyParseValue(p, out object bv, out string tv);
                //         valid[count] = bv;
                //         dat = (string.IsNullOrEmpty(dat) ? "" : dat + " ") + "arg" + count + "=" + bv;
                //         count++;
                //     }
                //     switch (count)
                //     {
                //         // case 1: udon_comps.SendCustomNetworkEvent(NetworkEventTarget.Self, _mem2, valid[0]); break;
                //         // case 2: udon_comps.SendCustomNetworkEvent(NetworkEventTarget.Self, _mem2, valid[0], valid[1]); break;
                //         // case 3: udon_comps.SendCustomNetworkEvent(NetworkEventTarget.Self, _mem2, valid[0], valid[1], valid[2]); break;
                //         // case 4: udon_comps.SendCustomNetworkEvent(NetworkEventTarget.Self, _mem2, valid[0], valid[1], valid[2], valid[3]); break;
                //         // case 5: udon_comps.SendCustomNetworkEvent(NetworkEventTarget.Self, _mem2, valid[0], valid[1], valid[2], valid[3], valid[4]); break;
                //         // default: udon_comps.SendCustomNetworkEvent(NetworkEventTarget.Self, _mem2); break;
                //         // udon_comps.SendCustomEvent(_mem2);
                //     }
                //     if (!string.IsNullOrEmpty(dat)) dat = "send " + _mem2 + " " + dat;
                // }

                else
                {
                    MyParseValue(valset, out object b, out string typ);
                    udon_comps.SetProgramVariable("eventData", b);
                    dat = "set " + _mem2 + " eventData=" + b;
                    udon_comps.SendCustomEvent(_mem2);
                }
            }
            else
            {
                udon_comps.SendCustomEvent(_mem2);
            }
            return "Sent event " + _mem2 + (string.IsNullOrEmpty(dat) ? "" : " (" + dat + ")");
        }

        private string GetSetUdon(Transform tf, string _mem2, string valset)
        {
            if (tf == null)
            {
                return "[err] :No active Transform selected";
            }
            var udon_comps = tf.GetComponent<UdonSharpBehaviour>();
            if (udon_comps == null)
            {
                return "[err] : No UdonBehaviour component found on the selected Transform";
            }

            if (string.IsNullOrEmpty(_mem2))
            {
                return udon_comps.enabled.ToString();
            }

            if (_mem2 == "enable" || _mem2 == "active" || _mem2 == "isenabled" || _mem2 == "en")
            {
                if (string.IsNullOrEmpty(valset))
                {
                    return udon_comps.enabled.ToString();
                }

                if(TypParseBool(valset, out bool b))
                {
                    udon_comps.enabled = b;
                }
                return "set enabled to " + udon_comps.enabled.ToString();
            }

            var obj = udon_comps.GetProgramVariable(_mem2);
            if (obj == null)
            {
                return "[err] : Variable " + _mem2 + " not found";
            }
            var typ = obj.GetType();
            var val = FormatUdonValue(obj);

            if (string.IsNullOrEmpty(valset))
            {
                return ".u." + _mem2 + " : typ = " + typ + " , val = " + val;
            }
            
            {
                // valset = GetParsedValue(valset, typ);
                if(typ == typeof(bool))
                {  
                    if(TypParseBool(valset, out bool b))
                    {
                        udon_comps.SetProgramVariable(_mem2, b);
                        return "set " + _mem2 + " to " + b.ToString();
                    }
                }
                else if(typ == typeof(int))
                {
                    if(int.TryParse(valset, out int i))
                    {
                        udon_comps.SetProgramVariable(_mem2, i);
                        return "set " + _mem2 + " to " + i.ToString();
                    }
                }
                else if(typ == typeof(float))
                {
                    if(float.TryParse(valset, out float f))
                    {
                        udon_comps.SetProgramVariable(_mem2, f);
                        return "set " + _mem2 + " to " + f.ToString();
                    }
                }

                else if(typ == typeof(string))
                {
                    udon_comps.SetProgramVariable(_mem2, valset);
                    return "set " + _mem2 + " to " + valset;
                }

                else if(typ == typeof(Vector3))
                {
                    if(TryParseFloatList(valset, out float[] v3))
                    {
                        if(v3.Length != 3)
                        {
                            return "[err] : Set " + _mem2 + " failed, invalid value: " + valset;
                        }
                        udon_comps.SetProgramVariable(_mem2, new Vector3(v3[0], v3[1], v3[2]));
                        return "set " + _mem2 + " to " + new Vector3(v3[0], v3[1], v3[2]).ToString();
                    }
                }
                else if (typ == typeof(Vector3Int))
                {
                    if (TryParseIntList(valset, out int[] v3i))
                    {
                        if (v3i.Length != 3)
                        {
                            return "[err] : Set " + _mem2 + " failed, invalid value: " + valset;
                        }
                        udon_comps.SetProgramVariable(_mem2, new Vector3Int(v3i[0], v3i[1], v3i[2]));
                        return "set " + _mem2 + " to " + new Vector3Int(v3i[0], v3i[1], v3i[2]).ToString();
                    }
                }
                else if (typ == typeof(Vector2))
                {
                    if (TryParseFloatList(valset, out float[] v2f))
                    {
                        if (v2f.Length != 2)
                        {
                            return "[err] : Set " + _mem2 + " failed, invalid value: " + valset;
                        }
                        udon_comps.SetProgramVariable(_mem2, new Vector2(v2f[0], v2f[1]));
                        return "set " + _mem2 + " to " + new Vector2(v2f[0], v2f[1]).ToString();
                    }
                }
                else if (typ == typeof(Vector2Int))
                {
                    if (TryParseIntList(valset, out int[] v2i))
                    {
                        if (v2i.Length != 2)
                        {
                            return "[err] : Set " + _mem2 + " failed, invalid value: " + valset;
                        }
                        udon_comps.SetProgramVariable(_mem2, new Vector2Int(v2i[0], v2i[1]));
                        return "set " + _mem2 + " to " + new Vector2Int(v2i[0], v2i[1]).ToString();
                    }
                }
                else if (typ == typeof(int[]))
                {
                    // int 数组
                    if (!TryParseIntList(valset, out int[] intArray))
                    {
                        return "[err] : Set " + _mem2 + " failed, invalid value: " + valset;
                    }
                    udon_comps.SetProgramVariable(_mem2, intArray);
                    return "set " + _mem2 + " to " + intArray.ToString();
                }
                else if (typ == typeof(float[]))
                {
                    // float 数组
                    if (!TryParseFloatList(valset, out float[] floatArray))
                    {
                        return "[err] : Set " + _mem2 + " failed, invalid value: " + valset;
                    }
                    udon_comps.SetProgramVariable(_mem2, floatArray);
                    return "set " + _mem2 + " to " + floatArray.ToString();
                }
                else
                {
                    return "[err] : Set " + _mem2 + " failed, invalid value: " + valset;
                }
            }
            return "[err] : Unknown";
        }

        private string FormatUdonValue(object obj)
        {
            if (obj == null) return "null";

            // 使用 ToString 并针对已知类型稍作格式化
            // Udon 对反射支持有限，优先使用直接转型判断
            if (obj.GetType() == typeof(string)) return "\"" + (string)obj + "\"";
            if (obj.GetType() == typeof(bool)) return (bool)obj ? "true" : "false";
            if (obj.GetType() == typeof(int)) return ((int)obj).ToString();
            if (obj.GetType() == typeof(float)) return ((float)obj).ToString("0.##");

            if (obj.GetType() == typeof(Vector3)) { var v = (Vector3)obj; return "(" + v.x + ", " + v.y + ", " + v.z + ")"; }
            if (obj.GetType() == typeof(Vector2)) { var v = (Vector2)obj; return "(" + v.x + ", " + v.y + ")"; }
            if (obj.GetType() == typeof(Color)) { var c = (Color)obj; return "(" + c.r + ", " + c.g + ", " + c.b + ", " + c.a + ")"; }
            if (obj.GetType() == typeof(GameObject)) { var g = (GameObject)obj; return (g != null ? g.name : "null") + " (GameObject)"; }
            if (obj.GetType() == typeof(Transform)) { var t = (Transform)obj; return (t != null ? t.name : "null") + " (Transform)"; }

            // 数组：10 个一组，组间用 | 分隔
            if (obj.GetType() == typeof(int[]))
            {
                var arr = (int[])obj;
                var s = "[";
                for (int k = 0; k < arr.Length; k++)
                {
                    if (k > 0) s += (k % 10 == 0 ? " | " : ", ");
                    s += k + ":" + arr[k];
                }
                s += "]";
                return s;
            }
            if (obj.GetType() == typeof(float[]))
            {
                var arr = (float[])obj;
                var s = "[";
                for (int k = 0; k < arr.Length; k++)
                {
                    if (k > 0) s += (k % 10 == 0 ? " | " : ", ");
                    s += k + ":" + arr[k].ToString("0.##");
                }
                s += "]";
                return s;
            }
            if (obj.GetType() == typeof(string[]))
            {
                var arr = (string[])obj;
                var s = "[";
                for (int k = 0; k < arr.Length; k++)
                {
                    if (k > 0) s += (k % 10 == 0 ? " | " : ", ");
                    s += k + ":\"" + arr[k] + "\"";
                }
                s += "]";
                return s;
            }
            if (obj.GetType() == typeof(bool[]))
            {
                var arr = (bool[])obj;
                var s = "[";
                for (int k = 0; k < arr.Length; k++)
                {
                    if (k > 0) s += (k % 10 == 0 ? " | " : ", ");
                    s += k + ":" + (arr[k] ? "true" : "false");
                }
                s += "]";
                return s;
            }
            if (obj.GetType() == typeof(GameObject[]))
            {
                var arr = (GameObject[])obj;
                var s = "[";
                for (int k = 0; k < arr.Length; k++)
                {
                    if (k > 0) s += (k % 10 == 0 ? " | " : ", ");
                    s += k + ":" + (arr[k] != null ? arr[k].name : "null");
                }
                s += "]";
                return s;
            }
            if (obj.GetType() == typeof(Transform[]))
            {
                var arr = (Transform[])obj;
                var s = "[";
                for (int k = 0; k < arr.Length; k++)
                {
                    if (k > 0) s += (k % 10 == 0 ? " | " : ", ");
                    s += k + ":" + (arr[k] != null ? arr[k].name : "null");
                }
                s += "]";
                return s;
            }

            return obj.ToString();
        }
        #endregion udon

        #region unity compnent

        /// <summary>
        /// 设置Toggle组件的属性
        /// </summary>
        /// <param name="tf">要操作组件的Transform</param>
        /// <param name="_mem2">要设置的属性名</param>
        /// <param name="valset">要设置的属性值</param>
        /// <returns>设置结果</returns>
        private string GetSetToggle(Transform tf, string _mem2, string valset)
        {
            if (tf == null)
            {
                return "[err] :No active Transform selected";
            }
            var toggle_comp = tf.GetComponent<Toggle>();
            if (toggle_comp == null)
            {
                return "[err] : No Toggle component found on the selected Transform";
            }
            if (string.IsNullOrEmpty(valset))
            {
                return toggle_comp.isOn.ToString();
            }

            _mem2 = _mem2.ToLower();
            if (_mem2 == "ison" || _mem2 == "is_on" || _mem2 == "on" || _mem2 == "")
            {
                if(TypParseBool(valset, out bool b))
                {
                    toggle_comp.isOn = b;
                    return "set isOn to " + toggle_comp.isOn.ToString();
                }
                else
                {
                    return "[err] : Set isOn failed, invalid value: " + valset;
                }
            }
            
            else if (_mem2 == "enable" || _mem2 == "active" || _mem2 == "isenabled" || _mem2 == "en")
            {
                 if (TypParseBool(valset, out bool b))
                {
                    toggle_comp.enabled = b;    
                    return "set enabled to " + toggle_comp.enabled.ToString();
                }
                else
                {
                    return "[err] : Set enabled failed, invalid value: " + valset;
                }
            }
            else
            {
                return "[err] : Set isOn failed, invalid value: " + valset;
            }
            return "[err] : Set isOn failed, invalid value: " + valset;
        }

        private string GetSetText(Transform tf, string _mem2, string valset)
        {
            if (tf == null)
            {
                return "[err] :No active Transform selected";
            }
            var text_comp = tf.GetComponent<Text>();
            TMPro.TMP_Text tmp_comp = null;
            if (text_comp == null)
            {
                tmp_comp = tf.GetComponent<TMPro.TMP_Text>();
                if (tmp_comp == null)
                {
                    return "[err] : No Text or TextMeshPro/TMP_Text component found on the selected Transform";
                }
            }

            if (string.IsNullOrEmpty(valset))
            {
                if (text_comp != null)
                    return text_comp.text;
                else
                    return tmp_comp.text;
            }

            if (_mem2 == "text" || _mem2 == "" || _mem2 == "t")
            {
                MyParseValue(valset, out object val, out string typ);
                if (text_comp != null)
                    text_comp.text = val.ToString();
                else
                    tmp_comp.text = val.ToString(); 
                return val.ToString();
            }

            else if (_mem2 == "fontsize" || _mem2 == "fts")
            {
                if (text_comp != null)
                {
                    if (int.TryParse(valset, out int ftsInt))
                    {
                        text_comp.fontSize = ftsInt;
                        return "set fontsize to " + ftsInt.ToString();
                    }
                }
                else
                {
                    if (float.TryParse(valset, out float ftsFloat))
                    {
                        tmp_comp.fontSize = ftsFloat;
                        return "set fontsize to " + ftsFloat.ToString();
                    }
                }
                return "[err] : Set fontsize failed, invalid value: " + valset;
            }

            else if (_mem2 == "color" || _mem2 == "c")
            {
                if (TryParseFloatList(valset, out float[] list) && list.Length >= 3)
                {
                    Color col = new Color(list[0], list[1], list[2], list.Length >= 4 ? list[3] : 1f);
                    if (text_comp != null)
                        text_comp.color = col;
                    else
                        tmp_comp.color = col;
                    return "set color to " + col.ToString();
                }
                return "[err] : Set color failed, invalid value: " + valset;
            }

            else if (_mem2 == "enable" || _mem2 == "active" || _mem2 == "isenabled" || _mem2 == "en")
            {
                if (TypParseBool(valset, out bool b))
                {
                    if (text_comp != null)
                        text_comp.enabled = b;
                    else
                        tmp_comp.enabled = b;
                }
                else
                {
                    return "[err] : Set active failed, invalid value: " + valset;
                }
                return "set active to " + b.ToString();
            }

            return "[err] illege";
        }

        private string GetSetActiveState(Transform tf, string valset)
        {
            if (tf == null)
            {
                return "[err] :No active Transform selected";
            }
            if (string.IsNullOrEmpty(valset))
            {
                return tf.gameObject.activeSelf.ToString();
            }

           if(TypParseBool(valset, out bool b))
            {
                tf.gameObject.SetActive(b);
                return tf.gameObject.activeSelf.ToString();
            }
            else
            {
                return "[err] : Set active failed, invalid value: " + valset;
            }
        }

        // Helper method to get Transform property value and type
        private void GetSetTransformPropertyValue(Transform tf, string propertyName, string valset, out object val, out string typ)
        {
            
            if (propertyName == "")
            {
                val = $"name : {tf.name},active : {tf.gameObject.activeSelf}, position : {tf.position}, localPosition : {tf.localPosition}, localScale : {tf.localScale}";
                typ = "string";
                return;
            }

            var sp = propertyName.Split('.');
            propertyName = sp[0];
            switch (propertyName.ToLower())
            {
                case "position":
                case "p":
                    val = tf.position;
                    typ = "Vector3";

                    if (!string.IsNullOrEmpty(valset))
                    {
                        MyParseValue(valset, out object vpos, out string typpos);
                        if (typpos == "Vector3")
                        {
                            tf.position = (Vector3)vpos;
                            val = tf.position;
                            typ = "Vector3";
                            return;
                        }
                        else
                        {
                            val = $"[err] Set position failed, invalid value: {valset}";
                            typ = "[err]";
                            return;
                        }
                    }
                    return;

                case "localposition":
                case "lp":
                    val = tf.localPosition;
                    typ = "Vector3";

                    if (!string.IsNullOrEmpty(valset))
                    {
                        MyParseValue(valset, out object vpos, out string typpos);
                        if (typpos == "Vector3")
                        {
                            tf.localPosition = (Vector3)vpos;
                        }
                    }
                    return;
                case "localscale":
                case "ls":
                    val = tf.localScale;
                    typ = "Vector3";

                    if (!string.IsNullOrEmpty(valset))
                    {
                        MyParseValue(valset, out object vpos, out string typpos);
                        if (typpos == "Vector3")
                        {
                            tf.localScale = (Vector3)vpos;
                        }
                    }
                    return;

                case "quaternion":
                    val = tf.rotation;
                    typ = "Quaternion";
                    return;

                case "localrotation":
                case "lr":
                    val = tf.localRotation.eulerAngles;
                    typ = "Vector3";

                    if (!string.IsNullOrEmpty(valset))
                    {
                        MyParseValue(valset, out object vpos, out string typpos);
                        if (typpos == "Vector3")
                        {
                            tf.localRotation = Quaternion.Euler((Vector3)vpos);
                        }
                    }
                    return;

                case "eulerangles":
                case "euler":
                case "el":
                case "rotation":
                case "r":
                    val = tf.rotation.eulerAngles;
                    typ = "Vector3";

                    if (!string.IsNullOrEmpty(valset))
                    {
                        MyParseValue(valset, out object vpos, out string typpos);
                        if (typpos == "Vector3")
                        {
                            tf.rotation = Quaternion.Euler((Vector3)vpos);
                        }
                    }
                    return;
                case "localeulerangles":
                case "leuler":
                case "lel":
                    val = tf.localRotation.eulerAngles;
                    typ = "Vector3";

                    if (!string.IsNullOrEmpty(valset))
                    {
                        MyParseValue(valset, out object vpos, out string typpos);
                        if (typpos == "Vector3")
                        {
                            tf.localRotation = Quaternion.Euler((Vector3)vpos);
                        }
                    }
                    return;
                default:
                    val = "null";
                    typ = "null";
                    return;
            }
        }

        private string GetSetImage(Transform tf, string _mem2, string valset)
        {
            if (tf == null)
            {
                return "[err] :No active Transform selected";
            }
            var imgComp = tf.GetComponent<UnityEngine.UI.Image>();
            var rawComp = tf.GetComponent<UnityEngine.UI.RawImage>();
            if (imgComp == null && rawComp == null)
            {
                return "[err] : No Image or RawImage component found";
            }

            // no sub-member specified — return current state
            if (string.IsNullOrEmpty(_mem2))
            {
                if (imgComp != null)
                    return "Image: enabled=" + imgComp.enabled + ", color=" + imgComp.color;
                else
                    return "RawImage: enabled=" + rawComp.enabled + ", color=" + rawComp.color;
            }

            _mem2 = _mem2.ToLower();

            if (_mem2 == "en" || _mem2 == "active" || _mem2 == "isenabled" || _mem2 == "enable")
            {
                if (string.IsNullOrEmpty(valset))
                {
                    return imgComp != null ? imgComp.enabled.ToString() : rawComp.enabled.ToString();
                }
                if (TypParseBool(valset, out bool b))
                {
                    if (imgComp != null) imgComp.enabled = b;
                    else rawComp.enabled = b;
                    return "set enabled to " + b;
                }
                return "[err] : Set enabled failed, invalid value: " + valset;
            }

            if (_mem2 == "color" || _mem2 == "c")
            {
                if (string.IsNullOrEmpty(valset))
                {
                    if (imgComp != null) return imgComp.color.ToString();
                    else return rawComp.color.ToString();
                }
                if (TryParseFloatList(valset, out float[] list) && list.Length >= 3)
                {
                    Color col = new Color(list[0], list[1], list[2], list.Length >= 4 ? list[3] : 1f);
                    if (imgComp != null) imgComp.color = col;
                    else rawComp.color = col;
                    return "set color to " + col;
                }
                return "[err] : Set color failed, invalid value: " + valset;
            }

            return "[err] illege";
        }

        private string GetSetSlider(Transform tf, string _mem2, string valset)
        {
            if (tf == null)
            {
                return "[err] :No active Transform selected";
            }
            var sliderComp = tf.GetComponent<UnityEngine.UI.Slider>();
            if (sliderComp == null)
            {
                return "[err] : No Slider component found";
            }

            // no sub-member specified — return current state
            if (string.IsNullOrEmpty(_mem2))
            {
                return "Slider: value=" + sliderComp.value + ", min=" + sliderComp.minValue
                    + ", max=" + sliderComp.maxValue + ", enabled=" + sliderComp.enabled
                    + ", interactable=" + sliderComp.interactable;
            }

            _mem2 = _mem2.ToLower();

            if (_mem2 == "val" || _mem2 == "value" || _mem2 == "")
            {
                if (string.IsNullOrEmpty(valset))
                    return sliderComp.value.ToString();
                if (float.TryParse(valset, out float v))
                {
                    sliderComp.value = v;
                    return "set value to " + v;
                }
                return "[err] : Set value failed, invalid value: " + valset;
            }

            if (_mem2 == "max" || _mem2 == "maxval" || _mem2 == "maxvalue")
            {
                if (string.IsNullOrEmpty(valset))
                    return sliderComp.maxValue.ToString();
                if (float.TryParse(valset, out float v))
                {
                    sliderComp.maxValue = v;
                    return "set maxValue to " + v;
                }
                return "[err] : Set maxValue failed, invalid value: " + valset;
            }

            if (_mem2 == "min" || _mem2 == "minval" || _mem2 == "minvalue")
            {
                if (string.IsNullOrEmpty(valset))
                    return sliderComp.minValue.ToString();
                if (float.TryParse(valset, out float v))
                {
                    sliderComp.minValue = v;
                    return "set minValue to " + v;
                }
                return "[err] : Set minValue failed, invalid value: " + valset;
            }

            if (_mem2 == "en" || _mem2 == "active" || _mem2 == "isenabled" || _mem2 == "enable")
            {
                if (string.IsNullOrEmpty(valset))
                    return sliderComp.enabled.ToString();
                if (TypParseBool(valset, out bool b))
                {
                    sliderComp.enabled = b;
                    return "set enabled to " + b;
                }
                return "[err] : Set enabled failed, invalid value: " + valset;
            }

            if (_mem2 == "interactable" || _mem2 == "int")
            {
                if (string.IsNullOrEmpty(valset))
                    return sliderComp.interactable.ToString();
                if (TypParseBool(valset, out bool b))
                {
                    sliderComp.interactable = b;
                    return "set interactable to " + b;
                }
                return "[err] : Set interactable failed, invalid value: " + valset;
            }

            return "[err] illege";
        }

        private string GetSetAudioSource(Transform tf, string _mem2, string valset)
        {
            if (tf == null)
            {
                return "[err] :No active Transform selected";
            }
            var aud = tf.GetComponent<AudioSource>();
            if (aud == null)
            {
                return "[err] : No AudioSource component found";
            }

            // no sub-member — return summary
            if (string.IsNullOrEmpty(_mem2))
            {
                return "AudioSource: mute=" + aud.mute + ", loop=" + aud.loop
                    + ", enabled=" + aud.enabled + ", volume=" + aud.volume
                    + ", pitch=" + aud.pitch + ", minDist=" + aud.minDistance
                    + ", maxDist=" + aud.maxDistance;
            }

            _mem2 = _mem2.ToLower();

            if (_mem2 == "mute")
            {
                if (string.IsNullOrEmpty(valset))
                    return aud.mute.ToString();
                if (TypParseBool(valset, out bool b))
                {
                    aud.mute = b;
                    return "set mute to " + b;
                }
                return "[err] : Set mute failed, invalid value: " + valset;
            }

            if (_mem2 == "loop")
            {
                if (string.IsNullOrEmpty(valset))
                    return aud.loop.ToString();
                if (TypParseBool(valset, out bool b))
                {
                    aud.loop = b;
                    return "set loop to " + b;
                }
                return "[err] : Set loop failed, invalid value: " + valset;
            }

            if (_mem2 == "en" || _mem2 == "active" || _mem2 == "isenabled" || _mem2 == "enable")
            {
                if (string.IsNullOrEmpty(valset))
                    return aud.enabled.ToString();
                if (TypParseBool(valset, out bool b))
                {
                    aud.enabled = b;
                    return "set enabled to " + b;
                }
                return "[err] : Set enabled failed, invalid value: " + valset;
            }

            if (_mem2 == "volume" || _mem2 == "vol")
            {
                if (string.IsNullOrEmpty(valset))
                    return aud.volume.ToString();
                if (float.TryParse(valset, out float v))
                {
                    aud.volume = v;
                    return "set volume to " + v;
                }
                return "[err] : Set volume failed, invalid value: " + valset;
            }

            if (_mem2 == "pitch")
            {
                if (string.IsNullOrEmpty(valset))
                    return aud.pitch.ToString();
                if (float.TryParse(valset, out float v))
                {
                    aud.pitch = v;
                    return "set pitch to " + v;
                }
                return "[err] : Set pitch failed, invalid value: " + valset;
            }

            if (_mem2 == "mindistance" || _mem2 == "mindist" || _mem2 == "min")
            {
                if (string.IsNullOrEmpty(valset))
                    return aud.minDistance.ToString();
                if (float.TryParse(valset, out float v))
                {
                    aud.minDistance = v;
                    return "set minDistance to " + v;
                }
                return "[err] : Set minDistance failed, invalid value: " + valset;
            }

            if (_mem2 == "maxdistance" || _mem2 == "maxdist" || _mem2 == "max")
            {
                if (string.IsNullOrEmpty(valset))
                    return aud.maxDistance.ToString();
                if (float.TryParse(valset, out float v))
                {
                    aud.maxDistance = v;
                    return "set maxDistance to " + v;
                }
                return "[err] : Set maxDistance failed, invalid value: " + valset;
            }

            return "[err] illege";
        }

        private string GetSetBoxCollider(Transform tf, string _mem2, string valset)
        {
            if (tf == null)
            {
                return "[err] :No active Transform selected";
            }
            var bc = tf.GetComponent<BoxCollider>();
            if (bc == null)
            {
                return "[err] : No BoxCollider component found";
            }

            if (string.IsNullOrEmpty(_mem2))
            {
                return "BoxCollider: enabled=" + bc.enabled + ", isTrigger=" + bc.isTrigger;
            }

            _mem2 = _mem2.ToLower();

            if (_mem2 == "en" || _mem2 == "active" || _mem2 == "isenabled" || _mem2 == "enable")
            {
                if (string.IsNullOrEmpty(valset))
                    return bc.enabled.ToString();
                if (TypParseBool(valset, out bool b))
                {
                    bc.enabled = b;
                    return "set enabled to " + b;
                }
                return "[err] : Set enabled failed, invalid value: " + valset;
            }

            if (_mem2 == "istrigger" || _mem2 == "trigger" || _mem2 == "trig")
            {
                if (string.IsNullOrEmpty(valset))
                    return bc.isTrigger.ToString();
                if (TypParseBool(valset, out bool b))
                {
                    bc.isTrigger = b;
                    return "set isTrigger to " + b;
                }
                return "[err] : Set isTrigger failed, invalid value: " + valset;
            }

            return "[err] illege";
        }

        #endregion unity compnent

        #region Parse Value

        public string SubStringList(string valset)
        {
            if (valset.StartsWith("(") && valset.EndsWith(")"))
            {
                valset = valset.Substring(1, valset.Length - 2);
                return valset;
            }
            else if (valset.StartsWith("[") && valset.EndsWith("]"))
            {
                valset = valset.Substring(1, valset.Length - 2);
                return valset;  
            }
            else if (valset.StartsWith("{") && valset.EndsWith("}"))
            {
                valset = valset.Substring(1, valset.Length - 2);
                return valset;  
            }
            else
            {
                return "";
            }
        }

        public bool TryParseIntList(string valset, out int[] intArray)
        {
            intArray = null;
            valset = SubStringList(valset);
            var sp = valset.Split(',');
            if(sp.Length == 0)
            {
                return false;
            }
            intArray = new int[sp.Length];
            for (int i = 0; i < sp.Length; i++)
            {
                if(int.TryParse(sp[i], out int j))
                {
                    intArray[i] = j;
                }
                else
                {
                    return false;
                }
            }
            return true;    
        }

        public bool TryParseFloatList(string valset, out float[] floatArray)
        {
            floatArray = null;
            valset = SubStringList(valset);
            var sp = valset.Split(',');
            if (sp.Length == 0)
            {
                return false;
            }
            floatArray = new float[sp.Length];
            for (int i = 0; i < sp.Length; i++)
            {
                if (float.TryParse(sp[i], out float j))
                {
                    floatArray[i] = j;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        private void MyParseValue(string valueStr, out object value, out string typ)
        {
            if (valueStr.StartsWith("$")) // variable reference
            {
                valueStr = GetVariable(valueStr.Substring(1).Trim());
                if (string.IsNullOrEmpty(valueStr))
                {
                    value = "no find msg";
                    typ = "[err]";
                    return;
                }
            }

            // Remove parentheses if present
            valueStr = valueStr.Trim();
            {
                if (valueStr.StartsWith("\'") && valueStr.EndsWith("\'"))
                {
                    value = valueStr.Substring(1, valueStr.Length - 2);
                    typ = "string";
                    return;
                }

                else if (valueStr.StartsWith("\"") && valueStr.EndsWith("\""))
                {
                    value = valueStr.Substring(1, valueStr.Length - 2);
                    typ = "string";
                    return;
                }
            }

            {
                if (valueStr.StartsWith("(") && valueStr.EndsWith(")"))
                {
                    valueStr = valueStr.Substring(1, valueStr.Length - 2).Trim();
                }

                // Try to parse as Vector3 (e.g., "1, 2, 3")
                string[] parts = valueStr.Split(',');
                if (parts.Length == 3)
                {

                    float x = float.Parse(parts[0].Trim());
                    float y = float.Parse(parts[1].Trim());
                    float z = float.Parse(parts[2].Trim());

                    value = new Vector3(x, y, z);
                    typ = "Vector3";
                    return;
                }

                // Try to parse as Quaternion (e.g., "1, 2, 3, 4")
                if (parts.Length == 4)
                {

                    float x = float.Parse(parts[0].Trim());
                    float y = float.Parse(parts[1].Trim());
                    float z = float.Parse(parts[2].Trim());
                    float w = float.Parse(parts[3].Trim());
                    value = new Quaternion(x, y, z, w);
                    typ = "Quaternion";
                    return;
                }

                // Try to parse as float
                if (float.TryParse(valueStr, out float floatValue))
                {
                    value = floatValue;
                    typ = "float";
                    return;
                }

                // Try to parse as int
                if (int.TryParse(valueStr, out int intValue))
                {
                    value = intValue;
                    typ = "int";
                    return;
                }

                // Try to parse as bool
                if (bool.TryParse(valueStr, out bool boolValue))
                {
                    value = boolValue;
                    typ = "bool";
                    return;
                }
            }

            {
                if (valueStr == "on" || valueStr == "true")
                {
                    value = true;
                    typ = "bool";
                    return;
                }
                else if (valueStr == "off" || valueStr == "false")
                {
                    value = false;
                    typ = "bool";
                    return;
                }
            }

            // Return as string if no other type matches
            value = valueStr;
            typ = "string";
            return;
        }

        public bool TypParseBool(string valueStr, out bool result)
        {
            valueStr = valueStr.Trim().ToLower();
            if (valueStr.StartsWith("$"))
            {
                valueStr = GetVariable(valueStr.Substring(1).Trim());
                if (string.IsNullOrEmpty(valueStr))
                {
                    result = false;
                    return false;
                }
            }

            if (valueStr == "true" || valueStr == "1" || valueStr == "on")
            {
                result = true;
                return true;
            }
            if (valueStr == "false" || valueStr == "0" || valueStr == "off")
            {
                result = false;
                return true;
            }
            result = false;
            return false;
        }

        /// <summary>
        /// Try parse string to Color via TryParseFloatList
        /// Supports: (r, g, b) or (r, g, b, a), brackets optional
        /// alpha defaults to 1
        /// </summary>
        public bool TryParseColor(string valset, out Color result)
        {
            result = Color.white;
            if (string.IsNullOrEmpty(valset)) return false;

            if (valset.StartsWith("$"))
            {
                valset = GetVariable(valset.Substring(1).Trim());
                if (string.IsNullOrEmpty(valset)) return false;
            }

            if (!TryParseFloatList(valset, out float[] list)) return false;
            if (list.Length < 3 || list.Length > 4) return false;

            result = new Color(list[0], list[1], list[2], list.Length >= 4 ? list[3] : 1f);
            return true;
        }

        #endregion

        #endregion Command Processing

        #region Input cmdline
        private string _pre_cmd = ">>>";

        public void InputCommand(string command)
        {
            // 去除换行符
            command = command.Replace("\n", "").Replace("\r", "");
            if (string.IsNullOrEmpty(command))
            {
                return;
            }
            if (command[0] == '>')
            {
                command = command.Substring(1).Trim();
            }
            PrintLine(_pre_cmd + command);
            if(string.IsNullOrEmpty(command))
            {
                return;
            }
            AddCommandToHistory(command);
            ProcessCommand(command);
        }

        public void InputCommandText(string command)
        {
            // \r\n 分割成不同行命令
            var ss = command.Split('\n');
            if(ss.Length == 0)
            {
                return;
            }
            for(int i = 0; i < ss.Length; i++)
            {
                if(string.IsNullOrEmpty(ss[i]))
                {
                    continue;
                }
                var s = ss[i].Trim();

                if(ss[i].StartsWith("#cmd"))
                {
                    s = s.Substring(5).Trim();                
                }
                else if(s.StartsWith("#") || s.StartsWith("//"))
                {
                    continue;
                }
                // msgid 转为 "cd "
               else if(ss[i].StartsWith("msgid"))
                {
                    s = s.Substring(5).Trim();
                    s = "cd " + s;
                }
                // msgstr 转为 ".t = "
                else if(ss[i].StartsWith("msgstr"))
                {
                    s = s.Substring(6).Trim();
                    s = ".t = " + s;
                }

                PrintLine(GetVariable("pwd") + ">>> " + s);
                ProcessCommand(s);
            }								
        }
		
        public object eventData;
        public object eventData1;
        public object eventData2;
        public void ex()
        {
            var s = (string) this.eventData;
            InputCommandText(s);
        }

        #endregion Input cmdline

        #region History System
        // 环形缓冲区历史命令系统
        private string[] historyCmd;
        [SerializeField] public int historyMaxLines = 50;
        private int historyCurrentIndex = 0;  // 当前写入位置
        private int historySize = 0;          // 历史记录实际数量
        private int historyNavigateIndex = 0; // 导航时的当前索引
        private bool isNavigatingHistory = false; // 是否正在浏览历史

        // 初始化历史命令缓冲区
        private void InitializeHistory()
        {
            if (historyCmd == null || historyCmd.Length != historyMaxLines)
            {
                historyCmd = new string[historyMaxLines];
                historyCurrentIndex = 0;
                historySize = 0;
                historyNavigateIndex = 0;
                isNavigatingHistory = false;
            }
        }

        private void AddCommandToHistory(string command)
        {
            if (string.IsNullOrEmpty(command)) return;            
            // 确保历史缓冲区已初始化
            InitializeHistory();            
            // 添加到环形缓冲区
            historyCmd[historyCurrentIndex] = command;
            historyCurrentIndex = (historyCurrentIndex + 1) % historyMaxLines;            
            // 更新历史记录数量
            if (historySize < historyMaxLines)
            {
                historySize++;
            }            
            // 重置导航状态
            isNavigatingHistory = false;
            historyNavigateIndex = historyCurrentIndex;
        }

        public void OnCmdLineUpArrow()
        {
            if (historySize == 0) return;

            if (!isNavigatingHistory)
            {
                // navigate history from the latest
                isNavigatingHistory = true;
                historyNavigateIndex = historyCurrentIndex == 0 ? historySize - 1 : historyCurrentIndex - 1;
            }
            else
            {
                // navigate up
                int prevIndex = (historyNavigateIndex - 1 + historyMaxLines) % historyMaxLines;
                
                if (historySize < historyMaxLines && prevIndex >= historySize - 1)
                {
                    // reached earliest record, stay at first
                    historyNavigateIndex = 0;
                }
                else if (historySize == historyMaxLines && prevIndex == (historyCurrentIndex - 1 + historyMaxLines) % historyMaxLines)
                {
                    // buffer full and reached earliest
                    ClearCmdLine();
                    return;
                }
                else
                {
                    historyNavigateIndex = prevIndex;
                }
            }            
            SetCmdLine(">" + historyCmd[historyNavigateIndex]);
        }

        public void OnCmdLineDownArrow()
        {
            if (historySize == 0 || !isNavigatingHistory) return;

            int nextIndex = (historyNavigateIndex + 1) % historyMaxLines;

            if (nextIndex == historyCurrentIndex || (historySize < historyMaxLines && nextIndex >= historySize))
            {
                // back to current input
                isNavigatingHistory = false;
                SetCmdLine(">");
                return;
            }

            historyNavigateIndex = nextIndex;
            SetCmdLine(">" + historyCmd[historyNavigateIndex]);
        }

        private string ShowHistory()
        {
            if (historyCmd == null)
            {
                return "";
            }
            var s = "Command History:";
            for (int i = 0; i < historyCmd.Length; i++)
            {
                s += "  " + (i + 1) + ": " + historyCmd[i] + "\n";
            }
            return s;
        }

        #endregion History System

        #region Screen Methods extern API
        
        public UdonBehaviour _udon_api;
        public Text _text1;

        private void ClearCommand()
        {
            ClearScreen();
        }

        private void PrintLine(string text)
        {
            if (_udon_api != null)
            {
                _udon_api.SetProgramVariable("eventData", text);
                _udon_api.SendCustomEvent("Evn_PrintLine");
            }
            if(_text1 != null)
                _text1.text += text + "\n";
            LogMessage(text);
        }

        private void ClearScreen()
        {
            if (_udon_api != null)
            {
                _udon_api.SendCustomEvent("Evn_ClearScreen");
            }
            if (_text1 != null)
                _text1.text = "";
        }

        private void SetCmdLine(string text)
        {
            if (_udon_api != null)
            {
                _udon_api.SetProgramVariable("eventData", text);
                _udon_api.SendCustomEvent("Evn_SetCmdLine");
            }
        }

        public void ClearCmdLine()
        {
            if(_udon_api != null)
                return;
            _udon_api.SendCustomEvent("Evn_ClearCmdLine");
        }

        #endregion Screen Methods extern API

        public void LogMessage(string message)
        {
            Debug.Log($"-------------- shell [{System.DateTime.Now}] {message}");
        }
    }
}


																							 