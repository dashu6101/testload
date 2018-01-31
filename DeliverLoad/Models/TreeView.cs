using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DeliverLoad.Models
{
    public class TreeViewNodeVM
    {
        public TreeViewNodeVM()
        {
            ChildNode = new List<TreeViewNodeVM>();
        }

        public int NodeId { get; set; }

        [Required(ErrorMessage = "The Title field is required.")]
        [Display(Name = "Title")]
        public string Name { get; set; }

        public string NodeName
        {
            get { return GetNodeName(); }
        }
        public IList<TreeViewNodeVM> ChildNode { get; set; }

        public string GetNodeName()
        {
            string temp = "";

            return this.NodeId + "  " + this.Name + temp;
        }

        // public int Id { get; set; }

        //[Required(ErrorMessage = "The Question field is required.")]
        //public string Title { get; set; }
        [Display(Name = "Content Description")]
        [Required(ErrorMessage = "The Description field is required.")]
        public string ContentDescription { get; set; }

        public Nullable<int> CategoryId { get; set; }
        public Nullable<int> ParentId { get; set; }

        public IEnumerable<ContentCommentModel> ContentCommentModel { get; set; }
        public string ImageName { get; set; }
        public string VideoName { get; set; }

        public string VideoFrom { get; set; }
    }

}