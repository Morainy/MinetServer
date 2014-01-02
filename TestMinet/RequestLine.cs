using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMinet
{
    public class RequestLine
    {
        public string m_version;
        public string m_method;
        public List<string> m_otherParams;

        //default constructor
        public RequestLine()
        {
            m_otherParams = new List<string>();
        }

        //constructing by recieving parameters
        public RequestLine(string version, string method, List<string> otherParams)
        {
            if (otherParams != null)
            {
                m_otherParams = new List<string>(otherParams);
            }
            m_version = version;
            m_method = method;

        }


        //constrcuting by recieving a string
        public RequestLine(string str)
        {
            string[] strArray = str.Split(new Char[] { ' ' });//split the request line string by space
            m_version = strArray[0];
            m_method = strArray[1];

            m_otherParams = new List<string>();
            for (int i=2; i<strArray.Length; i++)
            {
                m_otherParams.Add(strArray[i]);
            }

        }

        //convert to string 
        public string ConvertToString()
        {
            string temp = m_version + " " + m_method;

            if (m_otherParams != null)
            {
                foreach (string line in m_otherParams)
                {
                    temp = temp + " " + line;
                }
            }
            temp += "\r\n";

            return temp;
        }


    }
}
