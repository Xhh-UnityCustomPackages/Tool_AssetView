using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.Pool;

namespace Game.Tool.AssetView
{
    internal interface IDataBase
    {
        string filter { get; }
        string GetColumnInfos(string guid, int column);
        int Sort(string guid1, string guid2, bool ascending, int sortedColumnIndex);
        MultiColumnHeader GetMultiColumnHeader();
        void Clear();
        int GetCount();
        string GetBtnName();
        GenericMenu GetGenericMenu(int column);
    }

    internal abstract class AbstractDataBase : IDataBase
    {
        public abstract string filter { get; }
        public abstract string GetColumnInfos(string guid, int column);
        public abstract int Sort(string guid1, string guid2, bool ascending, int sortedColumnIndex);
        public abstract MultiColumnHeader GetMultiColumnHeader();
        public abstract void Clear();
        public abstract int GetCount();
        public abstract string GetBtnName();
        public abstract GenericMenu GetGenericMenu(int column);

        protected void AddGenericMenu(GenericMenu menu, string name, GenericMenu.MenuFunction callback)
        {
            menu.AddItem(new GUIContent(name), true, callback);
        }
    }

    public class GUIDItem : TreeViewItem
    {
        public string guid = string.Empty;
    }

    internal partial class AssetView : TreeView
    {
        static ObjectPool<GUIDItem> m_ItemPool = new ObjectPool<GUIDItem>(CreateGUIDItem, null);
        static TreeViewItem m_Root = new TreeViewItem { id = -1, depth = -1, displayName = "root" };

        string[] m_AssetPaths;
        string m_AdvanceFilter = "";
        bool m_ReverseSerach = false;
        IDataBase m_DataBase;

        //当前选择的Item
        public static List<string> SelectedItems = new List<string>();

        #region Unity Pool
        static GUIDItem CreateGUIDItem()
        {
            return new GUIDItem();
        }
        #endregion


        public static AssetView Get(MultiColumnHeader multiColumnHeader)
        {
            var state = new TreeViewState();
            var view = new AssetView(state, multiColumnHeader);
            view.showAlternatingRowBackgrounds = true;  // 显示黑白交替
            view.showBorder = true;                     // 显示边框

            return view;
        }

        AssetView(TreeViewState state, MultiColumnHeader multiColumnHeader) : base(state, multiColumnHeader)
        {
            multiColumnHeader.sortingChanged += OnSortingChanged;    // 排序
        }

        /// <summary>
        /// 设置数据
        /// </summary>
        public void SetData(IDataBase dataBase, string[] assetPaths, bool reverse = false, string filter = "")
        {
            m_DataBase?.Clear();
            m_DataBase = dataBase;
            m_AssetPaths = assetPaths;
            m_AdvanceFilter = filter;
            m_ReverseSerach = reverse;
            if (m_AssetPaths == null)
            {
                m_AssetPaths = new string[] { "Assets/" };
            }

            Reload();
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        protected override TreeViewItem BuildRoot()
        {
            if (m_Root.children != null || m_DataBase == null)
            {
                ReleaseAll(m_Root.children);
                m_Root.children.Clear();
            }

            string[] guids = AssetDatabase.FindAssets(m_DataBase.filter + " " + m_AdvanceFilter, m_AssetPaths);
            List<string> finalGuidsList = new List<string>();

            //反选搜索
            if (m_ReverseSerach)
            {
                string[] guidsNoFilter = AssetDatabase.FindAssets(m_DataBase.filter, m_AssetPaths);

                List<string> guidsList = new List<string>(guids);
                List<string> guidsNoFilterList = new List<string>(guidsNoFilter);

                //在guidsNoFilter 而不在guids
                foreach (var item in guidsNoFilterList)
                {
                    if (guidsList.Contains(item)) continue;
                    finalGuidsList.Add(item);
                }
            }
            else
            {
                finalGuidsList.AddRange(guids);
            }


            // 更新数据
            for (int i = 0; i < finalGuidsList.Count; i++)
            {
                var guid = finalGuidsList[i];

                if (guid == null)
                {
                    continue;
                }

                var data = m_ItemPool.Get();
                data.id = i;
                data.displayName = AssetDatabase.GUIDToAssetPath(guid);
                data.guid = guid;
                m_Root.AddChild(data);
            }

            // 刷新结构
            if (m_Root.children != null && m_Root.children.Count > 0)
            {
                // 更新树状结构信息
                SetupParentsAndChildrenFromDepths(m_Root, m_Root.children);
            }

            SortItems(m_Root.children);
            return m_Root;
        }

        public override void OnGUI(Rect rect)
        {
            base.OnGUI(rect);

            int[] visibleColumns = multiColumnHeader.state.visibleColumns;

            if (Event.current.type == EventType.ContextClick)
            {
                for (int i = 0; i < visibleColumns.Length; i++)
                {
                    int columnIndex = visibleColumns[i];
                    var columnRect = multiColumnHeader.GetColumnRect(columnIndex);
                    //TODO 这里Rect高度是有问题的 后续修改
                    columnRect.height = rect.height;
                    if (columnRect.Contains(Event.current.mousePosition))
                    {
                        m_DataBase?.GetGenericMenu(columnIndex)?.ShowAsContext();
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// 绘制UI
        /// </summary>
        protected sealed override void RowGUI(RowGUIArgs args)
        {

            var data = args.item as GUIDItem;

            if (data == null || m_DataBase == null)
            {
                base.RowGUI(args);
                return;
            }


            bool hasIssue = false;
            foreach (var type in AssetViewToolSettings._cacheIssueRuleTypes.Values)
            {
                var issueRule = AssetViewToolSettings.GetIssueRuleInstance(type.Name);
                hasIssue = issueRule.IsIssueAsset(data.guid);
                if (hasIssue)
                    break;
            }

            var oldColor = GUI.color;
            if (hasIssue)
                GUI.color = Color.red;
            for (int i = 0; i < args.GetNumVisibleColumns(); i++)
            {
                var cellRect = args.GetCellRect(i);
                CenterRectUsingSingleLineHeight(ref cellRect);
                var column = args.GetColumn(i);
                var value = m_DataBase.GetColumnInfos(data.guid, column);
                DefaultGUI.Label(cellRect, value, args.selected, args.focused);
            }

            GUI.color = oldColor;
        }

        /// <summary>
        /// 回收
        /// </summary>
        void ReleaseAll(IList<TreeViewItem> itemList)
        {
            if (itemList == null)
            {
                return;
            }

            foreach (TreeViewItem item in itemList)
            {
                m_ItemPool.Release(item as GUIDItem);
            }

            itemList.Clear();
        }

        /// <summary>
        /// 排序
        /// </summary>
        void OnSortingChanged(MultiColumnHeader multiColumnHeader)
        {
            Reload();
        }

        /// <summary>
        /// 排序方法
        /// </summary>
        void SortItems(List<TreeViewItem> dataList)
        {
            if (m_DataBase == null)
            {
                return;
            }

            if (dataList == null || dataList.Count <= 0)
            {
                return;
            }

            int sortedColumnIndex = multiColumnHeader.state.sortedColumnIndex;

            if (sortedColumnIndex < 0)
            {
                return;
            }

            dataList.Sort((data1, data2) =>
            {
                var d1 = data1 as GUIDItem;
                var d2 = data2 as GUIDItem;

                if (d1 == null || d2 == null)
                {
                    return 0;
                }

                var sortedColumnIndex = multiColumnHeader.state.sortedColumnIndex;
                var ascending = multiColumnHeader.IsSortedAscending(sortedColumnIndex);
                return m_DataBase.Sort(d1.guid, d2.guid, ascending, sortedColumnIndex);
            });
        }

        /// <summary>
        /// 双击选中
        /// </summary>
        protected override void DoubleClickedItem(int id)
        {
            var item = FindItem(id, rootItem);
            var assetObject = AssetDatabase.LoadAssetAtPath(item.displayName, typeof(UnityEngine.Object));
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = assetObject;
            EditorGUIUtility.PingObject(assetObject);
        }


        protected override void SelectionChanged(IList<int> selectedIds)
        {
            SelectedItems.Clear();
            for (int i = 0; i < selectedIds.Count; i++)
            {
                SelectedItems.Add(FindItem(selectedIds[i], rootItem).displayName);
            }
        }
    }
}
