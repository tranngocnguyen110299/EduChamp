//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TN_academic.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class BlogComment
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public BlogComment()
        {
            this.ReplyingBlogComments = new HashSet<ReplyingBlogComment>();
        }
    
        public int CommentID { get; set; }
        public string Username { get; set; }
        public Nullable<int> BlogID { get; set; }
        public string Content { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<bool> Status { get; set; }
    
        public virtual Blog Blog { get; set; }
        public virtual User User { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReplyingBlogComment> ReplyingBlogComments { get; set; }
    }
}
