using System.ComponentModel.DataAnnotations;

namespace GameAccessService_API.Dtos
{
    public class UserAccessDto
    {
        public UserAccessDto()
        {
            Reason = string.Empty;
        }

        [Required]
        public Guid UserId { get; set; }
        [Required]
        public Guid GameId { get; set; }
        public string Reason { get; set; }
        [Required]
        public DateTime ExpireDate { get; set; }
    }
}