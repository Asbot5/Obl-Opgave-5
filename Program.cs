using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.IO;
using OblFootballPlayer;
using System.Text.Json;
using System.Collections.Generic;

namespace FootballPlayerServer1
{
    class Program
    {
        private static int _nextId = 1;
        private static readonly List<FootballPlayer> PlayerList = new List<FootballPlayer>
        {
            new FootballPlayer {Id = _nextId++, Name = "Asmus", Price = 1000000, ShirtNumber = 5},
            new FootballPlayer {Id = _nextId++, Name = "Jens", Price = 100, ShirtNumber = 10},
            new FootballPlayer {Id = _nextId++, Name = "Peter", Price = 10000, ShirtNumber = 20},
            new FootballPlayer {Id = _nextId++, Name = "Børge", Price = 20, ShirtNumber = 30},
        };
        //{"name":"Niklas","Price":50,"ShirtNumber":35}
        static void Main(string[] args)
        {
            Console.WriteLine("Fodbold client");
            TcpListener listener = new TcpListener(2121);
            listener.Start();
            while (true)
            {
                //TcpClient socket = listener.AcceptTcpClient();
                //Console.WriteLine("New Client");

                Task.Run(() => HandleClient(listener));
            }
        }

        public static void HandleClient(TcpListener listener)
        {
            TcpClient socket = listener.AcceptTcpClient();
            Console.WriteLine("Ny client!");
            //Console.WriteLine("Skriv 'hentalle' for at vise alle spillere");
            //Console.WriteLine("Skriv 'hentid' for at vise en specifik spiller");
            //Console.WriteLine("Skriv 'gem' for at gemme en ny spiller");
            NetworkStream ns = socket.GetStream();
            StreamReader reader = new StreamReader(ns);
            StreamWriter writer = new StreamWriter(ns);

            writer.WriteLine("Skriv 'HentAlle' for at vise alle spillere");
            writer.WriteLine("Skriv 'HentId' for at vise en specifik spiller");
            writer.WriteLine("Skriv 'Gem' for at gemme en ny spiller");
            writer.WriteLine("Skriv 'Stop' for at klienten");
            writer.Flush();

            while (true)
            {
                string method = reader.ReadLine();

                if (method.ToLower() == "gem")
                {
                    writer.WriteLine("Indsæt spilleren du vil gemme i json format");
                    writer.Flush();
                    string value = reader.ReadLine();
                    FootballPlayer fromJson = JsonSerializer.Deserialize<FootballPlayer>(value);
                    fromJson.Id = _nextId++;
                    PlayerList.Add(fromJson);
                    writer.WriteLine("Spilleren er blevet gemt");
                    writer.Flush();
                }
                else if (method.ToLower() == "hentalle")
                {
                    writer.WriteLine("skriv en blank linje for at vise alle spillere");
                    writer.Flush();
                    string value = reader.ReadLine();
                    writer.WriteLine(GetAll(PlayerList));
                    writer.Flush();
                }
                else if (method.ToLower() == "hentid")
                {
                    writer.WriteLine("Skriv id på den spiller du gerne vil se");
                    writer.Flush();
                    string value = reader.ReadLine();
                    int id = Convert.ToInt32(value);
                    string JsonId = JsonSerializer.Serialize(GetById(id));
                    writer.WriteLine(JsonId);
                    writer.Flush();
                }
                if (method.ToLower() == "stop")
                {
                    socket.Close();
                    break;
                }
            }
        }

        private static string GetAll(List<FootballPlayer> players)
        {
            string player = "";
            foreach (FootballPlayer p in players)
            {
                player = player + JsonSerializer.Serialize(p) + "\n";

                //player = player + "ID: " + p.Id + ", Navn: " + p.Name + ", Pris: " + p.Price + ", Trøje nummer: " + p.ShirtNumber + "\n";
            }
            return player;
        }

        public static FootballPlayer GetById(int id)
        {
            return PlayerList.Find(player => player.Id == id);
        }
    }
}
