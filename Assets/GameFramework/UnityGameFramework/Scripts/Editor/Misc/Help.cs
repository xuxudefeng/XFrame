﻿






using UnityEditor;
using UnityEngine;

namespace UnityGameFramework.Editor
{
    /// <summary>
    /// 帮助相关的实用函数。
    /// </summary>
    public static class Help
    {
        [MenuItem("Game Framework/Documentation", false, 90)]
        public static void ShowDocumentation()
        {
            ShowHelp("document/");
        }

        [MenuItem("Game Framework/API Reference", false, 91)]
        public static void ShowApiReference()
        {
            ShowHelp("api/");
        }

        private static void ShowHelp(string uri)
        {
            Application.OpenURL(uri);
        }
    }
}