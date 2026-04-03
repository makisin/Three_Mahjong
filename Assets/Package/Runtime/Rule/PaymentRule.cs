using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#nullable enable

namespace ThreeMahjong.Rules
{
    [System.Serializable]
    public class PaymentRule
    {
        public int 返し = 40000;
        public int[] ウマ = new[] { 20, 0, -20 };
    }
}
