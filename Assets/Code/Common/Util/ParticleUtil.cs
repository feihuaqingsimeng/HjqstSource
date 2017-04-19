using UnityEngine;
using System.Collections;

namespace Common.Util
{
    public class ParticleUtil
    {

        public static GameObject CreateParticle(string path, Canvas canvas)
        {
            return CreateParticle(path, canvas.sortingLayerName, canvas.sortingOrder);
        }
        public static GameObject CreateParticle(string path, string layerName, int sortingOrder)
        {
            GameObject original = ResMgr.ResMgr.instance.Load<GameObject>(path);
            if (original == null)
            {
                Debugger.LogError("can not load gameobject in path:" + path);
                return null;
            }
            GameObject go = GameObject.Instantiate<GameObject>(original);

            Renderer[] renders = go.GetComponentsInChildren<Renderer>(true);
            Renderer render;
            for (int i = 0, count = renders.Length; i < count; i++)
            {
                render = renders[i];
                render.sortingLayerName = layerName;
                render.sortingOrder += sortingOrder;
            }
            return go;
        }

    }
}

