using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class Program
{
    public static string ParseResponse(byte[] bytes, int recievedBytes)
    {
        string firstBits = Convert.ToString(bytes[0], 2).PadLeft(8, '0');

        string version = firstBits.Substring(0, 2);
        string type = firstBits.Substring(2, 2);
        int tkl = Convert.ToInt32(firstBits.Substring(4, 4), 2);
        string code = Convert.ToString(bytes[1], 2).PadLeft(8, '0');
        int messageID = Convert.ToInt32(Convert.ToString(bytes[2], 2).PadLeft(8, '0') +
            Convert.ToString(bytes[3], 2).PadLeft(8, '0'));

        string token = "";
        string optionDelta = "";
        int optionLength = 0;
        string option = "";
        string payload = "";
        if (tkl > 0)
            token = Encoding.ASCII.GetString(bytes, 4, tkl);
        if (recievedBytes > 4+tkl)
        {
            if (Convert.ToString(bytes[4+tkl], 2) == "11111111")
                payload = Encoding.ASCII.GetString(bytes, 5 + tkl, recievedBytes - 5 - tkl);
            else
            {
                string optionBits = Convert.ToString(bytes[4 + tkl], 2).PadLeft(8, '0');
                optionDelta = optionBits.Substring(0, 4);
                optionLength = Convert.ToInt32(optionBits.Substring(4, 4), 2);
                option = Encoding.ASCII.GetString(bytes, 5 + tkl, optionLength);
                if (recievedBytes > 5 + tkl + optionLength)
                    payload = Encoding.ASCII.GetString(bytes, 7 + tkl + optionLength,
                        recievedBytes - 7 - tkl - optionLength);
            }
        }


        string newResponse = "Version: " + version +
            "\nType: " + type +
            "\nToken Length: " + tkl +
            "\nCode: " + code +
            "\nMessage ID: " + messageID +
            "\nToken: " + token +
            "\nOptions: " + option +
            "\nPayload: " + payload;


        return newResponse;
    }
    public static void StartClient()
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

                byte[] msg = { 0x40, 0x01, 0x04, 0xd2, 0xb4, 0x74, 0x65, 0x73, 0x74 };
                int sentBytes = sender.Send(msg);
                int recievedBytes = sender.Receive(bytes);
                //Console.WriteLine("Response = "+ Encoding.ASCII.GetString(bytes, 0, recievedBytes));
                string response = ParseResponse(bytes, recievedBytes);
                Console.WriteLine(response);

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
        StartClient();
        return 0;
    }
}

