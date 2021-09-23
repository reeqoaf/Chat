using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebServer.Models;

namespace WebServer.Dto
{
    public class UserDto
    {
        public string Username { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public UserDto(string username, string name, string password = null)
        {
            Username = username;
            Name = name;
            Password = password;
        }
    }
}
