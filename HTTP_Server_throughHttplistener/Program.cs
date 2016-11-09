using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Web;
using System.Threading.Tasks;

namespace HTTP_Server_throughHttplistener
{
    class Program
    {
        public static void Main(string[] args)
        {
            string uri = @"http://127.0.0.1:8080/say/";
            HTTPServer server = new HTTPServer();
            server.StartServer(uri);
            Console.ReadLine();
        }
    }
}
