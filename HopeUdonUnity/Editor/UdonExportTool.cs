using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;
using VRC.Udon;
using TMPro;
using UdonSharpEditor;
using UdonSharp;

public class UdonExportTool : Editor
{
    // ==================== ExportTextPo ====================

    [MenuItem("GameObject/MyTool/ExportTextPo", false, 30)]
    static void ExportTextPo()
    {
        var selectN = Selection.gameObjects.Length;
        if (selectN != 1)
        {
            Debug.LogWarning("Please select exactly one GameObject as root.");
            return;
        }

        var root = Selection.gameObjects[0].transform;

        var uiTexts = root.GetComponentsInChildren<Text>(true);
        var tmpTexts = root.GetComponentsInChildren<TMP_Text>(true);

        var poContent = "# addroot \"/" + root.name + "\"\n";
        poContent += "# cd \"/" + root.name + "\"\n\n";

        foreach (var t in uiTexts)
        {
            if (string.IsNullOrEmpty(t.text)) continue;
            var path = GetGameObjectPath(root, t.transform);
            poContent += "msgid \"" + path + "\"\n";
            poContent += "msgstr \"" + EscapePoString(t.text) + "\"\n\n";
        }

        foreach (var t in tmpTexts)
        {
            if (string.IsNullOrEmpty(t.text)) continue;
            var path = GetGameObjectPath(root, t.transform);
            poContent += "msgid \"" + path + "\"\n";
            poContent += "msgstr \"" + EscapePoString(t.text) + "\"\n\n";
        }

        if (string.IsNullOrEmpty(poContent))
        {
            Debug.LogWarning("No Text or TMP_Text components found under selected GameObject.");
            return;
        }

        var savePath = EditorUtility.SaveFilePanel("Save .po file", Application.dataPath, root.name + ".po", "po");
        if (!string.IsNullOrEmpty(savePath))
        {
            File.WriteAllText(savePath, poContent);
            var entryCount = System.Text.RegularExpressions.Regex.Matches(poContent, "msgid").Count;
            Debug.Log("Exported " + entryCount + " text entries to " + savePath);
        }
    }

    // ==================== ExportHierarchyTxt ====================

    [MenuItem("GameObject/MyTool/ExportHierarchyTxt", false, 30)]
    static void ExportHierarchyTxt()
    {
        var selectN = Selection.gameObjects.Length;
        if (selectN != 1)
        {
            Debug.LogWarning("Please select exactly one GameObject as root.");
            return;
        }

        var root = Selection.gameObjects[0].transform;
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("# addroot \"/" + root.name + "\"");
        sb.AppendLine("# cd \"/" + root.name + "\"");
        BuildTreeWithFullPath(root, "/" + root.name, "", sb);

        var savePath = EditorUtility.SaveFilePanel("Save hierarchy .txt", Application.dataPath, root.name + "_paths.txt", "txt");
        if (!string.IsNullOrEmpty(savePath))
        {
            File.WriteAllText(savePath, sb.ToString());
            Debug.Log("Exported paths to " + savePath);
        }
    }

    static void BuildTreeWithFullPath(Transform node, string currentPath, string indent, System.Text.StringBuilder sb)
    {
        for (int i = 0; i < node.childCount; i++)
        {
            var child = node.GetChild(i);
            bool isLast = (i == node.childCount - 1);
            string childPath = currentPath + "/" + child.name;
            string prefix = indent + (isLast ? "└── " : "├── ");
            sb.AppendLine(prefix + "cd \"" + childPath + "\"");
            string childIndent = indent + (isLast ? "    " : "│   ");
            BuildTreeWithFullPath(child, childPath, childIndent, sb);
        }
    }

    // ==================== ExportUdonVariables ====================

    [MenuItem("GameObject/MyTool/ExportUdonVariables", false, 30)]
    static void ExportUdonVariables()
    {
        var selectN = Selection.gameObjects.Length;
        if (selectN != 1)
        {
            Debug.LogWarning("Please select exactly one GameObject as root.");
            return;
        }

        var root = Selection.gameObjects[0].transform;
        var udons = root.GetComponentsInChildren<UdonBehaviour>(true);
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("# " + root.name + " - Udon Public Variables");
        sb.AppendLine();

        int totalVars = 0;
        foreach (var udon in udons)
        {
            var path = GetGameObjectPath(root, udon.transform);
            sb.AppendLine("# cd \"" + path + "\"");

            var proxy = UdonSharpEditorUtility.GetProxyBehaviour(udon);
            if (proxy != null)
            {
                var type = proxy.GetType();
                var fields = type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                foreach (var field in fields)
                {
                    if (field.IsStatic) continue;
                    if (field.IsInitOnly) continue;
                    if (field.GetCustomAttributes(typeof(System.ObsoleteAttribute), true).Length > 0) continue;

                    var value = field.GetValue(proxy);
                    string valueStr = FormatValue(value);
                    sb.AppendLine("    .u." + field.Name + " = " + valueStr + "  // " + field.FieldType.Name);
                    totalVars++;
                }

                var methods = type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly);
                int methodCount = 0;
                foreach (var method in methods)
                {
                    if (method.IsSpecialName) continue;
                    if (method.IsAbstract) continue;
                    if (method.DeclaringType == typeof(UdonSharpBehaviour)) continue;
                    if (method.DeclaringType == typeof(MonoBehaviour)) continue;
                    if (method.DeclaringType == typeof(Behaviour)) continue;
                    if (method.DeclaringType == typeof(UnityEngine.Component)) continue;
                    if (method.DeclaringType == typeof(UnityEngine.Object)) continue;
                    if (method.DeclaringType == typeof(System.Object)) continue;

                    var parameters = method.GetParameters();
                    var paramStrs = new string[parameters.Length];
                    for (int p = 0; p < parameters.Length; p++)
                        paramStrs[p] = parameters[p].ParameterType.Name;
                    sb.AppendLine("    .ut." + method.Name + "  // params:(" + string.Join(", ", paramStrs) + ")");
                    methodCount++;
                }
                if (methodCount > 0) totalVars += methodCount;
            }
            sb.AppendLine();
        }

        var savePath = EditorUtility.SaveFilePanel("Save udon variables", Application.dataPath, root.name + "_udonvars.txt", "txt");
        if (!string.IsNullOrEmpty(savePath))
        {
            File.WriteAllText(savePath, sb.ToString());
            Debug.Log("Exported " + udons.Length + " Udon scripts, " + totalVars + " members to " + savePath);
        }
    }

    // ==================== Shared Helpers ====================

    static string GetGameObjectPath(Transform root, Transform target)
    {
        var path = "";
        var current = target;
        while (current != null && current != root.parent)
        {
            path = current.name + (string.IsNullOrEmpty(path) ? "" : "/" + path);
            current = current.parent;
        }
        return "/" + path;
    }

    static string EscapePoString(string input)
    {
        return input
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\n", "\\n")
            .Replace("\r", "\\r");
    }

    static string FormatValue(object value)
    {
        if (value == null) return "null";
        if (value is string s) return "\"" + s + "\"";
        if (value is UnityEngine.Object obj && obj != null) return obj.name + " (" + obj.GetType().Name + ")";
        if (value is GameObject go && go != null) return go.name + " (GameObject)";
        if (value is Transform t && t != null) return t.name + " (Transform)";
        if (value is Vector3 v3) return "(" + v3.x + ", " + v3.y + ", " + v3.z + ")";
        if (value is Vector2 v2) return "(" + v2.x + ", " + v2.y + ")";
        if (value is Color c) return "(" + c.r + ", " + c.g + ", " + c.b + ", " + c.a + ")";
        return value.ToString();
    }
}
