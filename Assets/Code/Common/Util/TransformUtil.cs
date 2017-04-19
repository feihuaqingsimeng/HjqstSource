using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Common.Util
{
    public static class TransformUtil
    {
        public static T FindComponentUp<T>(Transform trans) where T : Component
        {

            T c = null;
            do
            {
                trans = trans.parent;
                if (!trans) break;
                c = trans.GetComponent<T>();
            } while (!c);
            return c;
        }

        public static Transform Find(string name, Transform t, bool equal = false)
        {
            if (equal ? t.name == name : t.name.Contains(name))
            {
                return t;
            }

            foreach (Transform son in t)
            {
                Transform re = Find(name, son, equal);
                if (re)
                    return re;
            }
            return null;
        }

        public static Transform Find(System.Type type, Transform t)
        {
            if (t.GetComponent(type))
            {
                return t;
            }

            foreach (Transform son in t)
            {
                Transform re = Find(type, son);
                if (re)
                    return re;
            }
            return null;
        }

        public static Transform SwitchLayer(Transform trans, int layer)
        {
            if (trans == null) return trans;
            _SwitchLayer(trans, layer);
            return trans;
        }

        private static void _SwitchLayer(Transform trans, int layer)
        {
            trans.gameObject.layer = layer;
            foreach (Transform t in trans)
            {
                _SwitchLayer(t, layer);
            }
        }

        public static void ClearChildren(Transform rootTransfrom, bool immediate)
        {
            List<GameObject> childrenList = new List<GameObject>();
            int childrenCount = rootTransfrom.childCount;
            for (int childIndex = 0; childIndex < childrenCount; childIndex++)
            {
                childrenList.Add(rootTransfrom.GetChild(childIndex).gameObject);
            }

            if (immediate)
            {
                for (int childIndex = 0; childIndex < childrenCount; childIndex++)
                {
					GameObject.Destroy(childrenList[childIndex]);
                }
            }
            else
            {
                for (int childIndex = 0; childIndex < childrenCount; childIndex++)
                {
                    GameObject.Destroy(childrenList[childIndex]);
                }
            }
        }

        public static void ClearChildren(Transform rootTransfrom, string key)
        {
            if (!rootTransfrom) return;
            List<GameObject> childrenList = new List<GameObject>();
            int childrenCount = rootTransfrom.childCount;
            for (int childIndex = 0; childIndex < childrenCount; childIndex++)
            {
                childrenList.Add(rootTransfrom.GetChild(childIndex).gameObject);
            }
            for (int childIndex = 0; childIndex < childrenCount; childIndex++)
            {
                GameObject go = childrenList[childIndex];
                if (go.name.Contains(key))
                    GameObject.Destroy(go);
            }
        }
    }
}