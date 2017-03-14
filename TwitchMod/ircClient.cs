using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TwitchMod
{
    class ircClient
    {
        private string userName;
        private string channel;
        private TcpClient tcpClient;
        private StreamReader inputStream;
        private StreamWriter outpurStream;

        public ircClient(string ip, int port, string userName, string password)
        {
            this.userName = userName;
            tcpClient = new TcpClient(ip, port);
            inputStream = new StreamReader(tcpClient.GetStream());
            outpurStream = new StreamWriter(tcpClient.GetStream());
            outpurStream.WriteLine("PASS " + password);
            outpurStream.WriteLine("NICK " + userName);
            outpurStream.WriteLine("USER " + userName + " 8 * " + userName);
            outpurStream.Flush();

        }
        public void joinRoom(string channel)
        {
            this.channel = channel;
            outpurStream.WriteLine("JOIN #" + channel);
            outpurStream.Flush();

        }
        public void sendIrcMessage(string message)
        {
            outpurStream.WriteLine(message);
            outpurStream.Flush();

        }
        public void sendChatMessage(string message)
        {
            sendIrcMessage(":" + userName + "!" + userName + "@" + userName + "irc.chat.twitch.tv PRIVMSG #" + channel + " :" + message);
        }
        public string readMessage()
        {
            try
            {
                string message = inputStream.ReadLine();
                return message;
            }
            catch
            {
                return null;
            }
        }
    }
}
