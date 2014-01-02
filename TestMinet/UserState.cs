using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace TestMinet
{
    public class UserState
    {
        public bool m_isOnline;
        public Socket m_socket;

        public string m_ip;
        public Int32 m_listenPort;

        //message list
        public List<Message> m_offLineMessages;
        public List<Message> m_groupMessages;

        //default constructor
        public UserState() 
        {
            m_isOnline = false;
            m_offLineMessages = new List<Message>();
            m_groupMessages = new List<Message>();
        }

        //public Set state
        public void SetState(bool isOnline, Socket socket, string ip, Int32 port)
        {
            m_isOnline = isOnline;
            m_socket = socket;
            m_ip = ip;
            m_listenPort = port;
        }

    }
}
