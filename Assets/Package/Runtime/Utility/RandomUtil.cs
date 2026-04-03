#nullable enable
using System.Collections.Generic;

namespace ThreeMahjong
{
    public static class RandomUtil
    {
        public static int[] GenerateShuffledRange(int start, int count)
        {
            return RandomProvider.GetThreadRandom().GenerateShuffledRange(start, count);
        }

        public static T[] GenerateShuffledArray<T>(IReadOnlyList<T> list)
        {
            return RandomProvider.GetThreadRandom().GenerateShuffledArray(list);
        }

        public static void Shuffle<T>(ref List<T> list)
        {
            RandomProvider.GetThreadRandom().Shuffle(ref list);
        }

        public static void Shuffle<T>(ref T[] list)
        {
            RandomProvider.GetThreadRandom().Shuffle(ref list);
        }

        public static T Sample<T>(T[] array)
        {
            return Sample(array, out _);
        }

        public static T Sample<T>(T[] array, out int index)
        {
            index = Random.Range(0, array.Length);
            return array[index];
        }

        public static T Sample<T>(IReadOnlyList<T> list)
        {
            return Sample(list, out _);
        }

        public static T Sample<T>(IReadOnlyList<T> list, out int index)
        {
            index = Random.Range(0, list.Count);
            return list[index];
        }
    }
}
