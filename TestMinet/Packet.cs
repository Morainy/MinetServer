using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TestMinet
{
    public class Packet
    {
        public RequestLine m_requestLine;
        public List<HeaderLine> m_headers;
        public string m_data;

        //deafult constructor
        //add date as a header defaultly
        public Packet() 
        {
            m_data = "";
            m_headers = new List<HeaderLine>();
            AddHeader("Date", System.DateTime.Now.ToShortDateString() + " " + System.DateTime.Now.ToLongTimeString());
        }

        //constructing by recieveing an byte array
        //using UTF-8 Encoding
        public Packet(byte[] bytes)
        {
            m_data = "";
            //bytes can't be null
            if (bytes != null)
            {
                Stream stream = new MemoryStream(bytes);
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    //get request line
                    string requestLineStr = reader.ReadLine();
                    m_requestLine = new RequestLine(requestLineStr);

                    //get headers
                    string headerStr;
                    m_headers = new List<HeaderLine>();
                    while (true)
                    {
                        headerStr = reader.ReadLine();
                        if (headerStr == string.Empty)
                        {
                            break;
                        }
                        m_headers.Add(new HeaderLine(headerStr));
                    }
                    ///////////////////////////
                    string temp = GetHeaderValueByName("Content-Length");
                    int length = 0;
                    if (temp != null)
                    {
                        length = Convert.ToInt32(temp);
                    }

                    if (length > 0)
                    {
                        //get data
                        m_data = (reader.ReadToEnd()).Substring(0, length);
                    }
                        
                }

            }
            else
            {
                //////////////////////////////////////////////

            }
        }

        //convert packet to byte array
        public byte[] ConvertToByteArray()
        {
            string packetStr = m_requestLine.ConvertToString();

            if (m_headers != null)
            {
                foreach (HeaderLine header in m_headers)
                {
                    packetStr += header.ConvertToString();
                }
            }
            packetStr += "\r\n";
            packetStr += m_data;
            packetStr += "\r\n";

            return Encoding.UTF8.GetBytes(packetStr);

        }

        //set request line
        public void SetRequestLine(string version, string method, List<string> otherParams)
        {
            m_requestLine = new RequestLine(version, method, otherParams);
        }
        //get request line
        public RequestLine GetRequestLine()
        {
            return m_requestLine;

        }

        //add header
        public void AddHeader(string name, string value)
        {
            m_headers.Add(new HeaderLine(name, value));
        }

        //get header value by name
        //if the header does't exist return null
        public string GetHeaderValueByName(string name)
        {
            foreach (HeaderLine header in m_headers)
            {
                if (header.m_name == name)
                {
                    return header.m_value;
                }
            }
            return null;
        }

        //Set Data
        public void SetData(string data)
        {
            m_data = data;
            AddHeader("Content-Length", data.Length.ToString());
        }
        //return  data
        public string GetData()
        {
            return m_data;
        }


    }
}
