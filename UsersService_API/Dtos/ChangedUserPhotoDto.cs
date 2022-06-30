namespace UsersService_API.Dtos
{
    public class ChangedUserPhotoDto
    {
        public ChangedUserPhotoDto(Guid userId, string photoUrl)
        {
            this.userId = userId;
            this.photoUrl = photoUrl;
        }

        public Guid userId { get; private set; }
        public string photoUrl { get; private set; }
    }
}