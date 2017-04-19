using Observers.Interfaces;
using System;
using System.Reflection;

namespace Observers
{
	public class Observer : IObserver
	{
        public Observer(System.Func<INotification, bool> notifyFunction)
        {
            m_notifyFunction = notifyFunction;
        }
        //public Observer(string notifyMethod, object notifyContext)
        //{
        //    m_notifyMethod = notifyMethod;
        //    m_notifyContext = notifyContext;
        //}
		public virtual void NotifyObserver(INotification notification)
		{
            //object context;
            //string method;
            //lock (m_syncRoot)
            //{
            //    context = NotifyContext;
            //    method = NotifyMethod;
            //}

            //Type t = context.GetType();
            //BindingFlags f = BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase;
            //MethodInfo mi = t.GetMethod(method, f);
            //mi.Invoke(context, new object[] { notification });
            System.Func<INotification, bool> function;
            lock (m_syncRoot)
            {
                function = NotifyFunction;
            }
            function.Invoke(notification);

            //Type t = context.GetType();
            //BindingFlags f = BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase;
            //MethodInfo mi = t.GetMethod(method, f);
            //mi.Invoke(context, new object[] { notification });
		}

        //public virtual bool CompareNotifyContext(object obj)
        //{
        //    lock (m_syncRoot)
        //    {
        //        return NotifyContext.Equals(obj);
        //    }
        //}

        public virtual bool CompareNotifyFunction(System.Func<INotification, bool> func)
        {
            lock (m_syncRoot)
            {
                return NotifyFunction.Equals(func);
            }
        }

        //public virtual string NotifyMethod
        //{
        //    private get
        //    {
        //        return m_notifyMethod;
        //    }
        //    set
        //    {
        //        m_notifyMethod = value;
        //    }
        //}
        //public virtual object NotifyContext
        //{
        //    private get
        //    {
        //        return m_notifyContext;
        //    }
        //    set
        //    {
        //        m_notifyContext = value;
        //    }
        //}
        public virtual System.Func<INotification, bool> NotifyFunction
        {
            private get
            {
                return m_notifyFunction;
            }
            set
            {
                m_notifyFunction = value;
            }
        }

        //private string m_notifyMethod;
        //private object m_notifyContext;
        private System.Func<INotification, bool> m_notifyFunction;
		private readonly object m_syncRoot = new object();
	}
}
