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
    internal partial class TextureDataBase : AbstractDataBase
    {
        Dictionary<string, Data> m_DataMap = new Dictionary<string, Data>();
        public override string filter { get { return "t:texture2d"; } }
        public override int GetCount() { return m_DataMap.Count; }
        public override void Clear() { m_DataMap.Clear(); }
        public override string GetBtnName() { return "图片资源"; }

        /// <summary>
        /// 获取行信息
        /// </summary>
        public override string GetColumnInfos(string guid, int column)
        {
            Data data = GetTextureData(guid);
            var sortType = (PropCols)column;

            switch (sortType)
            {
                // case PropCols.FileName: return Path.GetFileName(data.assetPath);
                case PropCols.Path: return data.assetPath;
                case PropCols.FileExtension: return Path.GetExtension(data.assetPath);
                case PropCols.CompressSize: return EditorUtility.FormatBytes(data.CompressSize);
                case PropCols.Size: return $"x{data.Size}";
                case PropCols.MobileSize: return $"x{data.MobileSize}";
                case PropCols.WindeowFormat: return data.WindeowFormat.ToString();
                case PropCols.IOSFormat: return data.IOSFormat.ToString();
                case PropCols.AndroidFormat: return data.AndroidFormat.ToString();
                case PropCols.TextureType: return data.TextureType.ToString();
                // case PropCols.AlphaSource: return data.AlphaSource.ToString();
                case PropCols.sRGB: return data.sRGB.ToString();
                case PropCols.Read_Write: return data.Read_Write.ToString();
                case PropCols.CenerateMipMaps: return data.GenerateMipMaps.ToString();
                // case PropCols.WrapMode: return data.WrapMode.ToString();
                default: return "None";
            }
        }

        /// <summary>
        /// 排序
        /// </summary>
        public override int Sort(string guid1, string guid2, bool ascending, int sortedColumnIndex)
        {
            var data1 = GetTextureData(ascending ? guid1 : guid2);
            var data2 = GetTextureData(ascending ? guid2 : guid1);

            if (string.IsNullOrEmpty(data1.assetPath) || string.IsNullOrEmpty(data2.assetPath))
            {
                return 0;
            }

            var sortType = (PropCols)sortedColumnIndex;

            switch (sortType)
            {
                // case PropCols.FileName:
                case PropCols.Path: return data1.assetPath.CompareTo(data2.assetPath);
                case PropCols.FileExtension: return Path.GetExtension(data1.assetPath).CompareTo(Path.GetExtension(data2.assetPath));
                case PropCols.CompressSize: return data1.CompressSize.CompareTo(data2.CompressSize);
                case PropCols.Size: return data1.Size.CompareTo(data2.Size);
                case PropCols.MobileSize: return data1.MobileSize.CompareTo(data2.MobileSize);
                case PropCols.WindeowFormat: return data1.WindeowFormat.CompareTo(data2.WindeowFormat);
                case PropCols.IOSFormat: return data1.IOSFormat.CompareTo(data2.IOSFormat);
                case PropCols.AndroidFormat: return data1.AndroidFormat.CompareTo(data2.AndroidFormat);
                case PropCols.TextureType: return data1.TextureType.CompareTo(data2.TextureType);
                // case PropCols.AlphaSource: return data1.AlphaSource.CompareTo(data2.AlphaSource);
                case PropCols.sRGB: return data1.sRGB.CompareTo(data2.sRGB);
                case PropCols.Read_Write: return data1.Read_Write.CompareTo(data2.Read_Write);
                case PropCols.CenerateMipMaps: return data1.GenerateMipMaps.CompareTo(data2.GenerateMipMaps);
                // case PropCols.WrapMode: return data1.WrapMode.CompareTo(data2.WrapMode);
                default: return 0;
            }
        }

        /// <summary>
        /// 获取标题
        /// </summary>
        public override MultiColumnHeader GetMultiColumnHeader()
        {
            var columns = new[]
            {
                // new Column { headerContent = EditorGUIUtility.TrTextContent("文件名"),                              width = 100,  autoResize = false,  allowToggleVisibility = false,   },
                new Column { headerContent = EditorGUIUtility.TrTextContent("路径"),                                minWidth = 100, autoResize = true,  headerTextAlignment = TextAlignment.Right, },
                new Column { headerContent = EditorGUIUtility.TrTextContent("后缀"),                                width = 60,     autoResize = false, },
                new Column { headerContent = EditorGUIUtility.TrTextContent("CompressSize(KB)"),                    width = 120,    autoResize = false, },
                new Column { headerContent = EditorGUIUtility.TrTextContent("尺寸"),                                width = 60,     autoResize = false, },
                new Column { headerContent = EditorGUIUtility.TrTextContent("移动端尺寸"),                         width = 60,     autoResize = false, },
                new Column { headerContent = EditorGUIUtility.TrTextContent("PC格式", "Windows Format"),            width = 80,     autoResize = false, },
                new Column { headerContent = EditorGUIUtility.TrTextContent("IOS格式", "IOS Format"),               width = 80,     autoResize = false, },
                new Column { headerContent = EditorGUIUtility.TrTextContent("Android格式", "Android Format"),       width = 80,     autoResize = false, },
                new Column { headerContent = EditorGUIUtility.TrTextContent("TextureType"),                         width = 100,    autoResize = false, },
                // new Column { headerContent = EditorGUIUtility.TrTextContent("AlphaSource"),                         width = 100,    autoResize = false, },
                new Column { headerContent = EditorGUIUtility.TrTextContent("sRGB"),                                width = 70,     autoResize = false, },
                new Column { headerContent = EditorGUIUtility.TrTextContent("R|W"),                          width = 40,     autoResize = false, },
                new Column { headerContent = EditorGUIUtility.TrTextContent("MipMaps"),                             width = 40,    autoResize = false, },
                // new Column { headerContent = EditorGUIUtility.TrTextContent("WrapMode"),                            width = 100,    autoResize = false, },
            };

            Assert.AreEqual(columns.Length, Enum.GetValues(typeof(PropCols)).Length, "列数和枚举数量必须一致");
            var state = new MultiColumnHeaderState(columns);
            var multiColumnHeader = new MultiColumnHeader(state);
            multiColumnHeader.ResizeToFit();
            return multiColumnHeader;
        }

        /// <summary>
        /// 获取图片资源数据
        /// </summary>
        Data GetTextureData(string guid)
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
            var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;

            if (importer == null)
            {
                Data _data = default;
                _data.assetPath = assetPath;
                return _data;
            }


            Texture2D obj = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);

            if (obj == null)
            {
                Data _data = default;
                _data.assetPath = assetPath;
                return _data;
            }

            var iPhoneTextureSettings = importer.GetPlatformTextureSettings("iPhone");
            var androidTextureSettings = importer.GetPlatformTextureSettings("Android");

            Data textureData = new Data();
            textureData.assetPath = assetPath;
            textureData.CompressSize = Profiler.GetRuntimeMemorySizeLong(obj) / 2;
            textureData.Size = Mathf.Max(obj.width, obj.height);
            textureData.MobileSize = Mathf.Max(iPhoneTextureSettings.maxTextureSize, androidTextureSettings.maxTextureSize);
            textureData.MobileSize = Mathf.Min(textureData.Size, textureData.MobileSize);
            textureData.WindeowFormat = importer.GetPlatformTextureSettings("Standalone").format;
            textureData.IOSFormat = iPhoneTextureSettings.format;
            textureData.AndroidFormat = androidTextureSettings.format;
            textureData.TextureType = importer.textureType;
            textureData.AlphaSource = importer.alphaSource;
            textureData.sRGB = importer.sRGBTexture;
            textureData.Read_Write = importer.isReadable;
            textureData.GenerateMipMaps = importer.mipmapEnabled;
            // textureData.WrapMode = importer.wrapMode;

            m_DataMap[guid] = textureData;
            return textureData;
        }

        enum PropCols
        {
            // FileName,
            Path,
            FileExtension,
            CompressSize,
            Size,
            MobileSize,
            WindeowFormat,
            IOSFormat,
            AndroidFormat,
            TextureType,
            // AlphaSource,
            sRGB,
            Read_Write,
            CenerateMipMaps,
            // WrapMode,
        }

        struct Data
        {
            public string assetPath;
            public long CompressSize;
            public int Size;
            public int MobileSize;
            public TextureImporterFormat WindeowFormat;
            public TextureImporterFormat IOSFormat;
            public TextureImporterFormat AndroidFormat;
            public TextureImporterType TextureType;
            public TextureImporterAlphaSource AlphaSource;
            public bool sRGB;
            public bool Read_Write;
            public bool GenerateMipMaps;
            // public TextureWrapMode WrapMode;
        }


        public override GenericMenu GetGenericMenu(int column)
        {
            switch ((PropCols)column)
            {
                case PropCols.MobileSize:
                    {
                        void AddMobileSizeGenericMenu(GenericMenu menu, int size)
                        {
                            void ChangeMobileSize(int size)
                            {
                                void Do(string path)
                                {
                                    var importer = AssetImporter.GetAtPath(path) as TextureImporter;
                                    if (importer == null) return;

                                    var iPhoneTextureSettings = importer.GetPlatformTextureSettings("iPhone");
                                    var androidTextureSettings = importer.GetPlatformTextureSettings("Android");

                                    iPhoneTextureSettings.overridden = true;
                                    androidTextureSettings.overridden = true;
                                    iPhoneTextureSettings.maxTextureSize = size;
                                    androidTextureSettings.maxTextureSize = size;

                                    importer.SetPlatformTextureSettings(iPhoneTextureSettings);
                                    importer.SetPlatformTextureSettings(androidTextureSettings);


                                    importer.SaveAndReimport();

                                    var guid = AssetDatabase.AssetPathToGUID(path);
                                    m_DataMap.TryGetValue(guid, out Data data);
                                    data.MobileSize = size;
                                    m_DataMap[guid] = data;
                                };

                                foreach (var treeItem in AssetView.SelectedItems)
                                    Do(treeItem);

                                AssetDatabase.Refresh();
                            }

                            menu.AddItem(new GUIContent($"x{size}"), false, () => { ChangeMobileSize(size); });
                        }

                        GenericMenu menu = new GenericMenu();
                        AddMobileSizeGenericMenu(menu, 512);
                        AddMobileSizeGenericMenu(menu, 1024);
                        AddMobileSizeGenericMenu(menu, 2048);
                        return menu;
                    }
                case PropCols.IOSFormat:
                    {
                        GenericMenu menu = new GenericMenu();
                        AddTextureFormatGenericMenu(menu, TextureImporterFormat.AutomaticCompressed, "iPhone");
                        AddTextureFormatGenericMenu(menu, TextureImporterFormat.ASTC_4x4, "iPhone");
                        AddTextureFormatGenericMenu(menu, TextureImporterFormat.ASTC_6x6, "iPhone");
                        return menu;
                    }
                case PropCols.AndroidFormat:
                    {
                        GenericMenu menu = new GenericMenu();
                        AddTextureFormatGenericMenu(menu, TextureImporterFormat.AutomaticCompressed, "Android");
                        AddTextureFormatGenericMenu(menu, TextureImporterFormat.ASTC_4x4, "Android");
                        AddTextureFormatGenericMenu(menu, TextureImporterFormat.ASTC_6x6, "Android");
                        return menu;
                    }
                case PropCols.TextureType:
                    {
                        void AddTextureTypeGenericMenu(GenericMenu menu, TextureImporterType type)
                        {
                            void ChangeTextureType(TextureImporterType type)
                            {
                                void Do(string path)
                                {
                                    var importer = AssetImporter.GetAtPath(path) as TextureImporter;
                                    if (importer == null) return;

                                    importer.textureType = type;
                                    importer.SaveAndReimport();

                                    var guid = AssetDatabase.AssetPathToGUID(path);
                                    m_DataMap.TryGetValue(guid, out Data data);
                                    data.TextureType = type;
                                    m_DataMap[guid] = data;
                                };

                                foreach (var treeItem in AssetView.SelectedItems)
                                    Do(treeItem);

                                AssetDatabase.Refresh();
                            }

                            menu.AddItem(new GUIContent(type.ToString()), false, () => { ChangeTextureType(type); });
                        }

                        GenericMenu menu = new GenericMenu();
                        AddTextureTypeGenericMenu(menu, TextureImporterType.Default);
                        AddTextureTypeGenericMenu(menu, TextureImporterType.Sprite);
                        AddTextureTypeGenericMenu(menu, TextureImporterType.NormalMap);
                        return menu;
                    }
                case PropCols.sRGB:
                    {
                        void AddsRGBGenericMenu(GenericMenu menu, bool sRGB)
                        {
                            void ChangesRGB(bool sRGB)
                            {
                                void Do(string path)
                                {
                                    var importer = AssetImporter.GetAtPath(path) as TextureImporter;
                                    if (importer == null) return;

                                    importer.sRGBTexture = sRGB;
                                    importer.SaveAndReimport();

                                    var guid = AssetDatabase.AssetPathToGUID(path);
                                    m_DataMap.TryGetValue(guid, out Data data);
                                    data.sRGB = sRGB;
                                    m_DataMap[guid] = data;
                                };

                                foreach (var treeItem in AssetView.SelectedItems)
                                    Do(treeItem);

                                AssetDatabase.Refresh();
                            }

                            menu.AddItem(new GUIContent(sRGB.ToString()), false, () => { ChangesRGB(sRGB); });
                        }

                        GenericMenu menu = new GenericMenu();
                        AddsRGBGenericMenu(menu, true);
                        AddsRGBGenericMenu(menu, false);
                        return menu;
                    }
                case PropCols.CenerateMipMaps:
                    {
                        void AddGenerateMipMapsGenericMenu(GenericMenu menu, bool enable)
                        {
                            void ChangesGenerateMipMaps(bool enable)
                            {
                                void Do(string path)
                                {
                                    var importer = AssetImporter.GetAtPath(path) as TextureImporter;
                                    if (importer == null) return;

                                    importer.mipmapEnabled = enable;
                                    importer.SaveAndReimport();

                                    var guid = AssetDatabase.AssetPathToGUID(path);
                                    m_DataMap.TryGetValue(guid, out Data data);
                                    data.GenerateMipMaps = enable;
                                    m_DataMap[guid] = data;
                                };

                                foreach (var treeItem in AssetView.SelectedItems)
                                    Do(treeItem);

                                AssetDatabase.Refresh();
                            }

                            menu.AddItem(new GUIContent(enable.ToString()), false, () => { ChangesGenerateMipMaps(enable); });
                        }

                        GenericMenu menu = new GenericMenu();
                        AddGenerateMipMapsGenericMenu(menu, true);
                        AddGenerateMipMapsGenericMenu(menu, false);
                        return menu;
                    }

                default: return null;
            }
        }


        private void AddTextureFormatGenericMenu(GenericMenu menu, TextureImporterFormat format, string platform)
        {
            void ChangeTextureFormat(TextureImporterFormat format, string platform)
            {
                void Do(string path)
                {
                    var importer = AssetImporter.GetAtPath(path) as TextureImporter;
                    if (importer == null) return;

                    var platformTextureSettings = importer.GetPlatformTextureSettings(platform);
                    if (platformTextureSettings == null) return;

                    if (format == TextureImporterFormat.AutomaticCompressed)
                    {
                        platformTextureSettings.overridden = false;
                    }
                    else
                    {
                        platformTextureSettings.overridden = true;
                    }

                    platformTextureSettings.format = format;

                    importer.SetPlatformTextureSettings(platformTextureSettings);
                    importer.SaveAndReimport();

                    var guid = AssetDatabase.AssetPathToGUID(path);
                    m_DataMap.TryGetValue(guid, out Data data);
                    switch (platform)
                    {
                        case "Android": data.AndroidFormat = format; break;
                        case "IOS": data.IOSFormat = format; break;
                    }
                    m_DataMap[guid] = data;
                };

                foreach (var treeItem in AssetView.SelectedItems)
                    Do(treeItem);

                AssetDatabase.Refresh();
            }

            menu.AddItem(new GUIContent(format.ToString()), false, () => { ChangeTextureFormat(format, platform); });
        }

    }
}
