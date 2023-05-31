using System.Collections;
using System.Collections.Generic;
using Game.Tool.AssetView;
using UnityEditor;
using UnityEngine;

namespace Inutan.InutanEditor
{
    public class NormalFilterRule : IFilterRule
    {
        public GUIContent titleContent => m_TitleContent;
        private GUIContent m_TitleContent = EditorGUIUtility.TrTextContent("Scene Normal", SceneRootPath);

        public string[] path => m_Path;
        public const string SceneRootPath = "Assets/Scenes";
        public string advanceFilter => m_AdvanceFilter;

        private string[] m_Path = new string[] { SceneRootPath };
        private string m_AdvanceFilter = "glob:\"(**_N.*|**_Normal.*)\"";
    }
}
