using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using static UnityEditor.IMGUI.Controls.TreeView;

namespace Game.Tool.AssetView
{
    internal partial class AssetViewPanel : EditorWindow
    {

        [MenuItem("Tools/资源检查 (AssetViewPanel)")]
        internal static AssetViewPanel GetWindow()
        {
            var window = GetWindow<AssetViewPanel>();
            window.titleContent = new GUIContent("资源检查");
            window.Focus();
            window.Repaint();
            return window;
        }

        // string[] m_AssetPaths = new string[] { "Assets/Development/", ResManager.RootPath, "Assets/ResAssets", "Assets/Scenes/" };
        List<IDataBase> m_DataBases = new List<IDataBase>() { new TextureDataBase(), new ModelDataBase(), new AudioDataBase(), new ShaderDataBase() };
        AssetView m_View;
        SearchField m_SearchField = null;
        int m_Index = -1;
        int m_FilterIndex = -1;
        Object m_SelectedObject = null;
        string m_FilterString;
        bool m_ReverseSerach = false;

        private void OnEnable()
        {
            AssetViewToolSettings.instance.Init();
        }

        void OnGUI()
        {
            IDataBase dataBase = ShowToolbar();
            ShowFixBar(dataBase);
            InitData(dataBase);

            if (m_View == null) { return; }

            // 数据区域
            Rect viewRect = GUILayoutUtility.GetRect(Style.SearchContent, EditorStyles.textArea, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            m_View.OnGUI(viewRect);

            // 状态栏
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                GUILayout.FlexibleSpace();
                var content = EditorGUIUtility.TrTextContent($"数量:{dataBase.GetCount()}");
                EditorGUILayout.DropdownButton(content, FocusType.Passive, EditorStyles.toolbarButton, GUILayout.MinWidth(100f));
            }
        }

        /// <summary>
        /// 绘制按钮栏
        /// </summary>
        IDataBase ShowToolbar()
        {
            IDataBase data = null;

            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                // 类型按钮
                GUILayout.FlexibleSpace();
                for (int i = 0; i < m_DataBases.Count; i++)
                {
                    var dataBase = m_DataBases[i];

                    if (m_Index < 0)
                    {
                        m_Index = i;
                    }

                    var oldColor = GUI.color;
                    GUI.color = m_Index == i ? Color.green : oldColor;
                    var content = EditorGUIUtility.TrTextContent(dataBase.GetBtnName());
                    if (EditorGUILayout.DropdownButton(content, FocusType.Passive, EditorStyles.toolbarButton))
                    {
                        m_Index = i;
                        m_View = null;
                    }
                    GUI.color = oldColor;

                    if (m_Index == i)
                    {
                        data = dataBase;
                    }
                }

                GUILayout.FlexibleSpace();
                GUILayout.FlexibleSpace();
                GUILayout.FlexibleSpace();

                if (m_View != null)
                {
                    // 刷新按钮
                    Rect fileRect = GUILayoutUtility.GetRect(Style.FileMenuName, EditorStyles.toolbarButton);
                    if (EditorGUI.DropdownButton(fileRect, Style.FileMenuName, FocusType.Passive, EditorStyles.toolbarButton))
                    {
                        data.Clear();
                        m_View.multiColumnHeader.sortedColumnIndex = -1;
                        m_View.SetData(data, AssetViewToolSettings.instance.assetPaths);
                    }

                    // 搜索区域
                    Rect searchRect = GUILayoutUtility.GetRect(Style.SearchContent, EditorStyles.toolbarSearchField, GUILayout.MinWidth(260));
                    m_View.searchString = m_SearchField.OnGUI(searchRect, m_View.searchString);
                }
            }

            return data;
        }

        void ShowFixBar(IDataBase dataBase)
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                var count = AssetViewToolSettings._cacheFilterRuleTypesList.Count;
                for (int i = 0; i < count; i++)
                {
                    var type = AssetViewToolSettings._cacheFilterRuleTypesList[i];

                    var filterRule = AssetViewToolSettings.GetFilterRuleInstance(type.Name);

                    var oldColor = GUI.color;
                    GUI.color = m_FilterIndex == i ? Color.green : oldColor;

                    if (EditorGUILayout.DropdownButton(filterRule.titleContent, FocusType.Passive, EditorStyles.toolbarButton))
                    {
                        m_FilterIndex = i;
                        m_View.multiColumnHeader.sortedColumnIndex = -1;

                        string[] assetPaths = filterRule.path;

                        if (m_SelectedObject != null)
                            assetPaths = new string[] { AssetDatabase.GetAssetPath(m_SelectedObject) };

                        m_View.SetData(dataBase, assetPaths, filterRule.reverseSearch, filterRule.advanceFilter);
                    }
                    GUI.color = oldColor;
                }

                GUILayout.FlexibleSpace();
                GUILayout.Label("自定义搜索:");
                m_SelectedObject = EditorGUILayout.ObjectField(new GUIContent(), m_SelectedObject, typeof(Object), false, new GUILayoutOption[] { GUILayout.Width(100f) });
                m_FilterString = EditorGUILayout.TextField(m_FilterString);
                m_ReverseSerach = EditorGUILayout.Toggle(m_ReverseSerach, new GUILayoutOption[] { GUILayout.Width(30f) });
                GUIContent search = new GUIContent("搜索");
                if (EditorGUILayout.DropdownButton(search, FocusType.Passive, EditorStyles.toolbarButton))
                {
                    m_FilterIndex = -1;
                    string path = "Assets/";
                    if (m_SelectedObject != null)
                        path = AssetDatabase.GetAssetPath(m_SelectedObject);

                    string[] assetPaths = new string[] { path };
                    m_View.SetData(dataBase, assetPaths, m_ReverseSerach, m_FilterString);
                }

                GUILayout.FlexibleSpace();
                GUILayout.FlexibleSpace();
            }
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        void InitData(IDataBase dataBase)
        {
            if (m_SearchField == null)
            {
                m_SearchField = new SearchField();
            }

            if (m_View == null)
            {
                m_View = AssetView.Get(dataBase.GetMultiColumnHeader());
                m_View.SetData(dataBase, AssetViewToolSettings.instance.assetPaths);
                m_View.searchString = string.Empty;

                m_SearchField.downOrUpArrowKeyPressed -= m_View.SetFocusAndEnsureSelectedItem;
                m_SearchField.downOrUpArrowKeyPressed += m_View.SetFocusAndEnsureSelectedItem;
            }
        }

        class Style
        {
            internal static GUIContent FileMenuName = EditorGUIUtility.TrTextContent("刷新数据");
            internal static GUIContent SearchContent = EditorGUIUtility.TrTextContent("搜索", "输入内容搜索");
        }
    }
}
