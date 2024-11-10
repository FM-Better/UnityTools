using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
    [System.Serializable]
    public class CubicBezierCurve // 三次贝塞尔曲线
    {
        public Transform p0; // 起点
        public Transform p1; // 第一个控制点
        public Transform p2; // 第二个控制点
        public Transform p3; // 终点

        public CubicBezierCurve(Transform p0, Transform p1, Transform p2, Transform p3)
        {
            this.p0 = p0;
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
        }
    }

    public class CubicBezierCurveMgr : MonoBehaviour
    {
        public List<CubicBezierCurve> CurveSegments;
        public int SubSegmentCount = 100; // 用于细分曲线的数量，以得到平滑的视觉效果  

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            // 绘制三次贝塞尔曲线的四个点  
            for (var i = 0; i < CurveSegments.Count; i++)
            {
                var segment = CurveSegments[i];
                var p0 = i == 0
                    ? segment.p0.position
                    : CurveSegments[i - 1].p3.position; // 除了第一条曲线起点为自己的起点 剩下的起点都为上一个曲线的终点
                var p1 = segment.p1.position;
                var p2 = segment.p2.position;
                var p3 = segment.p3.position;
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(p0, 0.2f);
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(p1, 0.2f);
                Gizmos.DrawSphere(p2, 0.2f);
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(p3, 0.2f);
            }

            Gizmos.color = Color.blue;

            // 绘制曲线  
            for (var i = 0; i < CurveSegments.Count; i++)
            {
                var segment = CurveSegments[i];
                var p0 = i == 0
                    ? segment.p0.position
                    : CurveSegments[i - 1].p3.position; // 除了第一条曲线起点为自己的起点 剩下的起点都为上一个曲线的终点
                var p1 = segment.p1.position;
                var p2 = segment.p2.position;
                var p3 = segment.p3.position;

                for (int j = 0; j < SubSegmentCount; j++)
                {
                    var nowT = j / (SubSegmentCount - 1f);
                    var preT = j > 0 ? (j - 1) / (SubSegmentCount - 1f) : 0f;
                    var previousPoint = CalculateBezierPoint(preT, p0, p1, p2, p3);
                    var currentPoint = CalculateBezierPoint(nowT, p0, p1, p2, p3);
                    Gizmos.DrawLine(previousPoint, currentPoint);
                }
            }
        }

        private Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3) // 计算三次贝塞尔曲线的采样点  
        {
            var u = 1 - t;
            var tt = t * t;
            var uu = u * u;
            var ttt = tt * t;
            var uuu = uu * u;

            var p = uuu * p0; // (1-t)^3 * P0  
            p += 3 * uu * t * p1; // 3*(1-t)^2 * t * P1  
            p += 3 * u * tt * p2; // 3*(1-t) * t^2 * P2  
            p += ttt * p3; // t^3 * P3  

            return p;
        }
    }
}
