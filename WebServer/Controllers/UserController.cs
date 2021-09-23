using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebServer.Context;
using WebServer.Dto;
using WebServer.Models;

namespace WebServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        public DbService Context { get; set; }
        public UserController(DbService context)
        {
            Context = context;
            Context.Users.Include(x => x.ReceivedMessages).ToList();
            Context.Users.Include(x => x.SentMessages).ToList();
        }
        [HttpGet]
        public IEnumerable<User> Get()
        {
            return Context.Users.ToList();
        }
        [HttpGet("find")]
        public UserDto FindPublic(string username)
        {
            User user = Context.Users.Where(o => o.Username == username).FirstOrDefault();
            return user == null ? null : new UserDto(user.Username, user.Name);
        }
        public User Find(string username)
        {
            return Context.Users.Where(o => o.Username == username).FirstOrDefault();
        }
        [HttpPost("add")]
        public bool Add(UserDto user)
        {
            if(user.Username.Length < 6 || user.Name.Length < 6 || user.Password.Length < 6)
            {
                return false;
            }
            if (Context.Users.Where(o => o.Username == user.Username).Count() != 0)
            {
                return false;
            }
            Context.Users.Add(new User(user.Name, user.Username, user.Password));
            Context.SaveChanges();
            return true;
        }
        [HttpPost("login")]
        public bool Login(UserDto user)
        {
            if (Context.Users.Where(o => o.Username == user.Username && o.Password == user.Password).Count() == 1)
            {
                return true;
            }
            return false;
        }
        [HttpPost("writemessage")]
        public bool WriteMessage(SentMessageDto data)
        {
            var receiver = Find(data.ReceiverUsername);
            var sender = Find(data.SenderUsername);
            if (receiver == null || sender == null)
            {
                return false;
            }
            if (data.Text.Trim().Length < 5)
            {
                return false;
            }
            var message = new Message(data.Text, data.Theme);
            message.Sender = sender;
            message.Receiver = receiver;
            Context.Messages.Add(message);
            Context.SaveChanges();
            return true;
        }
        [HttpGet("getallmessages")]
        public IEnumerable<MessageDto> GetAllMessages(string username)
        {
            List<MessageDto> result = new List<MessageDto>();

            if (username == null)
            {
                Context.Messages.ForEachAsync(message => result.Add(new MessageDto(message.SendTime, message.Text, message.IsRead, new UserDto(message.Sender.Username, message.Sender.Name), message.Id, message.Theme)));
            }
            else
            {
                Find(username).ReceivedMessages.ForEach(message => result.Add(new MessageDto(message.SendTime, message.Text, message.IsRead, new UserDto(message.Sender.Username, message.Sender.Name), message.Id, message.Theme)));
            }
            return result;
        }
        [HttpGet("markmessageasread")]
        public bool MarkMessageAsRead(Guid messageId)
        {
            var message = Context.Messages.Find(messageId);
            if (message == null)
            {
                return false;
            }
            message.IsRead = true;
            Context.SaveChanges();
            return true;
        }
    }
}
