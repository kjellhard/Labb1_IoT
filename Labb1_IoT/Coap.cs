using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

public class Coap
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

    public enum MType : UInt32
    {
        Confirmable = 0,
        NonConfirmable = 1,
        Acknowledgement = 2,
        Reset = 3
    }
    public enum Method : UInt32
    {
        Empty = 0,
        Get = 1,
        Post = 2,
        Put = 3,
        Delete = 4
    }
    public enum OptionType : UInt32
    {
        IfMatch = 1,
        UriHost = 3,
        ETag = 4,
        IfNoneMatch = 5,
        UriPort = 7,
        LocationPath = 8,
        UriPath = 11,
        ContentFormat = 12,
        MaxAge = 14,
        UriQuery = 15,
        Accept = 17,
        LocationQuery = 20,
        Size2 = 28,
        ProxyUri = 35,
        ProxyScheme = 39,
        Size1 = 60
    }
    public struct Message
    {
        public int version = 1;
        public MType type = new();
        public Method method = new();
        public UInt16 id = new();
        public byte[] tokens = new byte[0];
        public byte[] options = new byte[0];
        public byte[] payload = new byte[0];

        public void AddOption(OptionType optionType, string optionValue)
        {
            byte type = (byte)((uint)optionType << 4);
            int i  = options.Length;
            byte[] newOption = Encoding.ASCII.GetBytes(optionValue);
            if (optionType == OptionType.UriPort || optionType == OptionType.ContentFormat || optionType == OptionType.MaxAge 
                || optionType == OptionType.Accept || optionType == OptionType.Size2 || optionType == OptionType.Size1)
            {
                uint optInt = Convert.ToUInt32(optionValue);
                newOption = BitConverter.GetBytes(optInt);
            }
            type += Convert.ToByte(newOption.Length);
            Array.Resize(ref options, i + 1 + newOption.Length);
            options[i] = type;
            Array.Copy(newOption, 0, options, i+1, newOption.Length);
        }

        public void SetPayload(string pload)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(pload);
            Array.Resize(ref payload, 1 + bytes.Length);
            payload[0] = 255;
            Array.Copy(bytes, 0, payload, 1, bytes.Length);
        }

    }


    public static string MessageType(int type)
    {
        switch (type)
        {
            case 0:
                return "CONfirmable";
            case 1:
                return "NON-Confirmable";
            case 2:
                return "ACKnowledgement";
            case 3:
                return "ReSeT";
            default:
                return "UNKNOWN RESPONSE MESSAGE TYPE";
        }
    }
    public static string CoapResponseCode(int code)
    {
        switch (code)
        {
            case 65:
                return"201: Created";
            case 66:
                return "202: Deleted";
            case 67:
                return "203: Valid";
            case 68:
                return "204: Changed";
            case 69:
                return "205: Content";
            case 95:
                return "231: Continue";
            case 128:
                return "400: Bad Request";
            case 129:
                return "401: Unauthorized";
            case 130:
                return "402: Bad Option";
            case 131:
                return "403: Forbidden";
            case 132:
                return "404: Not Found";
            case 133:
                return "405: Method Not Allowed";
            case 134:
                return "406: Not Acceptable";
            case 136:
                return "408: Request Entity Incomplete";
            case 140:
                return "412: Precondition Failed";
            case 141:
                return "413: Request Entity Too Large";
            case 143:
                return "415: Unsupported Content-Format";
            case 160:
                return "500: Internal Server Error";
            case 161:
                return "501: Not Implemented";
            case 162:
                return "502: Bad Gateway";
            case 163:
                return "503: Service Unavailable";
            case 164:
                return "504: Gateway Timeout";
            case 165:
                return "505: Proxying Not Supported";
            default:
                return "UNKNOWN REPSONSE CODE";
        }
    }
   
    public static string ContentFormat(int id)
    {
        switch(id)
        {
            case 0:
                return "text/plain;charset=utf-8";
            case 40:
                return "application/link-format";
            case 41:
                return "application/xml";
            case 42:
                return "application/octet-stream";
            case 47:
                return "application/exi";
            case 50:
                return "application/json";
            case 60:
                return "application/cbor";
            default:
                return "Unknown Content-Format";
        }
    }
    public static string GetOption(int optionDelta, byte[] bytes)
    {
        switch (optionDelta)
        {
            case 1:
                return "If-Match: " + Encoding.ASCII.GetString(bytes, 0, bytes.Length);
            case 3:
                return "Uri-Host: " + Encoding.ASCII.GetString(bytes, 0, bytes.Length);
            case 4:
                return "ETag+ " + Encoding.ASCII.GetString(bytes, 0, bytes.Length);
            case 5:
                return "If-None-Match";
            case 7:
                return "Uri-Port: " + BitConverter.ToUInt16(bytes);
            case 8:
                return "Location-Path: " + Encoding.ASCII.GetString(bytes, 0, bytes.Length);
            case 11:
                return "Uri-Path: " + Encoding.ASCII.GetString(bytes, 0, bytes.Length);
            case 12:
                return "Content-Format: " + ContentFormat(BitConverter.ToUInt16(bytes));
            case 14:
                return "Max-Age: " + BitConverter.ToUInt32(bytes);
            case 15:
                return "Uri-query: " + Encoding.ASCII.GetString(bytes, 0, bytes.Length);
            case 17:
                return "Accept: " + BitConverter.ToUInt16(bytes);
            case 20:
                return "Location-Query: " + Encoding.ASCII.GetString(bytes, 0, bytes.Length);
            case 28:
                return "Size2: " + BitConverter.ToUInt32(bytes);
            case 35:
                return "Proxy-Uri: " + Encoding.ASCII.GetString(bytes, 0, bytes.Length);
            case 39:
                return "Proxy-Scheme: " + Encoding.ASCII.GetString(bytes, 0, bytes.Length);
            case 60:
                return "Size1: " + BitConverter.ToUInt32(bytes);
            default:
                return "Option Type Not Specified";
        }
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
        if (recievedBytes > 4 + tkl)
        {
            int optionDelta = 0;
            for (int i = 4 + tkl; i < recievedBytes;)
            {
                if (bytes[i] == 255)
                {
                    payload = Encoding.ASCII.GetString(bytes, i + 1, recievedBytes - (i + 1));
                    break;
                }
                else
                {
                    string optionBits = Convert.ToString(bytes[i], 2).PadLeft(8, '0');
                    optionDelta += Convert.ToInt32(optionBits.Substring(0, 4), 2);
                    int optionLength = Convert.ToInt32(optionBits.Substring(4, 4), 2);
                    byte[] optBytes = new byte[optionLength];
                    if (optionLength < 4)
                        optBytes = new byte[4];
                    Array.Copy(bytes, i + 1, optBytes, 0, optionLength);
                    string opt = Coap.GetOption(optionDelta, optBytes);// + ": " + Encoding.ASCII.GetString(bytes, i + 1, optionLength);
                    option.Add(opt);
                    i += 1 + optionLength;
                }
            }


        }


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
    public static string SendMessage(Message message, Socket socket)
    {
        byte header = (byte)((uint)message.version << 6);
        header += (byte)((uint)message.type << 4);
        header += (byte)message.tokens.Length;
        byte[] id = BitConverter.GetBytes(message.id);
        Array.Reverse(id);
        byte[] msg = new byte[32 + message.tokens.Length + message.options.Length + message.payload.Length];
        msg[0] = header;
        msg[1] = (byte)message.method;
        Array.Copy(id, 0, msg, 2, id.Length);
        Array.Copy(message.tokens, 0, msg, 4, message.tokens.Length);
        Array.Copy(message.options, 0, msg, 4 + message.tokens.Length, message.options.Length);
        Array.Copy(message.payload, 0, msg, 4 + message.tokens.Length + message.options.Length, message.payload.Length);
        socket.Send(msg);
        byte[] response = new byte[1024];
        int recievedBytes = socket.Receive(response);
        return ParseResponse(response, recievedBytes);
    }
}
