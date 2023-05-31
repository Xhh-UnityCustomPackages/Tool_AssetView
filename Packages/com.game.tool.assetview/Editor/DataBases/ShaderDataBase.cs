using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.Assertions;
using static UnityEditor.IMGUI.Controls.MultiColumnHeaderState;

namespace Game.Tool.AssetView
{
    public class ShaderDataBase : IDataBase
    {
        Dictionary<string, Data> m_DataMap = new Dictionary<string, Data>();
        public string filter => "t:shader";
        public void Clear() { m_DataMap.Clear(); }
        public string GetBtnName() => "Shader资源";
        public int GetCount() => m_DataMap.Count;

        public string GetColumnInfos(string guid, int column)
        {
            Data data = GetShaderData(guid);
            var sortType = (PropCols)column;

            switch (sortType)
            {
                case PropCols.FileName: return Path.GetFileName(data.assetPath);
                case PropCols.Path: return data.assetPath;
                default: return "None";
            }
        }

        public MultiColumnHeader GetMultiColumnHeader()
        {
            var columns = new[]
            {
                new Column { headerContent = EditorGUIUtility.TrTextContent("文件名"),              width = 100,  autoResize = false,  allowToggleVisibility = false,   },
                new Column { headerContent = EditorGUIUtility.TrTextContent("路径"),                minWidth = 100, autoResize = true,  headerTextAlignment = TextAlignment.Right, },
            };

            Assert.AreEqual(columns.Length, Enum.GetValues(typeof(PropCols)).Length, "列数和枚举数量必须一致");
            var state = new MultiColumnHeaderState(columns);
            var multiColumnHeader = new MultiColumnHeader(state);
            multiColumnHeader.ResizeToFit();
            return multiColumnHeader;
        }

        public int Sort(string guid1, string guid2, bool ascending, int sortedColumnIndex)
        {
            var data1 = GetShaderData(ascending ? guid1 : guid2);
            var data2 = GetShaderData(ascending ? guid2 : guid1);

            if (string.IsNullOrEmpty(data1.assetPath) || string.IsNullOrEmpty(data2.assetPath))
            {
                return 0;
            }

            var sortType = (PropCols)sortedColumnIndex;

            switch (sortType)
            {
                case PropCols.FileName:
                case PropCols.Path: return data1.assetPath.CompareTo(data2.assetPath);
                default: return 0;
            }
        }


        Data GetShaderData(string guid)
        {
            if (m_DataMap.TryGetValue(guid, out Data data))
            {
                Data tData = data;

                if (!string.IsNullOrEmpty(tData.assetPath))
                {
                    return tData;
                }
            }

            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var importer = AssetImporter.GetAtPath(assetPath) as ShaderImporter;

            if (importer == null) { return default; }

            Data shaderData = new Data();
            shaderData.assetPath = assetPath;

            m_DataMap[guid] = shaderData;
            return shaderData;
        }

        enum PropCols
        {
            FileName,
            Path,
        }

        struct Data
        {
            public string assetPath;
        }

        public GenericMenu GetGenericMenu(int column)
        {
            return null;
        }
    }
}
