using Observers.Interfaces;
using System;

namespace Observers
{
    public class Notifier : INotifier
    {
		public virtual void SendNotification(string notificationName) 
		{
			m_facade.SendNotification(notificationName);
		}

		public virtual void SendNotification(string notificationName, object body)
		{
			m_facade.SendNotification(notificationName, body);
		}

		public virtual void SendNotification(string notificationName, object body, string type)
		{
            m_facade.SendNotification(notificationName, body, type);
		}

		private IFacade Facade
		{
			get { return m_facade; }
		}
        private IFacade m_facade = Observers.Facade.Instance;

	}
}
