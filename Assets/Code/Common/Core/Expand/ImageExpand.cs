using UnityEngine;
using System.Collections;
using System;
using Common.ResMgr;
namespace UnityEngine.UI
{
    public static class ImageExpand
    {
        public static void SetGray(this Image image, bool isGray)
        {
            Color color = image.color;
            if (isGray)
            {
                color.r = 0;
               // image.material = Logic.UI.UIUtil.GrayMat;
            }
            else
            {
                color.r = 1;
               // image.material = null;
            }
            image.color = color;
        }
		public static void SetSprite(this Image image,Sprite sprite)
		{
			image.sprite = sprite;
			if (image.sprite == null)
				return;
			String textureName = image.sprite.texture.name;
			string matPath = "sprite/"+textureName;
			Material mat = ResMgr.instance.Load<Material>( matPath);
			image.material = mat;
		}
        public static bool IsGray(this Image image)
        {
            bool result = false;
            result = image.material == Logic.UI.UIUtil.GrayMat;
            return result;
        }


    }
}