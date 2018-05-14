using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using 跳一跳外挂;
using System.Runtime.InteropServices;


namespace 挑一挑外挂
{
    public partial class Form1 : Form
    {
        int xStart = 0;
        int yStart = 0;
        int xEnd = 0;
        int yEnd = 0;
        bool isDoJump = false;
        private readonly int MOUSEEVENTF_LEFTDOWN = 0x0002;//模拟鼠标左键按下
        private readonly int MOUSEEVENTF_LEFTUP = 0x0004;//模拟鼠标左键抬起
        private readonly int MOUSEEVENTF_ABSOLUTE = 0x8000;//鼠标绝对位置       
        KeyboardHook k_hook;
        [DllImport("user32")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);
        public Form1()
        {
            InitializeComponent();
            k_hook = new KeyboardHook();
            //k_hook.KeyDownEvent += new System.Windows.Forms.KeyEventHandler(hook_KeyDown);//钩住键按下 
            k_hook.KeyPressEvent += K_hook_KeyPressEvent;
            k_hook.KeyDownEvent += K_hook_KeyDownEvent;
            k_hook.Start();//安装键盘钩子
            timer1.Tick += Timer1_Tick;
            this.Load += Form1_Load;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 1;
            comboBox2.SelectedIndex = 1;
        }

        private void K_hook_KeyDownEvent(object sender, KeyEventArgs e)
        {
            //System.Windows.Forms.MessageBox.Show(e.KeyValue.ToString()+":"+ comboBox1.SelectedIndex);
            //return;
            int startKey = 65;
            int endKey = 83;
            //开始：q: 81 a: 65 z: 90  ctrl:162: 数字1:49  
            int key = comboBox1.SelectedIndex;
            switch (key)
            {
                case 0:
                    startKey = 81;
                    break;
                case 1:
                    startKey = 65;
                    break;
                case 2:
                    startKey = 90;
                    break;
                case 3:
                    startKey = 162;
                    break;
                case 4:
                    startKey = 49;
                    break;
                default:
                    startKey = 65;
                    break;
            }
            //结束：w: 87 s: 83 x: 88  alt:164: 数字2:50
            key = comboBox2.SelectedIndex;
            switch (key)
            {
                case 0:
                    endKey = 87;
                    break;
                case 1:
                    endKey = 83;
                    break;
                case 2:
                    endKey = 88;
                    break;
                case 3:
                    endKey = 164;
                    break;
                case 4:
                    endKey = 50;
                    break;
                default:
                    endKey = 83;
                    break;
            }
            if (e.KeyValue == startKey)
            {               
                DoGetStartPi();
                isDoJump = true;
            }
            if (e.KeyValue == endKey)
            {
                if (isDoJump)
                {
                    DoGetEndPi();
                    int time = 0;
                    int timeConstant = 0;
                    double pi = Math.Sqrt((xStart - xEnd) * (xStart - xEnd) + (yStart - yEnd) * (yStart - yEnd));
                    try
                    {
                        timeConstant = Convert.ToInt32(textBox1.Text);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("时间常数必须是十进制整数");
                        return;
                    }                                     
                    time = Convert.ToInt32(pi * timeConstant/100);
                    Console.WriteLine(xStart + ":" + yStart + "," + xEnd + ":" + yEnd + "," + time);
                    if (time <= 0)
                    {
                        time = 100;
                    }
                    DoJump(time);
                    isDoJump = false;
                }              
            }
        }

        private void DoGetStartPi()
        {
            xStart =  Control.MousePosition.X;
            yStart = Control.MousePosition.Y;
        }

        private void DoGetEndPi()
        {
            xEnd = Control.MousePosition.X;
            yEnd = Control.MousePosition.Y;
        }

        private void DoJump(int time)
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_ABSOLUTE, xEnd, yEnd, 0, 0);//点击
            timer1.Enabled = true;
            timer1.Interval = time;
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            mouse_event(MOUSEEVENTF_LEFTUP | MOUSEEVENTF_ABSOLUTE, xEnd, yEnd, 0, 0);//抬起
        }


        private void K_hook_KeyPressEvent(object sender, KeyPressEventArgs e)
        {
            //tb1.Text += e.KeyChar;
            //if (e.KeyChar==97)
            //{
            //    x = Control.MousePosition.X;
            //    y = Control.MousePosition.Y;
            //    mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_ABSOLUTE, x, y, 0, 0);//点击
            //    timer1.Interval = 1000;
            //    timer1.Enabled = true;
            //    //System.Windows.Forms.MessageBox.Show(x+":"+y);
            //}
            
        }

        //private void hook_KeyDown(object sender, KeyEventArgs e)
        //{
            
        //    label1.Text += (char)e.KeyData;


        //    //判断按下的键（Alt + A） 
        //    //if (e.KeyValue == (int)Keys.A && (int)System.Windows.Forms.Control.ModifierKeys == (int)Keys.Alt)
        //    //{
        //    //    System.Windows.Forms.MessageBox.Show("ddd");
        //    //}
        //}

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            k_hook.Stop();
        }

    }
}
