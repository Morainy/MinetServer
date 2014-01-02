using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMinet
{
    public class Message
    {
        public string m_date;
        public string m_content;
        public string m_from;

        //default constructor
        public Message() { }

        //constructing by parameters
        public Message(string date, string content, string from)
        {
            m_date = date;
            m_content = content;
            m_from = from;
        }
    }
}
