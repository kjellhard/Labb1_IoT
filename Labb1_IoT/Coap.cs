using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Coap
{
    public enum MType
    {
        Confirmable = 0,
        NonConfirmable = 1,
        Acknowledgement = 2,
        Reset = 3
    }
    public enum Method
    {
        Empty = 0,
        Get = 1,
        Post = 2,
        Put = 3,
        Delete = 4
    }
    public struct Message
    {
        MType type;
        Method method;
        int id;
        byte[] tokens;

        byte[] payload;
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
    public static string OptionType(int optionDelta)
    {
        switch (optionDelta)
        {
            case 1:
                return "If-Match";
            case 3:
                return "Uri-Tag";
            case 4:
                return "ETag";
            case 5:
                return "If-None-Match";
            case 7:
                return "Uri-Port";
            case 8:
                return "Location-Path";
            case 11:
                return "Uri-Path";
            case 12:
                return "Content-Format";
            case 14:
                return "Max-Age";
            case 15:
                return "Uri-query";
            case 17:
                return "Accept";
            case 20:
                return "Location-Query";
            case 28:
                return "Size2";
            case 35:
                return "Proxy-Uri";
            case 39:
                return "Proxy-Scheme";
            case 60:
                return "Size1";
            default:
                return "Option Type Not Specified";
        }
    }
    public string ContentFormat(int id)
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
}
