using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleClient
{
    class Program
    {
        static HttpClient HttpClient = new HttpClient();
        const string ConnectionString = "http://localhost:5000/";
        static User CurrentUser { get; set; }
        static bool CycleTemp { get; set; } = true;
        static List<Message> Messages { get; set; } = new List<Message>();
        static List<Message> UnreadMessages { get; set; } = new List<Message>();
        static void Main(string[] args)
        {
            Start();
            Thread messageGetter = new Thread(() =>
            {
                try
                {
                    GetMessagesEveryTenSeconds().GetAwaiter().GetResult();
                }
                catch (Exception e) { }
            });
            messageGetter.Start();
            Menu();
            CycleTemp = false;
            messageGetter.Interrupt();
        }
        static async Task GetMessagesEveryTenSeconds()
        {
            while (CycleTemp)
            {
                var delayTask = Task.Delay(1000);
                Messages = await GetAllMessagesAsync(CurrentUser.Username);
                UnreadMessages = Messages.Where(x => !x.IsRead).ToList();
                await delayTask;
            }
        }
        static void Start()
        {
            bool temp = true;
            Console.WriteLine("Hello!");
            do
            {
                Console.Clear();
                Console.WriteLine("Choose action: ");
                Console.WriteLine("1 - Login");
                Console.WriteLine("2 - Register");
                var key = Console.ReadKey();
                Console.WriteLine();
                switch (key.Key)
                {
                    case ConsoleKey.D1:
                        Login();
                        if (CurrentUser != null)
                        {
                            temp = false;
                        }
                        break;
                    case ConsoleKey.D2:
                        Register();
                        break;
                }
            } while (temp);

        }
        static void Menu()
        {
            bool temp = true;
            do
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("You currently logged in as " + CurrentUser.Username);
                Console.ResetColor();
                Console.WriteLine("Choose action: ");
                Console.WriteLine($"1 - Show unread messages ({UnreadMessages?.Count})");
                Console.WriteLine($"2 - Show all messages ({Messages?.Count})");
                Console.WriteLine("3 - Write message");
                Console.WriteLine("4 - Exit");
                var key = Console.ReadKey();
                Console.WriteLine();
                switch (key.Key)
                {
                    case ConsoleKey.D1:
                        PrintMessages(UnreadMessages);
                        break;
                    case ConsoleKey.D2:
                        PrintMessages(Messages);
                        break;
                    case ConsoleKey.D3:
                        WriteMessage();
                        break;
                    case ConsoleKey.D4:
                        return;
                    default:
                        break;
                }
            } while (temp);
        }
        static void PrintMessages(List<Message> messages)
        {
            Console.Clear();
            int count = 0;
            for (int i = 0; i < messages.Count; i++)
            {
                Console.Write($"{count + 1}. ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"{messages[i].Sender.Username}: ");
                Console.ResetColor();
                Console.WriteLine($"{messages[i].Theme}\t ({messages[i].SendTime})");
                count++;
                if ((i + 1) % 5 == 0 || i == messages.Count - 1)
                {
                    Console.WriteLine("Choose message or action: ");
                    if (i != messages.Count - 1) Console.WriteLine("8 - Next messages");
                    Console.WriteLine("9 - Go to menu");
                    bool temp = true;
                    do
                    {
                        var key = Console.ReadKey();
                        Console.WriteLine();
                        switch (key.Key)
                        {
                            case ConsoleKey.D1 when (i - count + 1) < messages.Count:
                                PrintMessage(messages[i - count + 1]);
                                break;
                            case ConsoleKey.D2 when (i - count + 2) < messages.Count:
                                PrintMessage(messages[i - count + 2]);
                                break;
                            case ConsoleKey.D3 when (i - count + 3) < messages.Count:
                                PrintMessage(messages[i - count + 3]);
                                break;
                            case ConsoleKey.D4 when (i - count + 4) < messages.Count:
                                PrintMessage(messages[i - count + 4]);
                                break;
                            case ConsoleKey.D5 when (i - count + 5) < messages.Count:
                                PrintMessage(messages[i - count + 5]);
                                break;
                            case ConsoleKey.D8:
                                break;
                            case ConsoleKey.D9:
                                return;
                            default:
                                temp = false;
                                break;

                        }
                        i = 0;
                    }
                    while (!temp);
                    count = 0;
                    Console.Clear();
                }
            }
        }
        static void PrintMessage(Message message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(message.Sender.Name);
            Console.ResetColor();
            Console.Write("-> you ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("at ");
            Console.ResetColor();
            Console.WriteLine(message.SendTime);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Theme: ");
            Console.ResetColor();
            Console.WriteLine(message.Theme);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Text: ");
            Console.ResetColor();
            Console.WriteLine(message.Text);
            Console.WriteLine("\nPress any key to continue");
            Console.ReadKey();
            MarkAsReadMessage(message).GetAwaiter().GetResult();
        }
        static void Login()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Enter username: ");
            Console.ResetColor();
            string username = Console.ReadLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Enter password: ");
            Console.ResetColor();
            string password = Console.ReadLine();
            CurrentUser = Login(username, password).GetAwaiter().GetResult();
            if (CurrentUser == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error! Username or password is incorrect");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("You succesfully logged in");
                Console.ResetColor();
            }
        }
        static void WriteMessage()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Enter username of receiver: ");
            Console.ResetColor();
            var username = Console.ReadLine();
            if (GetUserAsync(username).Result == null)
            {
                Console.WriteLine("That username does not exist");
            }
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Enter theme: ");
            Console.ResetColor();
            var theme = Console.ReadLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Enter your message: ");
            Console.ResetColor();
            var text = Console.ReadLine();
            Console.ResetColor();
            if (WriteMessage(username, CurrentUser.Username, text, theme).Result == false)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Message too short!");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Message sent!");
                Console.ResetColor();
            }
        }
        static void Register()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Enter username: ");
            Console.ResetColor();
            string username = Console.ReadLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Enter name: ");
            Console.ResetColor();
            string name = Console.ReadLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Enter password: ");
            Console.ResetColor();
            string password = Console.ReadLine();
            var registered = Register(username, name, password).GetAwaiter().GetResult();
            if (!registered)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error! Data is short(< 6) or user with that username is currently exists");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("You succesfully created a user");
                Console.ResetColor();
            }
        }
        static async Task<User> Login(string username, string password)
        {
            bool check = false; ;
            HttpResponseMessage response = await HttpClient.PostAsJsonAsync(ConnectionString + "user/login/", new { username = username, password = password });
            if (response.IsSuccessStatusCode)
            {
                check = await response.Content.ReadAsAsync<bool>();
            }
            if (!check)
            {
                return null;
            }
            else
            {
                return await GetUserAsync(username);
            }
        }
        static async Task<bool> Register(string username, string name, string password)
        {
            bool registered = false;
            HttpResponseMessage response = await HttpClient.PostAsJsonAsync(ConnectionString + "user/add/", new { username = username, name = name, password = password });
            if (response.IsSuccessStatusCode)
            {
                registered = await response.Content.ReadAsAsync<bool>();
            }
            return registered;
        }
        static async Task<bool> WriteMessage(string receiverUsername, string senderUsername, string text, string theme)
        {
            bool sent = false;
            HttpResponseMessage response = await HttpClient.PostAsJsonAsync(ConnectionString + "user/writemessage/", new { ReceiverUsername = receiverUsername, SenderUsername = senderUsername, Text = text, Theme = theme });
            if (response.IsSuccessStatusCode)
            {
                sent = await response.Content.ReadAsAsync<bool>();
            }
            return sent;
        }
        static async Task<User> GetUserAsync(string username)
        {
            User user = null;
            HttpResponseMessage response = await HttpClient.GetAsync(ConnectionString + $"user/find?username={username}");
            if (response.IsSuccessStatusCode)
            {
                user = await response.Content.ReadAsAsync<User>();
            }
            return user;
        }
        static async Task<bool> MarkAsReadMessage(Message message)
        {
            bool isMarked = false;
            HttpResponseMessage response = await HttpClient.GetAsync(ConnectionString + $"user/markmessageasread?messageid={message.Id}");
            if (response.IsSuccessStatusCode)
            {
                isMarked = await response.Content.ReadAsAsync<bool>();
            }
            return isMarked;
        }
        static async Task<List<Message>> GetAllMessagesAsync(string username)
        {
            List<Message> messages = null;
            HttpResponseMessage response = await HttpClient.GetAsync(ConnectionString + $"user/getallmessages?username={username}");
            if (response.IsSuccessStatusCode)
            {
                messages = await response.Content.ReadAsAsync<List<Message>>();
            }
            return messages;
        }
    }
}
