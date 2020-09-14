namespace Wanghzh.Prism.Helpers
{
    public interface IExecuteWithObject
    {
        object Target
        {
            get;
        }
        void ExecuteWithObject(object parameter);
        void MarkForDeletion();
    }
}
