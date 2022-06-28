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
        private DateTime banExpires;
        [Required]
        public DateTime BanExpires 
        { 
            get => banExpires; 
            set => banExpires = value.ToUniversalTime();
        }
    }
}