using UnityEditor;
using UnityEngine;
using VRC.Udon;
using UdonSharp;
using VRC.Udon.Common.Interfaces;

[InitializeOnLoad]
public static class PropertyContextMenuExtender
{
    static PropertyContextMenuExtender()
    {
        EditorApplication.contextualPropertyMenu += OnPropertyRightClick;
    }

    private static void OnPropertyRightClick(GenericMenu menu, SerializedProperty property)
    {
        var obj = property.serializedObject.targetObject;
        if (obj == null) return;

        var capturedProperty = property.Copy();

        // --- Original Log menu ---
        menu.AddItem(new GUIContent("Log"), false, () => LogCallback(capturedProperty));

        // --- Generate HopeShell command ---
        string shellCmd = GenerateHopeShellCommand(capturedProperty);
        if (!string.IsNullOrEmpty(shellCmd))
        {
            // menu.AddItem(
            //     new GUIContent("Copy HopeShell Command"),
            //     false,
            //     () => GUIUtility.systemCopyBuffer = shellCmd
            // );
            menu.AddItem(
                new GUIContent("Send to Web Buffer"),
                false,
                () =>
                {
                    var win = EditorWindow.GetWindow<HtoolSimpleWebSever>("Htool Web Server", false);
                    if (win != null)
                    {
                        var go = obj as GameObject ?? (obj as Component)?.gameObject;
                        string objPath = GetGameObjectPath(go);
                        if (!string.IsNullOrEmpty(objPath))
                            win.AppendToBuffer("cd " + objPath);
                        win.AppendToBuffer(shellCmd);
                    }
                }
            );
        }
        else
        {
            menu.AddDisabledItem(new GUIContent("Copy HopeShell Command"));
            menu.AddDisabledItem(new GUIContent("Send to Htool Buffer"));
        }
    }

    #region Log / Debug

    private static void LogCallback(SerializedProperty property)
    {
        var obj = property.serializedObject.targetObject;
        if (obj == null) return;
        string componentName = obj.GetType().Name;
        string objPath = GetGameObjectPath((obj as Component)?.gameObject);
        object value = GetPropertyValue(property);
        Debug.Log($"Property path: {property.propertyPath}, Component: {componentName}, Path: {objPath}, Value: {value}");
    }

    private static object GetPropertyValue(SerializedProperty property)
    {
        return property.boxedValue;
    }

    private static string GetGameObjectPath(GameObject go)
    {
        if (go == null) return "";
        string path = go.name;
        Transform parent = go.transform.parent;
        while (parent != null)
        {
            path = parent.name + "/" + path;
            parent = parent.parent;
        }
        return "\"" + "/" + path + "\"";
    }
    #endregion

    #region HopeShell Command Generation

    /// <summary>
    /// Generate HopeShell command string from SerializedProperty.
    /// Dispatches to component-specific builders based on target object type.
    /// </summary>
    private static string GenerateHopeShellCommand(SerializedProperty property)
    {
        var targetObj = property.serializedObject.targetObject;
        if (targetObj == null) return "";

        string propName = property.name;
        string propValue = FormatSerializedValue(property);

        // Dispatch based on target object type
        if (targetObj is Transform)
        {
            return TransformComp(propName, propValue);
        }
        else if (targetObj is GameObject)
        {
            return GameObjectComp(propName, propValue);
        }
        else if (targetObj is UdonSharpBehaviour)
        {
            return UdonComp(propName, propValue);
        }
        else
        {
            // Match by type name to avoid TMPro assembly dependency
            string typeName = targetObj.GetType().Name;
            if (typeName == "Text" || typeName == "TMP_Text" || typeName == "TextMeshProUGUI" || typeName == "TextMeshPro")
            {
                return TextComp(propName, propValue);
            }
            if (typeName == "Image" || typeName == "RawImage")
            {
                return ImageComp(typeName, propName, propValue);
            }
            if (typeName == "Toggle")
            {
                return ToggleComp(propName, propValue);
            }
            if (typeName == "Slider")
            {
                return SliderComp(propName, propValue);
            }
            if (typeName == "AudioSource")
            {
                return AudioComp(propName, propValue);
            }
            if (typeName == "BoxCollider")
            {
                return BoxColliderComp(propName, propValue);
            }
            return "";
        }
    }

    /// <summary>
    /// Format SerializedProperty value as a HopeShell-compatible string.
    /// Vector3 -> (x, y, z)
    /// bool -> true / false
    /// string -> "value"
    /// int/float -> direct ToString
    /// </summary>
    private static string FormatSerializedValue(SerializedProperty property)
    {
        switch (property.propertyType)
        {
            case SerializedPropertyType.Vector3:
                var v3 = property.vector3Value;
                return "(" + v3.x + ", " + v3.y + ", " + v3.z + ")";

            case SerializedPropertyType.Vector2:
                var v2 = property.vector2Value;
                return "(" + v2.x + ", " + v2.y + ")";

            case SerializedPropertyType.Vector3Int:
                var v3i = property.vector3IntValue;
                return "(" + v3i.x + ", " + v3i.y + ", " + v3i.z + ")";

            case SerializedPropertyType.Vector2Int:
                var v2i = property.vector2IntValue;
                return "(" + v2i.x + ", " + v2i.y + ")";

            case SerializedPropertyType.Quaternion:
                var q = property.quaternionValue;
                var euler = q.eulerAngles;
                return "(" + euler.x + ", " + euler.y + ", " + euler.z + ")";

            case SerializedPropertyType.Boolean:
                return property.boolValue ? "true" : "false";

            case SerializedPropertyType.String:
                return "\"" + property.stringValue + "\"";

            case SerializedPropertyType.Integer:
                return property.intValue.ToString();

            case SerializedPropertyType.Float:
                return property.floatValue.ToString();

            case SerializedPropertyType.Color:
                var c = property.colorValue;
                return "(" + c.r + ", " + c.g + ", " + c.b + ", " + c.a + ")";

            default:
                // fallback to boxedValue
                var boxed = property.boxedValue;
                if (boxed != null)
                    return boxed.ToString();
                return "";
        }
    }

    #endregion

    #region Component-Specific Command Builders

    /// <summary>
    /// Generate HopeShell command for Transform component.
    /// Maps to HopeShell .tf.xxx syntax.
    /// </summary>
    private static string TransformComp(string propName, string propValue)
    {
        // Map serialized names to HopeShell .tf.xxx commands
        switch (propName)
        {
            case "m_LocalPosition":
                return ".tf.lp = " + propValue;

            case "m_LocalRotation":
                // m_LocalRotation is Quaternion in SerializedProperty,
                // but HopeShell .lr expects Euler angles
                return ".tf.lr = " + propValue;

            case "m_LocalEulerAnglesHint":
                return ".tf.lel = " + propValue;

            case "m_LocalScale":
                return ".tf.ls = " + propValue;

            default:
                return "";
        }
    }

    /// <summary>
    /// Generate HopeShell command for GameObject.
    /// Maps to HopeShell .en / active syntax.
    /// </summary>
    private static string GameObjectComp(string propName, string propValue)
    {
        switch (propName)
        {
            case "m_IsActive":
                return ".en = " + propValue;

            case "m_Name":
                // HopeShell has no direct name command, use $name syntax
                return "$name = " + propValue;

            default:
                return "";
        }
    }

    /// <summary>
    /// Generate HopeShell command for UdonBehaviour public variables.
    /// Maps to HopeShell .u.variableName = value syntax.
    /// </summary>
    private static string UdonComp(string propName, string propValue)
    {
        if (string.IsNullOrEmpty(propName)) return "";

        // UdonBehaviour public variables accessed via .u.variableName
        return ".u." + propName + " = " + propValue;
    }

    /// <summary>
    /// Generate HopeShell command for Text / TMP_Text components.
    /// Maps to HopeShell .t.xxx / .text.xxx syntax.
    /// </summary>
    private static string TextComp(string propName, string propValue)
    {
        switch (propName)
        {
            // Text content -> .t = "value"
            case "m_Text":
            case "m_text":
                return ".t = " + propValue;

            // Font size -> .t.fts = value
            case "m_FontSize":
            case "m_fontSize":
                return ".t.fts = " + propValue;

            // Color -> .t.c = (r, g, b, a)
            case "m_Color":
                return ".t.c = " + propValue;

            // Enabled -> .t.en = true/false
            case "m_Enabled":
                return ".t.en = " + propValue;

            default:
                return "";
        }
    }

    /// <summary>
    /// Generate HopeShell command for Image / RawImage components.
    /// Maps to HopeShell .img.xxx / .ri.xxx syntax.
    /// Only supports enabled and color.
    /// </summary>
    private static string ImageComp(string componentType, string propName, string propValue)
    {
        // RawImage uses .ri, Image uses .img
        string prefix = componentType == "RawImage" ? ".ri" : ".img";

        switch (propName)
        {
            case "m_Color":
                return prefix + ".c = " + propValue;

            case "m_Enabled":
                return prefix + ".en = " + propValue;

            default:
                return "";
        }
    }

    /// <summary>
    /// Generate HopeShell command for Toggle component.
    /// Maps to HopeShell .tg.xxx syntax.
    /// </summary>
    private static string ToggleComp(string propName, string propValue)
    {
        switch (propName)
        {
            // isOn -> .tg.isOn = true/false
            case "m_IsOn":
                return ".tg.isOn = " + propValue;

            // enabled -> .tg.en = true/false
            case "m_Enabled":
                return ".tg.en = " + propValue;

            default:
                return "";
        }
    }

    /// <summary>
    /// Generate HopeShell command for Slider component.
    /// Maps to HopeShell .sl.xxx syntax.
    /// </summary>
    private static string SliderComp(string propName, string propValue)
    {
        switch (propName)
        {
            // value -> .sl.val = float
            case "m_Value":
                return ".sl.val = " + propValue;

            // maxValue -> .sl.max = float
            case "m_MaxValue":
                return ".sl.max = " + propValue;

            // minValue -> .sl.min = float
            case "m_MinValue":
                return ".sl.min = " + propValue;

            // enabled -> .sl.en = true/false
            case "m_Enabled":
                return ".sl.en = " + propValue;

            // interactable -> .sl.int = true/false
            case "m_Interactable":
                return ".sl.int = " + propValue;

            default:
                return "";
        }
    }

    /// <summary>
    /// Generate HopeShell command for AudioSource component.
    /// Maps to HopeShell .as.xxx / .aud.xxx syntax.
    /// </summary>
    private static string AudioComp(string propName, string propValue)
    {
        switch (propName)
        {
            // mute -> .as.mute = true/false
            case "m_Mute":
                return ".as.mute = " + propValue;

            // loop -> .as.loop = true/false
            case "m_Loop":
                return ".as.loop = " + propValue;

            // enabled -> .as.en = true/false
            case "m_Enabled":
                return ".as.en = " + propValue;

            // volume -> .as.vol = 0.5
            case "m_Volume":
                return ".as.vol = " + propValue;

            // pitch -> .as.pitch = 1
            case "m_Pitch":
                return ".as.pitch = " + propValue;

            // minDistance -> .as.min = 1
            case "m_MinDistance":
                return ".as.min = " + propValue;

            // maxDistance -> .as.max = 500
            case "m_MaxDistance":
                return ".as.max = " + propValue;

            default:
                return "";
        }
    }

    /// <summary>
    /// Generate HopeShell command for BoxCollider component.
    /// Maps to HopeShell .bc.xxx / .box.xxx syntax.
    /// </summary>
    private static string BoxColliderComp(string propName, string propValue)
    {
        switch (propName)
        {
            // enabled -> .bc.en = true/false
            case "m_Enabled":
                return ".bc.en = " + propValue;

            // isTrigger -> .bc.trig = true/false
            case "m_IsTrigger":
                return ".bc.trig = " + propValue;

            default:
                return "";
        }
    }

    #endregion
}
