using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WebServer.Models;

namespace WebServer.Dto
{
    public class MessageDto
    {
        public Guid Id { get; set; }
        public UserDto Sender { get; set; }
        public DateTime SendTime { get; set; }
        public string Theme { get; set; }
        public string Text { get; set; }
        public bool IsRead { get; set; }

        public MessageDto(DateTime sendTime, string text, bool isRead, UserDto sender, Guid id, string theme)
        {
            SendTime = sendTime;
            Text = text;
            IsRead = isRead;
            Sender = sender;
            Id = id;
            Theme = theme;
        }
    }
}
