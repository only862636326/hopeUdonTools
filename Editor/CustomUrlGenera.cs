using BestHTTP.SecureProtocol.Org.BouncyCastle.Asn1.Crmf;
using System.Collections.Generic;
using System.Linq;
using UdonSharp;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRC.SDKBase;

namespace HopeTools
{
    public class UrlGenera : EditorWindow
    {
        private string _info = "sadfe";
        public GameObject url_obg;
        public string url_base = "http://127.0.0.1:5000/vrchat/gomoku?";
        public int url_start, url_end;
        UrlGenera()
        {
            this.titleContent = new GUIContent("VrcUrlTool");
        }

        [MenuItem("HopeTools/VrcUrlTool")]
        static void ShowUrlGenera()
        {
            EditorWindow.GetWindow(typeof(UrlGenera));
        }

        private void OnGUI()
        {
            GUILayout.Space(10);
            GUI.skin.label.fontSize = 20;
            GUILayout.Label("Rrl");

            url_obg = (GameObject) EditorGUILayout.ObjectField("操作对象", url_obg, typeof(GameObject), true);
            url_base = EditorGUILayout.TextField("URL 前缀", url_base);
            url_start = EditorGUILayout.IntField("url_start，包含", url_start);
            url_end = EditorGUILayout.IntField("url_end，包含", url_end);
            GUILayout.Space(10);

            if (GUILayout.Button("生成"))
            {
                if (url_obg != null)
                {
                    var udon = url_obg.GetComponent<UdonSharpBehaviour>();
                    var urls = new VRCUrl[url_end - url_start + 1];
                    for (int i = url_start; i < url_end + 1; i++)
                    {
                        urls[i] = new VRCUrl($"{url_base}key={i}");
                    }
                    udon.SetProgramVariable("urls_list", urls);
                    udon.SetProgramVariable("ApiBase", url_base);

                    var url_l = (VRCUrl[])udon.GetProgramVariable("urls_list");
                    var s = url_l[url_l.Length - 1].ToString();
                    EditorUtility.DisplayDialog("", s, "close");
                }
            }
        }
    }
}
