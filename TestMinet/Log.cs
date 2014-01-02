using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace minet
{
    
    public class Log
    {
        public StreamWriter writer;
        public int counter;

        //Constructor
        public Log()
        {          
            //If the file does exist then create it with the name of the date
            //If the file exist then append to the end of the file
            string path = "..\\..\\logs\\" + System.DateTime.Now.ToLongDateString() + ".txt";
            writer = new StreamWriter(path, true, Encoding.UTF8);
            counter = 1;

            writer.WriteLine("-----------------------------------------------**Starting At " + System.DateTime.Now.ToLongTimeString() + "**");
        }

        //Function to add a record 
        public void AddLogMessage(string message)
        {
            if (message != null)
            {
                writer.WriteLine(counter.ToString() + "    " + System.DateTime.Now.ToLongTimeString() + " : " + message);
                writer.Flush();
                counter++;
            }        
        }

        public void Close()
        {
            writer.WriteLine("\r\n\r\n\r\n");
            writer.Close();
        }
    }
    
}
