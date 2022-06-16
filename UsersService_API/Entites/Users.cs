using System.ComponentModel.DataAnnotations;
using UsersService_API.Helpers;

namespace UsersService_API.Entites
{
    public class Users : BaseUserData
    {
        public Users()
        {
            PhotoId = string.Empty;
            CurrencyValue = 0;
            LastOnlineStatus = Status.Online;
            IsDeletedAccount = false;
        }

        [Key]
        public int Id { get; set; }
        [Required]
        public string PhotoId { get; set; }
        [Required]
        public Status LastOnlineStatus { get; set; }
        [Required]
        public int CurrencyValue { get; set; }
        [Required]
        public bool IsDeletedAccount { get; set; }
    }
}