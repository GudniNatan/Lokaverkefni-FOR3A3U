using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace Lokaverkefni_Johann_Gudni_Server
{
    public partial class ServerForm : Form
    {
        public ServerForm()
        {
            InitializeComponent();
        }
        private byte[] board; // the local representation of the game board  
        private Player[] players; // two Player objects                      
        private Thread[] playerThreads; // Threads for client interaction    
        private TcpListener listener; // listen for client connection        
        private int currentPlayer; // keep track of whose turn it is         
        private Thread getPlayers; // Thread for acquiring client connections
        internal bool disconnected = false; // true if the server closes     
        public string currentQuery = null;
        public string currentAnswer = null;
        public string currentQuestion = null;
        public int questionNumber = 0;
        public bool[] playerDone = new bool[2];
        public int[] playerScore = new int[2];
        public int lastPlayer = 0;
        public int questionAmount = 3;

        private void ServerForm_Load(object sender, EventArgs e)
        {
            players = new Player[2];
            playerThreads = new Thread[2];
            currentPlayer = 0;
            playerDone[0] = false;
            playerDone[1] = false;
            playerScore[0] = 0;
            playerScore[1] = 0;
            NextQuestion();

            getPlayers = new Thread(new ThreadStart(SetUp));
            getPlayers.Start();
        }
        public void NextQuestion()
        {
            string question = null;
            string[] questions = new string[] { "What is 5 + 5?|1|3|10|20|30|10", "What year is it?|0|2016", "Ekki er allt...|2|...sem glóir|gull" };
            currentQuery = questions[questionNumber];
            string[] splitquery = currentQuery.Split('|');
            currentAnswer = splitquery[splitquery.Length - 1];
            for (int i = 0; i < splitquery.Length - 1; i++)
            {
                question += splitquery[i] + '|';
            }  
            questionNumber++;
            currentQuestion = question.Substring(0,question.Length-1);
        }
        public void LiftLock(int player)
        {
            lock (players[player])
            {
                players[player].threadSuspended = false;
                Monitor.Pulse(players[player]);
            }  
        }
        public void Message(string msg)
        {
            foreach (var player in players)
            {
                player.Message(msg);
            }
        }

        private delegate void DisplayDelegate(string message);

        internal void DisplayMessage(string message)
        {
            if (displayTextBox.InvokeRequired)
            {
                Invoke(new DisplayDelegate(DisplayMessage),
                   new object[] { message });
            }
            else
                displayTextBox.Text += message + "\n";
        }
        public void SetUp()
        {
            DisplayMessage("Waiting for players...\r\n");

            listener =
               new TcpListener(IPAddress.Any, 50000);
            listener.Start();

            players[0] = new Player(listener.AcceptSocket(), this, 0);
            playerThreads[0] =
               new Thread(new ThreadStart(players[0].Run));
            playerThreads[0].Start();

            players[1] = new Player(listener.AcceptSocket(), this, 1);
            playerThreads[1] =
               new Thread(new ThreadStart(players[1].Run));
            playerThreads[1].Start();

            lock (players[0])
            {
                players[0].threadSuspended = false;
                Monitor.Pulse(players[0]);
            }                              
        }

        private void ServerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            disconnected = true;
            Message("disconnect");
            System.Environment.Exit(System.Environment.ExitCode);
        }

        
    }
}
