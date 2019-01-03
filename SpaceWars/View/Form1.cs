using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GameController;
using World;
using ship;
using Projectile;
using Star;
/// <summary>
///   connect button to connect to the server
/// </summary>
namespace View
{
    public partial class SpaceWars : Form
    {
        private GController gc;// connect to the game
        private DrawingPanel panel;
        private ScorePanel score;
        private bool isLeft=false;
        private bool isRight;
        private bool isThrust;
        private bool isFire;
        //constructor, initlize the event handlers
        public SpaceWars()
        {
            InitializeComponent();
            gc = new GController();

            gc.RegisterTick(redraw);
            gc.RegisterKey(HandleKeyEvent);
            gc.RegisterStep(getStep);
            gc.RegisterResize(resize);
            panel = new DrawingPanel(gc.getWorld());
            score = new ScorePanel(gc.getWorld());
           
            gc.RegisterShip(score.addShips);
            gc.RegisterDie(score.DiedShips);
            panel.Location = new System.Drawing.Point(0, 40);
            panel.Size = new Size(750, 750);
            this.Size = new Size(1000, 835);
            panel.BackColor = Color.Black;
            


            this.Controls.Add(panel);
            isLeft = false;
            isRight = false;
            isThrust = false;
            isFire = false;
        }
/// <summary>
/// resize the panel after receiving startup information from server
/// this method will be used as the event passing to the Game controller 
/// </summary>
        public void resize()
        {
            MethodInvoker invoke = new MethodInvoker(
                () => {
                    panel.Size = new Size(gc.getSize(),gc.getSize());

                    this.Size = new Size(gc.getSize()+250,gc.getSize()+85);

                    score.Location = new Point(gc.getSize(),40);
                    score.Size = new Size(230,gc.getSize());
                    score.BackColor = Color.White;
                    this.Controls.Add(score);
                }
                );
            this.Invoke(invoke);//create a new thread for the current form 
        }
        /// <summary>
        /// connect to pass the ip address and player name to the server 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void connect_Click(object sender, EventArgs e)
        {
            gc.setName(PlayerName.Text);
            gc.ConnectButton(IP.Text);
            
        }
        /// <summary>
        /// redraw the form 
        /// </summary>
        private void redraw()
        {
            try
            {
                MethodInvoker invokes = new MethodInvoker(() => this.Invalidate(true));

                this.Invoke(invokes);
            }
            catch(Exception e) { }
        }
        /// <summary>
        /// disable the kep
        /// </summary>
        public void getStep()
        {
            MethodInvoker invokes = new MethodInvoker(() =>
         {
             IP.Enabled = false;
             connect.Enabled = false;
             PlayerName.Enabled = false;
             helpButton.Enabled = false;
         });
            this.Invoke(invokes);
        }
        /// <summary>
        /// keep track of the key press information
        /// </summary>
        public void HandleKeyEvent()
        {
            StringBuilder sb = new StringBuilder();
            if (this.isLeft == true)
            {
                sb.Append("L");
            }
            if (this.isRight == true)
            {
                sb.Append("R");
            }
            if (this.isThrust == true)
            {
                sb.Append("T");
            }
            if (this.isFire == true)
            {
                sb.Append("F");
            }
            gc.sendData("(" + sb.ToString() + ")");



        }
        /// <summary>
        /// handle the kep press if the key is pressing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                if (this.isLeft == false)
                {
                    this.isLeft = true;
                }
            }
            if (e.KeyCode == Keys.Right)
            {
                if (this.isRight == false)
                {
                    this.isRight = true;
                }
            }
            if (e.KeyCode == Keys.Space)
            {
                if (this.isFire == false)
                {
                    this.isFire = true;
                }
            }
            if (e.KeyCode == Keys.Up)
            {
                if (this.isThrust == false)
                {
                    this.isThrust = true;
                }
            }
        }
        /// <summary>
        /// handle the key if the key is not pressing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                if (this.isLeft == true)
                {
                    this.isLeft = false;
                }
            }
            if (e.KeyCode == Keys.Right)
            {
                if (this.isRight == true)
                {
                    this.isRight = false;
                }
            }
            if (e.KeyCode == Keys.Space)
            {
                if (this.isFire == true)
                {
                    this.isFire = false;
                }
            }
            if (e.KeyCode == Keys.Up)
            {
                if (this.isThrust == true)
                {
                    this.isThrust = false;
                }
            }
        }
        /// <summary>
        /// click the help button to show the information
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void helpButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("ip address:spacewars.eng.utah.edu+playerId:anything you like");
        }

       
    }
    /// <summary>
    /// drawing the panel 
    /// </summary>
    public class DrawingPanel : Panel
    {
        private world theWorld;
        HashSet<Image> ships;
        HashSet<Image> thrusts;
        HashSet<Image> shoots;
        Image star;
        Image[] arrays;
        Image[] arrays1;
        Image[] arrays2;
        public DrawingPanel(world w)
        {
            DoubleBuffered = true;
            theWorld = w;


            ships = new HashSet<Image>();
            thrusts = new HashSet<Image>();
            shoots = new HashSet<Image>();
            star = Image.FromFile("..//..//..//Resources//Images//star.jpg");
          //import the images from the resources file
            Image ship1 = Image.FromFile("..//..//..//Resources//Images//ship-coast-blue.png");
            Image ship2 = Image.FromFile("..//..//..//Resources//Images//ship-coast-brown.png");
            Image ship3 = Image.FromFile("..//..//..//Resources//Images//ship-coast-green.png");
            Image ship4 = Image.FromFile("..//..//..//Resources//Images//ship-coast-grey.png");
            Image ship5 = Image.FromFile("..//..//..//Resources//Images//ship-coast-red.png");
            Image ship6 = Image.FromFile("..//..//..//Resources//Images//ship-coast-violet.png");
            Image ship7 = Image.FromFile("..//..//..//Resources//Images//ship-coast-white.png");
            Image ship8 = Image.FromFile("..//..//..//Resources//Images//ship-coast-yellow.png");
            Image ship9 = Image.FromFile("..//..//..//Resources//Images//ship-thrust-blue.png");
            Image ship10 = Image.FromFile("..//..//..//Resources//Images//ship-thrust-brown.png");
            Image ship11 = Image.FromFile("..//..//..//Resources//Images//ship-thrust-green.png");
            Image ship12 = Image.FromFile("..//..//..//Resources//Images//ship-thrust-grey.png");
            Image ship13 = Image.FromFile("..//..//..//Resources//Images//ship-thrust-red.png");
            Image ship14 = Image.FromFile("..//..//..//Resources//Images//ship-thrust-violet.png");
            Image ship15 = Image.FromFile("..//..//..//Resources//Images//ship-thrust-white.png");
            Image ship16 = Image.FromFile("..//..//..//Resources//Images//ship-thrust-yellow.png");
            ships.Add(ship1);
            ships.Add(ship2);
            ships.Add(ship3);
            ships.Add(ship4);
            ships.Add(ship5);
            ships.Add(ship6);
            ships.Add(ship7);
            ships.Add(ship8);
            arrays = ships.ToArray();
            thrusts.Add(ship9);
            thrusts.Add(ship10);
            thrusts.Add(ship11);
            thrusts.Add(ship12);
            thrusts.Add(ship13);
            thrusts.Add(ship14);
            thrusts.Add(ship15);
            thrusts.Add(ship16);
            arrays1 = thrusts.ToArray();
            Image ship17 = Image.FromFile("..//..//..//Resources//Images//shot-blue.png");
            Image ship18 = Image.FromFile("..//..//..//Resources//Images//shot-brown.png");
            Image ship19 = Image.FromFile("..//..//..//Resources//Images//shot-green.png");
            Image ship20 = Image.FromFile("..//..//..//Resources//Images//shot-grey.png");
            Image ship21 = Image.FromFile("..//..//..//Resources//Images//shot-red.png");
            Image ship22 = Image.FromFile("..//..//..//Resources//Images//shot-violet.png");
            Image ship23 = Image.FromFile("..//..//..//Resources//Images//shot-white.png");
            Image ship24 = Image.FromFile("..//..//..//Resources//Images//shot-yellow.png");
            shoots.Add(ship17);
            shoots.Add(ship18);
            shoots.Add(ship19);
            shoots.Add(ship20);
            shoots.Add(ship21);
            shoots.Add(ship22);
            shoots.Add(ship23);
            shoots.Add(ship24);
            arrays2 = shoots.ToArray();




        }

        /// <summary>
        /// Helper method for DrawObjectWithTransform
        /// </summary>
        /// <param name="size">The world (and image) size</param>
        /// <param name="w">The worldspace coordinate</param>
        /// <returns></returns>
        private static int WorldSpaceToImageSpace(int size, double w)
        {
            return (int)w + size / 2;
        }

        // A delegate for DrawObjectWithTransform
        // Methods matching this delegate can draw whatever they want using e  
        public delegate void ObjectDrawer(object o, PaintEventArgs e);


        /// <summary>
        /// This method performs a translation and rotation to drawn an object in the world.
        /// </summary>
        /// <param name="e">PaintEventArgs to access the graphics (for drawing)</param>
        /// <param name="o">The object to draw</param>
        /// <param name="worldSize">The size of one edge of the world (assuming the world is square)</param>
        /// <param name="worldX">The X coordinate of the object in world space</param>
        /// <param name="worldY">The Y coordinate of the object in world space</param>
        /// <param name="angle">The orientation of the objec, measured in degrees clockwise from "up"</param>
        /// <param name="drawer">The drawer delegate. After the transformation is applied, the delegate is invoked to draw whatever it wants</param>
        private void DrawObjectWithTransform(PaintEventArgs e, object o, int worldSize, double worldX, double worldY, double angle, ObjectDrawer drawer)
        {
            // Perform the transformation
            int x = WorldSpaceToImageSpace(worldSize, worldX);
            int y = WorldSpaceToImageSpace(worldSize, worldY);
            e.Graphics.TranslateTransform(x, y);
            e.Graphics.RotateTransform((float)angle);
            // Draw the object 
            drawer(o, e);
            // Then undo the transformation
            e.Graphics.ResetTransform();
        }

       

        /// <summary>
        /// Acts as a drawing delegate for DrawObjectWithTransform
        /// After performing the necessary transformation (translate/rotate)
        /// DrawObjectWithTransform will invoke this method
        /// </summary>
        /// <param name="o">The object to draw</param>
        /// <param name="e">The PaintEventArgs to access the graphics</param>
        private void drawShip(object o, PaintEventArgs e)
        {
            int shipWidth = 35;
            Ship s = o as Ship;
            Image image = null;
            if (s.getThrust() == false)
            {
                image = arrays[s.getID()%arrays.Length];
            }
            else
            {
                image = arrays1[s.getID()%arrays1.Length];
            }
            e.Graphics.DrawImage(image, 0 - (shipWidth / 2), 0 - (shipWidth / 2), shipWidth, shipWidth);
        }
        /// <summary>
        /// drawing the star
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        private void drawStar(object o, PaintEventArgs e)
        {
            int starWidth = 35;
            star s = o as star;
            Image image = Image.FromFile("../../../Resources/Images/star.jpg");
            e.Graphics.DrawImage(image, 0 - (starWidth / 2), 0 - (starWidth / 2), starWidth+5, starWidth+5);
        }
        /// <summary>
        /// drawing the projectile 
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        private void drawProjectile(object o, PaintEventArgs e)
        {
            int projWidth = 20;
            projectile s = o as projectile;
            
            Image image = arrays2[s.getOwner()%arrays2.Length];
            e.Graphics.DrawImage(image, 0 - (projWidth / 2), 0 - (projWidth / 2), projWidth, projWidth);
        }


        // This method is invoked when the DrawingPanel needs to be re-drawn
        protected override void OnPaint(PaintEventArgs e)
        {
            // Draw the players
            lock (theWorld)
            {
                foreach (Ship play in theWorld.getShip().Values)
                {
                    DrawObjectWithTransform(e, play, this.Size.Width, play.getloc().GetX(), play.getloc().GetY(), play.getdir().ToAngle(), drawShip);
                }
                foreach (star play in theWorld.getStar().Values)
                {
                    DrawObjectWithTransform(e, play, this.Size.Width, play.getloc().GetX(), play.getloc().GetY(), 0, drawStar);
                }
                foreach (projectile play in theWorld.getProj().Values)
                {
                    DrawObjectWithTransform(e, play, this.Size.Width, play.getloc().GetX(), play.getloc().GetY(), play.getdir().ToAngle(), drawProjectile);
                }
            }

            // Do anything that Panel (from which we inherit) needs to do
            base.OnPaint(e);
        }

    }
    /// <summary>
    /// properties for the panel
    /// </summary>

    public class ScorePanel : Panel
    {

        private world theWorld;
        private List<int> ships;

        public ScorePanel(world w)
        {
            this.DoubleBuffered = true;
            theWorld = w;
            ships = new List<int>();
        }   
        public void DiedShips(Ship s)
        {
            lock (ships)
            {
                ships.Remove(s.getID());
            }
            sorted();
        }
        public void addShips(Ship s)
        {
            lock (ships)
            {
                ships.Add(s.getID());
            }
            sorted();
        }
        /// <summary>
        /// sort the score 
        /// </summary>
        private void sorted()
        {
            {
                lock (ships)
                {
                    ships.Sort(Comparer<int>.Create((x, y) => theWorld.getShip()[y].getScore().CompareTo(theWorld.getShip()[x].getScore())));
                }
            }
        }
        /// <summary>
        /// paint the panel
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (theWorld != null)
            {    
                lock (ships)
                {
                    int y = 0;
                    foreach (int shipID in ships)
                    {
                        String drawString = theWorld.getShip()[shipID].getName()+" : "+ theWorld.getShip()[shipID].getScore();

                                    // Create font and brush.
                        Font drawFont = new Font("Arial", 16);
                        SolidBrush drawBrush = new SolidBrush(Color.Black);
                        SolidBrush greenBrush = new SolidBrush(Color.Blue);
                        // Create point for upper-left corner of drawing.

                        Pen blackPen = new Pen(Color.Black);


                        // Set format of string.
                        StringFormat drawFormat = new StringFormat();
                                   // drawFormat.FormatFlags = StringFormatFlags.DirectionVertical;

                                    // Draw string to screen.
                        e.Graphics.DrawString(drawString, drawFont, drawBrush, 0, y, drawFormat);
                        e.Graphics.DrawRectangle(blackPen, new Rectangle(3, y + 20, this.Size.Width-10 , 14));
                        e.Graphics.FillRectangle(greenBrush, new Rectangle(5, y + 22, (int)(((float)this.Size.Width - 14) * (theWorld.getShip()[shipID].getHp() / 5.0)), 11));

                        y += 60;
                    }
                 }
            }
           
            base.OnPaint(e);
        }

    }


}
