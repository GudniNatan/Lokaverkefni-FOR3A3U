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
        private Label lb_part1,lb_part2;
        private bool done = false;
        private TextBox tb_textGuess;
        private Label lb_question;
        private RadioButton[] rd_buttonGuess;
        private string current_question;
        private int score = 0;
        private string ipaddress;
        private int question_Type;
        

        private void ClientForm_Load(object sender, EventArgs e)
        {
            using(var ipForm = new chooseIP())
            {
                var result = ipForm.ShowDialog();

                if (result == DialogResult.OK)
                {
                    ipaddress = ipForm.ipaddress;

                }
                else
                {
                    ipaddress = "127.0.0.1";
                }
            }
            try
            {
                connection = new TcpClient(ipaddress, 50000);
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

        private void PlaceRadioButtons(int amount)
        {
            rd_buttonGuess = new RadioButton[amount];
            for (int i = 0; i < amount; i++)
            {
                rd_buttonGuess[i] = new RadioButton();
                rd_buttonGuess[i].Location = new Point(20, i * 20 + 50);
                Controls.Add(rd_buttonGuess[i]);
            }
        }
        private void PlaceTextBox()
        {
            tb_textGuess = new TextBox();
            tb_textGuess.Location = new Point(20, 70);
            Controls.Add(tb_textGuess);
        }
        private void PlaceQuestionLabel()
        {
            lb_question = new Label();
            lb_question.Location = new Point(20, 30);
            lb_question.AutoSize = true;
            Controls.Add(lb_question);
        }
        private void PlaceFillLabel(string part1, string part2)
        {
            lb_part1 = new Label();
            lb_part2 = new Label();
            lb_part1.Text = part1;
            lb_part2.Text = part2;
            lb_part1.Location = new Point(20, 70);
            lb_part2.Location = new Point(20, 130);
            lb_part1.AutoSize = true;
            lb_part2.AutoSize = true;
            tb_textGuess.Location = new Point(20, 100);

            Controls.Add(lb_part1);
            Controls.Add(tb_textGuess);
            Controls.Add(lb_part2);

        }

        
        
        private delegate void DisplayTextDelegate(string message);

        private void DisplayMessage(string message)
        {
            if (rtb_output.InvokeRequired)
            {
                Invoke(new DisplayTextDelegate(DisplayMessage),
                   new object[] { message });
            }
            else
                rtb_output.Text += message + "\n";
        }


        private delegate void DisplayLabelDelegate(string question);

        private void DisplayLabel(string question)
        {
            if (rtb_output.InvokeRequired)
            {
                Invoke(new DisplayLabelDelegate(DisplayLabel),
                   new object[] { question });
            }
            else
                lb_question.Text = question;
        }

        private delegate void DisplayRadioDelegate(string answer, int i);

        private void DisplayRadio(string answer, int i)
        {
            if (rd_buttonGuess[i].InvokeRequired)
            {
                Invoke(new DisplayRadioDelegate(DisplayRadio),
                   new object[] { answer, i});
            }
            else
                rd_buttonGuess[i].Text = answer;
        }

        private delegate void DisplayTextBoxDelegate(string answer, TextBox tb_answer);

        private void DisplayTextBox(string answer, TextBox tb_answer)
        {
            if (tb_answer.InvokeRequired)
            {
                Invoke(new DisplayTextBoxDelegate(DisplayTextBox),
                   new object[] { answer, tb_answer });
            }
            else
                tb_answer.Text = answer;
        }
        private void EnableSubmitButton()
        {
            bt_guess.Enabled = true;
        }
        private void AddScore()
        {
            score++;
            lb_score.Text = "Score: " + score;
        }

        public void ProcessMessage(string message)
        {
            if (message.Split('|').Length > 1)
            {
                //This is a Question
                current_question = message.Split('|')[0];
                this.Invoke((MethodInvoker)(() => EnableSubmitButton()));
                question_Type = Convert.ToInt32(message.Split('|')[1]);
                switch(question_Type)
                {
                    
                    case 0:
                        //Normal text answer
                        this.Invoke((MethodInvoker)(() => PlaceQuestionLabel()));
                        DisplayLabel(current_question);
                        this.Invoke((MethodInvoker)(() => PlaceTextBox()));

                        break;

                    case 1:
                        //Multiple choices
                        this.Invoke((MethodInvoker)(() => PlaceQuestionLabel()));
                        DisplayLabel(current_question);
                        int fields = message.Split('|').Length - 3;
                        this.Invoke((MethodInvoker)(() => PlaceRadioButtons(fields)));
                        for (int i = 3; i < message.Split('|').Length; i++)
                        {
                            string text = message.Split('|')[i];
                            DisplayRadio(text, i-3);
                        }
                        break;

                    case 2:
                        //Fill in the blank
                        this.Invoke((MethodInvoker)(() => PlaceQuestionLabel()));
                        string part1 = message.Split('|')[0];
                        string part2 = message.Split('|')[2];

                        DisplayLabel("Fylltu í eyðuna:");
                        this.Invoke((MethodInvoker)(() => PlaceFillLabel(part1, part2)));

                        break;
                }
            }
            else if (message == "win")
            {
                MessageBox.Show("You won with " + score +" points!");
                Environment.Exit(0);
            }
            else if (message == "lose")
            {
                MessageBox.Show("You lost with " + score + " points.");
                
                Environment.Exit(0);
            }
            else if (message == "tie")
            {
                MessageBox.Show("It's a tie. Both players had " + score + " points.");

                Environment.Exit(0);
            }
            else if (message == "correct")
            {
                DisplayMessage(message);
                this.Invoke((MethodInvoker)(() => AddScore()));
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

                connection.Close();
                stream.Close();
                writer.Close();
                reader.Close();
            }
            catch (IOException)
            {
                DisplayMessage("Server Is Down, Game Over!!");
                Environment.Exit(0);
            }
        }
        private void SubmitData()
        {
            bt_guess.Enabled = false;
            if (question_Type == 0)
            {
                writer.Write(tb_textGuess.Text.ToLower());
                tb_textGuess.Clear();
                Controls.Remove(tb_textGuess);
                Controls.Remove(lb_question);
            }
            else if (question_Type == 1)
            {
                Controls.Remove(lb_question);
                foreach (var rdb in rd_buttonGuess)
                {
                    if (rdb.Checked)
                    {
                        writer.Write(rdb.Text);
                    }
                    Controls.Remove(rdb);
                }
                rd_buttonGuess = null;
            }
            else if (question_Type == 2)
            {
                writer.Write(tb_textGuess.Text.ToLower());
                tb_textGuess.Clear();
                Controls.Remove(lb_part1);
                Controls.Remove(lb_part2);
                Controls.Remove(tb_textGuess);
                Controls.Remove(lb_question);
            }
        }
        

        private void bt_guess_Click_1(object sender, EventArgs e)
        {
            SubmitData();
        }

        private void ClientForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SubmitData();
            }
        }
        private void ClientForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            writer.Write("disconnect");
            stream.Close();
            connection.Close();
            writer.Close();
            reader.Close();
            stream.Dispose();
            System.Environment.Exit(System.Environment.ExitCode);
        }
    }
}
