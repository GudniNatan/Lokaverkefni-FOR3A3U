using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace Lokaverkefni_Johann_Gudni_Server
{
    class Player
    {
        internal Socket connection;
        private NetworkStream stream;          
        private ServerForm server;          
        private BinaryWriter writer;
        private BinaryReader reader; 
        private int number;               
        internal bool threadSuspended = true;

        public Player(Socket socket, ServerForm serverValue, int playerNumber)
        {
            connection = socket;
            server = serverValue;
            number = playerNumber;

            stream = new NetworkStream(connection);
            writer = new BinaryWriter(stream);
            reader = new BinaryReader(stream);
        }

        public void Run()
        {
            bool done = false;

            writer.Write("Connected");
            server.DisplayMessage("Player " + number + " connected.");

            if (number == 0)
            {
                writer.Write("Waiting for another player.");

                // wait for notification from server that another 
                // player has connected
                lock (this)
                {
                    while (threadSuspended)
                        Monitor.Wait(this);
                } // end lock               

                writer.Write("Other player connected.");
            } // end if
            if (number == 1)
            {
                writer.Write("Both players connected.");
            } // end if
            writer.Write(server.currentQuestion);
            // play game
            while (!done)
            {
                // wait for data to become available
                while (connection.Available == 0)
                {
                    Thread.Sleep(100);
                    if (server.disconnected)
                        return;
                } // end while
                // receive data
                string message = reader.ReadString();
                server.playerDone[number] = true;
                if (!server.playerDone[(number + 1) % 2])
                {
                    server.lastPlayer = (number + 1) % 2;
                    threadSuspended = true;
                    lock (this)
                    {
                        while (threadSuspended)
                            Monitor.Wait(this);
                    }
                }
                server.DisplayMessage(number + " done");
                ProcessMessage(message);
                server.DisplayMessage(number + " answered "+ message);
                if (number == server.lastPlayer)
                {
                    server.LiftLock((number + 1) % 2);
                    server.DisplayMessage("answer is: " + server.currentAnswer);
                    server.Message("answer is: " + server.currentAnswer);
                    Thread.Sleep(50);
                    if (server.questionNumber == server.questionAmount)
                    {
                        server.Message("Thanks for playing!");
                        server.Message("win");
                    }
                    server.NextQuestion();
                    server.playerDone[0] = false;
                    server.playerDone[1] = false;
                }
                else
                {
                    Thread.Sleep(100);
                }
                writer.Write(server.currentQuestion);
            } // end while loop 

            writer.Close();
            reader.Close();
            stream.Close();
            connection.Close();
        }
        void ProcessMessage(string message)
        {
            if (message == server.currentAnswer)
            {
                writer.Write("correct");
                server.playerScore[number] += 1;
            }
            else if (message == "disconnect")
            {

            }
            else
            {
                writer.Write("incorrect");
            }
        }
        public void Message(string message)
        {
            writer.Write(message);
        }

    }
}
