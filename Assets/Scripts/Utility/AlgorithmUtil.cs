using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
    public class AlgorithmUtil
    {
        #region Shuffle 洗牌函数

        /// <summary>
        /// 打乱整个序列
        /// </summary>
        /// <param name="original"> 要打乱的序列 </param>
        /// <typeparam name="T"> 传入类型 </typeparam>
        /// <returns></returns>
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

        /// <summary>
        /// 打乱整个序列
        /// </summary>
        /// <param name="original"> 要打乱的序列 </param>
        /// <typeparam name="T"> 传入类型 </typeparam>
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

        /// <summary>
        /// 打乱序列的前n个
        /// </summary>
        /// <param name="original"> 要打乱的序列 </param>
        /// <param name="shuffleNum"> 前面要打乱的个数 </param>
        /// <typeparam name="T"> 传入类型 </typeparam>
        /// <returns> 打乱后的序列 </returns>
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

        /// <summary>
        /// 打乱序列的前n个
        /// </summary>
        /// <param name="original"> 要打乱的序列 </param>
        /// <param name="shuffleNum"> 前面要打乱的个数 </param>
        /// <typeparam name="T"> 传入类型 </typeparam>
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
        
        /// <summary>
        /// 将一组平面上的点 根据原点 按照顺时针方向排序
        /// </summary>
        /// <param name="points"> 要排序的点的列表 </param>
        /// <returns> 返回排序后的点的列表 </returns>
        public static List<Vector2> SortPointsByClockwise(List<Vector2> points)
        {
            points.Sort((p1, p2) =>
            {
                var zero2P1 = p1 - Vector2.zero;
                if (zero2P1.x == 0f)
                {
                    zero2P1 = Vector2.up;
                }

                var zero2P2 = p2 - Vector2.zero;
                if (zero2P2.x == 0f)
                {
                    zero2P2 = Vector2.up;
                }

                var angle1 = Mathf.Acos(Vector2.Dot(zero2P1.normalized, Vector2.up)); // 选取和y轴正方向的夹角
                var angle2 = Mathf.Acos(Vector2.Dot(zero2P2.normalized, Vector2.up));

                var crossResult1 = Vector2Util.Cross(Vector2.up, zero2P1);
                var crossResult2 = Vector2Util.Cross(Vector2.up, zero2P2);

                if (crossResult1 > 0) // y轴左方
                    angle1 = -angle1;
                if (crossResult2 > 0) // y轴左方
                    angle2 = -angle2;

                if (crossResult1 == 0 && p1.y < 0f) // y轴下方 
                    angle1 = 180f;
                if (crossResult2 == 0 && p2.y < 0f) // y轴下方 
                    angle2 = 180f;

                if (angle1 >= 0 && angle2 >= 0) // 都在右边
                {
                    if (angle1 < angle2)
                    {
                        return -1;
                    }
                    else if (angle1 > angle2)
                    {
                        return 1;
                    }
                    else // 角度相同则比较距离 且使用不开方的 减少计算
                    {
                        var distance1 = zero2P1.sqrMagnitude;
                        var distance2 = zero2P2.sqrMagnitude;
                        return distance1.CompareTo(distance2);
                    }
                }
                else if (angle1 >= 0 && angle2 < 0) // 1右 2左
                {
                    return -1;
                }
                else if (angle1 < 0 && angle2 >= 0) // 1左 2右
                {
                    return 1;
                }
                else // 都在左边
                {
                    if (angle1 < angle2)
                    {
                        return -1;
                    }
                    else if (angle1 > angle2)
                    {
                        return 1;
                    }
                    else // 角度相同则比较距离 且使用不开方的 减少计算
                    {
                        var distance1 = zero2P1.sqrMagnitude;
                        var distance2 = zero2P2.sqrMagnitude;
                        return distance1.CompareTo(distance2);
                    }
                }
            });
            
            return points;
        }

        /// <summary>
        /// 将一组平面上的点 根据原点 按照顺时针方向排序
        /// </summary>
        /// <param name="points"> 要排序的点的列表 </param>
        public static void SortPointsByClockwise(ref List<Vector2> points)
        {
            points.Sort((p1, p2) =>
            {
                var zero2P1 = p1 - Vector2.zero;
                if (zero2P1.x == 0f)
                {
                    zero2P1 = Vector2.up;
                }

                var zero2P2 = p2 - Vector2.zero;
                if (zero2P2.x == 0f)
                {
                    zero2P2 = Vector2.up;
                }

                var angle1 = Mathf.Acos(Vector2.Dot(zero2P1.normalized, Vector2.up)); // 选取和y轴正方向的夹角
                var angle2 = Mathf.Acos(Vector2.Dot(zero2P2.normalized, Vector2.up));

                var crossResult1 = Vector2Util.Cross(Vector2.up, zero2P1);
                var crossResult2 = Vector2Util.Cross(Vector2.up, zero2P2);

                if (crossResult1 > 0) // y轴左方
                    angle1 = -angle1;
                if (crossResult2 > 0) // y轴左方
                    angle2 = -angle2;

                if (crossResult1 == 0 && p1.y < 0f) // y轴下方 
                    angle1 = 180f;
                if (crossResult2 == 0 && p2.y < 0f) // y轴下方 
                    angle2 = 180f;

                if (angle1 >= 0 && angle2 >= 0) // 都在右边
                {
                    if (angle1 < angle2)
                    {
                        return -1;
                    }
                    else if (angle1 > angle2)
                    {
                        return 1;
                    }
                    else // 角度相同则比较距离 且使用不开方的 减少计算
                    {
                        var distance1 = zero2P1.sqrMagnitude;
                        var distance2 = zero2P2.sqrMagnitude;
                        return distance1.CompareTo(distance2);
                    }
                }
                else if (angle1 >= 0 && angle2 < 0) // 1右 2左
                {
                    return -1;
                }
                else if (angle1 < 0 && angle2 >= 0) // 1左 2右
                {
                    return 1;
                }
                else // 都在左边
                {
                    if (angle1 < angle2)
                    {
                        return -1;
                    }
                    else if (angle1 > angle2)
                    {
                        return 1;
                    }
                    else // 角度相同则比较距离 且使用不开方的 减少计算
                    {
                        var distance1 = zero2P1.sqrMagnitude;
                        var distance2 = zero2P2.sqrMagnitude;
                        return distance1.CompareTo(distance2);
                    }
                }
            });
        }
    }
}