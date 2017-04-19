using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Common.Util
{
    public static class GameObjectUtil
    {
        /// <summary>
        /// AddComponent2GameObject
        /// </summary>
        /// <typeparam name="T">target component</typeparam>
        /// <param name="parent">game object</param>
        public static List<T> AddComponent2GameObject<T, K>(GameObject parent)
            where T : Component
            where K : Component
        {
            K[] ks = parent.GetComponentsInChildren<K>();
            List<T> result = new List<T>();
            for (int i = 0, length = ks.Length; i < length; i++)
            {
                result.Add(ks[i].gameObject.AddComponent<T>());
            }
            return result;
        }
    }
}