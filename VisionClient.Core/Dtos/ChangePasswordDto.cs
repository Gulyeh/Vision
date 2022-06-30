namespace VisionClient.Core.Dtos
{
    public class ChangePasswordDto
    {
        public ChangePasswordDto()
        {
            CurrentPassword = string.Empty;
            NewPassword = string.Empty;
            RepeatPassword = string.Empty;
        }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string RepeatPassword { get; set; }
    }
}
