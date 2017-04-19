namespace Observers.Interfaces
{
    public interface IObserver
    {
        //string NotifyMethod { set; }
        //object NotifyContext { set; }

        System.Func<INotification, bool> NotifyFunction { set; }
        void NotifyObserver(INotification notification);
        //bool CompareNotifyContext(object obj);
        bool CompareNotifyFunction(System.Func<INotification, bool> func);
    }
}
