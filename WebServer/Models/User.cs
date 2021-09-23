using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebServer.Models
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key] public Guid Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public virtual List<Message> ReceivedMessages { get; set; } = new List<Message>();
        public virtual List<Message> SentMessages { get; set; } = new List<Message>();
        public User(string name, string username, string password)
        {
            Id = Guid.NewGuid();
            Name = name;
            Username = username;
            Password = password;
        }
    }
}
