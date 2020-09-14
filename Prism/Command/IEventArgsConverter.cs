namespace Wanghzh.Prism.Command
{
    /// <summary>
    /// 
    /// </summary>
    public interface IEventArgsConverter
    {
        object Convert(object value, object parameter);
    }
}
