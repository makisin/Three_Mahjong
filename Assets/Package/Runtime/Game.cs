using System.Collections.Generic;
using System.Linq;
#nullable enable

namespace ThreeMahjong
{
    public class Game
    {
        public const int PlayerCount = 3;
        readonly TileType[] winds = new[] { TileType.東, TileType.南, TileType.西 };
        public int RoundWindCount { get; private set; } = 0;
        public TileType Wind => winds[RoundWindCount % winds.Length];
        public int DisplayRoundCount { get; private set; } = 1;
        public int riichiScore;
        public int 本場 { get; private set; }
        public int 連荘 { get; private set; }
        public readonly PlayerIndex firstDealer;
        public readonly Seat[] seats;
        public readonly RuleSetting rule;
        public readonly List<ScriptableRules.ICompletedResultModifier> completedHandModifiers = new();
        bool useDeterministicRoundSeeds;
        uint nextRoundSeed;

        public PlayerIndex Dealer => (PlayerIndex)(((int)firstDealer + DisplayRoundCount - 1) % PlayerCount);
        internal bool UseDeterministicRoundSeeds => useDeterministicRoundSeeds;
        internal uint NextRoundSeed => nextRoundSeed;

        static public AfterDraw Create(PlayerIndex firstDealer, RuleSetting rule)
        {
            var game = new Game(firstDealer, rule);
            return game.StartRound();
        }

        static public AfterDraw Create(PlayerIndex firstDealer, RuleSetting rule, uint seed)
        {
            var game = new Game(firstDealer, rule);
            game.EnableDeterministicRoundSeeds(seed);
            return game.StartRound();
        }

        Game(PlayerIndex firstDealer, RuleSetting rule)
        {
            this.rule = rule;
            this.firstDealer = firstDealer;
            seats = new Seat[PlayerCount];
            for (int i = 0; i < seats.Length; ++i)
            {
                seats[i] = new Seat(rule.initialScore);
            }
        }

        static public Game FromSerializable(in Serializables.Game source)
        {
            var result = new Game(source.firstDealer, source.rule);
            result.DisplayRoundCount = source.displayRoundCount;
            result.riichiScore = source.riichiScore;
            result.RoundWindCount = source.roundWindCount;
            for (int i = 0; i < source.scores.Length; ++i)
            {
                result.seats[i].score = source.scores[i];
            }
            result.本場 = source.本場;
            result.連荘 = source.連荘;
            result.useDeterministicRoundSeeds = source.useDeterministicRoundSeeds;
            result.nextRoundSeed = source.nextRoundSeed;

            return result;
        }

        public Serializables.Game ToSerializable()
        {
            return new Serializables.Game(this);
        }

        public AfterDraw StartRound(params TileType[]?[]? initialPlayerTilesByCheat)
        {
            var round = useDeterministicRoundSeeds
                ? new Round(this, Wind, Dealer, ConsumeRoundSeed(), initialPlayerTilesByCheat)
                : new Round(this, Wind, Dealer, initialPlayerTilesByCheat);
            return round.Start();
        }

        public AfterDraw StartRound(uint seed, params TileType[]?[]? initialPlayerTilesByCheat)
        {
            EnableDeterministicRoundSeeds(seed);
            var round = new Round(this, Wind, Dealer, ConsumeRoundSeed(), initialPlayerTilesByCheat);
            return round.Start();
        }

        void EnableDeterministicRoundSeeds(uint seed)
        {
            useDeterministicRoundSeeds = true;
            nextRoundSeed = NormalizeRoundSeed(seed);
        }

        uint ConsumeRoundSeed()
        {
            var seed = nextRoundSeed;
            nextRoundSeed = AdvanceRoundSeed(nextRoundSeed);
            return seed;
        }

        static uint NormalizeRoundSeed(uint seed)
        {
            return seed == 0 ? 1u : seed;
        }

        static uint AdvanceRoundSeed(uint seed)
        {
            return NormalizeRoundSeed(unchecked(seed * 1664525u + 1013904223u));
        }

        public AfterDraw? AdvanceRoundBy親上がり(out GameResult? gameResult)
        {
            ++連荘;
            ++本場;

            if (ShouldFinish)
            {
                gameResult = new GameResult(this);
                return null;
            }

            if (rule.end.アガリ止め == Rules.アガリ止め.あり)
            {
                if (IsFinalRound)
                {
                    gameResult = new GameResult(this);
                    if (gameResult.playerRanks[Dealer].rank == 1)
                    {
                        return null;
                    }
                }
            }

            gameResult = null;
            return StartRound();
        }
        public AfterDraw? AdvanceRoundBy子上がり(out GameResult? gameResult)
        {
            連荘 = 0;
            本場 = 0;
            if (DisplayRoundCount == PlayerCount)
            {
                ++RoundWindCount;
                DisplayRoundCount = 1;
            }
            else
            {
                ++DisplayRoundCount;
            }

            if (ShouldFinish)
            {
                gameResult = new GameResult(this);
                return null;
            }
            gameResult = null;
            return StartRound();
        }

        public AfterDraw? AdvanceRoundByノーテン流局(out GameResult? gameResult)
        {
            return AdvanceRoundBy子上がり(out gameResult);
        }


        public AfterDraw? AdvanceRoundBy途中流局(out GameResult? gameResult)
        {
            return AdvanceRoundByテンパイ流局(out gameResult);
        }

        public AfterDraw? AdvanceRoundByテンパイ流局(out GameResult? gameResult)
        {
            ++本場;

            if (ShouldFinish)
            {
                gameResult = new GameResult(this);
                return null;
            }
            gameResult = null;
            return StartRound();
        }


        bool ShouldFinish
        {
            get
            {
                if (rule.end.endWhenScoreUnderZero)
                {
                    if (seats.Select(_ => _.score).Min() < 0)
                    {
                        return true;
                    }
                }

                if (!IsExtraRound)
                {
                    return false;
                }

                // 延長判定
                // サドンデスでない場合、場風の切り替え時のみ判定
                if (rule.end.suddenDeathInExtraRound
                    || (連荘 == 0 && 本場 == 0 && DisplayRoundCount == 1))
                {
                    var topScore = seats.Max(_ => _.score);
                    if (topScore >= rule.end.extraRoundScoreThreshold)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public bool IsFinalRound
        {
            get
            {
                switch (rule.end.lengthType)
                {
                    case Rules.LengthType.東風戦: return (RoundWindCount == 0) && (DisplayRoundCount == PlayerCount);
                    case Rules.LengthType.半荘戦: return (RoundWindCount == 1) && (DisplayRoundCount == PlayerCount);
                    case Rules.LengthType.一荘戦: return (RoundWindCount == 2) && (DisplayRoundCount == PlayerCount);
                    default:
                        throw new System.ArgumentException(rule.end.lengthType.ToString());
                }
            }
        }

        bool IsExtraRound
        {
            get
            {
                switch(rule.end.lengthType)
                {
                    case Rules.LengthType.東風戦: return RoundWindCount > 0;
                    case Rules.LengthType.半荘戦: return RoundWindCount > 1;
                    case Rules.LengthType.一荘戦: return RoundWindCount > 2;
                    default:
                        throw new System.ArgumentException(rule.end.lengthType.ToString());
                }
            }
        }
    }
}
