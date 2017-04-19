using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Common.Animators
{
    public static class AnimationUtil
    {
        public static AnimationCurve CreateAnimationCurve(List<float> times, List<float> values)
        {
            AnimationCurve animationCurve = new AnimationCurve();
            if (times.Count == values.Count)
            {
                for (int i = 0, count = times.Count; i < count; i++)
                {
                    Keyframe keyframe = new Keyframe(times[i], values[i]);
                    keyframe.tangentMode = (int)TangentMode.Smooth;//并没有什么用
                    animationCurve.AddKey(keyframe);
                }
                for (int i = 0, count = animationCurve.keys.Length; i < count; i++)
                {
                    animationCurve.SmoothTangents(i, 0f);
                }
            }
            return animationCurve;
        }

        public static Vector3 GetVector3FromCurves(AnimationCurve curveX, AnimationCurve curveY, AnimationCurve curveZ, float time)
        {
            Vector3 result = Vector3.zero;
            result = new Vector3(curveX.Evaluate(time), curveY.Evaluate(time), curveZ.Evaluate(time));
            return result;
        }

        public static void gizmoDraw(AnimationCurve curveX, AnimationCurve curveY, AnimationCurve curveZ)
        {
            Vector3 prevPt = GetVector3FromCurves(curveX, curveY, curveZ, 0f);
            for (int i = 0, count = 120; i < count; i++)
            {
                float p = i / 120f;
                Vector3 currPt2 = GetVector3FromCurves(curveX, curveY, curveZ, p);
                Gizmos.color = Color.red;
                Gizmos.DrawLine(currPt2, prevPt);
                prevPt = currPt2;
            }
        }
    }

    public enum TangentMode
    {
        Editable = 0,
        Smooth = 1,
        Linear = 2,
        Stepped = Linear | Smooth,
    }
}
