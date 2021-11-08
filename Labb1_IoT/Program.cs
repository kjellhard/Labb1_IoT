using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class Program
{
    public static string S2b(string data)
    {
        StringBuilder sb = new StringBuilder();

        foreach (char c in data.ToCharArray())
        {
            sb.Append(Convert.ToString(c, 2).PadLeft(8, '0'));
        }
        return sb.ToString();
    }
    public static string ParseResponse(byte[] bytes, int recievedBytes)
    {
        string firstBits = Convert.ToString(bytes[0], 2).PadLeft(8, '0');

        int version = Convert.ToInt32(firstBits.Substring(0, 2), 2);
        string type = Coap.MessageType(Convert.ToInt32(firstBits.Substring(2, 2), 2));
        int tkl = Convert.ToInt32(firstBits.Substring(4, 4), 2);
        string code = Coap.CoapResponseCode(bytes[1]);

        //int code = Convert.ToInt32(Convert.ToString(bytes[1], 2).PadLeft(8, '0'), 2);
        int messageID = Convert.ToInt32(Convert.ToString(bytes[2], 2).PadLeft(8, '0') +
            Convert.ToString(bytes[3], 2).PadLeft(8, '0'), 2);

        string token = "";
        var option = new List<String>();
        string payload = "";
        if (tkl > 0)
            token = Encoding.ASCII.GetString(bytes, 4, tkl);
        if (recievedBytes > 4+tkl)
        {
            for(int i = 4+tkl; i < recievedBytes;)
            {
                if(bytes[i] == 255)
                {
                    payload = Encoding.ASCII.GetString(bytes, i + 1, recievedBytes - (i + 1));
                    break;
                }
                else
                {
                    string optionBits = Convert.ToString(bytes[i], 2).PadLeft(8, '0');
                    int optionDelta = Convert.ToInt32(optionBits.Substring(0, 4), 2);
                    int optionLength = Convert.ToInt32(optionBits.Substring(4, 4), 2);
                    string opt = Coap.OptionType(optionDelta) + ": " + Encoding.ASCII.GetString(bytes, i + 1, optionLength);
                    option.Add(opt);
                    i += 1 + optionLength;
                }
            }
            
            
        }

        Console.WriteLine(token);

        string newResponse = "Version: " + version +
            "\nType: " + type +
            "\nToken Length: " + tkl +
            "\nCode: " + code +
            "\nMessage ID: " + messageID +
            "\nToken: " + token +
            "\nOptions: " + String.Join(", ", option) +
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

                byte[] msg = { 0x40, 0x02, 0x04, 0xd2, 0xb4, 0x74, 0x65, 0x73, 0x74 };
                int sentBytes = sender.Send(msg);
                int recievedBytes = sender.Receive(bytes);
                //Console.WriteLine("Response = "+ Encoding.ASCII.GetString(bytes, 0, recievedBytes));

                //Console.WriteLine("Hex: " + Convert.ToString(bytes[14], 16));
   
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

