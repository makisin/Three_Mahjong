using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#nullable enable

namespace ThreeMahjong.Serializables
{
    [System.Serializable]
    public struct AfterDiscard
    {
        public Round round;

        public PlayerIndex discardPlayerIndex;

        public AfterDiscard(ThreeMahjong.AfterDiscard source)
        {
            round = source.Round.ToSerializable();

            discardPlayerIndex = source.DiscardPlayerIndex;
        }
        readonly public ThreeMahjong.AfterDiscard Deserialzie()
        {
            return ThreeMahjong.AfterDiscard.FromSerializable(this);
        }
    }

    [System.Serializable]
    public struct AfterDraw
    {
        public Round round;

        public PlayerIndex drawPlayerIndex;
        public int newTileInHand;
        public bool openDoraAfterDiscard;
        public bool 嶺上;

        public AfterDraw(ThreeMahjong.AfterDraw source)
        {
            round = source.Round.ToSerializable();

            drawPlayerIndex = source.DrawPlayerIndex;
            newTileInHand = source.newTileInHand?.index ?? -1;
            openDoraAfterDiscard = source.openDoraAfterDiscard;
            嶺上 = source.嶺上;
        }
        readonly public ThreeMahjong.AfterDraw Deserialzie()
        {
            return ThreeMahjong.AfterDraw.FromSerializable(this);
        }
    }

    [System.Serializable]
    public struct BeforeAddedOpenQuad
    {
        public Round round;

        public PlayerIndex declarePlayerIndex;
        public int tile;
        
        public BeforeAddedOpenQuad(ThreeMahjong.BeforeAddedOpenQuad source)
        {
            round = source.Round.ToSerializable();

            declarePlayerIndex = source.DeclarePlayerIndex;
            tile = source.tile.index;
        }
        readonly public ThreeMahjong.BeforeAddedOpenQuad Deserialzie()
        {
            return ThreeMahjong.BeforeAddedOpenQuad.FromSerializable(this);
        }
    }

    [System.Serializable]
    public struct BeforeClosedQuad
    {
        public Round round;

        public PlayerIndex declarePlayerIndex;
        public TileType tile;

        public BeforeClosedQuad(ThreeMahjong.BeforeClosedQuad source)
        {
            round = source.Round.ToSerializable();

            declarePlayerIndex = source.DeclarePlayerIndex;
            tile = source.tile;
        }
        readonly public ThreeMahjong.BeforeClosedQuad Deserialzie()
        {
            return ThreeMahjong.BeforeClosedQuad.FromSerializable(this);
        }
    }
}
