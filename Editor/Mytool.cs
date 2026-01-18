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

[CustomEditor(typeof(Mytool))]
public class Mytool : Editor
{
    [MenuItem("GameObject/MyTool/LineSet", false, 30)]
    static void SetCardLine()
    {
        var select_n = Selection.gameObjects.Length;
        if (select_n == 1)
        {
            var g = Selection.gameObjects[0].transform;

            // Debug.Log(true);
            var n = g.transform.childCount;
            var base_p = g.GetChild(0).position;
            var offset = g.GetChild(n - 1).position - g.GetChild(0).position;
            offset /= n - 1;
            for (int i = 1; i < n; i++)
            {
                g.GetChild(i).position = base_p + offset * i;
                EditorUtility.SetDirty(g.GetChild(i).gameObject);
            }
        }
    }

    [MenuItem("GameObject/MyTool/MatSet_xz", false, 30)]
    static void SetCardMat_xz()
    {
        var select_n = Selection.gameObjects.Length;
        if (select_n == 1)
        {
            var g = Selection.gameObjects[0].transform;

            var g1 = g.GetChild(0).transform;
            var g2 = g.GetChild(g.transform.childCount - 1).transform;

            // Debug.Log(true);
            var n = Math.Sqrt(g.transform.childCount);
            int int_n = (int)n;
            Debug.Log(int_n);
            var offset_x = (g2.position.x - g1.position.x) / (int_n - 1);
            var offset_y = (g2.position.z - g1.position.z) / (int_n - 1);

            for (int i = 0; i < int_n; i++)
                for (int j = 0; j < int_n; j++)
                {
                    g.GetChild(i * int_n + j).position = g1.position + new Vector3(offset_x * j, 0, offset_y * i);
                    // g.GetChild(i * int_n + j).name = "p_" + i.ToString() + "_" + j.ToString();
                }
        }
    }


    [MenuItem("GameObject/MyTool/MatSet_xy", false, 30)]
    static void SetCardMat_xy()
    {
        var select_n = Selection.gameObjects.Length;
        if (select_n == 1)
        {
            var g = Selection.gameObjects[0].transform;

            var g1 = g.GetChild(0).transform;
            var g2 = g.GetChild(g.transform.childCount - 1).transform;

            // Debug.Log(true);
            var n = Math.Sqrt(g.transform.childCount);
            int int_n = (int)n;
            Debug.Log(int_n);
            var offset_x = (g2.position.x - g1.position.x) / (int_n - 1);
            var offset_y = (g2.position.y - g1.position.y) / (int_n - 1);

            for (int i = 0; i < int_n; i++)
                for (int j = 0; j < int_n; j++)
                {
                    g.GetChild(i * int_n + j).position = g1.position + new Vector3(offset_x * j, offset_y * i, 0);
                    // g.GetChild(i * int_n + j).name = "gomokuInput_" + i.ToString() + "_" + j.ToString();
                }
        }
    }

    [MenuItem("GameObject/MyTool/SetAllChildTure", false, 30)]
    static void SetAllChildTure()
    {
        var select_n = Selection.gameObjects.Length;
        if (select_n == 1)
        {
            var g = Selection.gameObjects[0].transform;
            for (int i = 0; i < g.childCount; i++)
            {
                g.GetChild(i).gameObject.SetActive(true);
            }
        }
    }

    [MenuItem("GameObject/MyTool/SetAllChildFalse", false, 30)]
    static void SetAllChildFalse()
    {
        var select_n = Selection.gameObjects.Length;
        if (select_n == 1)
        {
            var g = Selection.gameObjects[0].transform;
            for (int i = 0; i < g.childCount; i++)
            {
                g.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
}






