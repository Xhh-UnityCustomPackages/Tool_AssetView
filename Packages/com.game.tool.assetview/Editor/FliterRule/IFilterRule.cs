using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Tool.AssetView
{
    public interface IFilterRule
    {
        GUIContent titleContent { get; }
        string advanceFilter { get; }
        string[] path { get; }


    }
}
