using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.Tool.AssetView
{
    class AssetViewSettingsProvider : SettingsProvider
    {
        private class Styles
        {
            // public static readonly GUIContent DefaultFramerateLabel = L10n.TextContent("Default frame rate", "The default frame rate for new Timeline assets.");
            // public static readonly GUIContent TimelineAssetLabel = L10n.TextContent("Timeline Asset", "");
            // public static readonly string WarningString = L10n.Tr("Locking playback cannot be enabled for this frame rate.");
        }

        [SettingsProvider]
        public static SettingsProvider CreateTimelineProjectSettingProvider()
        {
            var provider = new AssetViewSettingsProvider("Project/资源检查工具 (AssetViewTool)", SettingsScope.Project, GetSearchKeywordsFromGUIContentProperties<Styles>());
            return provider;
        }

        public AssetViewSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null) : base(path, scopes, keywords) { }

        private SerializedObject m_SerializedObject;
        private SerializedProperty m_AssetPaths;


        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            AssetViewToolSettings.instance.Save();
            //必须添加这句话 不然无法编辑  https://www.jianshu.com/p/a197a1b7e77e
            AssetViewToolSettings.instance.hideFlags &= ~HideFlags.NotEditable;

            m_SerializedObject = AssetViewToolSettings.instance.GetSerializedObject();
            m_AssetPaths = m_SerializedObject.FindProperty("m_AssetPaths");
        }

        public override void OnGUI(string searchContext)
        {
            using (CreateSettingsWindowGUIScope())
            {
                m_SerializedObject.Update();

                EditorGUILayout.BeginVertical();
                EditorGUI.BeginChangeCheck();

                EditorGUILayout.PropertyField(m_AssetPaths, new GUIContent("需要检查的资源路径"));

                if (EditorGUI.EndChangeCheck())
                {
                    m_SerializedObject.ApplyModifiedProperties();
                    AssetViewToolSettings.instance.Save();
                }

                EditorGUILayout.EndVertical();
            }
        }

        private IDisposable CreateSettingsWindowGUIScope()
        {
            var unityEditorAssembly = Assembly.GetAssembly(typeof(EditorWindow));
            var type = unityEditorAssembly.GetType("UnityEditor.SettingsWindow+GUIScope");
            return Activator.CreateInstance(type) as IDisposable;
        }
    }
}
