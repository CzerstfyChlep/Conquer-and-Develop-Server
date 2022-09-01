using System;
using System.Threading;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Configuration;
using System.Collections.Generic;

class EmployeeTCPServer
{
    static TcpListener listener;
   
    const int LIMIT = 20; //5 concurrent 1ents
    public static int[] Owners = new int[400];
    public static int[] Development = new int[400];
    public static int[] Fortifications = new int[400];
    public static int[] TradeValue = new int[400];
    public static bool[,] Trade = new bool[8, 8];
    public static bool[,] TradeAsk = new bool[8, 8];
    public static int[] Upgrades = new int[240];
    public static int[] PointsToAdd = new int[8];
    public static string[] Nicks = new string[8];
    public static int NumberOfPlayers;
    public static int CurrentNumber = 1;
    public static int PlayerTurnNumber = 1;
    public static  bool B = false;
    public static bool[] Updates;
    public static string MainChat ="";
    public static List<Unit> Units = new List<Unit>();
    public static List<BattleReport> BattleReports = new List<BattleReport>();
    public static int[] Techs = new int[1];
    public static void Main()
    {
        
        NumberOfPlayers = int.Parse(Console.ReadLine());
        Updates = new bool[NumberOfPlayers];
        for (int t = 0; t < NumberOfPlayers; t++)
        {
            Updates[t] = false;
        }
        listener = new TcpListener(2055);
        listener.Start();
        Random rand = new Random();
        for (int a = 0; a < 400; a++)
        {
            Owners[a] = 0;
            Development[a] = 0;
            Fortifications[a] = 0;
            TradeValue[a] = rand.Next(0, 3);
        }

        
        for(int k = 1; k <= NumberOfPlayers; k++)
        {
            losowanie:
            int r = rand.Next(0, 400);
            if (Owners[r] == 0)
            {
                Owners[r] = k;
            }
                
            else
                goto losowanie;

        }
        for (int k = 0; k < 40; k++)
        {
            losowanie:
            int r = rand.Next(0, 400);
            if (Owners[r] == 0)
                Owners[r] = -1;
            else
                goto losowanie;
        }
        for (int k = 0; k < 20; k++)
        {
            losowanie:
            int r = rand.Next(0, 400);
            if (Owners[r] == 0)
                Development[r]++;
            else
                goto losowanie;
        }
        for(int k = 0; k < 8; k++)
        {
            for(int c = 0; c < 8; c++)
            {
                Trade[k, c] = false;
            }
        }
        

        int ThreadNumber = CurrentNumber;
        CurrentNumber++;
        for (int i = 0; i < LIMIT; i++)
        {
            Thread t = new Thread(new ThreadStart(Service));
            t.Start();
        }
        Console.WriteLine("Ready");
    }
    public static void Service()
    {
        
        
        while (true)
        {
           
                Socket soc = listener.AcceptSocket();
            //soc.SetSocketOption(SocketOptionLevel.Socket,
            //        SocketOptionName.ReceiveTimeout,10000);

            try
            {
                Stream s = new NetworkStream(soc);
                StreamReader sr = new StreamReader(s);
                StreamWriter sw = new StreamWriter(s);
                sw.AutoFlush = true;
               
                string Verison = sr.ReadLine();
                if (Verison == "2.2.0.0")
                {
                    sw.WriteLine("right");
                }
                else
                {
                    sw.WriteLine("wrong");
                }
                string chat = sr.ReadLine();
                 
                if (chat == "false")
                {
                    int ThreadNumber = int.Parse(sr.ReadLine());
                    sw.WriteLine(NumberOfPlayers);
                    string color = "";
                    string msgcolor = "";                    
                    Nicks[ThreadNumber - 1] = sr.ReadLine();
 
                    switch (ThreadNumber)
                    {
                        case 1:
                            color = "Niebieski";
                            msgcolor = "Blue";
                            break;
                        case 2:
                            color = "Czerwony";
                            msgcolor = "Red";
                            break;
                        case 3:
                            color = "Zielony";
                            msgcolor = "Green";
                            break;
                        case 4:
                            color = "Pomarańczowy";
                            msgcolor = "Orange";
                            break;
                        case 5:
                            color = "Fioletowy";
                            msgcolor = "Purple";
                            break;
                        case 6:
                            color = "Różowy";
                            msgcolor = "Pink";
                            break;
                        case 7:
                            color = "Morski";
                            msgcolor = "Navy";
                            break;
                        case 8:
                            color = "Bordowy";
                            msgcolor = "Maroon";
                            break;
                    }
                    MainChat = "Black~[SERVER]:~ " + Nicks[ThreadNumber - 1] + "("+ color +") dołączył do gry!";
                    
                    foreach (int T in TradeValue)
                    {
                        sw.WriteLine(T);
                    }
                    
                    while (true)
                    {

                        string command = sr.ReadLine();
                        switch (command)
                        {
                            case "ask":
                                TradeAsk[int.Parse(sr.ReadLine()), ThreadNumber - 1] = true;
                                for (int a = 0; a < 8; a++)
                                {
                                    for (int b = 0; b < 8; b++)
                                    {
                                        Console.WriteLine(TradeAsk[a, b]);
                                    }
                                }
                                break;
                            case "give points":
                                int Number = int.Parse(sr.ReadLine()) - 1;
                                PointsToAdd[Number] += int.Parse(sr.ReadLine());
                                break;
                            case "checktrade":
                                for (int a = 0; a < 8; a++)
                                {
                                    if (TradeAsk[ThreadNumber - 1, a] == true)
                                    {
                                        sw.WriteLine("trade");
                                    }
                                    else
                                    {
                                        sw.WriteLine("null");
                                    }
                                    string result = sr.ReadLine();
                                    if (result == "true")
                                    {
                                        TradeAsk[ThreadNumber - 1, a] = false;

                                    }
                                    else
                                    {

                                    }

                                }
                                break;
                            case "report attack":
                                string ra = sr.ReadLine();
                                string[] splitedra= ra.Split('~');
                                BattleReports.Add(new BattleReport(int.Parse(splitedra[0]), int.Parse(splitedra[1]), int.Parse(splitedra[2]), int.Parse(splitedra[3]), int.Parse(splitedra[4])));
                                break;
                            case "check":
                                if (ThreadNumber == PlayerTurnNumber)
                                {
                                    sw.WriteLine("true");
                                    sw.WriteLine(PointsToAdd[ThreadNumber - 1]);
                                    PointsToAdd[ThreadNumber - 1] = 0;
                                }
                                else if (ThreadNumber != PlayerTurnNumber && Updates[ThreadNumber - 1] != true)
                                {
                                    sw.WriteLine("false");
                                    sw.WriteLine(PointsToAdd[ThreadNumber - 1]);
                                    PointsToAdd[ThreadNumber - 1] = 0;
                                }
                                else if (ThreadNumber != PlayerTurnNumber && Updates[ThreadNumber - 1] == true)
                                {
                                    sw.WriteLine("update");
                                    Updates[ThreadNumber - 1] = false;
                                }
                                /*
                                if (BattleReports.Count > 0)
                                {
                                    int b = BattleReports.Count;
                                    for(int a = 0; a < b; a++)
                                    {
                                        if(BattleReports[a].DefenderNumber == ThreadNumber)
                                        {
                                            sw.WriteLine("battle");
                                            sw.WriteLine($"{BattleReports[a].AttackerNumber}~{BattleReports[a].DefenderNumber}~{BattleReports[a].AttackerNumber}");
                                        }
                                        

                                    }
                                    sw.WriteLine("update");
                                }
                                */
                                break;
                            case "next turn":
                                int PreviousNumber = PlayerTurnNumber;
                                if (PlayerTurnNumber < NumberOfPlayers)
                                    PlayerTurnNumber++;
                                else
                                    PlayerTurnNumber = 1;
                                for (int k = 1; k <= NumberOfPlayers; k++)
                                {
                                    if (k != PreviousNumber && k != PlayerTurnNumber)
                                    {
                                        Updates[k - 1] = true;
                                    }
                                }

                                break;
                            case "get":
                                foreach (int O in Owners)
                                {
                                    sw.WriteLine(O);
                                }
                                foreach (int D in Development)
                                {
                                    sw.WriteLine(D);
                                }
                                foreach (int F in Fortifications)
                                {
                                    sw.WriteLine(F);
                                }
                               
                                for (int a = 0; a < 8; a++)
                                {
                                    for (int b = 0; b < 8; b++)
                                    {
                                        sw.WriteLine(Trade[a, b]);
                                    }
                                }                         
                                sw.WriteLine(Units.Count);
                                Console.WriteLine(Units.Count);
                                foreach(Unit u in Units)
                                {
                                    sw.WriteLine(u.position_id + "~" + u.hp + "~" + u.min_attack + "~" + u.max_attack +"~" + u.m_defence + "~" + u.r_defence + "~" + u.type + "~" + u.level+ "~" + u.dodge);
                                }
                                
                                for (int a = 0; a < 8; a++)
                                {
                                    string text = "";
                                    for (int b = 0; b < 30; b++)
                                    {
                                        text += Upgrades[b + (a * 30)] + "~";

                                    }
                                    sw.WriteLine(text);
                                }
                                break;
                            case "set":
                                for (int a = 0; a < Owners.Length; a++)
                                {
                                    Owners[a] = int.Parse(sr.ReadLine());
                                }
                                for (int a = 0; a < Development.Length; a++)
                                {
                                    Development[a] = int.Parse(sr.ReadLine());
                                }
                                for (int a = 0; a < Fortifications.Length; a++)
                                {
                                    Fortifications[a] = int.Parse(sr.ReadLine());
                                }
                                
                                for (int a = 0; a < 8; a++)
                                {
                                    for (int b = 0; b < 8; b++)
                                    {
                                        Trade[a, b] = Boolean.Parse(sr.ReadLine());
                                    }
                                }
                                int unitsnumber = int.Parse(sr.ReadLine());
                                Console.WriteLine(unitsnumber.ToString());
                                Units.Clear();
                                for(int a = 0; a < unitsnumber; a++)
                                {
                                    string text = sr.ReadLine();
                                    string[] splited = text.Split('~');
                                    Unit u = new Unit(splited[6]);
                                    u.position_id = int.Parse(splited[0]);
                                    u.hp = int.Parse(splited[1]);
                                    u.min_attack = int.Parse(splited[2]);
                                    u.max_attack = int.Parse(splited[3]);
                                    u.m_defence = int.Parse(splited[4]);
                                    u.r_defence = int.Parse(splited[5]);
                                    u.level = int.Parse(splited[7]);
                                    u.dodge = int.Parse(splited[8]);
                                }
                                for (int a = 0; a < 8; a++)
                                {
                                    string text = sr.ReadLine();
                                    string[] splited = text.Split('~');
                                    for (int b = 0; b < 30; b++)
                                    {
                                        Upgrades[b + (a * 30)] = int.Parse(splited[b]);
                                    }

                                }
                                break;
                            case "exit":
                                goto end;
                            case "write":
                                string r = sr.ReadLine();
                                switch (r)
                                {
                                    case "main":
                                        MainChat = msgcolor +"~[" + Nicks[ThreadNumber -1] +"]~: "+ sr.ReadLine();
                                        break;
                                  
 
                                }
                                break;                               
                        }
                    }
                }
                else
                {
                    while (true)
                    {
                        if(sr.ReadLine() == "check")
                            sw.WriteLine(MainChat);
                       

                        
                    }                  
                }
            }
            catch
            {
                soc.Close();
                
            }
            end:
            soc.Close();
            break;
        }
    }
    public class BattleReport
    {
        public int AttackerNumber { get; set; }
        public int DefenderNumber { get; set; }
        public int provinceid { get; set; }
        public int ALoses { get; set; }
        public int DLoses { get; set; }
        public BattleReport(int AN, int DN, int pi, int AL, int DL)
        {
            AttackerNumber = AN;
            DefenderNumber = DN;
            provinceid = pi;
            ALoses = AL;
            DLoses = DL;
        }
    }
    public class Unit
    {
        public int dodge { get; set; }
        public int hp { get; set; }
        public int level { get; set; }
        public int m_defence { get; set; }
        public int r_defence { get; set; }
        public int min_attack { get; set; }
        public int max_attack { get; set; }
        public string type { get; set; }
        public int owner { get; set; }
        public int position_id { get; set; }
        public Unit(string Type)
        {
            type = Type;
            Units.Add(this);
        }


    }
}