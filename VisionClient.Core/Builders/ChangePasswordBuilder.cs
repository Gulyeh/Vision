using VisionClient.Core.Dtos;

namespace VisionClient.Core.Builders
{
    public class ChangePasswordBuilder
    {
        ChangePasswordDto data = new ChangePasswordDto();

        public ChangePasswordDto Build()
        {
            return data;
        }

        public void SetNewPassword(string newPassword)
        {
            data.NewPassword = newPassword;
        }

        public void SetRepeatPassword(string repeatPassword)
        {
            data.RepeatPassword = repeatPassword;
        }

        public void SetCurrentPassword(string currentPassword)
        {
            data.CurrentPassword = currentPassword;
        }
    }
}
