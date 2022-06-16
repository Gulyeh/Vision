namespace UsersService_API.Dtos
{
    public class ChangedUserDataDto
    {
        public ChangedUserDataDto(Guid userId, string description, string nickname)
        {
            this.UserId = userId;
            Description = description;
            Username = nickname;
        }

        public Guid UserId { get; private set; }
        public string Description { get; private set; }
        public string Username { get; private set; }
    }
}