﻿namespace BLL.DTO
{
    public class UserLikesDTO
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public UserDTO User { get; set; }
        public bool? Likes { get; set; }
    }
}
