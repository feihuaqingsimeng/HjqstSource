using UnityEngine;
using System.Collections;
namespace UnityEngine.UI
{
    public static class TextExpand
    {
        public static void SetGray(this Text text, bool isGray)
        {
            Material mat = text.material;
            Color color = text.color;
            if (isGray)
            {
                UnityEngine.UI.Gradient g = text.GetComponent<UnityEngine.UI.Gradient>();
                if (g)
                    g.enabled = false;
                color.r = 0;
                if (mat)
                {
                    if (mat.name == "Default UI Material")
                        text.material = Logic.UI.UIUtil.GrayMat;
                }
                else
                    text.material = Logic.UI.UIUtil.GrayMat;
            }
            else
            {
                UnityEngine.UI.Gradient g = text.GetComponent<UnityEngine.UI.Gradient>();
                if (g)
                    g.enabled = true;
                color.r = 1;
                if (mat == Logic.UI.UIUtil.GrayMat)
                    text.material = null;
            }
            text.color = color;
        }
    }
}