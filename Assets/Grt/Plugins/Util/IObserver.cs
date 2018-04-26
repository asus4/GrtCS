namespace GRT
{
    public interface IObserver<NotifyType>
    {
        void Notify(NotifyType data);
    }
}
