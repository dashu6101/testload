//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DeliverLoad.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserDocument
    {
        public int UserDocumentId { get; set; }
        public string DocumentImage { get; set; }
        public Nullable<int> DocTypeId { get; set; }
        public Nullable<int> UserId { get; set; }
        public string IsActiveDocument { get; set; }
    }
}
