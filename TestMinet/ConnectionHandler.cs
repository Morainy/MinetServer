using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;

namespace TestMinet
{
    public class ConnectionHandler
    {
        public string m_clientIP;
        public Int32 m_clientListenPort;
        string m_clientName;
        bool m_hasClientLogin;
        

        bool m_isfirstTimeCommunicate;   
        Timer m_checkBeatTimer;
        public Socket m_socket;

  
        //Constructing by recieveing the uesr dictionary
        public ConnectionHandler()
        {         
            m_checkBeatTimer = new Timer(new TimerCallback(CheckBeat), null, Timeout.Infinite, Timeout.Infinite);
         
        }

                   
        //Function to handle connection between server and a client
        public  void HandleConnection(object socket)
        {
            m_socket = (Socket)socket;
            m_isfirstTimeCommunicate = true;
            m_hasClientLogin = false;

            //start timer
            m_checkBeatTimer.Change(15000, 15000);
            

            while (true)
            {
                Packet p = GetPacket();
                if (p == null)
                {
                    break;
                }


                string method = p.GetRequestLine().m_method;

                //We should share hand at the very first time
                if (m_isfirstTimeCommunicate)
                {
                    if (p.GetRequestLine().m_version == "MINET")
                    {
                        //log 
                        //Console.WriteLine("--Recieveing share hand packet");

                        SendShareHandPacket();

                        //log 
                        //Console.WriteLine("--Has send reply and share hand success...");

                        m_isfirstTimeCommunicate = false;
                    }
                    else
                    {
                        //Console.WriteLine("[attemption] Hsare hand fail and break connection with --" + m_clientName);
                        StopHandle();//if can't share hand stop the connection
                    }
                }
                else
                {
                    if (method == "LOGIN")
                    {
                        m_clientName = p.GetRequestLine().m_otherParams[0];
                        string password = p.GetRequestLine().m_otherParams[1];

                        //log 
                        //Console.WriteLine("--Recieveing Login packet from " + m_clientName);

                        if (Program.m_manager.m_userDict.ContainsKey(m_clientName) 
                            && Program.m_manager.m_userDict[m_clientName] == password)
                        {
                            //deny repeat login
                            if (!Program.userDict[m_clientName].m_isOnline)
                            {
                                m_hasClientLogin = true;
                                m_clientListenPort = Convert.ToInt32(p.GetHeaderValueByName("Port"));
                                m_clientIP = (((System.Net.IPEndPoint)m_socket.RemoteEndPoint).Address).ToString();


                                //set the specific user's state
                                Program.userDict[m_clientName].SetState(true, m_socket, m_clientIP, m_clientListenPort);


                                //log 
                                //Console.WriteLine("--Login success!--- " + m_clientName);

                                SendReplyForLoginRequest(true);//Send login success reply to the client 

                                SendUpdatePacketToOnlineFriends(true);//Inform firends that this client is online now
                            }
                            else
                            {
                                SendReplyForLoginRequest(false, "You Can't login repeatly!");
                            }

                          
                        }
                        else
                        {
                            SendReplyForLoginRequest(false, "User Name Or Password Error!");//Send login fail reply to the client
                            //log 
                            //Console.WriteLine("--Login faile and send fail packet " + m_clientName);
                        }
                    }
                    else if(method == "REGISTER")
                    {
                        string name = p.GetHeaderValueByName("Name");
                        string password = p.GetHeaderValueByName("Password");

                        if (Program.userDict.ContainsKey(name))
                        {
                            ReplyRegisterPacket(false);
                        }
                        else
                        {
                            Program.m_manager.AddUser(name, password);
                            Program.userDict.Add(name, new UserState());
                            ReplyRegisterPacket(true);
                        }
                    }
                    else if (method == "GETLIST")
                    {

                        //log 
                        //Console.WriteLine("--Recieveing GetList packet --" + m_clientName);

                        if (m_hasClientLogin)
                        {
                           
                            //log 
                            //Console.WriteLine("--Sending friend list as reply  to --" + m_clientName);
                            SendFriendListToClient();

                        }
                        else
                        {
                            ///////////////////////////////////////////////////////////
                            //log 
                            //Console.WriteLine("--Can't send friend list back, please login first" + m_clientName);
                        }
                    }
                    else if(method == "GETCS")
                    {
                        //log 
                        //Console.WriteLine("--Recieveing GetCS packet --" + m_clientName);

                        if (m_hasClientLogin)
                        {

                            //log 
                            //Console.WriteLine("--Sending group message list as reply  to --" + m_clientName);
                            SendGroupMessageToClient();

                        }
                        else
                        {
                            ///////////////////////////////////////////////////////////
                            //log 
                            //Console.WriteLine("--Can't send group message back, please login first" + m_clientName);
                        }

                    }
                    else if (method == "LEAVE")
                    {
                        //log 
                        //Console.WriteLine("--Recieceing leave packet from --" + m_clientName);

                        StopHandle();
                       

                    }
                    else if (method == "BEAT")
                    {
                       
                        //log 
                        //Console.WriteLine("--Recieveing Beat From --" + m_clientName);
                        m_checkBeatTimer.Change(15000, 15000);
                    }
                    else if (method == "GETOLLIST")
                    {                        
                        //log
                        //Console.WriteLine("--Recieveing GETOLLIST packet from: " + m_clientName);
                        SendOffLineMessageListToClient();
                    }
                    else if(method == "OLMESSAGE")
                    {                       
                        //log
                        //Console.WriteLine("--Recieve off line packet from: " + p.GetHeaderValueByName("From") + " to: " + p.GetHeaderValueByName("To"));
                        RecieveOffLineMessage(p);
                    }
                    else if(method == "RECIEVEOL")
                    {
                        string from = p.GetHeaderValueByName("From");
                        RemoveOffLineMessage(from);

                        //log
                        //Console.WriteLine("--Recieve off line packet from: " + m_clientName + " where from: " + from);
                    }
                    else if(method == "RECIEVECS")
                    {
                        RemoveGroupMessage();

                        //Console.WriteLine("Recieve RECIEVECS message from:" + m_clientName);
                    }
                    else if(method == "MESSAGE")
                    {
                        RecieveGroupMessage(p);

                        //Console.WriteLine("Recieve group MESSAGE: " + p.GetData() + " * from " + m_clientName );
                    }


                    else { 
                        //Console.WriteLine("--[ERROR] Recieve unknow packet with method: " + method); 
                    }

                    Thread.Sleep(50);
                }

            }

        }

        //Function to  send Update packet to online friend
        //isOnline indicates whether the client is online
        private void SendUpdatePacketToOnlineFriends(bool isOnline)
        {

            Packet update = new Packet();
            List<string> pa = new List<string>();


            if (isOnline)
            {
                pa.Add("1");

                update.AddHeader("IP", m_clientIP);
                update.AddHeader("Port", m_clientListenPort.ToString());
            }
            else
            {
                pa.Add("0");
            }
            pa.Add(m_clientName);

            update.SetRequestLine(Common.CS_VERSION, "UPDATE", pa);


            //Send Update to all friend whos is online
            foreach (string key in Program.userDict.Keys)
            {
                if (key != m_clientName)
                {
                    UserState temp = Program.userDict[key];

                    if (temp.m_isOnline == true)
                    {
                        temp.m_socket.Send(update.ConvertToByteArray());  
                         //log
                        //Console.WriteLine("Sending update packet to :" + key + " flag =" + isOnline.ToString() + " from " + m_clientName + "--reline--" + update.GetRequestLine().m_method); 
                    }

                }
                
            }    

        }

        //public Send friend list
        private void SendFriendListToClient()
        {
            Packet reply = new Packet();
            reply.SetRequestLine(Common.CS_VERSION, "LIST", null);

            string userlist = "";
            //if return user without ip and port, that indicates the user was offline
            //Send Update to all friend whos is online
            foreach (string key in Program.userDict.Keys)
            {               
                if (key != m_clientName && key != "Group")
                {
                    UserState temp = Program.userDict[key];

                    userlist += key;
                    if (temp.m_isOnline == true)
                    {   
                        userlist += (" " + temp.m_ip + " " + temp.m_listenPort.ToString());   
                    }
                    userlist += "\r\n";
                    
                }
            }
            reply.SetData(userlist);

            SendPacketToClient(reply);

         
        }

        //Function to stop handle
        private void StopHandle()
        {
            if (m_socket != null)
            {

                //stop timer
                m_checkBeatTimer.Dispose();
                if (m_hasClientLogin)
                {
                    //set user state
                    Program.userDict[m_clientName].m_isOnline = false;
                    SendUpdatePacketToOnlineFriends(false);
                }



                //close socket
                //m_socket.Shutdown(SocketShutdown.Both);
                //if remote client call shutdown to close the exception then it will return 0 without a exception

                m_socket.Close();
                m_socket = null;

                Thread.Sleep(1000);
            }

            try
            {
                //stop the current thread
                Thread.CurrentThread.Abort();
            }
            catch (Exception excep)
            { }
          
        }


        //Function to check beat message
        //If hasn't recieved beat message in every 15 seconds , m_keepAlive will set to false
        //If we do recieve beat message, then the function will be delay to excuted for 15seconds 
        private void CheckBeat(object o)
        {
            //Console.WriteLine("[attemption] lose onnection with --" + m_clientName);
            StopHandle();
        }

        //Function to send hello packet
        private void SendShareHandPacket()
        {
            if (m_socket != null)
            {
                Packet hello = new Packet();
                hello.SetRequestLine("MIRO", "SERVER", null);

                SendPacketToClient(hello);
            }

        }


        //Function to get a packet 
        private Packet GetPacket()
        {
            byte[] buffer = new byte[1024 * 2];

            int size = 0;
            try
            {
                size = m_socket.Receive(buffer);
                Packet reply = new Packet(buffer);
                return reply;

            }
            catch (Exception e)
            {     
                //Console.WriteLine("[ERROR] lose onnection with remote client! (Get)");
                StopHandle();
            }
            return null;
        }

        //Function to send packet to client
        private void SendPacketToClient(Packet p)
        {
            try
            {
                m_socket.Send(p.ConvertToByteArray());
            }
            catch (Exception e)
            {
                //Console.WriteLine("[ERROR] lose onnection with remote client! (Recieve)");
                StopHandle();
            }
        }


        //Function to send reply packet for a login request
        //isLoginsuccess indicates whether the client has logined successfully
        private void SendReplyForLoginRequest(bool isLoginSuccess, string brief=null)
        {
            Packet reply = new Packet();
            List<string> pa = new List<string>();

            if (isLoginSuccess)
            {
                pa.Add("1");
            }
            else
            {
                pa.Add("0");
                reply.SetData(brief);

                //Console.WriteLine("Error--" + brief);
            }

            reply.SetRequestLine(Common.CS_VERSION, "STATUS", pa);

            SendPacketToClient(reply);
        
        }


        //Function to Recieve offline message and put it to the specific user's message list
        private void RecieveOffLineMessage(Packet p)
        {
            string from = p.GetHeaderValueByName("From");
            string to = p.GetHeaderValueByName("To");
            string date = p.GetHeaderValueByName("Date");
            string content = p.GetData();

            Program.userDict[to].m_offLineMessages.Add(new Message(date, content, from));

        }

        //Function to send back offline list
        private void SendOffLineMessageListToClient()
        {
            string froms = "";
            string dates = "";
            string lengths = "";
            string data = "";

            List<Message> ms = Program.userDict[m_clientName].m_offLineMessages;
            int counter = ms.Count;


            if (counter > 0)
            {
                int i;
                for (i = 0; i < ms.Count - 1; i++)
                {
                    froms += ms[i].m_from + " ";
                    dates += ms[i].m_date + " ";
                    lengths += ms[i].m_content.Length.ToString() + " ";
                    data += ms[i].m_content;
                }
                froms += ms[counter - 1].m_from + "\r\n";
                dates += ms[counter - 1].m_date + "\r\n";
                lengths += ms[counter - 1].m_content.Length.ToString() + "\r\n";
                data += ms[counter - 1].m_content;
            }


            //Construct ol list packet and send it
            Packet p = new Packet();
            p.SetRequestLine(Common.CS_VERSION, "OLLIST", null);
            p.AddHeader("Counter", counter.ToString());
            p.SetData(froms + dates + lengths + data);

            SendPacketToClient(p);


            ///////////////////////////////////
            //Console.WriteLine("Sending: list = " + data);
        }
            //////////////////////////////////
        

         //Function to send Group message to client
        private void SendGroupMessageToClient()
        {
            string froms = "";
            string dates = "";
            string lengths = "";
            string data = "";

            List<Message> ms = Program.userDict[m_clientName].m_groupMessages;
            int counter = ms.Count;


            if (counter > 0)
            {
                int i;
                for (i = 0; i < ms.Count - 1; i++)
                {
                    froms += ms[i].m_from + " ";
                    dates += ms[i].m_date + " ";
                    lengths += ms[i].m_content.Length.ToString() + " ";
                    data += ms[i].m_content;
                }
                froms += ms[counter - 1].m_from + "\r\n";
                dates += ms[counter - 1].m_date + "\r\n";
                lengths += ms[counter - 1].m_content.Length.ToString() + "\r\n";
                data += ms[counter-1].m_content;
            }


            //Construct ol list packet and send it
            Packet p = new Packet();
            p.SetRequestLine(Common.CS_VERSION, "OLCSMESSAGE", null);
            p.AddHeader("Counter", counter.ToString());
            p.SetData(froms + dates + lengths + data);

            SendPacketToClient(p);

            ///////////////////////////////////
            //Console.WriteLine("Sending: group message = " + data);
        }

        //Function to remove offline message when recieve RECIEVEOL packet
        private void RemoveOffLineMessage(string from)
        {
            UserState state = Program.userDict[m_clientName];

            state.m_offLineMessages.RemoveAll(
                delegate(Message m)
                {
                    if (m.m_from == from)
                    {
                        return true;
                    }
                    return false;
                });
        }
        //Function to recieve group message
        public void RecieveGroupMessage(Packet p)
        {
            string from = m_clientName;
            string date = p.GetHeaderValueByName("Date");
            string content = p.GetData();

            p.m_requestLine.m_method = "CSMESSAGE";

            foreach (string to in Program.userDict.Keys)
            {
                if (to != "Group" && to != m_clientName)
                {
                    //resend to all group member
                    Program.userDict[to].m_groupMessages.Add(new Message(date, content, from));

                    //sne to all online users
                    if (Program.userDict[to].m_isOnline)
                    {
                        try
                        {
                            Program.userDict[to].m_socket.Send(p.ConvertToByteArray());
                        }
                        catch (Exception) { }
                    }

                }
            }
          

        }

        //Function to remove group message
        private void RemoveGroupMessage()
        {
            Program.userDict[m_clientName].m_groupMessages.Clear();
        }

        //Function to reply register packet
        private void ReplyRegisterPacket(bool flag)
        {
            Packet reply = new Packet();
            List<string> pa = new List<string>();

            if (flag)
            {
                pa.Add("1");
                //Console.WriteLine("Register successfully!");
            }
            else
            {
                pa.Add("0");
                //Console.WriteLine("Register fail!");
            }

            reply.SetRequestLine(Common.CS_VERSION, "STATUS", pa);

            SendPacketToClient(reply);
        }


    }
}
