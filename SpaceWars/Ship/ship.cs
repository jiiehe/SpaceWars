using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpaceWars;
using Star;
/// <summary>
/// this library provides the properties of the ship
/// </summary>
namespace ship
{
    [JsonObject(MemberSerialization.OptIn)]
     public class Ship
{    
       
  [JsonProperty(PropertyName = "ship")]
    private int ID;

    [JsonProperty]
    private Vector2D loc;

    [JsonProperty]
    private Vector2D dir;

    [JsonProperty]
    private bool thrust;

    [JsonProperty]
    private string name;

    [JsonProperty]
    private int hp;

    [JsonProperty]
        private int score;

        private Vector2D velocity; 
        private String operate;
        private Vector2D speedup;
        private int deathTime;
        private int shootPoint;
       
        // constructor to initialize the date filed of the ship

      public Ship (int ID, Vector2D loc, Vector2D dir,bool thrust,string name, int hp,int score)
        {
           this. ID = ID;
           this.loc = loc;
           this.dir = dir;
           this.thrust = thrust;
           this.name = name;
           this.hp = hp;
            this.score = score;
            this.velocity = new Vector2D(0, 0);
            this.operate = "";
            this.speedup = new Vector2D(0, 0);
            deathTime = 0;
            this.shootPoint = 0;

        }
        /// <summary>
        /// when hit by projectile, the hp will decreased
        /// </summary>
        public void hpdecrease()
        {
            this.hp--;
        }
        /// <summary>
        ///  check whether the ship can fire or not, 
        /// </summary>
        /// <param name="time">current frame time </param>
        /// <param name="Mspershoot"></param>
        /// <returns></returns>
        public bool checkFire(int time, int Mspershoot)
        {
            if (this.hp < 0)
            {
                return false;
            }
            if (time-this.shootPoint < Mspershoot)
            {
                return false;
            }
            else
            { //assign the last shooting time as the currenttime 
                this.shootPoint = time;
                return true;
            }
        }
        // access the ID of the ship
        public int getID()
        {
            return this.ID;
        }
        //access the location of the ship
        public Vector2D getloc()
        {
            return this.loc;
        }
        // access the direction of the ship
        public Vector2D getdir()
        {
            return this.dir;
        }
        // check the ship thrust or not
        public bool getThrust()
        {
            return this.thrust;
        }
        // get the name of the player
        public string getName()
        {
            return this.name;
        }
        // get the hit point of the ship
        public int getHp()
        {
            return this.hp;
        }
        // get the socre of the ship
        public int getScore()
        {
            return this.score;
        }
        /// <summary>
        /// 
        ///override toString method
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
        /// <summary>
        /// get the command of opertaions
        /// </summary>
        /// <param name="temp"></param>
        public void doOperate( char temp)
        {
            if (temp == 'L')
            {
                this.operate = "left";
            }else
            if (temp == 'R')
            {
                this.operate = "right";
            }
            else if (temp == 'T')
            {
                this.thrust = true;
                this.operate = "thrust";
                
            }
        }
        /// <summary>
        /// do operation by  specific commands 
        /// </summary>
        public void refresh()
        {
            if (this.operate == "left")
            {
                this.dir.Rotate(-2);
                
            }
            if (this.operate == "right")
            {
                this.dir.Rotate(2);
                
            }
            if (this.operate == "thrust")
            {
                this.speedup = this.dir * 0.08;
                
            }
            this.operate = "";
            
        }
        /// <summary>
        /// set thrust status 
        /// </summary>
        public void setThrust()
        {
            this.thrust = false;
        }
        /// <summary>
        /// make ship be respawn
        /// </summary>
        public void respawn()
        {
            this.hp = 5;
            this.velocity = new Vector2D(0, 0);
            this.speedup = new Vector2D(0, 0);
            this.thrust = false;

        }
        /// <summary>
        /// if the ship reaches the corner,wraparound
        /// </summary>
        /// <param name="xory"></param>
        public void wrapAround(bool xory)
        {
            if (xory == true)
            {
                this.loc = new Vector2D(this.loc.GetX() * -1, this.loc.GetY());
            }
            else
            {
                this.loc = new Vector2D(this.loc.GetX(), this.loc.GetY()*-1);
            }
        }
        /// <summary>
        /// increase the socre is ship shoot successfully
        /// </summary>
        public void increaseScore()
        {
            this.score++;
        }
       
        /// <summary>
        /// update the ship
        /// </summary>
        /// <param name="stars"></param>
        /// <param name="time"></param>
        public void update(IEnumerable<star> stars, int time)
        {
            refresh();
            this.operate = "";
            if (this.speedup.GetX() == 0 && this.speedup.GetY() == 0)
            {
                this.thrust = false;
            }
            Vector2D acceleration = new Vector2D(this.speedup);
            
            this.speedup = new Vector2D(0, 0);
           
            // collision detection 
            foreach(star s in stars)
            {
                if ((s.getloc() - this.loc).Length() < 35)
                {
                    
                    
                        this.hp = 0;
                        this.deathTime = time;
                    
                  
                }
                double mass = s.getMass();
                Vector2D gravity = s.getloc() - loc;
                gravity.Normalize();
                gravity = gravity * mass;
                acceleration = acceleration + gravity;
            }
            velocity = velocity + acceleration;

            loc = loc + velocity;
            
        }
        /// <summary>
        /// get  last death time 
        /// </summary>
        /// <returns></returns>
        public int getDeath()
        {
            return this.deathTime;
        }
        /// <summary>
        /// set the locaton of the ship
        /// </summary>
        /// <param name="loc"></param>
        public void setLoc(Vector2D loc)
        {
            this.loc = loc;
        }
        /// <summary>
        /// if ship is diconnected, make it die immediatelt
        /// </summary>
        public void setLost()
        {
            this.hp = 0;
        }
    }
}
