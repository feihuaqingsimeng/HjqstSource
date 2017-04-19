namespace Observers.Interfaces
{
    public interface IFacade : INotifier
	{
		void NotifyObservers(INotification note);
        void RegisterObserver(string notificationName, System.Func<INotification, bool> func);
        void RemoveObserver(string notificationName, System.Func<INotification,bool> func);
        bool HasObserver(string notificationName);
        void RemoveAllObserver();

	}
}
