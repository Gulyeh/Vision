namespace UsersService_API.Dtos
{
    public class EditableUserDataDto
    {
        public EditableUserDataDto()
        {
            Description = string.Empty;
            Username = string.Empty;
        }

        public string Description { get; set; }
        public string Username { get; set; }
    }
}