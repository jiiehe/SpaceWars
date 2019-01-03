using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NetworkController;
using ship;
using Star;
using World;
using Projectile;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
/// <summary>
/// this project is used to control the game
/// </summary>

namespace GameController
{
    public delegate void FrameTick();// redraw the panel
    public delegate void HandleKey();//press the key event 
    public delegate void HandleStep();// disable the key press
    public delegate void HandleResize();// resize the panel
    public delegate void HandleScore(Ship s);// handle the socre of the ship
    public delegate void HandleDie(Ship s);// handle the die of the ship
    public class GController
    {
        private String name;// player name 
        private Socket socket;
        private int PlayerID;
        private int size;
        private world theworld;
        private FrameTick frametick;
        private HandleKey Handlekey;
        private HandleStep Handlestep;
        private HandleResize Handleresize;
        private HandleScore handlescore;
        private HandleDie handledie;
        List<int> ships = new List<int>();// list to store the number of ship is the game
        List<int> died = new List<int>();// list to store the numes of the died ship 
        //contructor
        public GController()
        {
            theworld = new world();
        }
        //event handler
        public void RegisterDie(HandleDie h)
        {
            this.handledie += h;
        }
        public void RegisterShip(HandleScore h)
        {
            this.handlescore += h;
        }
        public int getSize()
        {
            return this.size;
        }
        public void RegisterResize(HandleResize h)
        {
            this.Handleresize += h;
        }
        public void RegisterTick(FrameTick h)
        {
            this.frametick += h;
        }
        public void RegisterKey(HandleKey h)
        {
            this.Handlekey += h;
        }
        public void RegisterStep(HandleStep h)
        {
            this.Handlestep += h;
        }
        //the connect button to connect to the server
        public void ConnectButton(String ipaddress)
        {
            socket = 
            NController.ConnectToServer(FirstContact, ipaddress);


        }
        //first connect to the server
        private void FirstContact(SocketState state)
        {
            state.callMe = ReceiveStartup;
            NController.Send(socket, name+"\n");
         
            NController.GetData(state);
            
        }
        //get the response from the server and remove the processed message
        private void ReceiveStartup(SocketState state)
        {
            //String builder to receive the message
            StringBuilder strBuilder = state.sb;
            String[] group = strBuilder.ToString().Split('\n');
            PlayerID = Int32.Parse(group[0]);
            size = Int32.Parse(group[1]);
            strBuilder.Remove(0, group[0].Length);
            strBuilder.Remove(0, group[1].Length);
            state.callMe = ReceiveWorld;
            Handlestep();
            Handleresize();
            NController.GetData(state);

        }
        //get the content information of the current world from the server 
        // and using the JSon to get the objects
        private void ReceiveWorld(SocketState state)
        {
            string totalData = state.sb.ToString();
            string[] parts = Regex.Split(totalData, @"(?<=[\n])");

            
            // Loop until we have processed all messages.
            // We may have received more than one.
            Ship sp = null;
            star st = null;
            projectile pj = null;
            foreach (string p in parts)
            {
                // Ignore empty strings added by the regex splitter
                if (p.Length == 0)
                    continue;
                // The regex splitter will include the last string even if it doesn't end with a '\n',
                // So we need to ignore it if this happens. 
                if (p[p.Length - 1] != '\n')
                    break;
                if(p[0]=='{'&& p[p.Length - 2] == '}')
                {
                    
                    JObject jsonObject = JObject.Parse(p);
                    JToken tokenShip = jsonObject["ship"];//gte the ship object from the server
                    JToken tokenStar = jsonObject["star"];// get the star object from the server
                    JToken tokenProj = jsonObject["proj"];// get the projectiles from the server
                    if (tokenShip != null)
                    {
                        sp = JsonConvert.DeserializeObject<Ship>(p);// if ship is not null,get the ship 
                    }
                    if (tokenStar != null)
                    {
                        st = JsonConvert.DeserializeObject<star>(p);// if the star is not null, get the star
                    }
                    if (tokenProj != null)
                    {
                        pj = JsonConvert.DeserializeObject<projectile>(p);// if the projectile is not null, get the projectile
                    }
                    //avoid the race condition 
                    lock (theworld)
                    {
                        if (sp != null)
                        {
                         
                            if (sp.getHp() > 0)
                            {
                                
                                theworld.addShip(sp);// if the hp is greater than 0, add to the current world
                                if (ships.Contains(sp.getID()) == false)
                                {
                                    ships.Add(sp.getID());
                                    handlescore(sp);

                                }
                            }
                            else
                            {
                                handledie(sp);
                                ships.Remove(sp.getID());
                                theworld.removeShip(sp);//remove the destoryed ship
                                
                                
                            }
                           

                        }

                        if (pj != null)
                        {
                            // if the projectile is still alive, add it to the projectils group
                            if (pj.checkAlive() == true)
                            {
                                theworld.addproj(pj);
                            }
                            // if projectile is not alive, remove it from the projectile group
                            else
                            {
                                theworld.removeProjectile(pj);
                            }
                        }
                        //check the star then add to the star group
                      
                        if (st != null)
                        {
                            theworld.addStar(st);
                        }



                       
                    }
                }
          


                // Then remove it from the SocketState's growable buffer
                state.sb.Remove(0, p.Length);
     
                frametick();
            }

            state.callMe = ReceiveWorld;
            Handlekey();
            NController.GetData(state);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void sendData(String data)
        {
            NController.Send(socket, data + "\n");
        }
        public void setName(String name)
        {
           this. name = name;
        }
        public world getWorld()
        {
            return this.theworld;
        }

    }
    
}
