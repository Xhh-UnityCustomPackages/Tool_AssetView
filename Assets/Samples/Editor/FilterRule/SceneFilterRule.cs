using System.Collections;
using System.Collections.Generic;
using Game.Tool.AssetView;
using UnityEditor;
using UnityEngine;

namespace Inutan.InutanEditor
{
    public class SceneFilterRule : IFilterRule
    {
        public GUIContent titleContent => m_TitleContent;
        private GUIContent m_TitleContent = EditorGUIUtility.TrTextContent("Scene", SceneRootPath);


        public string[] path => m_Path;
        private const string SceneRootPath = "Assets/Scenes";
        private string[] m_Path = new string[] { SceneRootPath };

        public string advanceFilter => m_AdvanceFilter;
        private string m_AdvanceFilter = "";
    }
}
