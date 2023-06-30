using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Game.Tool.AssetView
{
    public class PCTexture2048Function : IFunction
    {
        public bool enable => true;

        public GUIContent titleContent => EditorGUIUtility.TrTextContent("PC 贴图 2048");

        public bool CanShow(DataBaseType type)
        {
            return type == DataBaseType.Texture;
        }

        public void DoCustomFunction(List<string> guids)
        {
            for (int i = 0; i < guids.Count; i++)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[i]);
                var importer = AssetImporter.GetAtPath(path) as TextureImporter;
                importer.maxTextureSize = 2048;
                importer.SaveAndReimport();
            }
        }
    }
}
