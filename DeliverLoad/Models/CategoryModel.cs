using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace DeliverLoad.Models
{
    public class CategoryModel
    {
        public int CategoryId { get; set; }


        [Required(ErrorMessage = "The name field is required.")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "The description field is required.")]
        public string Description { get; set; }

        public string Image { get; set; }

        [Display(Name = "Image")]
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

        [Display(Name = "Is Free Vehicle?")]
        public bool IsFreeChannel { get; set; }

        [Display(Name = "Is Free Vehicle?")]
        public bool IsFreeVehicle { get; set; }

        public string ProfileImage { get; set; }

        public bool IsBlockedParticepant { get; set; }

        public bool IsChannelAvailable { get; set; }

        public bool IsAuthenticated { get; set; }

        [Display(Name = "Pickup Location")]
        [Required(ErrorMessage = "The Pick Up Location field is required.")]
        public string PickupLocation { get; set; }

        [Display(Name = "Pick Up Date")]
        [Required(ErrorMessage = "The Pick Up Date field is required.")]
        public DateTime? PickupDate { get; set; }

        [Display(Name = "Drop off Location")]
        [Required(ErrorMessage = "The Drop off Location field is required.")]
        public string DropOffLocation { get; set; }

        [Display(Name = "Drop off Date")]
        [Required(ErrorMessage = "The Drop off Date field is required.")]
        public DateTime? DropOffDate { get; set; }


        [Required]
        [Display(Name = "Load Type")]
        public int? LoadSpaceId { get; set; }
        public SelectList LoadSpaceList { get; set; }


        public string LoadSpaceTitle { get; set; }

    }
}