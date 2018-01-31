using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CCC.Models
{
    public class CategoryContentModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The Question field is required.")]
        [Display(Name = "Title")]
        public string Name { get; set; }

        //[Required(ErrorMessage = "The Description field is required.")]
        public string Description { get; set; }

        public Nullable<int> CategoryId { get; set; }
        public Nullable<int> ParentId { get; set; }

        
    }
}