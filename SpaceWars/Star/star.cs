using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpaceWars;
/// <summary>
/// this libary represent the properties of the star 
/// </summary>
namespace Star
{
    [JsonObject(MemberSerialization.OptIn)]
 public class star
{    
  [JsonProperty(PropertyName = "star")]
    private int ID;

    [JsonProperty]
    private SpaceWars.Vector2D loc;

    [JsonProperty]
    private double mass;

        private int lastUpdate;
        // constructor to intilize the field data of the star
        public star(int ID, Vector2D loc,double mass)
        {
            this.ID = ID;
            this.loc = loc;
            this.mass = mass;

            this.lastUpdate = 0;
           
        }
        // access the star ID
        public int getID()
        {
            return this.ID;
        }
        //access to the location of the star
        public Vector2D getloc()
        {
            return this.loc;
        }
        public void setLoc(Vector2D loc)
        {
            this.loc = loc;
        }
        //access the mass of the star
        public double getMass()
        {
            return this.mass;
        }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
        public void update(int time)
        {
            //this.loc = this.loc + new Vector2D(0.08, 0.08);
            if (time - lastUpdate > 50)
            {
                lastUpdate = time;
                if (this.loc.GetX() == 0 && this.loc.GetY() == 0)
                {
                    this.loc = new Vector2D(0, -200);
                    return;
                }
                if (this.loc.GetX() == 0 && this.loc.GetY() == -200)
                {
                    this.loc = new Vector2D(-200, -200);
                    return;

                }
                if (this.loc.GetX() == -200 && this.loc.GetY() == -200)
                {
                    this.loc = new Vector2D(-200, 200);
                    return;
                }
                if (this.loc.GetX() == -200 && this.loc.GetY() == 200)
                {
                    this.loc = new Vector2D(200, 200);
                    return;
                }
                if (this.loc.GetX() == 200 && this.loc.GetY() == 200)
                {
                    this.loc = new Vector2D(200, -200);
                    return;
                }
                if (this.loc.GetX() == 200 && this.loc.GetY() == -200)
                {
                    this.loc = new Vector2D(-200, -200);
                    return;
                }
            }


        }
    }
}

