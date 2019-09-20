using System;
using System.Windows.Forms;
using System.IO.Ports;

namespace SerialCan
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private SerialPort sp;
        private void Button1_Click(object sender, EventArgs e)
        {
            sp = new SerialPort(comboBox1.Text)
            {
                BaudRate = Convert.ToInt32(comboBox2.Text),
                Parity = ParseEnum<Parity>(comboBox5.Text),
                StopBits = ParseEnum<StopBits>(comboBox4.Text),
                DataBits = Convert.ToInt32(comboBox3.Text),
                Handshake = Handshake.None
            };
            sp.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
            sp.Disposed += Disconnected;
            sp.PinChanged += Disconnected;
            sp.ErrorReceived += Disconnected;
            sp.Open();
            button1.Enabled = false;
            button4.Enabled = true;
        }
        private void Disconnected(object sender, EventArgs e)
        {
            button1.Enabled = true;
            button4.Enabled = false;
        }
        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string indata = sp.ReadExisting();
            listBox1.Items.Insert(0, indata.Replace(Environment.NewLine, " "));
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            button4.Enabled = false;
            foreach (var item in groupBox1.Controls)
                if (item is LinkLabel lnk)
                {
                    var i = lnk.Name.Replace("linkLabel", "");
                    lnk.Text = (i.ToString().Length == 1 ? "0" : "") + i;
                    lnk.LinkClicked += LinkClicked;
                }
            comboBox1.Items.AddRange(SerialPort.GetPortNames());
            if (comboBox1.Items.Count > 0)
                comboBox1.SelectedIndex = 0;
            else
                MessageBox.Show("Comport yok.");
            comboBox2.SelectedIndex = 3;
            comboBox3.SelectedIndex = 3;
            comboBox4.SelectedIndex = 0;
            comboBox5.SelectedIndex = 0;
        }
        private void Button2_Click(object sender, EventArgs e)
        {
            if (sp != null && sp.IsOpen)
                sp.Write("#" + textBox1.Text + "\r");
            else
                MessageBox.Show("Port disconnected");
        }
        private void LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (sp != null && sp.IsOpen)
            {
                var self = (LinkLabel)sender;
                sp.Write("#" + self.Text + "0\r");
            }
            else
                MessageBox.Show("Port disconnected");
        }
        private void Button4_Click(object sender, EventArgs e)
        {
            if (sp.IsOpen && sp != null)
                sp.Close();
        }
        public T ParseEnum<T>(string value) => (T)Enum.Parse(typeof(T), value, true);
        private void Clear(object sender, EventArgs e) => listBox1.Items.Clear();
    }
}