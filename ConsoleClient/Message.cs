using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleClient
{
    public class Message
    {
        public Guid Id { get; set; }
        public User Sender { get; set; }
        public DateTime SendTime { get; set; }
        public string Theme { get; set; }
        public string Text { get; set; }
        public bool IsRead { get; set; }
    }
}
