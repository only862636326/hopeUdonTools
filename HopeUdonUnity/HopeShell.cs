
using librsync.net;
using Newtonsoft.Json.Linq;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace HopeTools
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class HopeShell : UdonSharpBehaviour
    {
        [SerializeField] public int maxHistoryLines = 50;
        // Udon compatible arrays instead of Lists and Dictionaries
        private string[] commandHistory;
        private int historyCount = 0;
        private int currentHistoryIndex = -1;
        private string currentInput = "";

        // Variables for triple Enter detection 
        private float tripleEnterTimeWindow = 1.0f; // 1 second window for triple press

        // Simple variable storage using arrays`
        private string[] variableNames;
        private string[] variableValues;
        private string[] variableType;
        private int variableCount = 0;
        [SerializeField] private int maxVariables = 20;

        [SerializeField] private Transform active_transform;
        public Transform[] _managered_transform;

        private bool _compar_result = false;
        void Start()
        {
            // Initialize arrays
            commandHistory = new string[maxHistoryLines];

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
            if (Input.GetKeyDown(KeyCode.Return))
            {
                OnCmdLineSubmit();
            }
        }

        private void AddCommandToHistory(string command)
        {
            if (string.IsNullOrEmpty(command)) return;

            // Add to history (circular buffer)
            commandHistory[historyCount % maxHistoryLines] = command;
            historyCount++;
            currentHistoryIndex = historyCount;
        }

        private void ProcessCommand(string command)
        {
            if (string.IsNullOrWhiteSpace(command)) return;

            // Check if this is a member variable access (e.g., ".variableName")
            if (command.StartsWith(".") && command.Length > 1)
            {
                var ret = MemberVariableAccess(active_transform, command);
                PrintLine(ret);
                return;
            }

            // Process variable access and assignment with $ symbol
            if (command.StartsWith("$") && command.Length > 1)
            {
                var ret = ProcessVariableAccess(command);
                PrintLine(ret);
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
                ShowHistory();
            }
            else if (cmd == "time")
            {
                ShowTime();
            }
            else if (cmd == "ls")
            {
                ListChildren();
            }
            else if (cmd == "cd")
            {
                ChangeDirectory(par);
            }

            else if (cmd == "addroot")
            {
                FindAndAddTfToManager(par);
            }

            else
            {
                PrintLine("Command not found: " + cmd + ". Type 'help' for available commands.");
            }
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

        private void ShowHelp()
        {
            PrintLine("Available commands:");
            PrintLine("  help     - Show this help message");
            PrintLine("  clear    - Clear the console");
            PrintLine("  history  - Show command history");
            PrintLine("  vars     - List all variables");
            PrintLine("  time     - Show current time");
            PrintLine("  ls       - List children of active_transform");
            PrintLine("  cd       - Change directory (usage: cd PATH)");
            PrintLine("            - Use '..' to go to parent directory");
            PrintLine("            - Use '/path/to/object' for absolute path");
            PrintLine("            - Use 'path/to/object' for relative path");
            PrintLine("            - Use quotes for names with spaces: cd \"My Object\"");
            PrintLine("  addroot  - Add a transform to manager (usage: addroot PATH)");
            PrintLine("  $var     - Access variable value (usage: $variableName)");
            PrintLine("            - Variables can be used in assignment: newVar = $oldVar");
            PrintLine("            - Mixed strings: msg = \"Hello $name, value is $val\"");
            PrintLine("            - Assignment with $ prefix: $var = 6 or $var = \"value\"");
            PrintLine("  $pwd     - System variable that stores current directory path");
            PrintLine("            - Automatically updated when using cd command");
            PrintLine("            - Read-only variable (cannot be assigned directly)");
            PrintLine("  .member  - Access UdonSharpBehaviour member variable");
            PrintLine("  .active  - Get or set GameObject active state");
            PrintLine("  .transform- Get Transform component information");
            PrintLine("  .transform.member - Access Transform member variable");
            PrintLine("  .member = value - Set UdonSharpBehaviour member variable");
            PrintLine("  .transform.member = value - Set Transform member variable");
            PrintLine("  .active = true/false - Set GameObject active state");
        }



        // Helper method to process variable substitution in strings

        private void ClearCommand()
        {
            ClearScreen();
        }

        private void ShowHistory()
        {
            PrintLine("Command History:");
            for (int i = 0; i < historyCount; i++)
            {
                PrintLine("  " + (i + 1) + ": " + commandHistory[i]);
            }
        }

        // Helper method to set a variable
        private void SetVariable(string name, string value)
        {
            for (int i = 0; i < variableCount; i++)
            {
                if (value == variableNames[i])
                {
                    value = variableValues[i];
                    return;
                }
            }

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

        private void ListVariables()
        {
            PrintLine("Variables:");
            for (int i = 0; i < variableCount; i++)
            {
                PrintLine("  " + variableNames[i] + " = " + variableValues[i]);
            }
        }

        private void ShowTime()
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
            PrintLine("Current time: " + currentTime);
        }

        private void ListChildren()
        {
            var s = "";
            if (active_transform == null)
            {
                for (int i = 0; i < _managered_transform.Length; i++)
                {
                    if (_managered_transform[i] == null) continue;
                    s += _managered_transform[i].name + ",";
                }
                PrintLine(s);
            }
            else
            {
                for (int i = 0; i < active_transform.childCount; i++)
                {
                    s += active_transform.GetChild(i).name + ",";
                }
                PrintLine(s);
            }
        }

        private string MemberVariableEq(Transform tf, string memberPath, string value)
        {
            memberPath = memberPath.ToLower();
            if (memberPath.StartsWith("."))
            {
                memberPath = memberPath.Substring(1);
            }

            int equalPos = memberPath.IndexOf(".");

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

            switch (_mem1)
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
                    return GetSetToggle(tf,_mem2 ,value);
                    break;
                case "udon":
                    return GetSetUdon(tf, _mem2, value);
                    break;
                default:
                    ;
                    break;
            }
            return "[err] illege";
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

                ParseValue(valset, out object val, out string typ);
                if (typ == "bool")
                {
                    udon_comps.enabled = (bool)val;
                }
            }

            if (string.IsNullOrEmpty(valset))
            {
                return udon_comps.GetProgramVariable(_mem2).ToString();
            }
            {
                ParseValue(valset, out object val, out string typ);
                udon_comps.SetProgramVariable(_mem2, val);
                return "set " + _mem2 + " to " + valset;
            }
        }

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

            if (_mem2 == "ison" || _mem2 == "is_on" || _mem2 == "on" || _mem2 == "")
            {
                ParseValue(valset, out object val, out string typ);
                if (typ == "bool")
                {
                    toggle_comp.isOn = (bool)val;
                    return "set isOn to " + toggle_comp.isOn.ToString();
                }                
                else
                {
                    return "[err] : Set isOn failed, invalid value: " + valset;
                }
            }
            
            else if (_mem2 == "enable" || _mem2 == "active" || _mem2 == "isenabled" || _mem2 == "en")
            {
                ParseValue(valset, out object val, out string typ);
                if (typ == "bool")
                {
                    toggle_comp.enabled = (bool)val;
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
            if (text_comp == null)
            {
                return "[err] : No Text component found on the selected Transform";
            }
            if (string.IsNullOrEmpty(valset))
            {
                return text_comp.text;
            }


            if (_mem2 == "text" || _mem2 == "" || _mem2 == "t")
            {
                ParseValue(valset, out object val, out string typ);
                text_comp.text = val.ToString();
                return text_comp.text;
            }

            else if (_mem2 == "fontsize" || _mem2 == "fts")
            {
                ParseValue(valset, out object val, out string typ);
                if (typ == "int")
                {
                    text_comp.fontSize = (int)val;
                }
                else
                {
                    return "[err] : Set fontsize failed, invalid value: " + valset;
                }
                return "set fontsize to " + text_comp.fontSize.ToString();
            }

            else if (_mem2 == "enable" || _mem2 == "active" || _mem2 == "isenabled" || _mem2 == "en")
            {
                ParseValue(valset, out object val, out string typ);
                if (typ == "bool")
                {
                    text_comp.enabled = (bool)val;
                }
                else
                {
                    return "[err] : Set active failed, invalid value: " + valset;
                }
                return "set active to " + text_comp.enabled.ToString();
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

            ParseValue(valset, out object val, out string typ);
            if (typ == "bool")
            {
                tf.gameObject.SetActive((bool)val);
                return tf.gameObject.activeSelf.ToString();
            }
            else
            {
                return "[err] : Set active failed, invalid value: " + valset;
            }
        }

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
                        ParseValue(valset, out object vpos, out string typpos);
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
                        ParseValue(valset, out object vpos, out string typpos);
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
                        ParseValue(valset, out object vpos, out string typpos);
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
                        ParseValue(valset, out object vpos, out string typpos);
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
                        ParseValue(valset, out object vpos, out string typpos);
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
                        ParseValue(valset, out object vpos, out string typpos);
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

        private void ParseValue(string valueStr, out object value, out string typ)
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

        private void FindAndAddTfToManager(string path)
        {
            path = path.Trim();
            if (string.IsNullOrEmpty(path))
            {
                PrintLine("Path is empty.");
                return;
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
                PrintLine("No such GameObject: " + path);
                return;
            }

            for (int i = 0; i < _managered_transform.Length; i++)
            {
                if (_managered_transform[i] == null)
                {
                    _managered_transform[i] = tf;
                    PrintLine("Add " + path + " to manager.");
                    return;
                }
            }
            PrintLine("_managered_transform is Full");
        }

        private string ChangeDirectory(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return "[err] dir is null !!!";
            }

            if (path.StartsWith("$"))
            {
                path = path.Substring(1);
                path = GetVariable(path);
            }

            if (path.StartsWith("/"))
            {
                path = path.Substring(1);
                var sp = path.Split('/', 2);
                for (int i = 0; i < _managered_transform.Length; i++)
                {
                    if (_managered_transform[i] != null && _managered_transform[i].name == sp[0])
                    {
                        active_transform = _managered_transform[i];
                        if (sp.Length > 1)
                        {
                            path = sp[1];
                        }
                        else
                        {
                            path = "";
                        }
                        break;
                    }
                }
            }

            // 递归处理路径导航，直接传入原始路径
            Transform targetTransform = NavigateToPath(active_transform, path);
            active_transform = targetTransform;
            if (targetTransform != null)
            {
                return GenerPwdVar(targetTransform);
            }

            else
            {
                return "[err] No such GameObject: " + path;
            }
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

        // 递归导航到指定路径，同时处理路径解析
        private Transform NavigateToPath(Transform current, string path)
        {
            // 跳过路径开头的空格
            while (path.Length > 0 && char.IsWhiteSpace(path[0]))
            {
                path = path.Substring(1);
            }

            // 基本情况：已经处理完整个路径
            if (string.IsNullOrEmpty(path))
            {
                return current;
            }

            // 解析当前路径部分
            string part;
            string remainingPath;

            // 处理引号包围的部分
            if (path[0] == '\'' || path[0] == '\"')
            {
                char quote = path[0];
                path = path.Substring(1); // 跳过开始引号

                int partEnd = path.IndexOf(quote);
                if (partEnd == -1)
                {
                    PrintLine("未闭合的引号");
                    return null;
                }

                part = path.Substring(0, partEnd);
                remainingPath = path.Substring(partEnd + 1); // 跳过结束引号
            }
            else
            {
                // 处理普通路径部分，直到遇到分隔符或路径结束
                int partEnd = path.IndexOf('/');
                if (partEnd == -1)
                {
                    part = path;
                    remainingPath = "";
                }
                else
                {
                    part = path.Substring(0, partEnd);
                    remainingPath = path.Substring(partEnd + 1); // 跳过分隔符
                }
            }

            // 处理 ".." 返回父目录
            if (part == "..")
            {
                if (current != null && current.parent != null)
                {
                    return NavigateToPath(current.parent, remainingPath);
                }
                else if (current == null)
                {
                    // 在根目录时，尝试从管理器中查找
                    // 跳过下一个部分
                    int nextSlash = remainingPath.IndexOf('/');
                    string nextPart = (nextSlash == -1) ? remainingPath : remainingPath.Substring(0, nextSlash);
                    string skipPath = (nextSlash == -1) ? "" : remainingPath.Substring(nextSlash + 1);

                    Transform rootTransform = FindChildTransform(null, nextPart);
                    if (rootTransform != null)
                    {
                        return NavigateToPath(rootTransform, skipPath);
                    }
                    else
                    {
                        PrintLine($"找不到根物体: {nextPart}");
                        return null;
                    }
                }
                else
                {
                    PrintLine("已在根目录，无法返回上级");
                    return null;
                }
            }

            // 处理 "." 或空字符串（当前目录）
            if (part == "." || string.IsNullOrEmpty(part))
            {
                return NavigateToPath(current, remainingPath);
            }

            // 查找子物体
            Transform childTransform = FindChildTransform(current, part);
            if (childTransform != null)
            {
                return NavigateToPath(childTransform, remainingPath);
            }
            else
            {
                PrintLine($"找不到子物体: {part}");
                return null;
            }
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

        private string _pre_cmd = ">>>";

        public void InputCommand(string command)
        {
            if (command[0] == '>')
            {
                command = command.Substring(1).Trim();
            }
            PrintLine(_pre_cmd + command);
            clear_cmdline();
            AddCommandToHistory(command);
            ProcessCommand(command);
        }

        public InputField cmd_line;
        public void OnCmdLineSubmit()
        {
            if (cmd_line != null)
            {
                string command = cmd_line.text.Trim();
                LogMessage($"CmdLine Submit: {command}");
                if (!string.IsNullOrEmpty(command))
                {
                    InputCommand(command);
                }
            }
        }

        public void clear_cmdline()
        {
            if (cmd_line != null)
            {
                cmd_line.text = ">";
            }
        }

        public void LogMessage(string message)
        {
            Debug.Log($"-------------- shell [{System.DateTime.Now}] {message}");
        }


        #region toekng get 


        #endregion

                #region Screen Methods extern API
        public HopeShellScreen _screen;
        private void PrintLine(string text)
        {
            if (_screen != null)
                _screen.PrintLine(text);
        }
        private void ClearScreen()
        {
            if (_screen != null)
                _screen.ClearScreen();
        }
        #endregion Screen Methods extern API
        private void PrintPrompt()
        {
            ;
        }
    }
}