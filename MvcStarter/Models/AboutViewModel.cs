namespace MvcStarter.Models
{
    public class AboutViewModel
    {
        public string Message { get; }

        public AboutViewModel(string message)
        {
            Message = message;
        }
    }
}
