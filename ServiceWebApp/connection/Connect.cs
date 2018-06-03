using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceWebApp.connection
{
    class Connect
    {
            private static Connect instance;
            private Stream stm;
            private TcpClient tcpclnt;
            private ASCIIEncoding asen;
            public bool connected = false;
            private object lockObj = new object();
            public event EventHandler<MessageEventArgs> MessageRecieved;

            /// <summary>
            /// Creates an instance of the connect class
            /// </summary>
            /// <return and instance of connect></return>
            public static Connect Instance
            {
                get
                {
                    if (instance == null)
                    {
                        instance = new Connect();

                    }
                    return instance;
                }
            }

            /// <summary>
            /// Subscribe to the event
            /// </summary>
            /// <param Ireceiver="receiver"></param>
            public void SubscribeToMessage(IReceiver receiver)
            {
                MessageRecieved += receiver.Subscribe;
            }


            /// <summary>
            /// Connects to ther sever
            /// </summary>
            private Connect()
            {

                try
                {
                    tcpclnt = new TcpClient();
                    Console.WriteLine("Connecting.....");

                    tcpclnt.Connect("127.0.0.1", 8000);
                    // use the ipaddress as in the server program

                    Console.WriteLine("Connected");
                    //Console.Write("Enter the string to be transmitted : ");

                    //String str = Console.ReadLine();
                    stm = tcpclnt.GetStream();
                    asen = new ASCIIEncoding();
                    connected = true;
                }
                catch (Exception e)
                {

                    Console.WriteLine("Error..... " + e.StackTrace);

                }
            }

            /// <summary>
            /// Read from the server
            ///invokes an event based in the output
            /// </summary>
            public string ReadConnection()
            {
                while (true)
                {
                    try
                    {
                        byte[] bb = new byte[1000];
                        string output = "";

                        int k = stm.Read(bb, 0, 1000);
                        for (int i = 0; i < k; i++)
                            output += Convert.ToChar(bb[i]).ToString();


                        if (output[0] == '1') MessageRecieved?.Invoke(this, new MessageEventArgs(output, "Log"));
                        else if (output[0] == '2') MessageRecieved?.Invoke(this, new MessageEventArgs(output, "Settings"));
                        else MessageRecieved?.Invoke(this, new MessageEventArgs(output, "Close"));

                    }

                    catch (Exception e)
                    {
                        //Console.WriteLine("Error..... " + e.StackTrace);
                        return "error";
                    }
                }
            }

            /// <summary>
            /// Writes to the server
            /// </summary>

            public void Write(string path)
            {
                try
                {
                    byte[] ba = asen.GetBytes(path);
                    Console.WriteLine("Transmitting.....");
                    lock (lockObj)
                    {
                        stm.Write(ba, 0, ba.Length);
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine("Error..... " + e.StackTrace);

                    return;
                }
            }

            /// <summary>
            /// Creates a write conenection with the server
            /// </summary>
            /// <param string="path"></param>
            /// <return the ouput string></return>
            public string WriteConnection(string path)
            {
                lock (lockObj)
                {
                    try
                    {

                        byte[] ba = asen.GetBytes(path);
                        Console.WriteLine("Transmitting.....");

                        stm.Write(ba, 0, ba.Length);

                        byte[] bb = new byte[1000];
                        string output = "";

                        int k = stm.Read(bb, 0, 1000);
                        for (int i = 0; i < k; i++)
                            output += Convert.ToChar(bb[i]).ToString();


                        return output;


                    }

                    catch (Exception e)
                    {
                        Console.WriteLine("Error..... " + e.StackTrace);

                        return "error";
                    }
                }


            }

            /// <summary>
            /// Closes the connection
            /// </summary>
            public void CloseConnction()
            {
                if (tcpclnt != null) tcpclnt.Close();
                if (stm != null) stm.Close();
            }

        
    }
}