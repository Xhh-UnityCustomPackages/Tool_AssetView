using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Tool.AssetView
{
    public interface IFunction
    {
        bool enable { get; }
        GUIContent titleContent { get; }


        bool CanShow(DataBaseType type);
        void DoCustomFunction(List<string> guids);
    }
}
