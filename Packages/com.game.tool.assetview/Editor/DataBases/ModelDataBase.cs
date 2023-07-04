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
    internal partial class ModelDataBase : IDataBase
    {
        Dictionary<string, Data> m_DataMap = new Dictionary<string, Data>();
        public string filter { get { return "t:model"; } }
        public int GetCount() { return m_DataMap.Count; }
        public void Clear() { m_DataMap.Clear(); }
        public string GetBtnName() { return "模型资源"; }

        /// <summary>
        /// 获取行信息
        /// </summary>
        public string GetColumnInfos(string guid, int column)
        {
            Data data = GetModelData(guid);
            var sortType = (PropCols)column;

            switch (sortType)
            {
                case PropCols.RuleName: return Path.GetFileName(data.assetPath);
                case PropCols.Path: return data.assetPath;
                case PropCols.MeshCompression: return data.MeshCompression.ToString();
                case PropCols.Read_Write: return data.Read_Write.ToString();
                case PropCols.OptimizeMeshPolygons: return data.OptimizeMeshPolygons.ToString();
                case PropCols.Normal: return data.Normal.ToString();
                case PropCols.UVS: return data.UVS.ToString();
                case PropCols.MeshCount: return data.MeshCount.ToString();
                case PropCols.VertexCount: return data.VertexCount.ToString();
                case PropCols.TriCount: return data.TriCount.ToString();
                case PropCols.SkinCount: return data.SkinCount.ToString();
                case PropCols.BoneCount: return data.BoneCount.ToString();
                case PropCols.ImportAnimation: return data.ImportAnimation.ToString();
                case PropCols.AnimationType: return data.AnimationType.ToString();
                case PropCols.StripBones: return data.StripBones.ToString();
                case PropCols.OptimizeGameObjects: return data.OptimizeGameObjects.ToString();
                case PropCols.AnimCompression: return data.AnimCompression.ToString();
                case PropCols.AnimationClipLength: return data.AnimationClipLength.ToString();
                case PropCols.IsLoop: return data.IsLoop ? "Loop" : string.Empty;
                case PropCols.AnimationClipSize: return EditorUtility.FormatBytes(data.AnimationClipSize);
                case PropCols.MaterialImportMode: return data.MaterialImportMode.ToString();
                default: return string.Empty;
            }
        }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort(string guid1, string guid2, bool ascending, int sortedColumnIndex)
        {
            var data1 = GetModelData(ascending ? guid1 : guid2);
            var data2 = GetModelData(ascending ? guid2 : guid1);

            if (string.IsNullOrEmpty(data1.assetPath) || string.IsNullOrEmpty(data2.assetPath))
            {
                return 0;
            }

            var sortType = (PropCols)sortedColumnIndex;

            switch (sortType)
            {
                case PropCols.RuleName:
                case PropCols.Path: return data1.assetPath.CompareTo(data2.assetPath);
                case PropCols.MeshCompression: return data1.MeshCompression.CompareTo(data2.MeshCompression);
                case PropCols.Read_Write: return data1.Read_Write.CompareTo(data2.Read_Write);
                case PropCols.OptimizeMeshPolygons: return data1.OptimizeMeshPolygons.CompareTo(data2.OptimizeMeshPolygons);
                case PropCols.Normal: return data1.Normal.CompareTo(data2.Normal);
                case PropCols.UVS: return data1.UVS.CompareTo(data2.UVS);
                case PropCols.MeshCount: return data1.MeshCount.CompareTo(data2.MeshCount);
                case PropCols.VertexCount: return data1.VertexCount.CompareTo(data2.VertexCount);
                case PropCols.TriCount: return data1.TriCount.CompareTo(data2.TriCount);
                case PropCols.SkinCount: return data1.SkinCount.CompareTo(data2.SkinCount);
                case PropCols.BoneCount: return data1.BoneCount.CompareTo(data2.BoneCount);
                case PropCols.StripBones: return data1.StripBones.CompareTo(data2.StripBones);
                case PropCols.ImportAnimation: return data1.ImportAnimation.CompareTo(data2.ImportAnimation);
                case PropCols.AnimationType: return data1.AnimationType.CompareTo(data2.AnimationType);
                case PropCols.OptimizeGameObjects: return data1.OptimizeGameObjects.CompareTo(data2.OptimizeGameObjects);
                case PropCols.AnimCompression: return data1.AnimCompression.CompareTo(data2.AnimCompression);
                case PropCols.AnimationClipLength: return data1.AnimationClipLength.CompareTo(data2.AnimationClipLength);
                case PropCols.IsLoop: return data1.IsLoop.CompareTo(data2.IsLoop);
                case PropCols.AnimationClipSize: return data1.AnimationClipSize.CompareTo(data2.AnimationClipSize);
                case PropCols.MaterialImportMode: return data1.MaterialImportMode.CompareTo(data2.MaterialImportMode);
                default: return 0;
            }
        }

        /// <summary>
        /// 获取标题
        /// </summary>
        public MultiColumnHeader GetMultiColumnHeader()
        {
            var columns = new[]
            {
                new Column { headerContent = EditorGUIUtility.TrTextContent("文件名"), allowToggleVisibility = false, minWidth = 50, autoResize = true, },
                new Column { headerContent = EditorGUIUtility.TrTextContent("路径"), minWidth = 100, autoResize = true, headerTextAlignment = TextAlignment.Right, },
                new Column { headerContent = EditorGUIUtility.TrTextContent("MeshCompression"), width = 80, autoResize = false, },
                new Column { headerContent = EditorGUIUtility.TrTextContent("Read|Write"), width = 80, autoResize = false, },
                new Column { headerContent = EditorGUIUtility.TrTextContent("OptimizeMesh"), width = 80, autoResize = false, },
                new Column { headerContent = EditorGUIUtility.TrTextContent("Normal"), width = 60, autoResize = false, },
                new Column { headerContent = EditorGUIUtility.TrTextContent("UVS"), width = 50, autoResize = false, },
                new Column { headerContent = EditorGUIUtility.TrTextContent("MeshCount"), width = 80, autoResize = false, },
                new Column { headerContent = EditorGUIUtility.TrTextContent("顶点数"), width = 50, autoResize = false, },
                new Column { headerContent = EditorGUIUtility.TrTextContent("面数量"), width = 50, autoResize = false, },
                new Column { headerContent = EditorGUIUtility.TrTextContent("StripBones"), width = 100, autoResize = false, },
                new Column { headerContent = EditorGUIUtility.TrTextContent("蒙皮数"), width = 50, autoResize = false, },
                new Column { headerContent = EditorGUIUtility.TrTextContent("骨骼数"), width = 50, autoResize = false, },
                new Column { headerContent = EditorGUIUtility.TrTextContent("ImportAnimation"), width = 100, autoResize = false, },
                new Column { headerContent = EditorGUIUtility.TrTextContent("AnimationType"), width = 100, autoResize = false, },
                new Column { headerContent = EditorGUIUtility.TrTextContent("OptimizeGameObjects"), width = 130, autoResize = false, },
                new Column { headerContent = EditorGUIUtility.TrTextContent("Anim.Compression"), width = 120, autoResize = false, },
                new Column { headerContent = EditorGUIUtility.TrTextContent("动作时长"), width = 80, autoResize = false, },
                new Column { headerContent = EditorGUIUtility.TrTextContent("循环"), width = 40, autoResize = false, },
                new Column { headerContent = EditorGUIUtility.TrTextContent("动作大小"), width = 60, autoResize = false, },
                new Column { headerContent = EditorGUIUtility.TrTextContent("材质导入模式"), width = 60, autoResize = false, },
            };

            Assert.AreEqual(columns.Length, Enum.GetValues(typeof(PropCols)).Length, "列数和枚举数量必须一致");
            var state = new MultiColumnHeaderState(columns);
            var multiColumnHeader = new MultiColumnHeader(state);
            multiColumnHeader.ResizeToFit();
            return multiColumnHeader;
        }

        Data GetModelData(string guid)
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
            var importer = AssetImporter.GetAtPath(assetPath) as ModelImporter;

            if (importer == null) { return default; }

            Data modelData = new Data();
            GetOtherInfo(assetPath, out long meshCount1, out long vertexCount1, out long triCount1, out int skinCount1);
            GetAnimationInfo(importer, out float clipLength1, out bool isLoop1);

            modelData.assetPath = assetPath;
            modelData.MeshCompression = importer.meshCompression;
            modelData.Read_Write = importer.isReadable;
            modelData.OptimizeMeshPolygons = importer.optimizeMeshPolygons;
            modelData.Normal = importer.importNormals;
            modelData.UVS = importer.generateSecondaryUV;
            modelData.MeshCount = meshCount1;
            modelData.VertexCount = vertexCount1;
            modelData.TriCount = triCount1;
            modelData.SkinCount = skinCount1;
            modelData.StripBones = importer.optimizeBones;
            modelData.BoneCount = importer.transformPaths.Length;
            modelData.ImportAnimation = importer.importAnimation;
            modelData.AnimationType = importer.animationType;
            modelData.OptimizeGameObjects = importer.optimizeGameObjects;
            modelData.AnimCompression = importer.animationCompression;
            modelData.AnimationClipLength = clipLength1;
            modelData.IsLoop = isLoop1;
            modelData.MaterialImportMode = importer.materialImportMode;
            // modelData.AnimationClipSize = RuleHelper.GetResSize<AnimationClip>(assetPath);

            m_DataMap[guid] = modelData;
            return modelData;
        }

        /// <summary>
        /// 获取动画信息
        /// </summary>
        static void GetAnimationInfo(ModelImporter importer, out float clipLength, out bool isLoop)
        {
            clipLength = 0;
            isLoop = false;

            if (importer == null || importer.clipAnimations == null || importer.clipAnimations.Length <= 0)
            {
                return;
            }

            var clip = importer.clipAnimations[0];

            if (clip != null)
            {
                isLoop = clip.loopTime;
            }

            if (importer.importedTakeInfos == null || importer.importedTakeInfos.Length <= 0)
            {
                return;
            }

            clipLength = importer.importedTakeInfos[0].bakeStopTime - importer.importedTakeInfos[0].bakeStartTime;
        }

        /// <summary>
        /// 获取模型相关信息
        /// </summary>
        static void GetOtherInfo(string assetPath, out long meshCount, out long vertexCount, out long triCount, out int skinCount)
        {
            vertexCount = 0;
            meshCount = 0;
            triCount = 0;
            skinCount = 0;

            var obj = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

            if (obj == null)
            {
                return;
            }

            var skinnedMeshRenderers = obj.GetComponentsInChildren<SkinnedMeshRenderer>();

            if (skinnedMeshRenderers != null)
            {
                skinCount = skinnedMeshRenderers.Length;

                foreach (var smr in skinnedMeshRenderers)
                {
                    vertexCount += smr.sharedMesh.vertexCount;
                    triCount += smr.sharedMesh.triangles.Length / 3;
                    meshCount++;
                }
            }

            MeshFilter[] filters = obj.GetComponentsInChildren<MeshFilter>(true);

            if (filters == null)
            {
                return;
            }

            for (int j = 0; j < filters.Length; j++)
            {
                MeshFilter f = filters[j];
                vertexCount += f.sharedMesh.vertexCount;
                triCount += f.sharedMesh.triangles.Length / 3;
                meshCount++;
            }

            //Debug.LogWarning("总共Mesh=" + meshCount + "   总共顶点=" + vertexCount + "   总共三角形=" + triCount);
        }

        enum PropCols
        {
            RuleName,
            Path,
            MeshCompression,
            Read_Write,
            OptimizeMeshPolygons,
            Normal,
            UVS,
            MeshCount,
            VertexCount,
            TriCount,
            StripBones,
            SkinCount,
            BoneCount,
            ImportAnimation,
            AnimationType,
            OptimizeGameObjects,
            AnimCompression,
            AnimationClipLength,
            IsLoop,
            AnimationClipSize,
            MaterialImportMode,
        }

        struct Data
        {
            public string assetPath;
            public ModelImporterMeshCompression MeshCompression;
            public bool Read_Write;
            public bool OptimizeMeshPolygons;
            public ModelImporterNormals Normal;
            public bool UVS;
            public long MeshCount;
            public long VertexCount;
            public long TriCount;
            public int SkinCount;
            public int BoneCount;
            public bool StripBones;
            public bool OptimizeGameObjects;
            public bool ImportAnimation;
            public ModelImporterAnimationType AnimationType;
            public ModelImporterAnimationCompression AnimCompression;
            public float AnimationClipLength;
            public bool IsLoop;
            public long AnimationClipSize;
            public ModelImporterMaterialImportMode MaterialImportMode;
        }


        public GenericMenu GetGenericMenu(int column)
        {
            switch ((PropCols)column)
            {
                case PropCols.StripBones:
                    {
                        void AddsStripBonesGenericMenu(GenericMenu menu, bool stripBones)
                        {
                            void ChangesStripBones(bool stripBones)
                            {
                                void Do(string path)
                                {
                                    var importer = AssetImporter.GetAtPath(path) as ModelImporter;
                                    if (importer == null) return;

                                    importer.optimizeBones = stripBones;
                                    importer.SaveAndReimport();

                                    var guid = AssetDatabase.AssetPathToGUID(path);
                                    m_DataMap.TryGetValue(guid, out Data data);
                                    data.StripBones = stripBones;
                                    m_DataMap[guid] = data;
                                };

                                foreach (var treeItem in AssetView.SelectedItems)
                                    Do(treeItem);

                                AssetDatabase.Refresh();
                            }

                            menu.AddItem(new GUIContent(stripBones.ToString()), false, () => { ChangesStripBones(stripBones); });
                        }

                        GenericMenu menu = new GenericMenu();
                        AddsStripBonesGenericMenu(menu, true);
                        AddsStripBonesGenericMenu(menu, false);
                        return menu;
                    }
                case PropCols.MaterialImportMode:
                    {
                        void AddsMaterialImportModeGenericMenu(GenericMenu menu, ModelImporterMaterialImportMode mode)
                        {
                            void ChangesMaterialImportMode(ModelImporterMaterialImportMode mode)
                            {
                                void Do(string path)
                                {
                                    var importer = AssetImporter.GetAtPath(path) as ModelImporter;
                                    if (importer == null) return;

                                    importer.materialImportMode = mode;
                                    importer.SaveAndReimport();

                                    var guid = AssetDatabase.AssetPathToGUID(path);
                                    m_DataMap.TryGetValue(guid, out Data data);
                                    data.MaterialImportMode = mode;
                                    m_DataMap[guid] = data;
                                }

                                foreach (var treeItem in AssetView.SelectedItems)
                                    Do(treeItem);

                                AssetDatabase.Refresh();
                            }

                            menu.AddItem(new GUIContent(mode.ToString()), false, () => { ChangesMaterialImportMode(mode); });
                        }

                        GenericMenu menu = new GenericMenu();
                        AddsMaterialImportModeGenericMenu(menu, ModelImporterMaterialImportMode.None);
                        AddsMaterialImportModeGenericMenu(menu, ModelImporterMaterialImportMode.ImportStandard);
                        AddsMaterialImportModeGenericMenu(menu, ModelImporterMaterialImportMode.ImportViaMaterialDescription);
                        return menu;
                    }

            }
            return null;
        }
    }
}
