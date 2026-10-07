using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Xml.Linq;

namespace Social_Media.Entities
{

    //Column        Type            Notes
    //Id            Guid / int      PK
    //RecipientId   FK → Users      Indexed
    //LatestActorId FK → Users      Most recent actor
    //ActorCount    int             Default 1; increments on aggregation
    //Type          int (enum)	    NewComment, PostReaction, etc.
    //EntityType    int (enum)	    Post, Comment, Friendship
    //EntityId      Guid/int        For deep linking
    //GroupKey      nvarchar(100)   For aggregation matching, indexed
    //IsRead        bit             Default false, indexed
    //CreatedAt     datetime2       When first created
    //LastUpdatedAt datetime2       Bumped on each aggregation, indexed for ordering.

    // https://eu.chat.pwc.com/c/7094dedc-acee-4954-95f5-2652fa4e87cf
    public class Notification
    {
        public int Id { get; set; }

        public int RecipientId { get; set; } // Who gets the notification (the inbox owner)
        public int LatestActorId { get; set; } // Most recent actor

        public string GroupKey { get; set; } = string.Empty; // For aggregation matching, indexed
        public int ActorCount { get; set; } //Default 1; increments on aggregation
        // Post, Comment Id
        public int EntityId { get; set; } // For deep linking

        public NotificationType NotificationType { get; set; } // NewComment, PostReaction, etc.
        public NotificationEntityType NotificationEntityType { get; set; } // Post, Comment, Friendship
        public bool IsRead { get; set; } = false; // Default false, indexed

        public DateTime CreatedAt { get; set; } // When first created
        public DateTime? LastUpdatedAt { get; set; } // Bumped on each aggregation, indexed for ordering
    
        // Navigation properties
        public User Recipient { get; set; } = null!;
        public User LatestActor { get; set; } = null!;
    }
    // Triggers
    //Action(Trigger)                     ||  Recipient(s)  ||  Notification Type.

    //Someone comments on your post       || Post author    ||  NewComment
    //Someone replies to your comment     || Comment author ||  CommentReply
    //Someone reacts to your post         || Post author    ||  PostReaction
    //Someone reacts to your comment      || Comment author ||  CommentReaction
    //Someone sends a friend request      || Addressee      ||  FriendRequest
    //Someone accepts your friend request || Requester      ||  FriendAccepted

    public enum NotificationType
    {
        //NewComment = 0, CommentReply = 1, PostReaction = 2,
        //CommentReaction = 3, FriendRequest = 4, FriendAccepted = 5
        FriendRequest = 0, FriendAccepted = 1, PostReaction=2,
        NewComment = 3, CommentReaction=4, CommentReply = 5
    }
    public enum NotificationEntityType
    {
        User = 0, Post = 1, Comment = 2, Friendship = 3
    }
}
