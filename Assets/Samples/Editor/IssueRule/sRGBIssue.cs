using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Game.Tool.AssetView;

public class sRGBIssue : IIssueRule
{
    public bool IsIssueAsset(string guid)
    {
        //这只是一个测试实例
        var path = AssetDatabase.GUIDToAssetPath(guid);
        if (AssetImporter.GetAtPath(path) is TextureImporter importer)
        {
            if (importer.sRGBTexture)
            {
                return true;
            }
        }

        return false;
    }
}
