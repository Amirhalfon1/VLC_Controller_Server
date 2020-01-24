using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace VLC_Controller
{
    class Program
    {
        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);
        [DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(String lpClassName, String lpWindowName);

        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);


        const int VK_UP = 0x26; //up key
        const int VK_DOWN = 0x28;  //down key
        const int VK_LEFT = 0x25;
        const int VK_RIGHT = 0x27;
        const int VK_SPACE = 0x20;
        const uint KEYEVENTF_KEYUP = 0x0002;
        const uint KEYEVENTF_EXTENDEDKEY = 0x0001;

        static void Main(string[] args)
        {
            byte[] data = new byte[1024];
            UdpClient newsock = null;
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 9050);
            try
            {
                newsock = new UdpClient(ipep);
            }catch(Exception e)
            {
                return;
            }
            Console.WriteLine("Waiting for a client...");
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);

            while (true)
            {
                data = newsock.Receive(ref sender);
                Process p = Process.GetProcessesByName("vlc").FirstOrDefault();
                if (p != null)
                {
                    IntPtr h = p.MainWindowHandle;
                    SetForegroundWindow(h);
                    //Console.WriteLine("Now Playing: " + p.MainWindowTitle.Split('.')[0]);
                }
                //Console.WriteLine(Encoding.ASCII.GetString(data, 0, data.Length));
                if (data[0] == 0x1)
                {
                    keybd_event((byte)VK_SPACE, 0, KEYEVENTF_EXTENDEDKEY | 0, 0);
                    Console.WriteLine("Play/Pause");
                }
                else if (data[0] == 0x2)
                {
                    keybd_event((byte)VK_UP, 0, KEYEVENTF_EXTENDEDKEY | 0, 0);
                    Console.WriteLine("Volume UP");
                }
                else if (data[0] == 0x3)
                {
                    keybd_event((byte)VK_DOWN, 0, KEYEVENTF_EXTENDEDKEY | 0, 0);
                    Console.WriteLine("Volume DOWN");
                }
                else if (data[0] == 0x4)
                {
                    keybd_event((byte)VK_RIGHT, 0, KEYEVENTF_EXTENDEDKEY | 0, 0);
                    Console.WriteLine("Forward");
                }
                else if (data[0] == 0x5)
                {
                    keybd_event((byte)VK_LEFT, 0, KEYEVENTF_EXTENDEDKEY | 0, 0);
                    Console.WriteLine("Backward");
                }
            }
        }
    }
}
