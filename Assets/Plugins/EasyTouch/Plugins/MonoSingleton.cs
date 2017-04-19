/// <summary>
/// Generic Mono singleton.
/// 为了区别自己写的SingletonMono，以免混淆，给该类加命名空间，原来是没有的
/// </summary>
using UnityEngine;
namespace Plugins.EasyTouch
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {

        private static T m_Instance = null;

        public static T instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = GameObject.FindObjectOfType(typeof(T)) as T;
                    if (m_Instance == null)
                    {
                        m_Instance = new GameObject("Singleton of " + typeof(T).ToString(), typeof(T)).GetComponent<T>();
                        m_Instance.Init();
                    }

                }
                return m_Instance;
            }
        }

        private void Awake()
        {

            if (m_Instance == null)
            {
                m_Instance = this as T;
            }
        }

        public virtual void Init() { }


        private void OnApplicationQuit()
        {
            m_Instance = null;
        }
    }
}