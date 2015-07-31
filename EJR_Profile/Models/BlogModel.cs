using System;
using System.Web.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EJR_Profile.Models 
{
    public class Post
    {
        public Post()
        {
            this.Comments = new HashSet<Comment>();
            this.Deleted = false;
        }

        public int Id { get; set; }

        // This is how to change the displayed name...
        //
        //[Display(Name="My New Name for 'Created'")]
        //
        // But I don't want to do that yet, so comment out

        public DateTimeOffset Created { get; set; }
        public Nullable<DateTimeOffset> Updated { get; set; }
        [Required]
        public string Title { get; set; }
        [AllowHtml]
        [Required]
        public string Body { get; set; }

        // I want the option of displaying a caption for the image I download
        public string MediaURL { get; set; }
        [Display(Name="Image Caption")]
        public string MediaCaption { get; set; }
        public bool Published { get; set; }
        public bool Deleted { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
    }

    public class Comment
    {
        public Comment()
        {
            this.Comments = new HashSet<Comment>();
            this.Deleted = false;
        }
        public int Id { get; set; }
        public int PostId { get; set; }         // Because of 'Post' property below, this tells compiler this is a foreign key
        public string AuthorId { get; set; }    // - ditto -
        [Required]
        [Display(Name = "Comment")]
        public string Body { get; set; }
        [Required]
        [Display(Name = "Display Name")]
        public string DisplayName { get; set; } // Store a copy here to prevent having to look it up that way, as soon as we have a comment
                                                //   we have everything we need in order to display it
        public DateTimeOffset Created { get; set; }     
        public Nullable<DateTimeOffset> Updated { get; set; }
        [Display(Name = "Update Reason")]
        public string UpdateReason { get; set; }
        public Nullable<int> ParentCommentId { get; set; }
        public Nullable<int> Level { get; set; } // the nesting depth
        public bool Deleted { get; set; }

        public virtual Post Post { get; set; }
        public virtual ApplicationUser Author { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual Comment ParentComment { get; set; }
    }
}