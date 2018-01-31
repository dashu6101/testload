using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeliverLoad.Models
{
    public class ContentCommentModel
    {
        public int Id { get; set; }
        public string Comments { get; set; }
        public Nullable<int> ContentId { get; set; }
        public Nullable<int> UserId { get; set; }
        public Nullable<System.DateTime> CommentDate { get; set; }
        public string ScreenName { get; set; }
        public string Image { get; set; }
    }
}