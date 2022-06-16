using System.ComponentModel.DataAnnotations;

namespace GameAccessService_API.Helpers
{
    public class BaseUser
    {
        [Required]
        public Guid UserId { get; set; }
    }
}