using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ControlTest01
{

    public partial class Form1 : Form
    {

        const int NUM_DATA = 200;
        int dataCounter = 0;
        int[] dataPoints;
        int[] targetPoints;
        const int WAIT_HEADER = 0;
        const int WAIT_FOOTER = 1;

        public Form1()
        {
            InitializeComponent();
            ControlTest01.Data d = new ControlTest01.Data();
            d.Value = 2000;
            this.bindingSource1.DataSource = d;// = 2000; ;
            d = new ControlTest01.Data();
            d.Value = 20;
            this.bindingSource2.DataSource = d; ;
            d = new ControlTest01.Data();
            d.Value = 9999;
            this.bindingSource3.DataSource = d; ;
            d = new ControlTest01.Data();
            d.Value = 0;
            this.bindingSource4.DataSource = d; ;

            dataPoints = new int[NUM_DATA];
            targetPoints = new int[NUM_DATA];
            for (int i = 0; i < NUM_DATA; i++)
            {
                dataPoints[i] = 2000;
                targetPoints[i] = 2000;
            }

            timer2.Enabled = true;
        }

        private void write(string ss)
        {
            byte[] b = new byte[1];
            for (int i = 0; i < ss.Length; i++)
            {
                b[0] = (byte)ss[i];
                serialPort1.Write(b, 0, 1);
                System.Threading.Thread.Sleep(10);
            }

        }

        private string read()
        {
            string buf = "";
            while (true)
            {
                char c = (char)serialPort1.ReadChar();

                if (read_mode == WAIT_HEADER)
                {
                    if (c == '$')
                    {
                        read_mode = WAIT_FOOTER;
                    }
                }
                else
                {
                    if (c == '#')
                    {
                        read_mode = WAIT_HEADER;
                        return buf;
                    }
                    else
                    {
                        buf += c.ToString();
                    }
                }
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            lock (serialPort1)
            {
            }
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            this.serialPort1.Open();
            timer1.Enabled = true;
            timer2.Enabled = true;
        }

        int target = 2000;
        int read_mode = WAIT_HEADER;

        private void timer1_Tick(object sender, EventArgs e)
        {
            lock (serialPort1)
            {

                write("$set" + trackBar1.Value.ToString("D4") + "#");
                string retVal = read();
                if (!retVal.Equals("set"))
                {
                    MessageBox.Show("FAILED: " + retVal);
                    serialPort1.DiscardInBuffer();
                }

                write("$get#");
                string curStr = read();
                int dat = Int32.Parse(curStr);
                textBox1.Text = dat.ToString();
                dataPoints[dataCounter] = dat;
                targetPoints[dataCounter] = trackBar1.Value;
                dataCounter++;
                if (dataCounter >= NUM_DATA)
                {
                    dataCounter = 0;

                }
            }

        }

        private void showGraph()
        {
            Graphics g = pictureBox1.CreateGraphics();
            int height = pictureBox1.Height;
            int width = pictureBox1.Width;
            g.FillRectangle(Brushes.White, 0, 0, width, height);

            double data2pixel = (double)height / 4096;
            double time2pixel = (double)width / NUM_DATA;

            Pen pen = new Pen(Color.Red);
            for (int j = 0; j < NUM_DATA-1; j++)
            {
                int i = dataCounter + j;
                if (i >= NUM_DATA) i -= NUM_DATA;
                Point p1 = new Point((int)(time2pixel * j), (int)(data2pixel * (4096 - dataPoints[i])));
                i++;
                if (i >= NUM_DATA) i -= NUM_DATA;
                Point p2 = new Point((int)(time2pixel * (j+1)), (int)(data2pixel * (4096 - dataPoints[i])));
                g.DrawLine(pen, p1, p2);
            }

            pen = new Pen(Color.Black);
            for (int j = 0; j < NUM_DATA - 1; j++)
            {
                int i = dataCounter + j;
                if (i >= NUM_DATA) i -= NUM_DATA;
                Point p1 = new Point((int)(time2pixel * j), (int)(data2pixel * (4096 - targetPoints[i])));
                i++;
                if (i >= NUM_DATA) i -= NUM_DATA;
                Point p2 = new Point((int)(time2pixel * (j + 1)), (int)(data2pixel * (4096 - targetPoints[i])));
                g.DrawLine(pen, p1, p2);
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {

            showGraph();
        }


        private void trackBar2_Scroll(object sender, EventArgs e)
        {

        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {

        }

        private void trackBar4_Scroll(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            write("$kp" + trackBar2.Value.ToString("D4") + "#");
            string retVal = read();
            if (!retVal.Equals("kp"))
            {
                MessageBox.Show("FAILED: " + retVal);
                serialPort1.DiscardInBuffer();
            }

            write("$ti" + trackBar3.Value.ToString("D4") + "#");
            retVal = read();
            if (!retVal.Equals("ti"))
            {
                MessageBox.Show("FAILED: " + retVal);
                serialPort1.DiscardInBuffer();
            }

            write("$td" + trackBar4.Value.ToString("D4") + "#");
            retVal = read();
            if (!retVal.Equals("td"))
            {
                MessageBox.Show("FAILED: " + retVal);
                serialPort1.DiscardInBuffer();
            }
        }
    }

}
