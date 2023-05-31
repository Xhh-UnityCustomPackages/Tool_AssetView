using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Game.Tool.AssetView
{
    public class NormalMapIssue : IIssueRule
    {
        public bool IsIssueAsset(string guid)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            if (path.Contains("_N."))
            {
                var importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer.textureType != TextureImporterType.NormalMap)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
