namespace UsersService_API.Dtos
{
    public class EditableUserDataDto
    {
        public EditableUserDataDto()
        {
            Description = string.Empty;
            Nickname = string.Empty;
        }

        public string Description { get; set; }
        public string Nickname { get; set; }
    }
}