﻿using Microsoft.AspNetCore.Identity;

namespace DataAccessLibrary.Models
{
    public class ProfileEntity : IdentityUser
    {
        // public Guid Id { get; set; } --- Inherited from IdentityUser
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public CourseData CourseInfo { get; set; }
        public profileCreationDate CreationDate { get; set; }
        public List<TopicEntity> TopicsToLearn { get; set; } = new List<TopicEntity>();
        public List<TopicEntity> TopicsToTeach { get; set; } = new List<TopicEntity>();
        public List<UserItem> UserItems { get; set; } = new List<UserItem>();
        public List<Connection>? Connections { get; set; }
        public List<ConnectionRequest>? ConnectionRequests { get; set; }
    }
}
