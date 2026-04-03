#nullable enable
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using ThreeMahjong.Rounds;

namespace ThreeMahjong.Serializables
{
    [System.Serializable]
    public struct Session
    {
        public bool hasAfterDiscard;
        public AfterDiscard afterDiscard;
        public bool hasAfterDraw;
        public AfterDraw afterDraw;
        public bool hasBeforeClosedQuad;
        public BeforeClosedQuad beforeClosedQuad;
        public bool hasBeforeAddedOpenQuad;
        public BeforeAddedOpenQuad beforeAddedOpenQuad;

        public Session(ThreeMahjong.AfterDiscard source)
        {
            hasAfterDiscard = true;
            hasAfterDraw = false;
            hasBeforeAddedOpenQuad = false;
            hasBeforeClosedQuad = false;

            afterDiscard = source.ToSerializable();
            afterDraw = default;
            beforeAddedOpenQuad = default;
            beforeClosedQuad = default;
        }

        public Session(ThreeMahjong.AfterDraw source)
        {
            hasAfterDiscard = false;
            hasAfterDraw = true;
            hasBeforeAddedOpenQuad = false;
            hasBeforeClosedQuad = false;

            afterDiscard = default;
            afterDraw = source.ToSerializable();
            beforeAddedOpenQuad = default;
            beforeClosedQuad = default;
        }

        public Session(ThreeMahjong.BeforeAddedOpenQuad source)
        {
            hasAfterDiscard = false;
            hasAfterDraw = false;
            hasBeforeAddedOpenQuad = true;
            hasBeforeClosedQuad = false;

            afterDiscard = default;
            afterDraw = default;
            beforeAddedOpenQuad = source.ToSerializable();
            beforeClosedQuad = default;
        }

        public Session(ThreeMahjong.BeforeClosedQuad source)
        {
            hasAfterDiscard = false;
            hasAfterDraw = false;
            hasBeforeAddedOpenQuad = false;
            hasBeforeClosedQuad = true;

            afterDiscard = default;
            afterDraw = default;
            beforeAddedOpenQuad = default;
            beforeClosedQuad = source.ToSerializable();
        }

        readonly public IController Deserialize()
        {
            if (hasAfterDiscard)
            {
                return afterDiscard.Deserialzie();
            }
            if (hasAfterDraw)
            {
                return afterDraw.Deserialzie();
            }
            if (hasBeforeAddedOpenQuad)
            {
                return beforeAddedOpenQuad.Deserialzie();
            }
            if (hasBeforeClosedQuad)
            {
                return beforeClosedQuad.Deserialzie();
            }
            throw new System.Exception("no controller");
        }

        readonly public string ToJson(bool prettyPrint = false)
        {
            return JsonUtility.ToJson(this, prettyPrint);
        }
        public static IController FromJson(string json)
        {
            return JsonUtility.FromJson<Session>(json).Deserialize();
        }
    }

    [System.Serializable]
    public struct Tile
    {
        public int index;
        public TileType type;
        public bool red;

        public Tile(ThreeMahjong.Tile source)
        {
            index = source.index;
            red = source.red;
            type = source.type;
        }
        readonly public ThreeMahjong.Tile Deserialzie()
        {
            return ThreeMahjong.Tile.FromSerializable(this);
        }
    }

    [System.Serializable]
    public struct Game
    {
        public int roundWindCount;
        public int displayRoundCount;
        public int riichiScore;
        public int 本場;
        public int 連荘;
        public bool useDeterministicRoundSeeds;
        public uint nextRoundSeed;
        public PlayerIndex firstDealer;
        public int[] scores;
        public RuleSetting rule;

        public Game(ThreeMahjong.Game source)
        {
            displayRoundCount = source.DisplayRoundCount;
            firstDealer = source.firstDealer;
            riichiScore = source.riichiScore;
            roundWindCount = source.RoundWindCount;
            rule = source.rule;
            scores = source.seats.Select(_ => _.score).ToArray();
            本場 = source.本場;
            連荘 = source.連荘;
            useDeterministicRoundSeeds = source.UseDeterministicRoundSeeds;
            nextRoundSeed = source.NextRoundSeed;
        }

        public readonly ThreeMahjong.Game Deserialize()
        {
            return ThreeMahjong.Game.FromSerializable(this);
        }
    }

    [System.Serializable]
    public struct Round
    {
        public Game game;
        public WallTile wallTile;
        public DeadWallTile deadWallTile;
        public Player[] players;
        public TileType roundWind;
        public PlayerIndex dealer;
        public int[] totalDiscardedTiles;

        public Round(ThreeMahjong.Round source)
        {
            game = source.game.ToSerializable();
            deadWallTile = source.deadWallTile.ToSerializable();
            dealer = source.dealer;
            players = source.players.Select(_ => _.ToSerializable()).ToArray();
            roundWind = source.roundWind;
            totalDiscardedTiles = source.totalDiscardedTiles.Select(_ => _.index).ToArray();
            wallTile = source.wallTile.ToSerializable();
        }

        public readonly ThreeMahjong.Round Deserialize()
        {
            return ThreeMahjong.Round.FromSerializable(this);
        }
    }

    [System.Serializable]
    public struct Player
    {
        public PlayerIndex index;
        public Hand hand;
        public int[] discardPile;
        public int[] discardedTiles;
        public int[] nukiTiles;
        public TileType wind;
        public int riichiIndexInDiscardPile;
        public int riichiIndexInTotalDiscardTiles;

        public bool doubleRiichi;
        public bool openRiichi;
        public bool 一発;
        public bool furitenByOtherPlayers;

        public Player(Rounds.Player source)
        {
            index = source.index;
            discardedTiles = source.discardedTiles.Select(_ => _.index).ToArray();
            discardPile = source.discardPile.Select(_ => _.index).ToArray();
            nukiTiles = source.nukiTiles.Select(_ => _.index).ToArray();
            doubleRiichi = source.DoubleRiichi;
            hand = source.hand.ToSerializable();
            openRiichi = source.OpenRiichi;
            riichiIndexInDiscardPile = source.RiichiIndexInDiscardPile ?? -1;
            riichiIndexInTotalDiscardTiles = source.RiichiIndexInTotalDiscardTiles ?? -1;
            wind = source.wind;
            furitenByOtherPlayers = source.FuritenByOtherPlayers;
            一発 = source.一発;
        }

        public readonly Rounds.Player Deserialize(ThreeMahjong.Round round)
        {
            return Rounds.Player.FromSerializable(this, round);
        }
    }

    [System.Serializable]
    public struct Hand
    {
        public int[] tiles;
        public Meld[] melds;

        public Hand(ThreeMahjong.Hand source)
        {
            tiles = source.tiles.Select(_ => _.index).ToArray();
            melds = source.melds.Select(_ => _.ToSerializable()).ToArray();
        }

        public readonly ThreeMahjong.Hand Deserialize(Rounds.Player owner)
        {
            return ThreeMahjong.Hand.FromSerializable(this, owner);
        }
    }

    [System.Serializable]
    public struct Meld
    {
        [System.Serializable]
        public struct Pair
        {
            public int tile;
            public PlayerIndex fromPlayerIndex;
        }
        public Pair[] tileFroms;

        public Meld(ThreeMahjong.Meld source)
        {
            tileFroms = source.tileFroms
                .Select(_ => new Pair() { tile = _.tile.index, fromPlayerIndex = _.fromPlayerIndex })
                .ToArray();
        }

        public ThreeMahjong.Meld Deserialize(ThreeMahjong.WallTile wallTile)
        {
            return ThreeMahjong.Meld.FromSerializable(this, wallTile);
        }
    }

    [System.Serializable]
    public struct WallTile
    {
        public int[] tiles;
        public Tile[] allTiles;
        public uint randomSeed;

        public WallTile(ThreeMahjong.WallTile source)
        {
            allTiles = source.allTiles.Select(_ => _.ToSerializable()).ToArray();
            tiles = source.tiles.Select(_ => _.index).ToArray();
            randomSeed = source.randomSeed;
        }

        public readonly ThreeMahjong.WallTile Deserialize()
        {
            return ThreeMahjong.WallTile.FromSerializable(this);
        }
    }

    [System.Serializable]
    public struct DeadWallTile
    {
        public int[] tiles;
        public int[] doraIndicatorTiles;
        public int[] uraDoraIndicatorTiles;
        public int drawnCount;

        public DeadWallTile(ThreeMahjong.DeadWallTile source)
        {
            doraIndicatorTiles = source.doraIndicatorTiles.Select(_ => _.index).ToArray();
            drawnCount = source.DrawnCount;
            tiles = source.tiles.Select(_ => _.index).ToArray();
            uraDoraIndicatorTiles = source.uraDoraIndicatorTiles.Select(_ => _.index).ToArray();
        }

        public readonly ThreeMahjong.DeadWallTile Deserialize(ThreeMahjong.WallTile wallTile)
        {
            return ThreeMahjong.DeadWallTile.FromSerializable(this, wallTile);
        }
    }
}
