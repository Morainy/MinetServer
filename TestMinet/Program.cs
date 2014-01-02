using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Collections;

namespace TestMinet
{ 
    class Program
    {

        public static Dictionary<string, UserState> userDict = new Dictionary<string, UserState>();
        public static UserManager m_manager = new UserManager();
                   
        //Function to handle connection between server and a client
        public static void HandleConnection(object socket)
        {
            ConnectionHandler handler = new ConnectionHandler();

            Thread th = new Thread(new ParameterizedThreadStart(handler.HandleConnection));
            th.IsBackground = true;
            th.Start(socket);
            th.Join();
        }

       //main function
        static void Main(string[] args)
        {
            foreach (string key in m_manager.m_userDict.Keys)
            {
                userDict.Add(key, new UserState());
            }   
    
            //add a group
            userDict.Add("Group", new UserState());
           
            //Create a listener
            IPAddress addr = IPAddress.Any;
            TcpListener listener = new TcpListener(addr, 8888);
            listener.Start();


            while (true)
            {
                Socket client = listener.AcceptSocket();

                Thread th = new Thread(new ParameterizedThreadStart(HandleConnection));
                th.Start(client);
            }

        }
    }
}
