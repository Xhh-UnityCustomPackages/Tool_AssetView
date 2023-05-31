using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Game.Tool.AssetView
{
    public class AllFilterRule : IFilterRule
    {
        public GUIContent titleContent => m_TitleContent;
        private GUIContent m_TitleContent = EditorGUIUtility.TrTextContent("All");

        public string[] path => AssetViewToolSettings.instance.assetPaths;

        public string advanceFilter => m_AdvanceFilter;
        private string m_AdvanceFilter = "";
    }
}
