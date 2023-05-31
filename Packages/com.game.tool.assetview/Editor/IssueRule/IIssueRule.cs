using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Tool.AssetView
{
    //问题列表
    public interface IIssueRule
    {
        bool IsIssueAsset(string guid);
    }
}
