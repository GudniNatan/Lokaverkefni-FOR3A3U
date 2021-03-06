﻿using System;
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
        public int questionAmount = 0;
        private List<string> questions;

        private void ServerForm_Load(object sender, EventArgs e)
        {
            players = new Player[2];
            playerThreads = new Thread[2];
            currentPlayer = 0;
            playerDone[0] = false;
            playerDone[1] = false;
            playerScore[0] = 0;
            playerScore[1] = 0;
            questions = ReadFile("questions.txt");
            questions.Shuffle();
            questionAmount = questions.Count;

            NextQuestion();

            getPlayers = new Thread(new ThreadStart(SetUp));
            getPlayers.Start();


        }
        public void NextQuestion()
        {
            if (questionNumber < questionAmount)
            {
                string question = null;
                currentQuery = questions[questionNumber];
                string[] splitquery = currentQuery.Split('|');
                currentAnswer = splitquery[splitquery.Length - 1];
                for (int i = 0; i < splitquery.Length - 1; i++)
                {
                    question += splitquery[i] + '|';
                }
                currentQuestion = question.Substring(0, question.Length - 1);
            }

            
            questionNumber++;
        }
        public void LiftLock(int player)
        {
            if (players[player] != null)
            {
                lock (players[player])
                {
                    players[player].threadSuspended = false;
                    Monitor.Pulse(players[player]);
                }
            }
        }
        public void Message(string msg)
        {
            foreach (var player in players)
            {
                if (player != null)
                {
                    player.Message(msg);
                }
            }
        }
        public void EndGame()
        {
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i] != null && players[i].connection.Connected)
                {
                    players[i].Disconnect();
                    LiftLock(i);                    
                }
            }
            getPlayers.Abort();
            players = new Player[2];
            playerThreads = new Thread[2];
            currentPlayer = 0;
            playerDone[0] = false;
            playerDone[1] = false;
            playerScore[0] = 0;
            playerScore[1] = 0;
            questionNumber = 0;
            listener.Stop();

            getPlayers = new Thread(new ThreadStart(SetUp));
            getPlayers.Start();
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
        public List<string> ReadFile(string filename)
        {
            List<string> list = new List<string>();
            try
            {
                using (StreamReader reader = new StreamReader(filename, Encoding.Default, true))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        list.Add(line); // Add to list.
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayMessage("Exception: " + ex);
            }

            return list;
        }

        private void ServerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            disconnected = true;
            if (players[0] != null && players[1] != null && players[0].connection.Connected && players[1].connection.Connected)
                Message("disconnect");
            System.Environment.Exit(System.Environment.ExitCode);
        }

        private void displayTextBox_TextChanged(object sender, EventArgs e)
        {
            displayTextBox.SelectionStart = displayTextBox.Text.Length;
            displayTextBox.ScrollToCaret();
        }

        
    }
    static class MyExtensions
    {
        private static Random rng = new Random();

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
