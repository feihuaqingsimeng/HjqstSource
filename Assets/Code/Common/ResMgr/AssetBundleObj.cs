using UnityEngine;
using System.Collections;
namespace Common.ResMgr
{
    public sealed class AssetBundleObj
    {
        public string subPath;
        public string dependerPath;//依赖者路径
        public AssetBundle assetBundle;
    }
}