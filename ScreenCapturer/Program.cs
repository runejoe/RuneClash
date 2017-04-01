using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;

namespace ScreenCapturer
{
    public static class VirtualMouse
    {
        // import the necessary API function so .NET can
        // marshall parameters appropriately
        [DllImport("user32.dll")]
        static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        [DllImport("user32.dll")]
        static extern uint keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        private const int Z_Key_Code = 0x5A;

        // constants for the mouse_input() API function
        private const int MOUSEEVENTF_MOVE = 0x0001;
        private const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const int MOUSEEVENTF_LEFTUP = 0x0004;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
        private const int MOUSEEVENTF_RIGHTUP = 0x0010;
        private const int MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        private const int MOUSEEVENTF_MIDDLEUP = 0x0040;
        private const int MOUSEEVENTF_ABSOLUTE = 0x8000;


        // simulates movement of the mouse.  parameters specify changes
        // in relative position.  positive values indicate movement
        // right or down
        public static void Move(int xDelta, int yDelta)
        {
            mouse_event(MOUSEEVENTF_MOVE, xDelta, yDelta, 0, 0);
        }


        // simulates movement of the mouse.  parameters specify an
        // absolute location, with the top left corner being the
        // origin
        public static void MoveTo(int x, int y)
        {
            mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE, x, y, 0, 0);
        }


        // simulates a click-and-release action of the left mouse
        // button at its current position
        public static void LeftClick(int x, int y)
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN, x, y, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, x, y, 0, 0);
        }

        public static void PressZ()
        {
            keybd_event(Z_Key_Code, 0, 0, 0);
        }
    }

    public class MouseOperations
    {
        [Flags]
        public enum MouseEventFlags
        {
            LeftDown = 0x00000002,
            LeftUp = 0x00000004,
            MiddleDown = 0x00000020,
            MiddleUp = 0x00000040,
            Move = 0x00000001,
            Absolute = 0x00008000,
            RightDown = 0x00000008,
            RightUp = 0x00000010
        }

        [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out MousePoint lpMousePoint);

        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        public static void SetCursorPosition(int X, int Y)
        {
            SetCursorPos(X, Y);
        }

        public static void SetCursorPosition(MousePoint point)
        {
            SetCursorPos(point.X, point.Y);
        }

        public static MousePoint GetCursorPosition()
        {
            MousePoint currentMousePoint;
            var gotPoint = GetCursorPos(out currentMousePoint);
            if (!gotPoint) { currentMousePoint = new MousePoint(0, 0); }
            return currentMousePoint;
        }

        public static void MouseEvent(MouseEventFlags value)
        {
            MousePoint position = GetCursorPosition();

            mouse_event
                ((int)value,
                 position.X,
                 position.Y,
                 0,
                 0)
                ;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MousePoint
        {
            public int X;
            public int Y;

            public MousePoint(int x, int y)
            {
                X = x;
                Y = y;
            }

        }

    }
    class Program
    {
        static void captureScreenShot()
        {
            // Start the process... 
            //Console.WriteLine("Initializing the variables...");
            //Console.WriteLine();
            Bitmap memoryImage;
            memoryImage = new Bitmap(768, 1368);
            Size s = new Size(memoryImage.Width, memoryImage.Height);

            // Create graphics 
            //Console.WriteLine("Creating Graphics...");
            //Console.WriteLine();
            Graphics memoryGraphics = Graphics.FromImage(memoryImage);

            // Copy data from screen 
            //Console.WriteLine("Copying data from screen...");
            //Console.WriteLine();
            memoryGraphics.CopyFromScreen(1526, 427, 0, 0, s);

            //That's it! Save the image in the directory and this will work like charm. 
            string str = "";
            try
            {
                str = string.Format(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
                      @"\Screenshot.png");
            }
            catch (Exception er)
            {
                Console.WriteLine("Sorry, there was an error: " + er.Message);
                Console.WriteLine();
            }

            // Save it! 
            //Console.WriteLine("Saving the image...");
            
            memoryImage.Save(str);

            // Write the message, 
            //Console.WriteLine("Picture has been saved...");
            //Console.WriteLine(); 
        }

        static void captureScreenShotAndDoAction()
        {
            // Start the process... 
            //Console.WriteLine("Initializing the variables...");
            //Console.WriteLine();
            Bitmap memoryImage;
            memoryImage = new Bitmap(768, 1368);
            Size s = new Size(memoryImage.Width, memoryImage.Height);

            // Create graphics 
            //Console.WriteLine("Creating Graphics...");
            //Console.WriteLine();
            Graphics memoryGraphics = Graphics.FromImage(memoryImage);

            // Copy data from screen 
            //Console.WriteLine("Copying data from screen...");
            //Console.WriteLine();
            int xBase = 1470;
            int yBase = 380;
            memoryGraphics.CopyFromScreen(xBase, yBase, 0, 0, s);

            Color val = memoryImage.GetPixel(550, 1330);

            if (val.R > 200)
            {
                VirtualMouse.PressZ();
                VirtualMouse.MoveTo(373 + xBase, 715 + yBase);
                VirtualMouse.LeftClick(373 + xBase, 715 + yBase);
                Console.WriteLine("Action!");
                Thread.Sleep(200);
            }
            else
            {
                Console.WriteLine(val.R);
            }

            
        }

        static void Main(string[] args)
        {
            Thread.Sleep(1000);
            MouseOperations.SetCursorPosition(100, 0);
            MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftDown);
            MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftUp);

            //VirtualMouse.MoveTo(10*(373 + 1470), 20*(715+380));
            //VirtualMouse.LeftClick(0, 0);
            /*while(true)
            {
                captureScreenShotAndDoAction();
            }*/
        }
        /*static void Main(string[] args)
        {
            Stopwatch sw = Stopwatch.StartNew();
            int num = 1;
            Thread.Sleep(2000);
            for (int i = 0; i < num; i++)
            {
                captureScreenShot();
                if (i == 0)
                    sw.Restart();
            }
            long time = sw.ElapsedMilliseconds;
            Console.WriteLine("Took " + time + " ms to capture " + (num - 1)+ " screenshots");
        }*/
    }
}
