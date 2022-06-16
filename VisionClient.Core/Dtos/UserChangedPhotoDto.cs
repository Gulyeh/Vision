namespace VisionClient.Core.Dtos
{
    public class UserChangedPhotoDto
    {
        public UserChangedPhotoDto()
        {
            PhotoUrl = string.Empty;
        }

        public Guid UserId { get; set; }
        public string PhotoUrl { get; set; }
    }
}
