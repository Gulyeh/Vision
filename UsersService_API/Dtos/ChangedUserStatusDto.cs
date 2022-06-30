using UsersService_API.Helpers;

namespace UsersService_API.Dtos
{
    public class ChangedUserStatusDto
    {
        public ChangedUserStatusDto(Guid userId, Status status)
        {
            this.UserId = userId;
            this.Status = status;
        }

        public Guid UserId { get; private set; }
        public Status Status { get; private set; }
    }
}