using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;

namespace Lokaverkefni_Johann_Gudni_Client
{
    public partial class ClientForm : Form
    {
        public ClientForm()
        {
            InitializeComponent();
        }
        private BinaryReader reader;
        private BinaryWriter writer;
        private Thread outputThread; 
        private TcpClient connection; 
        private NetworkStream stream;        
        private bool done = false;
        private TextBox tb_textGuess;
        private RadioButton[] rd_buttonGuess;

        private void ClientForm_Load(object sender, EventArgs e)
        {
            try
            {
                connection = new TcpClient("127.0.0.1", 50000);
                stream = connection.GetStream();
                writer = new BinaryWriter(stream);
                reader = new BinaryReader(stream);

                outputThread = new Thread(new ThreadStart(Run));
                outputThread.Start();
            }
            catch (Exception)
            {
                MessageBox.Show("Server Is Currently Down! Please Try Again Later.");
                Environment.Exit(0);
            }

        }

        
        private delegate void DisplayWordDelegate(string message);

        private void DisplayMessage(string message)
        {
            if (rtb_output.InvokeRequired)
            {
                Invoke(new DisplayWordDelegate(DisplayMessage),
                   new object[] { message });
            }
            else
                rtb_output.Text = message;
        }

        public void ProcessMessage(string message)
        {
            if (message == "Win")
            {
                MessageBox.Show("Takk Fyrir Leikinn");


                Environment.Exit(0);
            }
            else if (message.Split()[0] == "Lose")
            {
                MessageBox.Show("You lost, The correct word is " + message.Split()[1]);


                Environment.Exit(0);

            }
            else
            {
                DisplayMessage(message);
            }
      
               
        }

        public void Run()
        {

            try
            {
                while (!done)
                    ProcessMessage(reader.ReadString());
            }
            catch (IOException)
            {
                MessageBox.Show("Server Is Down, Game Over!!", "Error",
                   MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }
        }

        private void ClientForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                done = true;
                System.Environment.Exit(System.Environment.ExitCode);
            }
            catch (Exception)
            {

                Environment.Exit(0);
            }
            
        }

        private void bt_guess_Click_1(object sender, EventArgs e)
        {
            writer.Write(tb_textGuess.Text.ToLower());
            tb_textGuess.Clear(); 
        }

    }
}
