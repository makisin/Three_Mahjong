using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using ThreeMahjong;
using System.Linq;
#nullable enable

namespace ThreeMahjong.Test
{
    public class Hand
    {
        [Test]
        [TestCase(true, TileType.M1, TileType.M2, TileType.M3, TileType.M3)]
        [TestCase(false, TileType.M1, TileType.M2, TileType.M3, TileType.M6)]
        [TestCase(false, TileType.M2, TileType.M2, TileType.M2, TileType.M6)]
        [TestCase(true, TileType.M2, TileType.M2, TileType.M2, TileType.M2)]
        [TestCase(true, TileType.M2, TileType.M3, TileType.M4, TileType.M1)]
        [TestCase(true, TileType.M2, TileType.M3, TileType.M4, TileType.M4)]
        [TestCase(false, TileType.M2, TileType.M3, TileType.M4, TileType.M2)]
        [TestCase(false, TileType.M2, TileType.M3, TileType.M4, TileType.M3)]
        public void 喰い替え(bool expected, TileType tile1, TileType tile2, TileType tileFromOtherPlayer, TileType tileToDiscard)
        {
            var meld = new Meld((new Tile(0, tile1, false), PlayerIndex.Index0),
                (new Tile(0, tile2, false), PlayerIndex.Index0),
                (new Tile(0, tileFromOtherPlayer, false), PlayerIndex.Index1));

            Assert.AreEqual(expected, meld.Is喰い替え(new Tile(0, tileToDiscard, false)));
        }

        [Test]
        [TestCase(0,
            TileType.M1, TileType.M1,
            TileType.M3, TileType.M3,
            TileType.M5, TileType.M5,
            TileType.M8, TileType.M8,
            TileType.P1, TileType.P1,
            TileType.白, TileType.白,
            TileType.中
        )]
        [TestCase(1,
            TileType.M1, TileType.M1,
            TileType.M3, TileType.M3,
            TileType.M5, TileType.M5,
            TileType.M8, TileType.M8,
            TileType.P1, TileType.P1,
            TileType.白, TileType.白, TileType.白
        )]
        [TestCase(2,
            TileType.M1, TileType.M1,
            TileType.S3, TileType.S3,
            TileType.P5, TileType.P5,
            TileType.中, TileType.中,
            TileType.白, TileType.白,
            TileType.白, TileType.白,
            TileType.發
        )]
        public void 七対子向聴数(int expected, params TileType[] tiles)
        {
            var round = Game.Create(0, new RuleSetting()).Round;

            var hand = round.players[0].hand;
            hand.tiles.Clear();
            hand.tiles.AddRange(RandomUtil.GenerateShuffledArray(tiles.Select(_ => new Tile(0, _, red: false)).ToList()));
            Assert.IsTrue(hand.向聴数IsLessThanOrEqual(expected));
            Assert.IsFalse(hand.向聴数IsLessThanOrEqual(expected - 1));
            if (expected == 0)
            {
                Assert.AreEqual(1, hand.GetWinningTiles().Length);
            }
            else
            {
                Assert.AreEqual(0, hand.GetWinningTiles().Length);
            }
            Assert.AreEqual(expected, hand.Solve().向聴数);
        }
        [Test]
        [TestCase(new[] { 役.門前清自摸和, 役.七対子 },
            TileType.P6, TileType.P6,
            TileType.P7, TileType.P7,
            TileType.P9, TileType.P9,
            TileType.S1, TileType.S1,
            TileType.S2, TileType.S2,
            TileType.北, TileType.北,
            TileType.中, TileType.中
        )]
        [TestCase(new[] { 役.門前清自摸和, 役.七対子, 役.混老頭 },
            TileType.M1, TileType.M1,
            TileType.P1, TileType.P1,
            TileType.S1, TileType.S1,
            TileType.S9, TileType.S9,
            TileType.北, TileType.北,
            TileType.中, TileType.中,
            TileType.發, TileType.發
        )]
        public void Yaku(役[] expecteds, params TileType[] tiles)
        {
            var round = Game.Create(0, new RuleSetting()).Round;
            var hand = round.players[0].hand;
            hand.tiles.Clear();
            hand.tiles.AddRange(tiles.Select(_ => new Tile(0, _, red: false)));
            var yakus = hand.Solve().ChoiceCompletedHand(round.players[0], tiles[0], null, false, false, false, false, false, false, false).Yakus;
            foreach (var it in expecteds)
            {
                Assert.IsTrue(yakus.ContainsKey(it), string.Join(", ", yakus.Keys.Select(_ => _.ToString())));
            }
            Assert.AreEqual(expecteds.Length, yakus.Count);
        }

        [Test]
        public void 七対子形でも二盃口が優先される()
        {
            var round = Game.Create(0, new RuleSetting()).Round;
            var hand = round.players[0].hand;
            var tiles = new[]
            {
                TileType.S8, TileType.S8,
                TileType.P2, TileType.P2,
                TileType.P3, TileType.P3,
                TileType.P4, TileType.P4,
                TileType.M5, TileType.M5,
                TileType.M6, TileType.M6,
                TileType.M7, TileType.M7,
            };

            hand.tiles.Clear();
            hand.tiles.AddRange(tiles.Select(_ => new Tile(0, _, red: false)));
            var yakus = hand.Solve().ChoiceCompletedHand(round.players[0], TileType.S8, null, false, false, false, false, false, false, false).Yakus;

            Assert.IsTrue(yakus.ContainsKey(役.二盃口), string.Join(", ", yakus.Keys.Select(_ => _.ToString())));
            Assert.IsFalse(yakus.ContainsKey(役.七対子), string.Join(", ", yakus.Keys.Select(_ => _.ToString())));

            var expecteds = new[] { 役.門前清自摸和, 役.タンヤオ, 役.二盃口 };
            foreach (var it in expecteds)
            {
                Assert.IsTrue(yakus.ContainsKey(it), string.Join(", ", yakus.Keys.Select(_ => _.ToString())));
            }
            Assert.AreEqual(expecteds.Length, yakus.Count);
        }

        [Test]
        public void 基本点が同じ場合は翻数が高い手を優先する()
        {
            var round = Game.Create(0, new RuleSetting()).Round;
            var hand = round.players[0].hand;
            var tiles = new[]
            {
                new Tile(0, TileType.P3, red: false),
                new Tile(0, TileType.P3, red: false),
                new Tile(0, TileType.P4, red: false),
                new Tile(0, TileType.P4, red: false),
                new Tile(0, TileType.P5, red: true),
                new Tile(0, TileType.P5, red: false),
                new Tile(0, TileType.S3, red: false),
                new Tile(0, TileType.S3, red: false),
                new Tile(0, TileType.S4, red: false),
                new Tile(0, TileType.S4, red: false),
                new Tile(0, TileType.S5, red: false),
                new Tile(0, TileType.S5, red: false),
                new Tile(0, TileType.S7, red: false),
                new Tile(0, TileType.S7, red: false),
            };

            hand.tiles.Clear();
            hand.tiles.AddRange(tiles);
            var completed = hand.Solve().ChoiceCompletedHand(
                newTileInHand: TileType.P5,
                ownWind: TileType.東,
                roundWind: TileType.東,
                ronTarget: round.players[1],
                riichi: false,
                doubleRiichi: false,
                openRiichi: false,
                一発: false,
                嶺上: false,
                海底: false,
                河底: false,
                天和: false,
                地和: false,
                人和: false,
                doraTiles: new TileType[0],
                uraDoraTiles: new TileType[0],
                槍槓: false,
                nukiDoraCount: 4,
                handCap: round.game.rule.handCap);
            var yakus = completed.Yakus;

            Assert.IsTrue(yakus.ContainsKey(役.二盃口), string.Join(", ", yakus.Keys.Select(_ => _.ToString())));
            Assert.IsTrue(yakus.ContainsKey(役.平和), string.Join(", ", yakus.Keys.Select(_ => _.ToString())));
            Assert.IsFalse(yakus.ContainsKey(役.七対子), string.Join(", ", yakus.Keys.Select(_ => _.ToString())));
            Assert.AreEqual(10, completed.Han);
            Assert.AreEqual(30, completed.Fu);
            Assert.AreEqual(ScoreType.倍満, completed.基本点(round.game.rule.handCap).type);
        }
    }
}
