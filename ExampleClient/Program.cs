using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace ExampleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            UdpClient client = new UdpClient();
            IPEndPoint IP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3502);
            EncodePacket encodePacket = new EncodePacket();
            client.Connect(IP);

            Packet packet = new Packet();
            Packet gottenPacket = new Packet();

            packet.Command = 0x01;
            packet.Text = "Aaaaaaaaang???????????";

            byte[] buffer = new byte[128];
            byte[] gottenBuffer = new byte[128];

            buffer = encodePacket.Encode(packet);

            client.Send(buffer, buffer.Length);

            gottenBuffer = client.Receive(ref IP);
            gottenPacket = encodePacket.Decode(gottenBuffer);
            
            Console.WriteLine("Command : {0}\nData : {1}", gottenPacket.Command, gottenPacket.Text);
        }
    }
    struct Packet
    {
        public byte Command;
        public string Text;
    }
    class EncodePacket
    {
        public Packet Decode(byte[] buffer)
        {
            byte cmd = buffer[0];
            string txt = Encoding.UTF8.GetString(buffer, 1, buffer.Length - 1);

            return new Packet()
            {
                Command = cmd,
                Text = txt
            };
        }
        public byte[] Encode(Packet packet)
        {
            byte[] buffer = new byte[128];

            byte cmd = packet.Command;
            byte[] txt = Encoding.UTF8.GetBytes(packet.Text);

            buffer[0] = cmd;
            Array.Copy(txt, 0, buffer, 1, txt.Length);

            return buffer;
        }
    }
}
