using UnityEngine;
using System.Collections;
namespace Common.ResMgr
{
    public class AssetPacker : ScriptableObject
    {
        public Object[] mAssets;

        public Object GetAsset(string objName)
        {
            for (int i = 0; i < mAssets.Length; ++i)
            {
                Object obj = mAssets[i];
                if (obj != null && obj.name == objName)
                {
                    return obj;
                }
            }
            return null;
        }

        public Sprite GetSprite(string objName)
        {
            Object obj = GetAsset(objName);
            return obj as Sprite;
        }

        public Texture2D GetTexture(string objName)
        {
            Object obj = GetAsset(objName);
            return obj as Texture2D;
        }
    }
}