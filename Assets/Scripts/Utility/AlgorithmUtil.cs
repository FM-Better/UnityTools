using System;
using System.Collections.Generic;

namespace Utility
{
    public class AlgorithmUtil
    {
        #region Shuffle 洗牌函数
        
        public static List<T> Shuffle<T>(List<T> original)
        {
            for (int i = 0; i < original.Count; i++)
            {
                var index = UnityEngine.Random.Range(0, original.Count);
                if (index != i)
                {
                    (original[i], original[index]) = (original[index], original[i]);
                }
            }
            
            return original;
        }
        
        public static void Shuffle<T>(ref List<T> original)
        {
            for (int i = 0; i < original.Count; i++)
            {
                var index = UnityEngine.Random.Range(0, original.Count);
                if (index != i)
                {
                    (original[i], original[index]) = (original[index], original[i]);
                }
            }
        }
        
        public static List<T> Shuffle<T>(List<T> original, int shuffleNum)
        {
            var endIndex = Math.Min(shuffleNum, original.Count);
            for (int i = 0; i < endIndex; i++)
            {
                var index = UnityEngine.Random.Range(0, original.Count);
                if (index != i)
                {
                    (original[i], original[index]) = (original[index], original[i]);
                }
            }
            
            return original;
        }
    
        public static void Shuffle<T>(ref List<T> original, int shuffleNum)
        {
            var endIndex = Math.Min(shuffleNum, original.Count);
            for (int i = 0; i < endIndex; i++)
            {
                var index = UnityEngine.Random.Range(0, original.Count);
                if (index != i)
                {
                    (original[i], original[index]) = (original[index], original[i]);
                }
            }
        }
        
        #endregion
    }
}