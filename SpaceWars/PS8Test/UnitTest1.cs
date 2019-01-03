using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ship;
using Star;
using World;
using Projectile;
using SpaceWars;
using Newtonsoft.Json;

namespace PS8Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestShipMethod()
        {
            world world1 = new world();
            star s = new star(0, new Vector2D(200, 200), 1);
            world1.addStar(s);
            world1.setFrame(50);
            world1.setRespawn(300);
            world1.setSize(750);
            Ship ship = world1.generateShip("ship");
            ship.hpdecrease();
            Assert.AreEqual(ship.getHp(), 4);
            ship.getDeath();
            Assert.AreEqual(ship.getDeath(), 0);

        }

        [TestMethod]
        public void TestShipMethod1()
        {
            world world1 = new world();
            star s = new star(0, new Vector2D(200, 200), 1);
            world1.addStar(s);
            world1.setFrame(50);
            world1.setRespawn(300);
            world1.setSize(750);
            Ship ship = world1.generateShip("ship");
            ship.hpdecrease();
            //    Assert.AreEqual(ship.getHp(), 4);
            ship.getDeath();
            //  Assert.AreEqual(ship.getDeath(), 0);

            Assert.AreEqual(ship.getName(), "ship");
            Assert.AreEqual(ship.getScore(), 0);
            Assert.AreEqual(ship.checkFire(10, 15), false);
            Assert.AreEqual(ship.checkFire(16, 15), true);


        }

        [TestMethod]
        public void TestShipMethod2()
        {
            world world1 = new world();
            star s = new star(0, new Vector2D(200, 200), 1);
            world1.addStar(s);
            world1.setFrame(50);
            world1.setRespawn(300);
            world1.setSize(750);
            Ship ship = world1.generateShip("ship");
            ship.hpdecrease();
            //    Assert.AreEqual(ship.getHp(), 4);
            ship.getDeath();
            //  Assert.AreEqual(ship.getDeath(), 0);

            Assert.AreEqual(ship.getName(), "ship");
            Assert.AreEqual(ship.getScore(), 0);
            Assert.AreEqual(ship.checkFire(10, 15), false);
            Assert.AreEqual(ship.checkFire(16, 15), true);

            ship.doOperate('L');
            world1.update();

        }

        [TestMethod]
        public void TestShipMethod3()
        {
            world world1 = new world();
            star s = new star(0, new Vector2D(200, 200), 1);
            world1.addStar(s);
            world1.setFrame(50);
            world1.setRespawn(300);
            world1.setSize(750);
            Ship ship = world1.generateShip("ship");
            ship.hpdecrease();
            //    Assert.AreEqual(ship.getHp(), 4);
            ship.getDeath();
            //  Assert.AreEqual(ship.getDeath(), 0);

            Assert.AreEqual(ship.getName(), "ship");
            Assert.AreEqual(ship.getScore(), 0);
            Assert.AreEqual(ship.checkFire(10, 15), false);
            Assert.AreEqual(ship.checkFire(16, 15), true);
            Assert.AreEqual(ship.getThrust(), false);
            Assert.AreEqual(ship.getID(), 1);
            for (int i = 0; i <= 4; i++)
            {
                ship.hpdecrease();
            }
            Assert.AreEqual(ship.checkFire(16, 1), false);


            ship.doOperate('L');
            ship.doOperate('R');
            ship.refresh();
            ship.doOperate('T');
            ship.refresh();
            ship.getdir();
            ship.increaseScore();
            Assert.AreEqual(ship.getScore(), 1);
            ship.respawn();
            world1.update();

        }
        [TestMethod]
        public void TestShipMethod4()
        {
            Ship s = new Ship(1, new Vector2D(0, 0), new Vector2D(0, 0), false, "s", 5, 0);
            String A = s.ToString();
            Ship s1 = JsonConvert.DeserializeObject<Ship>(A);
            Assert.AreEqual(s1.getID(), 1);
            Assert.AreEqual(s1.getName(), "s");
            s1.wrapAround(true);
            s1.setLoc(new Vector2D(0, 0));
            s1.setLost();
            s1.setThrust();
            s1.wrapAround(false);
        }

        [TestMethod]
        public void TestStarMethod()
        {
            world world1 = new world();
            star s = new star(0, new Vector2D(200, 200), 1);
            world1.addStar(s);
            world1.setFrame(50);
            world1.setRespawn(300);
            world1.setSize(750);
            world1.update();
            s.update(51);
            s.update(102);
            s.update(160);
            s.update(230);
        }

        [TestMethod]
        public void TestStar1Method()
        {
            world world1 = new world();
            star s = new star(0, new Vector2D(0, -200), 1);
            s.setLoc(new Vector2D(0, 0));
            Assert.AreEqual(new Vector2D(0,0),s.getloc());
            world1.addStar(s);
            world1.setFrame(50);
            world1.setRespawn(300);
            world1.setSize(750);
            world1.update();
            s.update(51);
            s.update(102);
            s.update(160);
            s.update(230);
        }
        [TestMethod]
        public void TestStar2Method()
        {
            world world1 = new world();
            star s = new star(0, new Vector2D(0, 200), 1);
            String A = s.ToString();
            star s1 = JsonConvert.DeserializeObject<star>(A);
            world1.addStar(s);
            world1.setFrame(50);
            world1.setRespawn(300);
            world1.setSize(750);
            world1.update();
            s.update(51);
            s.update(102);
            s.update(160);
            s.update(230);
        }

        [TestMethod]
        public void TestProjectileMethod()
        {
            world world1 = new world();

            projectile p = new projectile(0, new Vector2D(0, 0), new Vector2D(0, 0), true, 1);
            star s = new star(0, new Vector2D(0, 0), 1);
            world1.addStar(s);
            String A =p.ToString();
            projectile s1 = JsonConvert.DeserializeObject<projectile>(A);
            world1.setFrame(50);
            world1.setRespawn(300);
            world1.setSize(750);
            world1.update();
            Assert.AreEqual(p.getID(), 0);
            Assert.AreEqual(new Vector2D(0, 0), p.getloc());
            Assert.AreEqual(new Vector2D(0, 0), p.getdir());
            Assert.AreEqual(p.checkAlive(), true);
            Assert.AreEqual(p.getOwner(), 1);
             p = new projectile(0, new Vector2D(0, 0), new Vector2D(0, 0), true, 1);
            p.update(750,world1.getStar().Values);
            p.die();

            projectile p1 = new projectile(1, new Vector2D(0, 376), new Vector2D(0, 0), true, 2);
            p1.update(750, world1.getStar().Values);
        }

        [TestMethod]
        public void TestWorldMethod()
        {
            world world1 = new world();
            star s = new star(0, new Vector2D(0, 200), 1);
            projectile p1 = new projectile(1, new Vector2D(0, 376), new Vector2D(0, 0), true, 2);
            Ship ship = new Ship(1, new Vector2D(0, 0), new Vector2D(0, 0), false, "s", 5, 0);
            String A = s.ToString();
            star s1 = JsonConvert.DeserializeObject<star>(A);
            world1.addStar(s);
            world1.setFrame(0);
            world1.addShip(ship);
            world1.Fire(1);
          
            world1.addproj(p1);
            world1.setRespawn(300);
            world1.setSize(750);
           
         Assert.AreEqual( world1.getProj().Values.Count,1);
            Assert.AreEqual(world1.getShip().Values.Count, 1);
            Assert.AreEqual(world1.getStar().Values.Count, 1);
            ship = new Ship(1, new Vector2D(0, 0), new Vector2D(0, 0), true, "s", 5, 0);
            for (int i = 0; i <= 4; i++)
            {
                world1.getShip()[1].hpdecrease();
            }
            world1.respawn(ship);
            world1.addLostID(1);
            world1.cleanup();
            world1.removeProjectile(p1);
            world1.removeStar(s);
            world1.removeShip(ship);
            Assert.AreEqual(world1.getShip().Values.Count, 0);
            Assert.AreEqual(world1.getShip().Values.Count, 0);
            Assert.AreEqual(world1.getStar().Values.Count, 0);
            world1.update();
            s.update(51);
            s.update(102);
            s.update(160);
            s.update(230);
        }

        [TestMethod]
        public void TestworldMethod1()
        {
            world world1 = new world();
            star s = new star(0, new Vector2D(0, -200), 1);
            Ship ship = new Ship(1, new Vector2D(0, 0), new Vector2D(0, 0), false, "s", 5, 0);
            projectile p1 = new projectile(1, new Vector2D(0, 10), new Vector2D(0, 0), true, 2);
            world1.addproj(p1);
            world1.addShip(ship);
           
            s.setLoc(new Vector2D(0, 0));
            Assert.AreEqual(new Vector2D(0, 0), s.getloc());
            world1.addStar(s);
            world1.setFrame(50);
            world1.setRespawn(300);
            world1.setSize(750);
            world1.update();
            s.update(51);
            s.update(102);
            s.update(160);
            s.update(230);
        }

    }
}
