using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Common.Util
{
    public static class Extention
    {
        #region wa7e20150704 DelayAction 延时调用

        /// <summary>
        /// 延时类型 条件没有满足之前每帧执行inTimeAction  满足条件执行onTimeAction
        /// </summary>
        public static void DelayAction(this MonoBehaviour mono, YieldInstruction yieldDelay, Action inTimeAction,
            Action onTimeAction)
        {
            if (mono)
                mono.StartCoroutine(DoAction(mono, yieldDelay, inTimeAction, onTimeAction));
        }

        private static IEnumerator DoAction(MonoBehaviour mono, YieldInstruction yieldDelay, Action inTimeAction,
            Action onTimeAction)
        {

            bool b = false;
            DelayAction(mono, yieldDelay, () => b = true);
            while (!b)
            {
                if (inTimeAction != null)
                    inTimeAction();
                yield return null;
            }
            //yield return yieldDelay;
            if (onTimeAction != null && mono)
                onTimeAction();
        }

        /// <summary>
        /// 20150506延时类型　 条件满足才执行一次
        /// </summary>
        public static void DelayAction(this MonoBehaviour mono, YieldInstruction yieldDelay, Action action)
        {
            if (mono)
                mono.StartCoroutine(DoAction(mono, yieldDelay, action));
        }

        private static IEnumerator DoAction(MonoBehaviour mono, YieldInstruction yieldDelay, Action action)
        {
            yield return yieldDelay;
            if (mono)
                action();
            yield return null;
        }

        /// <summary>
        /// 延时条件 2
        /// </summary>
        public static void DelayAction(this MonoBehaviour mono, Func<bool> funcBool, Action intAction, Action endAtion)
        {
            if (mono)
                mono.StartCoroutine(DoAction(mono, funcBool, intAction, endAtion));
        }
        /// <summary>
        /// 延时条件
        /// </summary>
        public static void DelayAction(this MonoBehaviour mono, Func<bool> funcBool, Action action)
        {
            if (mono)
                mono.StartCoroutine(DoAction(mono, funcBool, null, action));
        }

        private static IEnumerator DoAction(MonoBehaviour mono, Func<bool> funcBool, Action inAction, Action endAtion)
        {
            while (mono != null && !funcBool())
            {
                if (inAction != null)
                    inAction();
                yield return null;
            }
            if (mono != null)
                endAtion();
            yield return null;
        }

        /// <summary>
        /// 延时帧数 
        /// </summary>
        public static void DelayAction(this MonoBehaviour mono, int framTimes, Action action)
        {
            if (mono)
                mono.StartCoroutine(DoAction(mono, framTimes, action));
        }

        private static IEnumerator DoAction(MonoBehaviour mono, int framTimes, Action action)
        {
            while (framTimes > 0 && mono != null)
            {
                //Debugger.Logger.Log(string.Format("framTimes {0}", framTimes));
                yield return null;
                //Debugger.Logger.Log(string.Format("framTimes {0}", framTimes));
                framTimes--;

            }
            if (mono)
                action();
            yield return null;
        }

        #endregion

        #region wa7e20150821 查找界面内匹配条件predicate的gameobject

        /// <summary>
        /// 查找界面内匹配条件predicate的gameobject
        /// </summary>
        public static GameObject FindByTrans(this GameObject root, string targetName)
        {
            Transform findGo = null;
            GameObject outGo = root.FindChildGameObject(
                o =>
                {
                    if (o.name == targetName)
                        return true;
                    if ((findGo = o.transform.Find(targetName)) != null)

                        return true;
                    return false;
                });
            if (findGo != null)
                return findGo.gameObject;
            return outGo;
        }

        /// <summary>
        /// 查找界面内匹配条件predicate的gameobject
        /// </summary>
        public static GameObject FindChildGameObject(this Component com, Predicate<GameObject> predicate)
        {
            return com.gameObject.FindChildGameObject(predicate);
        }

        /// <summary>
        /// 查找界面内匹配条件predicate的gameobject
        /// </summary>
        public static GameObject FindChildGameObject(this GameObject rootGo, Predicate<GameObject> predicate)
        {
            if (rootGo == null)
                return null;
            GameObject outGo = null;
            //int count = 0;
            bool isReturn = false;
            ForObjectsSubTree(rootGo,
                o =>
                {
                    //count++;
                    //Debugger.Logger.Log("count:" + count + string.Format("---o.name:{0}    o.activeSelf:{1}  o.activeInHierarchy:{2}", o.name, o.activeSelf, o.activeInHierarchy));
                    if (predicate(o))
                    {
                        outGo = o;
                        isReturn = true;
                        return true;
                    }
                    return false;
                }, ref isReturn);
            return outGo;
        }

        /// <summary>
        /// 遍历子物体（func 如果返回true。中断子物体查找
        /// </summary>
        public static void ForObjectsSubTree(this GameObject root, Predicate<GameObject> func, ref bool isReturn)
        {
            if (root == null || func == null)
                return;
            if (isReturn) return;
            isReturn = func(root);
            for (var i = 0; i < root.transform.childCount; ++i)
            {
                ForObjectsSubTree(root.transform.GetChild(i).gameObject, func, ref isReturn);
            }
        }

        /// <summary>
        /// 找匹配条件predicate的　Component
        /// </summary>
        public static T[] GetComponentsInChildren<T>(this Component com, Predicate<T> predicate) where T : Component
        {
            return com.gameObject.GetComponentsInChildren(predicate);
        }

        /// <summary>
        /// 找匹配条件predicate的　Component
        /// </summary>
        public static T[] GetComponentsInChildren<T>(this GameObject go, Predicate<T> predicate) where T : Component
        {
            T[] coms = go.GetComponentsInChildren<T>(true);
            if (predicate == null)
                return coms;
            List<T> comsOut = new List<T>();
            for (int i = 0; i < coms.Length; i++)
            {
                T t = coms[i];
                if (t == null)
                    continue;
                if (predicate(t))
                    comsOut.Add(t);
            }
            return comsOut.ToArray();
        }
        /// <summary>
        /// 向上搜索，直到找到包含组件T的GameObject
        /// </summary>

        public static GameObject GetGameObjectInParents<T>(this GameObject gameObject)
      where T : Component
        {
            T component = null;
            var p = gameObject;
            while (p != null)
            {
                component = p.GetComponent<T>();
                if (component != null)
                {
                    //Debug.Log(string.Format("20131116xiexiubo向上搜索，直到找到包含组件T的GameObject name:{0}  ", p.name));
                    return p;
                }

                p = (p.transform.parent == null) ? null : p.transform.parent.gameObject;
            }
            return null;
        }
        /// <summary>
        /// 向上搜索，直到找到包含组件T的GameObject 返回T
        /// </summary>
        static public T GetRootObj<T>(this GameObject go) where T : Component
        {
            if (go == null)
                return null;
            Transform t = go.transform;
            T obj = t.GetComponent<T>();

            for (; ; )
            {
                Transform parent = t.parent;
                if (parent == null || parent.name == "UIRoot") break;
                t = parent;
                T tobj = t.GetComponent<T>();
                if (tobj != null)
                    obj = tobj;
            }
            return obj;
        }
        #endregion

        #region wa7e20150828 String

        public static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }

        #endregion

        #region 得到所有的父节点
        public static Stack<Transform> GetAllParentNode(Transform go)
        {
            Stack<Transform> parentTransforms = new Stack<Transform>();
            for (; ; )
            {
                go = go.parent;
                if (go == null)
                {
                    return parentTransforms;
                }
                else
                {
                    parentTransforms.Push(go);
                }
            }
        }
        #endregion
    }
}