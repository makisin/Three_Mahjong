#nullable enable
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using ThreeMahjong.Rounds;

namespace ThreeMahjong.Hands
{
    public class Solution
    {
        public readonly int 向聴数;
        public readonly Structure[] structures;

        public Solution(Hand hand)
        {
            (向聴数, structures) = Structure.Build(hand);
        }

        public CompletedHand ChoiceCompletedHand(Player player,
            TileType newTileInHand,
            Player? ronTarget,
            bool 嶺上,
            bool 海底,
            bool 河底,
            bool 天和,
            bool 地和,
            bool 人和,
            bool 槍槓)
        {
            return ChoiceCompletedHand(
                newTileInHand,
                ownWind: player.wind,
                roundWind: player.round.roundWind,
                ronTarget: ronTarget,
                riichi: player.Riichi,
                doubleRiichi: player.DoubleRiichi,
                openRiichi: player.OpenRiichi,
                一発: player.一発,
                嶺上: 嶺上,
                海底: 海底,
                河底: 河底,
                天和: 天和,
                地和: 地和,
                人和: 人和,
                doraTiles: player.round.deadWallTile.DoraTiles,
                uraDoraTiles: player.round.deadWallTile.UraDoraTiles,
                槍槓: 槍槓,
                nukiDoraCount: player.NukiDoraCount,
                handCap: player.round.game.rule.handCap);
        }

        public CompletedHand ChoiceCompletedHand(TileType newTileInHand, TileType ownWind, TileType roundWind,
            Player? ronTarget,
            bool riichi,
            bool doubleRiichi,
            bool openRiichi,
            bool 一発,
            bool 嶺上,
            bool 海底,
            bool 河底,
            bool 天和,
            bool 地和,
            bool 人和,
            TileType[] doraTiles,
            TileType[] uraDoraTiles,
            bool 槍槓,
            int nukiDoraCount,
            Rules.HandCap handCap)
        {
            var result = (score: int.MinValue, yakuman: int.MinValue, han: int.MinValue, fu: int.MinValue, completed: default(CompletedHand));

            foreach (var it in structures)
            {
                var item = new CompletedHand(it, newTileInHand, ownWind: ownWind, roundWind: roundWind,
                    ronTarget: ronTarget,
                    riichi: riichi,
                    doubleRiichi: doubleRiichi,
                    openRiichi: openRiichi,
                    一発: 一発,
                    嶺上: 嶺上,
                    海底: 海底,
                    河底: 河底,
                    天和: 天和,
                    地和: 地和,
                    人和: 人和,
                    槍槓: 槍槓,
                    doraTiles: doraTiles,
                    uraDoraTiles: uraDoraTiles,
                    nukiDoraCount: nukiDoraCount);
                var itemScore = item.基本点(handCap).score;
                var itemYakuman = item.役満.Values.Sum();
                var itemHan = item.Han;
                var itemFu = item.Fu;
                if (IsBetter(itemScore, itemYakuman, itemHan, itemFu, result.score, result.yakuman, result.han, result.fu))
                {
                    result = (itemScore, itemYakuman, itemHan, itemFu, item);
                }
            }

            return result.completed;
        }

        static bool IsBetter(int score, int yakuman, int han, int fu, int currentScore, int currentYakuman, int currentHan, int currentFu)
        {
            if (score != currentScore)
            {
                return score > currentScore;
            }
            if (yakuman != currentYakuman)
            {
                return yakuman > currentYakuman;
            }
            if (han != currentHan)
            {
                return han > currentHan;
            }
            return fu > currentFu;
        }
    }
}
