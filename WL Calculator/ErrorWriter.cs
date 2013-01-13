using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace WL_Calculator
{
    class ErrorWriter
    {
        private StreamWriter output;

        public ErrorWriter() 
        {
            output = new StreamWriter(".//Log.txt", false);
            output.WriteLine("[" + DateTime.Now.ToString() + "] " + "Program started");
            output.Close();
        }

        public void Write(string message)
        {
            output = new StreamWriter(".//Log.txt", true);
            output.WriteLine("[" + DateTime.Now.ToString() + "] " + message);
            output.Close();
        }
    }
}
