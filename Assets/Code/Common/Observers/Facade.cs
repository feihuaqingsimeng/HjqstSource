using Observers.Interfaces;
using System.Collections.Generic;
using UnityEngine;

namespace Observers
{
    public class Facade : UnityEngine.MonoBehaviour,IFacade
	{
        private IDictionary<string, IList<IObserver>> _observerMap;
#if UNITY_EDITOR
        [LuaInterface.NoToLua]
        public IDictionary<string, IList<IObserver>> EDITOR_ObserverMap
        {
            get { return _observerMap; }
        }
#endif

        private static readonly object _staticSyncRoot = new object();
        private readonly object _syncRoot = new object();

        private static volatile IFacade instance;

        void Awake()
        {
            lock (_staticSyncRoot)
            {
                if (instance != null)
                    throw new System.Exception("Facade must be singleton");
                instance = this;
                _observerMap = new Dictionary<string, IList<IObserver>>();
            }
        }

        void OnDestroy()
        {
            _observerMap.Clear();
            instance = null;
        }

        public static IFacade Instance
        {
            get
            {
                return instance;
            }
        }

        #region INotifier Interface
        public virtual void SendNotification(string notificationName)
		{
			NotifyObservers(new Notification(notificationName));
		}

        public virtual void SendNotification(string notificationName, object body)
		{
			NotifyObservers(new Notification(notificationName, body));
		}

        public virtual void SendNotification(string notificationName, object body, string type)
		{
			NotifyObservers(new Notification(notificationName, body, type));
		}
        #endregion
        #region IFacade Interface
        public virtual void NotifyObservers(INotification notification)
        {
            IList<IObserver> observers = null;
            lock (_syncRoot)
            {
                if (_observerMap.ContainsKey(notification.Name))
                {
                    IList<IObserver> observers_ref = _observerMap[notification.Name];
                    observers = new List<IObserver>(observers_ref);
                }
            }
            if (observers != null)
            {
                for (int i = 0; i < observers.Count; i++)
                {
                    IObserver observer = observers[i];
                    observer.NotifyObserver(notification);
                }
            }
        }

        public virtual void RegisterObserver(string notificationName, System.Func<INotification,bool> func)
        {
            lock (_syncRoot)
            {
                IObserver observer = new Observer(func);
                if (!_observerMap.ContainsKey(notificationName))
                {
                    _observerMap[notificationName] = new List<IObserver>();
                }
                //不重复注册
                IList<IObserver> observers = _observerMap[notificationName];
                bool find = false;
                for (int i = 0; i < observers.Count; i++)
                {
                    if (observers[i].CompareNotifyFunction(func))
                    {
                        find = true;
                        break;
                    }
                }
                if(!find)
                {
                    _observerMap[notificationName].Add(observer);
                }
            }
        }

        public virtual void RemoveObserver(string notificationName, System.Func<INotification, bool> func)
        {
            lock (_syncRoot)
            {
                if (_observerMap.ContainsKey(notificationName))
                {
                    IList<IObserver> observers = _observerMap[notificationName];
                    for (int i = 0; i < observers.Count; i++)
                    {
                        if (observers[i].CompareNotifyFunction(func))
                        {
                            observers.RemoveAt(i);
                            break;
                        }
                    }
                    if (observers.Count == 0)
                    {
                        _observerMap.Remove(notificationName);
                    }
                }
            }
        }

        public virtual void RemoveAllObserver()
        {
            lock (_syncRoot)
            {
                _observerMap.Clear();
            }
        }

        public virtual bool HasObserver(string notificationName)
        {
            lock (_syncRoot)
            {
                return _observerMap.ContainsKey(notificationName);
            }
        }

        #endregion


        
    }
}
