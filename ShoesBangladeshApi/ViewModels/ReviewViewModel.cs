using System;
using System.Collections.Generic;

namespace ShoesBangladesh.API.ViewModels
{
    public class ReviewViewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? CustomerName { get; set; }
        public int UserId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";
        public string? ImageUrls { get; set; }
        public int LikeCount { get; set; }
        public int DislikeCount { get; set; }
        public bool IsLikedByCurrentUser { get; set; }
        public bool IsDislikedByCurrentUser { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<ReviewReplyViewModel> Replies { get; set; } = new();
    }

    public class ReviewReplyViewModel
    {
        public int Id { get; set; }
        public int ReviewId { get; set; }
        public string ReplierName { get; set; } = string.Empty;
        public string ReplyText { get; set; } = string.Empty;
        public bool IsSeller { get; set; }
        public string? AttachmentUrls { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
