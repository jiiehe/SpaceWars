using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Star;
using SpaceWars;
using World;
using NetworkController;
using System.Diagnostics;
using ship;
using Projectile;
/// <summary>
/// server project 
/// </summary>
namespace Server
{
   class Server
    {
        //data belongs XML
        private static int UniverSize;
        private static int MSperFrame;
        private static int FramesPerShot;
        private static int RespawnRate;
        private static star sta;
        private static world theworld;
        private static Stopwatch watch;
        private static Stopwatch liveStar;
        // list of connected clients 
        private static LinkedList<SocketState> list;
        //location of star
        private static double x = 0;
        private static double y = 0;
        private static double mass =0;
        /// <summary>
        /// there are four steps for the server 
        /// (1) read xml to get necessary information 
        /// (2)initalize a new world and add info to the new world
        /// (3)uisng serverawaitClientloop to awit client join in 
        /// (4)using update method to send update info to the client 
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            xmlRead();
            watch = new Stopwatch();
            list = new LinkedList<SocketState>();
            watch.Start();
            liveStar = new Stopwatch();
            liveStar.Start();
            theworld = new world();
            sta.setLoc(new Vector2D(200, -200));
            star ss = new star(1, new Vector2D(200, 200), 0.01);
            theworld.addStar(sta);
            theworld.addStar(ss);
            theworld.setSize(UniverSize);
            theworld.setRespawn(RespawnRate);
            theworld.setFrame(MSperFrame);
            NController.ServerAwaitClientLoop(HandleNewClient);
            Console.WriteLine("our server start, and client could join in now");
            //create a new thread to update the world and send the updated info to the client 
            Task task = new Task(() =>
             {
                 while (true)
                 {
                     update();
                 }

             });
            task.Start();
            Console.Read();
            
        }
        /// <summary>
        /// update method use to send info to the client
        /// </summary>
        public static void update()
        {
            // wait until enough time 
            while (watch.ElapsedMilliseconds < MSperFrame)
            {

            }
            watch.Restart();
         
            // use to append message 
            StringBuilder sb = new StringBuilder();
            lock (theworld)
            {
                theworld.update();
                foreach (star s in theworld.getStar().Values)
                {
                    sb.Append(s.ToString() + '\n');
                }
                foreach(Ship s in theworld.getShip().Values)
                {
                    sb.Append(s.ToString() + "\n");
                }
                foreach(projectile s in theworld.getProj().Values)
                {
                    sb.Append(s.ToString() + "\n");
                }
                // clean up the unconnected ship and died stuffs
                theworld.cleanup();
              
            }
            //check all client statuses
            lock (list)
            {
                foreach (SocketState ss in list.ToArray())
                {
                    if (ss.theSocket.Connected == true)
                    {
                        NController.Send(ss.theSocket, sb.ToString());
                    }
                    // deal with the unconnected clients
                    else
                    {
                        lock (theworld)
                        {

                            theworld.addLostID(ss.uid);
                            theworld.getShip()[ss.uid].setLost();
                        }
                        list.Remove(ss);
                    }
                }
            }
        }
        /// <summary>
        /// callback method will be called in accpeted new client method 
        /// </summary>
        /// <param name="ss"></param>
        public static void HandleNewClient(SocketState ss)
        {
            ss.callMe = ReceiveName;
            NController.GetData(ss);
        }
        /// <summary>
        /// call back, if the client connects get the user name then generate a new ship, finally send back startup info,and add the client to the list
        /// </summary>

        public static void ReceiveName(SocketState ss)
        {
            String[] parts = ss.sb.ToString().Split('\n');
            String name = parts[0];
            Ship ship;
            lock (theworld)
            {
                ship = theworld.generateShip(name);
                ss.uid = ship.getID();
            }

            lock (list)
            {
                list.AddLast(ss);
                Console.WriteLine("welcome player: "+ship.getName());
            }
            NController.Send(ss.theSocket, ship.getID() + "\n" + UniverSize + "\n");
            ss.sb.Clear();
            ss.callMe = userInput;
            NController.GetData(ss);
        }
        /// <summary>
        /// deal with the iunput info about the operations of ship 
        /// </summary>
        /// <param name="ss"></param>
        public static void userInput(SocketState ss)
        {
            String input = ss.sb.ToString();
            String[] parts = input.Split('\n');
            foreach(char temp in parts[0])
            {
                if (temp != '(' && temp != ')')
                {
                    if (temp != 'F')
                    {
                        lock (theworld)
                        {
                            theworld.getShip()[ss.uid].doOperate(temp);

                        }
                    }
                    else
                    {
                        lock (theworld)
                        {
                            theworld.Fire(ss.uid);
                        }
                    }
                }
                ss.sb.Remove(0, 1);
            }
            ss.sb.Remove(0, 1);
            NController.GetData(ss);
        }
        /// <summary>
        /// get info from the xml file 
        /// </summary>
        public static void xmlRead()
        {
            try
            {
                using (XmlReader reader = XmlReader.Create("../../../Resources/settings.xml"))
                {
                    while (reader.Read())
                    {
                        if (reader.IsStartElement())
                        {
                            if(reader.Name== "UniverseSize")
                            {
                                reader.Read();
                                UniverSize = Int32.Parse(reader.Value);
                            }
                            if(reader.Name== "MSPerFrame")
                            {
                                reader.Read();
                                MSperFrame = Int32.Parse(reader.Value);
                            }
                            if(reader.Name== "FramesPerShot")
                            {
                                reader.Read();
                                FramesPerShot = Int32.Parse(reader.Value);
                            }
                            if(reader.Name== "RespawnRate")
                            {
                                reader.Read();
                                RespawnRate = Int32.Parse(reader.Value);
                            }
                            if(reader.Name== "Star")
                            {
                                readStar(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
               // Server.ErrorExit("unable to read settings file: " + filepath);
            }

        }
        /// <summary>
        /// helper method for xmlread method 
        /// </summary>
        /// <param name="reader"></param>
        public static void readStar(XmlReader reader)
        {
            try
            {
                
                bool hasX = false;
                bool hasY = false;
                bool hasMass = false;
                    while (reader.Read())
                    {
                        if (reader.IsStartElement())
                        {
                            if (reader.Name == "x")
                            {
                                reader.Read();
                                x = Double.Parse(reader.Value);
                                     hasX = true;
                            }
                            if (reader.Name == "y")
                            {
                                reader.Read();
                                y = Double.Parse(reader.Value);
                                  hasY = true;
                            }
                            if (reader.Name == "mass")
                            {
                                reader.Read();
                                mass = Double.Parse(reader.Value);
                                 hasMass = true;
                            }
                       
                        }
                    }
                if (hasX && hasY && hasMass)
                {
                    Vector2D loc = new Vector2D(x, y);
                    sta = new star(0, loc,mass);
                    
                }
                    
                
            }
            catch (Exception e)
            {
                // Server.ErrorExit("unable to read settings file: " + filepath);
            }

        }
    }
}
