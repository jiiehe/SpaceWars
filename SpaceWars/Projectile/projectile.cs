using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpaceWars;
using Star;
/// <summary>
/// properties of the projectile 
/// </summary>
namespace Projectile
{
    [JsonObject(MemberSerialization.OptIn)]
   public class projectile
{    
  [JsonProperty(PropertyName = "proj")]
    private int ID;

    [JsonProperty]
    private Vector2D loc;

    [JsonProperty]
    private Vector2D dir;

    [JsonProperty]
    private bool alive;

        [JsonProperty]
        private int owner;
        // contructor 
        public projectile(int ID, Vector2D loc,Vector2D dir, bool alive, int owner)
        {
            this.ID = ID;
            this.loc = loc;
            this.dir = dir;
            this.alive = alive;
            this.owner = owner;

        }
        // have the access to the projectile ID
        public int getID()
        {
            return this.ID;
        }
        // have the access to the location of the projectile 
        public Vector2D getloc()
        {
            return this.loc;
        }
        // have the acces to the direction of the projectile 
        public Vector2D getdir()
        {
            return this.dir;
        }
        // check the alive the projectile 
        public bool checkAlive()
        {
            return this.alive;
        }
        // access to the owner of the projectile 
        public int getOwner()
        {
            return this.owner;
        }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
        /// <summary>
        /// update the projectiles, and keep track of status
        /// </summary>
        /// <param name="size"></param>
        /// <param name="stars"></param>
        public void update(int size,IEnumerable<star> stars)
        {
            
            this.loc = this.loc + (this.dir * 15.0);
            if (this.loc.GetX() > size / 2||this.loc.GetX()<-size/2||this.loc.GetY()>size/2||this.loc.GetY()<-size/2)
            {
                die();
            }
            foreach(star s in stars)
            {
               
                    if ((s.getloc() - this.loc).Length() < 35)
                    {
                        die();
                    }
                
            }
        }
        // check the status
        public void die()
        {
            this.alive = false;
        }
    }
}
