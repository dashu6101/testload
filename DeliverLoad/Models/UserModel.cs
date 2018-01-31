using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeliverLoad.Models
{
    public class UserModel
    {
        public int UserId { get; set; }

        public string UserName { get; set; }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "The First Name field is required.")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "The Last Name field is required.")]
        public string LastName { get; set; }

        [Display(Name = "Screen Name")]
        [Required(ErrorMessage = "The Screen Name field is required.")]
        public string ScreenName { get; set; }

        [Display(Name = "Switch User")]
        public string UserType { get; set; }

        //[Required(ErrorMessage = "The ProfilePicture field is required.")]
        [Display(Name = "Profile Picture")]
        public string ProfilePicture { get; set; }

        public string ChannelNo { get; set; }

        [Display(Name = "Image Upload")]
        public HttpPostedFileBase ImageUpload { get; set; }

        public DateTime JoinDate { get; set; }

        [Display(Name = "My Balance")]
        public Decimal Balance { get; set; }

        public string Password { get; set; }

        public bool IsBlockedParticepant { get; set; }

        public bool IsBloked { get; set; }
    }

    public class ContactModel
    {

        [Required()]
        [Display(Name="Email")]
        public string UserName { get; set; }

       
        public string FirstName { get; set; }

        [Required()]
        [Display(Name = "Message")]
        public string Message { get; set; }
       
    }
}