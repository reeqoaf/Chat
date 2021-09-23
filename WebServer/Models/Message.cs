using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebServer.Models
{
    public class Message
    {
        public Guid Id { get; set; }
        public virtual User Sender { get; set; }
        public virtual User Receiver { get; set; }
        public DateTime SendTime { get; set; }
        [Required] public string Theme { get; set; }
        public string Text { get; set; }
        public bool IsRead { get; set; }

        public Message(string text, string theme)
        {
            Id = Guid.NewGuid();
            SendTime = DateTime.Now;
            Text = text;
            Theme = theme;
            IsRead = false;
        }
    }
}
