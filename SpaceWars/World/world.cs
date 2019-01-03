using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ship;
using Projectile;
using Star;
using SpaceWars;
/// <summary>
/// this library provide the properties of the world we need to draw
/// </summary>
namespace World
{
    public class world
    {
        private Dictionary<int, Ship> shipgroup;// store the ships in the current world with specific ID
        private Dictionary<int, projectile> projectileGroup;//store projectiles fired by ship in the current world with specific player ID
        private Dictionary<int, star> starGroup;// store the star in the current world with the player ID 
        private int size;
        private static int shipID;
        private int time;
        private int respawns;
        private static int projID;
        private List<int> lostID;
        private static int lifeID;
        private int shootFrame;
        private List<int> dieStar;
        /// <summary>
        /// constructor to initialize
        /// </summary>

        public world()
        {
            shipID = 0;
            this.shipgroup = new Dictionary<int, Ship>();
            this.projectileGroup = new Dictionary<int, projectile>();
            this.starGroup = new Dictionary<int, star>();
            time = 0;
            this.lostID = new List<int>();
            lifeID = 2;
            this.dieStar = new List<int>();
        }
        
        /// <summary>
        /// get frame info from the server
        /// </summary>
        /// <param name="frame"></param>
        public void setFrame(int frame)
        {
            this.shootFrame = frame;
        }
        /// <summary>
        /// get disconnected ship and add to the lost list
        /// </summary>
        /// <param name="id"></param>
        public void addLostID(int id)
        {
            lostID.Add(id);
        }
        /// <summary>
        /// calculate the projectile ID 
        /// </summary>
        /// <returns></returns>
        public int generatePorj()
        {
            projID = projID + 1;
            return projID;
        }
        /// <summary>
        /// calculte ship ID
        /// </summary>
        /// <returns></returns>
        public int generateID()
        {
            shipID = shipID+1;
            return shipID;
        }
        /// <summary>
        /// this is the helper method used in the GController to add the current ship into the ship group
        /// </summary>
        /// <param name="ship"></param>
        public void addShip(Ship ship)
        {
            this.shipgroup[ship.getID()] = ship;
        }
        /// <summary>
        /// this is the helper method used in the GController to add the current star into the star group
        /// </summary>
        /// <param name="tar"></param>
        public void addStar(star tar)
        {
            this.starGroup[tar.getID()] = tar;
        }
        /// <summary>
        /// this is the helper method used in the GController to add the current proj into the proj group
        /// </summary>
        /// <param name="proj"></param>
        public void addproj(projectile proj)
        {
            this.projectileGroup[proj.getID()] = proj;
        }
        /// <summary>
        /// if the ship is gone, we need to remove the current ship from the ship group
        /// </summary>
        /// <param name="ship"></param>
        public void removeShip(Ship ship)
        {
            this.shipgroup.Remove(ship.getID());
        }
        /// <summary>
        /// if we  redraw the world, we need to remove the current star from the star groups
        /// </summary>
        /// <param name="star"></param>
        public void removeStar(star star)
        {
            this.starGroup.Remove(star.getID());
        }
        /// <summary>
        /// if the projectile disappered we need to remove it from the projectile group
        /// </summary>
        /// <param name="proj"></param>
        public void removeProjectile(projectile proj)
        {
            this.projectileGroup.Remove(proj.getID());
        }
        /// <summary>
        /// helper method to let the game controller has the access to the star group
        /// </summary>
        /// <returns></returns>
        public Dictionary<int,star> getStar()
        {
            return this.starGroup;
        }
        /// <summary>
        /// helper method to let the game controleer has the access to the projectile group
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, projectile> getProj()
        {
            return this.projectileGroup;
        }
        /// <summary>
        /// helper method to let the game controller has the access to the ship group
        /// </summary>
        /// <returns></returns>
        public Dictionary<int,Ship> getShip()
        {
            return this.shipgroup;
        }
        /// <summary>
        /// get info from the server
        /// </summary>
        /// <param name="size"></param>
        public void setSize(int size)
        {
            this.size = size;
        }
        /// <summary>
        /// get the info from the server
        /// </summary>
        /// <param name="respawn"></param>
        public void setRespawn(int respawn)
        {
            this.respawns = respawn;
        }
        /// <summary>
        /// generate a safe location for the ship 
        /// </summary>
        /// <returns></returns>
        public Vector2D getPostion()
        {
            Random rand = new Random();
            double x = rand.NextDouble();
            double y = rand.NextDouble();
            x = x * (size / 2);
            y = y * (size / 2);
            Vector2D result = new Vector2D(x, y);
            bool safe = false;
            while (safe == false)
            {
                foreach(star s in this.getStar().Values)
                {
                    double length = (result - s.getloc()).Length();
                    if (length < 35)
                    {
                         x = rand.NextDouble();
                         y = rand.NextDouble();
                        x = x * (size / 2);
                        y = y * (size / 2);
                        result = new Vector2D(x, y);
                        break;
                    }
                    else
                    {
                        safe = true;
                    }
                }
                
            }
            return result;
        }
        /// <summary>
        /// generate a new ship
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Ship generateShip(String name)
        {
            int ID = generateID();
            Vector2D loc = getPostion();
            Vector2D dir = new Vector2D(0, -1);
            int hp = 5;
            bool thrust = false;
            int score = 0;
            Ship ship = new Ship(ID, loc, dir, thrust, name, hp, score);
            this.addShip(ship);
            return ship;
        }
        /// <summary>
        /// make the ship be respawn
        /// </summary>
        /// <param name="s"></param>
        public void respawn(Ship s)
        {
            s.respawn();
            s.setLoc(getPostion());
        }
        /// <summary>
        /// fire projectile for a ship 
        /// </summary>
        /// <param name="shipID"></param>
        public void Fire(int shipID)
        {
            if (shipgroup[shipID].checkFire(this.time, this.shootFrame))
            {
                projectile proj = new projectile(generatePorj(), shipgroup[shipID].getloc(), shipgroup[shipID].getdir(), true, shipID);
                this.projectileGroup[proj.getID()] = proj;
            }
        }
        /// <summary>
        /// clean up the diconnected ship 
        /// </summary>
        public void cleanup()
        {
            foreach(int temp in lostID)
            {
                shipgroup.Remove(temp);
            }
            LinkedList<int> whole = new LinkedList<int>(projectileGroup.Keys);
            foreach(int id in whole)
            {
                if (projectileGroup[id].checkAlive() == false)
                {
                    projectileGroup.Remove(id);
                }
            }
           
        }
      
     
        /// <summary>
        /// update all info about projectile and ship 
        /// </summary>
        public void update()
        {
            foreach(star s in starGroup.Values)
            {
                s.update(this.time);
            }
            
            foreach(Ship s in this.shipgroup.Values)
            {

                if (s.getHp() > 0)
                {
                    s.update(this.starGroup.Values, time);
                    if (s.getloc().GetX() > size / 2 || s.getloc().GetX() < -size / 2)
                    {
                        s.wrapAround(true);
                    }
                    if (s.getloc().GetY() > size / 2 || s.getloc().GetY() < -size / 2)
                    {
                        s.wrapAround(false);
                    }
                }
               
                if (s.getHp() == 0)
                {
                    if (this.time - s.getDeath() > respawns)
                    {
                        respawn(s);
                    }
                }
                

            }
            foreach(projectile s in this.projectileGroup.Values)
            {
                if (s.checkAlive() == true)
                {
                    s.update(size, starGroup.Values);
                    foreach(Ship ship in this.shipgroup.Values)
                    {
                        if ((s.getloc() - ship.getloc()).Length() < 20)
                        {
                            if (s.getOwner() != ship.getID()&&s.getOwner()!=-1)
                            {
                                if (ship.getHp() > 0)
                                {
                                    ship.hpdecrease();
                                    s.die();
                                    if (ship.getHp() <= 0)
                                    {
                                        this.shipgroup[s.getOwner()].increaseScore();
                                    }
                                }
                                
                                

                                
                            }
                        }
                    }
                }
            }
            time++;

        }
    }
}
