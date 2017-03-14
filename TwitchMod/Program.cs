using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Text.RegularExpressions;
using System.Net;
using System.Runtime.InteropServices;

namespace TwitchMod
{
    class Program
    {
        static ircClient irc = new ircClient("irc.chat.twitch.tv", 6667, "USERNAME", "oauth:aksk8rbg0pvhb12z3u684xxxxxxxxx");
        static int color = 0;
        static string userCount;
        static void Main(string[] args)
        {
            logo();
            Thread.Sleep(1000);
            irc.joinRoom("sodapoppin");
            Thread t = new Thread(readchat);
            t.Start();
            Thread user = new Thread(viewerCount);
            user.Start();

            while (true)
            {
                Console.ReadKey();               
            }
        }
        static void readchat()
        {
            Regex regx = new Regex(@"\b(?:https?://|www\.)\S+\b");
            Random r = new Random();
            ConsoleColor userStatus = new ConsoleColor();
            int nr = 0;
            int pongs = 0;
            irc.sendIrcMessage("CAP REQ :twitch.tv/tags");
            while (true)
            {
                try
                {
                    string message = irc.readMessage();
                    string FileName = string.Format("RawLog-{0:yyyy-MM-dd_hh}.txt", DateTime.Now);
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"logs\" + FileName, true))
                    {
                        file.WriteLine(DateTime.Now.ToString("h:mm:ss tt")+ " " + message);
                    }
                    nr++;
                    Console.Title = "Connected to #Sodapoppin - Messages: " + nr + " - Pongs: " + pongs + " - Viewers: " + userCount;
                    if (message.Contains("PING"))
                    {
                        pongs++;
                        irc.sendIrcMessage(message.Replace("PING", "PONG"));
                    }
                    //badges =; color =#FF69B4;display-name=Igelness;emotes=;id=4465aac8-196e-47b9-a6dc-9f2cb9448288;mod=0;room-id=26301881;sent-ts=1488845195133;subscriber=0;tmi-sent-ts=1488845195686;turbo=0;user-id=73579482;user-type= :igelness: gachiGASM
                    var username = Regex.Match(message, @"display-name=(.+?);").Groups[1].Value;
                    var usertype = Regex.Match(message, @"user-type=(.+?):").Groups[1].Value;
                    var subscriber = Regex.Match(message, @"subscriber=(.+?);").Groups[1].Value;
                    if (usertype == "mod ")
                    {
                        username = username + "(Mod)";
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.BackgroundColor = ConsoleColor.Red;
                    }     
                    else if (usertype == "admin ")
                    {
                        username = username + "(Admin)";
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.BackgroundColor = ConsoleColor.Red;
                    }
                    else if (usertype == "staff ")
                    {
                        username = username + "(staff)";
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.BackgroundColor = ConsoleColor.Red;
                    }
                    else if (subscriber == "1")
                    {
                        username = username + "(Sub)";
                        Console.ForegroundColor = (ConsoleColor)r.Next(8, 14);
                        
                    }
                    message = message.Substring(message.IndexOf("#sodapoppin :") + 13);
                    //Console.WriteLine(username + ": " +  message);
                    //var offset = message.IndexOf(':');
                    //var result = message.IndexOf(':', offset + 1);
                    //string username = message.Substring(1, message.IndexOf("!") - 1);

                    //if (username.Contains("tmi.twitch.tv"))
                    //{
                    //    username = "Server";
                    //}
                    //message = message.Substring(result + 1);
                    if (regx.IsMatch(message))
                    {
                        message = "Link";
                    }

                    Console.Write(username);
                    Console.ResetColor();
                    Console.Write(": ");
                    Console.WriteLine(message);
                    string FileName2 = string.Format("Log-{0:yyyy-MM-dd_hh}.txt", DateTime.Now);
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"logs\" + FileName2, true))
                    {
                        file.WriteLine(DateTime.Now.ToString("h:mm:ss tt") + " " + username + ": " +message);
                    }
                }
                catch
                {

                }
            }            
        }
        static void viewerCount()
        {
            while (true)
            {
                Thread.Sleep(1000);
                using (var client = new WebClient())
                {
                    string result = client.DownloadString("https://tmi.twitch.tv/group/user/sodapoppin/chatters");
                    //var username = Regex.Match(message, @"display-name=(.+?);").Groups[1].Value;
                    userCount = Regex.Match(result, @"""chatter_count"": (.+?),").Groups[1].Value;
                }
            }
        }
        static void logo ()
        {
            Console.ForegroundColor =ConsoleColor.Red;
            Console.WriteLine();
            Console.WriteLine(@"      ______         _ __       __    __  ___          __");
            Console.WriteLine(@"     /_  __/      __(_) /______/ /_  /  |/  /___  ____/ /");
            Console.WriteLine(@"      / / | | /| / / / __/ ___/ __ \/ /|_/ / __ \/ __  /");
            Console.WriteLine(@"     / /  | |/ |/ / / /_/ /__/ / / / /  / / /_/ / /_/ /");
            Console.WriteLine(@"    /_/   |__/|__/_/\__/\___/_/ /_/_/  /_/\____/\__,_/ ");
            Console.WriteLine(@"    V1.0 - Valleion - TwitchMod.nu");
            Console.WriteLine();
            Console.ResetColor();
            Thread.Sleep(1000);
        }
    }
}
