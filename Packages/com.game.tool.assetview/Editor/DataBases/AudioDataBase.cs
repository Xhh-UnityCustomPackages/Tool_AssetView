using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Profiling;
using static UnityEditor.IMGUI.Controls.MultiColumnHeaderState;

namespace Game.Tool.AssetView
{
    internal partial class AudioDataBase : IDataBase
    {
        public string filter => "t:audioclip";
        Dictionary<string, Data> m_DataMap = new Dictionary<string, Data>();

        public void Clear() { m_DataMap.Clear(); }
        public int GetCount() { return m_DataMap.Count; }

        public string GetBtnName() { return "音频资源"; }

        public string GetColumnInfos(string guid, int column)
        {
            Data data = GetAudioClipData(guid);
            var sortType = (PropCols)column;

            switch (sortType)
            {
                case PropCols.FileName: return Path.GetFileName(data.assetPath);
                case PropCols.Path: return data.assetPath;
                case PropCols.CompressSize: return EditorUtility.FormatBytes(data.CompressSize);
                case PropCols.ForceToMono: return data.forceToMono.ToString();
                case PropCols.LoadInBackground: return data.loadInBackground.ToString();
                case PropCols.Ambisonic: return data.ambisonic.ToString();
                case PropCols.defaultLoadType: return data.defaultLoadType.ToString();
                case PropCols.defaultCompressionFormat: return data.defaultCompressionFormat.ToString();
                case PropCols.androidLoadType: return data.androidLoadType.ToString();
                default: return "None";
            }
        }

        public MultiColumnHeader GetMultiColumnHeader()
        {
            var columns = new[]
            {
                new Column { headerContent = EditorGUIUtility.TrTextContent("文件名"),                    width = 100,  autoResize = false,  allowToggleVisibility = false,   },
                new Column { headerContent = EditorGUIUtility.TrTextContent("路径"),                                minWidth = 150, autoResize = false,  headerTextAlignment = TextAlignment.Right, },
                new Column { headerContent = EditorGUIUtility.TrTextContent("CompressSize(KB)"),                    width = 120,    autoResize = false, },
                new Column { headerContent = EditorGUIUtility.TrTextContent("单通道"),                    width = 80,    autoResize = false, },
                new Column { headerContent = EditorGUIUtility.TrTextContent("延时加载"),                  width = 80,    autoResize = false, },
                new Column { headerContent = EditorGUIUtility.TrTextContent("环绕声"),                    width = 80,    autoResize = false, },
                new Column { headerContent = EditorGUIUtility.TrTextContent("LoadType"),                  width = 120,    autoResize = false, },
                new Column { headerContent = EditorGUIUtility.TrTextContent("CompressionFormat"),                    width = 120,    autoResize = false, },
                new Column { headerContent = EditorGUIUtility.TrTextContent("Android LoadType"),                    width = 120,    autoResize = false, },
            };

            Assert.AreEqual(columns.Length, Enum.GetValues(typeof(PropCols)).Length, "列数和枚举数量必须一致");
            var state = new MultiColumnHeaderState(columns);
            var multiColumnHeader = new MultiColumnHeader(state);
            multiColumnHeader.ResizeToFit();
            return multiColumnHeader;
        }

        public int Sort(string guid1, string guid2, bool ascending, int sortedColumnIndex)
        {
            var data1 = GetAudioClipData(ascending ? guid1 : guid2);
            var data2 = GetAudioClipData(ascending ? guid2 : guid1);

            if (string.IsNullOrEmpty(data1.assetPath) || string.IsNullOrEmpty(data2.assetPath))
            {
                return 0;
            }

            var sortType = (PropCols)sortedColumnIndex;

            switch (sortType)
            {
                case PropCols.FileName:
                case PropCols.Path: return data1.assetPath.CompareTo(data2.assetPath);
                case PropCols.CompressSize: return data1.CompressSize.CompareTo(data2.CompressSize);
                case PropCols.ForceToMono: return data1.forceToMono.CompareTo(data2.forceToMono);
                case PropCols.LoadInBackground: return data1.loadInBackground.CompareTo(data2.loadInBackground);
                case PropCols.Ambisonic: return data1.ambisonic.CompareTo(data2.ambisonic);
                case PropCols.defaultLoadType: return data1.defaultLoadType.CompareTo(data2.defaultLoadType);
                case PropCols.defaultCompressionFormat: return data1.defaultCompressionFormat.CompareTo(data2.defaultCompressionFormat);
                case PropCols.androidLoadType: return data1.androidLoadType.CompareTo(data2.androidLoadType);
                default: return 0;
            }
        }


        Data GetAudioClipData(string guid)
        {
            if (m_DataMap.TryGetValue(guid, out Data data))
            {
                Data tData = (Data)data;

                if (!string.IsNullOrEmpty(tData.assetPath))
                {
                    return tData;
                }
            }

            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var importer = AssetImporter.GetAtPath(assetPath) as AudioImporter;

            if (importer == null) { return default; }

            AudioClip obj = AssetDatabase.LoadAssetAtPath<AudioClip>(assetPath);

            if (obj == null) { return default; }

            Data audioData = new Data();
            audioData.assetPath = assetPath;
            audioData.CompressSize = Profiler.GetRuntimeMemorySizeLong(obj);
            audioData.forceToMono = importer.forceToMono;
            audioData.loadInBackground = importer.loadInBackground;
            audioData.ambisonic = importer.ambisonic;

            var defaultSettings = importer.defaultSampleSettings;
            audioData.defaultLoadType = defaultSettings.loadType;
            audioData.defaultCompressionFormat = defaultSettings.compressionFormat;
            audioData.androidLoadType = audioData.defaultLoadType;
            if (importer.ContainsSampleSettingsOverride("Android"))
            {
                var androidSettings = importer.GetOverrideSampleSettings("Android");
                audioData.androidLoadType = androidSettings.loadType;
            }

            m_DataMap[guid] = audioData;
            return audioData;
        }

        enum PropCols
        {
            FileName,
            Path,
            CompressSize,
            ForceToMono,
            LoadInBackground,
            Ambisonic,
            defaultLoadType,
            defaultCompressionFormat,
            androidLoadType,
        }

        struct Data
        {
            public string assetPath;
            public long CompressSize;
            public bool forceToMono;
            public bool loadInBackground;
            public bool ambisonic;
            public AudioClipLoadType defaultLoadType;
            public AudioCompressionFormat defaultCompressionFormat;
            public AudioClipLoadType androidLoadType;
        }


        public GenericMenu GetGenericMenu(int column)
        {
            return null;
        }
    }
}
