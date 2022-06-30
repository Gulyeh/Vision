namespace VisionClient.Core.Helpers.Aggregators
{
    public class LoginWindowLoading
    {
        public LoginWindowLoading(bool visibility)
        {
            LoadingVisibility = visibility;
        }

        public bool LoadingVisibility { get; private set; } = false;
    }
}
