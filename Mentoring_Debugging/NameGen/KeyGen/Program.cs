using System;
using System.Linq;
using System.Net.NetworkInformation;

namespace KeyGen
{
    public class Program
    {
        static void Main(string[] args)
        {
            var addressBytes = NetworkInterface.GetAllNetworkInterfaces().First().GetPhysicalAddress().GetAddressBytes();

            var dateBytes = BitConverter.GetBytes(DateTime.Now.Date.ToBinary());

            var array = addressBytes.Select((byteValue, index) => byteValue ^ dateBytes[index]).Select(value =>
            {
                if (value <= 999) {
                    return value * 10;
                }
                return value;
            }).ToArray();

            var mainKey = array.Select(item => Convert.ToString(item));

            var key = string.Join("-", mainKey);

            Console.WriteLine(key);
        }
    }
}
