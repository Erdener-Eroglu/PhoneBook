using PhoneBookEntityLayer.Entities;
using System.ComponentModel.DataAnnotations;

namespace PhoneBookEntityLayer.ViewModels
{
    public class MemberPhoneViewModel
    {

        public int Id { get; set; }

        public DateTime CreatedDate { get; set; }
        public bool IsRemoved { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string FriendNameSurname { get; set; }

        public byte PhoneTypeId { get; set; } //ForeignKey

        [Required]
        [StringLength(13, MinimumLength = 13)] //+905302794998
        public string Phone { get; set; }


        public string MemberId { get; set; } //ForeignKey
        public PhoneType? PhoneType { get; set; }

        public  Member? Member { get; set; }
    }
}
