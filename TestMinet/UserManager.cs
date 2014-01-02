using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMinet
{
    //Used to manager user's info
    public class UserManager
    {
        public Dictionary<string, string> m_userDict;
       // private List<string> m_groupList;
        private string m_basePath;
        
        //Constructor will get user list as <name, password> pairs
        public UserManager()
        {
            m_userDict = new Dictionary<string, string>();
            m_basePath = "..\\..\\userInfo\\";

            StreamReader userListReader = new StreamReader(m_basePath + "UserList.txt", Encoding.UTF8);

            //get user list pair <username, password>
            while (true)
            {
                string line = userListReader.ReadLine();
                if (line == null)
                {
                    break;
                }
                string[] arr = line.Split(new char[] { ' ' });

                m_userDict.Add(arr[0], arr[1]);

            }
            userListReader.Dispose();

            /*
            //get group list
            StreamReader groupListReader = new StreamReader(m_basePath + "GroupList.txt", Encoding.UTF8);
            m_groupList = (groupListReader.ReadToEnd()).Split(new char[] { ' ' }).ToList();
            groupListReader.Dispose();
             * */

        }

        //Add a new user to user list 
        public bool AddUser(string name, string password)
        {
            //if the user name does not exist
            if (!m_userDict.ContainsKey(name))
            {
                //add to the UserList.txt
                StreamWriter writer = new StreamWriter(m_basePath + "UserList.txt", true, Encoding.UTF8);
                writer.WriteLine(name + " " + password);
                writer.Flush();
                writer.Dispose();

                //create a info file for the user
              //  File.Create(m_basePath + name + "FriendList.txt");

                m_userDict.Add(name, password);

                return true;
            }
            return false;
        }
        /*

        //Add a Group
        public bool AddGroup(string groupName, string creator)
        {
            //if the user name does not exist
            if (!m_groupList.Contains(groupName))
            {
                //add to the UserList.txt
                StreamWriter writer = new StreamWriter(m_basePath + "GroupList.txt", true, Encoding.UTF8);
                writer.Write(" " + groupName);
                writer.Flush();

                //create a info file for the user
                File.Create(m_basePath + groupName + "MemberList.txt");

                m_groupList.Add(groupName);

                AddMemberToGroup(groupName, creator);

                return true;
            }
            return false;
        }

        //Add a member to a group
        public void AddMemberToGroup(string groupName, string member)
        {
            StreamWriter writer = new StreamWriter(m_basePath + groupName + "MemberList.txt", true, Encoding.UTF8);
            writer.Write(" " + member);
            writer.Flush();
            writer.Dispose();
        }


        //Get specific user's friend list
        public List<string> GetFriendList(string name)
        {
            List<string> friendList = new List<string>();

            StreamReader reader= new StreamReader(m_basePath + name + "FriendList.txt");

            string list = reader.ReadToEnd();

            reader.Dispose();

            friendList = (list.Split(new Char[] { ' ' })).ToList();

            return friendList;    
        }

        //Get specific group's member list
        public List<string> GetGroupMemberList(string groupName)
        {
            List<string> memberList = new List<string>();

            StreamReader reader = new StreamReader(m_basePath + groupName + "MemberList.txt");

            string list = reader.ReadToEnd();

            reader.Dispose();

            memberList = (list.Split(new Char[] { ' ' })).ToList();

            return memberList;
        }
        */

    }
}
