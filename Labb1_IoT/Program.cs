using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class Program
{
    
    
    public static void StartClient(Coap.Message message)
    {
        byte[] bytes = new byte[1024];

        try
        {
            IPHostEntry ipHostEntry = Dns.GetHostEntry("coap.me");
            IPAddress ip = ipHostEntry.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ip, 5683);
                
            Socket sender = new Socket(ip.AddressFamily, SocketType.Dgram, ProtocolType.Udp);

            try
            {
                sender.Connect(endPoint);
                Console.WriteLine("Socket connected to " + sender.RemoteEndPoint.ToString());

                string response = Coap.SendMessage(message, sender);
                Console.WriteLine(response);

                //byte[] msg = { 0x40, 0x01, 0x04, 0xd2, 0xb4, 0x74, 0x65, 0x73, 0x74 };
                //int sentBytes = sender.Send(msg);
                //int recievedBytes = sender.Receive(bytes);

                //Console.WriteLine("Response = "+ Encoding.ASCII.GetString(bytes, 0, recievedBytes));

                //Console.WriteLine("Hex: " + Convert.ToString(bytes[14], 16));

                //string response = ParseResponse(bytes, recievedBytes);
                //Console.WriteLine(response);


                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
            } catch (ArgumentNullException ane)
            {
                Console.WriteLine(ane.Message);
            } catch(SocketException se)
            {
                Console.WriteLine(se.Message);
            } catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        } catch (Exception ex)
        {
            Console.WriteLine(ex);
        }

    }
    public static int Main()
    {
        Coap.Message message = new Coap.Message();
        message.type = Coap.MType.Confirmable;
        message.method = Coap.Method.Post;
        message.id = 1234;
        message.AddOption(Coap.OptionType.UriPath, "test");

        StartClient(message);
        
        return 0;
    }
}

