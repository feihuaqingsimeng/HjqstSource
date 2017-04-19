using UnityEngine;
using System.Collections;
namespace Common.Util
{
    public static class VectorUtil
    {
        /// <summary>
        /// 顺时针旋转（平面）
        /// 将已知向量m=(x1,y1)，顺时针旋转a度得到向量n=(y1*sin(a)+x1*cos(a),-x1*sin(a)+y1*cos(a))
        /// 1度=pi/180弧度
        /// </summary>
        /// <returns></returns>
        public static Vector2 RotateAngle(Vector2 start, Vector2 vector2, float angle)
        {
            float a = angle;
            if (a == 0)
                return vector2;
            if (a < 0f)
            {
                a = a % 360f;
                a = 360 + a;
            }
            Vector2 original = vector2 - start;//终点减起点，得到向量
            Vector2 target = new Vector2();
            target.x = original.y * Mathf.Sin(a * Mathf.PI / 180) + original.x * Mathf.Cos(a * Mathf.PI / 180);
            target.y = -original.x * Mathf.Sin(a * Mathf.PI / 180) + original.y * Mathf.Cos(a * Mathf.PI / 180);
            Vector2 result = target + start;
            return result;
        }
    }
}