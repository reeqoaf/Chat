using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebServer.Dto
{
    public class SentMessageDto
    {
        public string ReceiverUsername { get; set; }
        public string SenderUsername { get; set; }
        public string Text { get; set; }
        public string Theme { get; set; }
    }
}
