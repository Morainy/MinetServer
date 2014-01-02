using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMinet
{
    public class HeaderLine
    {
        public string m_name;
        public string m_value;


        //default constructor
        public HeaderLine() { }

        //constructing from a input string
        public HeaderLine(string name, string value)
        {
            m_name = name;
            m_value = value;
        }

        //constrcuting from a string
        public HeaderLine(string str)
        {
            string[] strArray = str.Split(new Char[] { ' ' });
            m_name = strArray[0];
            m_value = strArray[1];
        }


        //Convert to string
        public string ConvertToString()
        {
            string temp = m_name + " " + m_value + "\r\n";
            return temp;   
        }

    }
}
