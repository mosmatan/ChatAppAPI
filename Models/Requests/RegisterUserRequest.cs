﻿namespace ChatAPI.Models.Requests
{
    public class RegisterUserRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
    }
}
