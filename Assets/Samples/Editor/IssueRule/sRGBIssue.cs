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
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer.sRGBTexture)
        {
            return true;
        }

        return false;
    }
}
