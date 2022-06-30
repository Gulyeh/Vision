namespace VisionClient.Core.Dtos
{
    public class ChangedUserDataDto
    {
        public ChangedUserDataDto()
        {
            Username = string.Empty;
            Description = string.Empty;
        }

        public Guid UserId { get; set; }
        public string Description { get; set; }
        public string Username { get; set; }
    }
}
