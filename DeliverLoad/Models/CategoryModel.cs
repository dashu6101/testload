using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeliverLoad.Models
{
    public class CategoryModel
    {
        public int CategoryId { get; set; }


        [Required(ErrorMessage = "The title field is required.")]
        [Display(Name = "Title")]
        public string Name { get; set; }

        [Required(ErrorMessage = "The description field is required.")]
        public string Description { get; set; }

        public string Image { get; set; }

        [Display(Name = "Image Upload")]
        public HttpPostedFileBase ImageUpload { get; set; }

        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public bool HasJoinedCategory { get; set; }

        public string ChannelNo { get; set; }

        public DateTime CreatedDate { get; set; }


        [Required(ErrorMessage = "The Price field is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid Price ")]
        public Decimal Price { get; set; }

        public DateTime JoinedDate { get; set; }

        [Display(Name = "Is Free Channel?")]
        public bool IsFreeChannel { get; set; }

        public string ProfileImage { get; set; }

        public bool IsBlockedParticepant { get; set; }

        public bool IsChannelAvailable { get; set; }

        public bool IsAuthenticated { get; set; }

    }
}