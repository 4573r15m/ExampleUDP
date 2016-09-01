using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace ExampleUDP
{
    class Program
    {
        static void Main(string[] args)
        {
            Socket udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint myAddress = new IPEndPoint(IPAddress.Any, 3502);
            EndPoint ClientAddress = new IPEndPoint(IPAddress.None, 3502);
            EncodePacket encodePacket = new EncodePacket();

            udpSocket.Bind(myAddress);

            byte[] inputBuffer = new byte[128];
            byte[] outputBuffer;

            while (true)
            {
                udpSocket.ReceiveFrom(inputBuffer, 0, inputBuffer.Length, SocketFlags.None, ref ClientAddress);
                Packet input = encodePacket.Decode(inputBuffer);
                Packet output = new Packet();

                switch (input.Command)
                {


                    case 0x00:
                        output.Command = 0x00;
                        output.Text = input.Text;
                        break;
                    case 0x01:
                        output.Command = 0x01;
                        output.Text = Convert.ToString(DateTime.Today);
                        break;
                    case 0x02:
                        output.Command = 0x01;
                        output.Text = "Ang?";
                        break;
                }
                outputBuffer = encodePacket.Encode(output);
                udpSocket.SendTo(outputBuffer, ClientAddress);
            }
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
