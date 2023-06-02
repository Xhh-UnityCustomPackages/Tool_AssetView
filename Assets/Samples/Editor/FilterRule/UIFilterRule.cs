using System.Collections;
using System.Collections.Generic;
using Game.Tool.AssetView;
using UnityEditor;
using UnityEngine;

namespace Inutan.InutanEditor
{
    public class UIFilterRule : IFilterRule
    {
        public GUIContent titleContent => m_TitleContent;
        private GUIContent m_TitleContent = EditorGUIUtility.TrTextContent("UI", UIRootPath);

        public string[] path => m_Path;
        private const string UIRootPath = "Assets/Development/Arts/UI";
        private string[] m_Path = new string[] { UIRootPath };

        public string advanceFilter => m_AdvanceFilter;
        private string m_AdvanceFilter = "";

        public bool reverseSearch => false;
    }
}
