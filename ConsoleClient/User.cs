using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleClient
{
    public class User
    {
        public string Username { get; set; }
        public string Name { get; set; }

        public User(string name, string username)
        {
            Name = name;
            Username = username;
        }
    }
}
