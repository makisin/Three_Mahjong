#nullable enable
using System.Collections;
using System.Collections.Generic;
using ThreeMahjong.Rounds;
using UnityEngine;

namespace ThreeMahjong.ScriptableRules
{
    public interface ICompletedResultModifier
    {
        void Modify(ref CompletedResult source);
    }
}
