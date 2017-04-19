using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Common.ResMgr
{
    public sealed class AssetsObj
    {
        private List<Object> _assetsList;

        public AssetsObj(List<Object> assets)
        {
            _assetsList = assets;
        }

        public AssetsObj(Object[] assets)
        {
            if (assets == null)
                Debugger.LogError("assets is null !!");
            else
                _assetsList = assets.ToList();
        }

        public Object GetAssetByName(string name)
        {
            if (_assetsList == null)
                return null;
            Object o = null;
            for (int i = 0, count = _assetsList.Count; i < count; i++)
            {
                o = _assetsList[i];
                if (o.name == name)
                    return o;
            }
            return null;
        }

        public void Clear()
        {
            if (_assetsList != null)
            {
                //for (int i = 0, count = _assetsList.Count; i < count; i++)
                //{
                //    Object obj = _assetsList[i];
                //    if (obj)
                //        Resources.UnloadAsset(obj);
                //}
                _assetsList.Clear();
                _assetsList = null;
            }
        }

        public override string ToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("AssetsObj:[");
            sb.Append("_assetsList count:");
            if (_assetsList != null)
            {
                sb.Append(_assetsList.Count);
                for (int i = 0, count = _assetsList.Count; i < count; i++)
                {
                    sb.Append(",");
                    sb.Append(_assetsList[i].name);
                }
            }
            else
                sb.Append("0");
            sb.Append("]");
            return sb.ToString();
        }
    }
}
