using UnityEngine;
using System.Collections;
using UnityEngine.UI;
namespace Common.Util
{
    public class ImageUtil
    {
        /// <summary>
        /// 设置数字图片
        /// </summary>
        /// <param name="images">顺序图片集合</param>
        /// <param name="prefix">前缀</param>
        /// <param name="num">数值</param>
        /// <param name="hideFrontZero">是否隐藏前面的0</param>
        public static void SetNumberImages(Image[] images, string prefix, uint num, bool hideFrontZero = true)
        {
            int length = num.ToString().Length;
            string str = "";
            for (int i = images.Length; i >= 0; i--)
            {
                if (i > length)
                    str += "0";
            }
            str += num.ToString();
            //Debugger.LogError(length+"   "+str + "  " + sprites.Length);
            for (int i = 0; i < images.Length; i++)
            {
                if (i < images.Length - length)
                    images[i].enabled = !hideFrontZero;
                else
                    images[i].enabled = true;
				images[i].SetSprite(ResMgr.ResMgr.instance.Load<Sprite>(prefix + str.Substring(i, 1)));
            }
        }

        ///// <summary>
        ///// 设置数字图片，后面的图片隐藏
        ///// </summary>
        ///// <param name="images">顺序图片集合</param>    
        ///// <param name="num">数值</param>
        ///// <param name="prefix">前缀</param>
        public static void SetNumberImages(Image[] images, uint num, string prefix)
        {
            int length = num.ToString().Length;
            string str = num.ToString();

            for (int i = 0; i < images.Length; i++)
            {
                if (i >= length)
                    images[i].enabled = false;
                else
                {
                    images[i].enabled = true;
					images[i].SetSprite(ResMgr.ResMgr.instance.Load<Sprite>(prefix + str.Substring(i, 1)));
                }
            }
        }

        ///// <summary>
        ///// 设置时间图片，最大为值为59分59秒
        ///// </summary>
        ///// <param name="images"></param>
        ///// <param name="seconds"></param>
        ///// <param name="prefix"></param>
        public static void SetTimeImages(Image[] images, uint seconds, string prefix)
        {
            uint minutes = seconds / 60;
            uint second = seconds % 60;
            string str = "";
            if (minutes.ToString().Length == 1)
                str = "0";
            str += minutes;
            if (second.ToString().Length == 1)
                str += "0";
            str += second;
            for (int i = 0; i < images.Length; i++)
            {
				images[i].SetSprite(ResMgr.ResMgr.instance.Load<Sprite>(prefix + str.Substring(i, 1)));
            }
        }
    }
}