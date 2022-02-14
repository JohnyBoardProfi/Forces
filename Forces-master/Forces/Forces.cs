using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

namespace Forces
{
    public partial class Forces : Form
    {
        public Vector Gravity, Buyoancy, R = new Vector(0, 0), V, AddedBuyoancy, AddedGravity, G = new Vector(0, 981);
        public double areaDensity = 1, itemDensity = 1, addedVolume = 0, addedMass = 0;
        Item item;
        Dictionary<string, double> PlanetG = new Dictionary<string, double>
        {
            ["Earth"] = 9.81,
            ["Moon"] = 1.62,
            ["Venus"] = 8.88,
            ["Jupiter"] = 24.79,
            ["Uranium"] = 8.86,
            ["Eride"] = 0.84,
            ["Sun"] = 273.1,
            ["Mercury"] = 3.7,
            ["Мars"] = 3.86,
            ["Saturn"] = 10.44,
            ["Neptune"] = 11.09,
            ["Pluto"] = 0.617,
            ["Europe"] = 1.315,
            ["Ganimed"] = 1.428,
            ["Titanium"] = 1.352,
            ["Triton"] = 0.77
        };
        Entity[] entities =
        {
            new Entity("Gold", 19300, @"Images\Gold.png"),
            new Entity("Lead", 11300, @"Images\Lead.png"),
            new Entity("Silver", 10500, @"Images\Silver.png"),
            new Entity("Porcelain", 2360, @"Images\Porcelain.png"),
            new Entity("Ice", 900, @"Images\Ice.png"),
            new Entity("Brick", 1800, @"Images\Brick.png"),
            new Entity("Platinum", 23500, @"Images\Platinum.png"),
            new Entity("Diamond", 3500, @"Images\Diamond.png"),           
            new Entity("Gypsum", 2300, @"Images\Gypsum.png"),            
            new Entity("Amber", 1100, @"Images\Amber.png"),
            new Entity("Coal", 1450, @"Images\Coal.png"),
            new Entity("Butter", 865, @"Images\Butter.png"),
            new Entity("Brass", 8500, @"Images\Brass.png"),
            new Entity("Quartz", 2650, @"Images\Quartz.png")
        };
        Area[] areas =
        {
            new Area("Honey", 1450, Color.Yellow),
            new Area("Mercury", 13600, Color.Gray),
            new Area("Water", 1000, Color.Aqua),
            new Area("Мilk", 1030, Color.White),
            new Area("Petrol", 900, Color.Violet),
            new Area("Beer", 1010, Color.Gold),
            new Area("Oil", 800, Color.Black),
            new Area("Whale oil", 925, Color.LightGoldenrodYellow),
            new Area("Linseed oil", 940, Color.Wheat),
            new Area("Sunflower oil", 920, Color.Olive),
            new Area("Diesel fuel", 885, Color.Wheat)
        };

        public Forces()
        {
            InitializeComponent();
            item = new Item(new Vector(pbItem.Left, pbItem.Top), new Vector(0, 0), 1, 1);
            string[] Planets = ((IEnumerable<string>)PlanetG.Keys).ToArray(), Areas = areas.Select(x => x.Name).ToArray(), Entities = entities.Select(x => x.Name).ToArray();
            cbPlanets.Items.AddRange(Planets);
            cbAreas.Items.AddRange(Areas);
            cbEntities.Items.AddRange(Entities);
            pbBalloon.Hide();
            pbWeight.Hide();
            lblInfo.Text = "";
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            double dt = timer.Interval / 1000.0;
            item.Mass = item.Volume * itemDensity;
            Gravity = item.Mass * G;
            Buyoancy = -(areaDensity * G) * item.Volume;
            AddedBuyoancy = -(areaDensity * G) * addedVolume;
            AddedGravity = addedMass * G;
            item.Move(dt, Gravity + Buyoancy + AddedBuyoancy + AddedGravity);
            pbItem.Location = new Point(Convert.ToInt32(item.R.X), Convert.ToInt32(item.R.Y));
            pbBalloon.Top = pbItem.Top - pbBalloon.Height;
            pbWeight.Top = pbItem.Top + pbItem.Height;
            double M = item.Mass + addedMass, V = item.Volume + addedVolume;
            Vector F1 = Gravity + AddedGravity, F2 = Buyoancy + AddedBuyoancy;
            lblMass.Text = "Summary mass: " + M + " kg";
            lblVolume.Text = "Summary volume: " + V + " м ^ 3";
            lblItemDensity.Text = "Body density: " + itemDensity + " kg / м ^ 3";
            lblAreaDensity.Text = "Area density: " + areaDensity + " kg / м ^ 3";
            lblGravity.Text = "Force of Gravity: " + F1.Y + " H";
            lblBuyoancy.Text = "Archimed Force: " + Math.Abs(F2.Y) + " H";
            lblG.Text = "Acceleration of gravity: " + G.Y + " H / kg";
        }

        private void cbPlanets_SelectedIndexChanged(object sender, EventArgs e) => G = new Vector(0, PlanetG[(string)cbPlanets.SelectedItem]);

        private void cbAreas_SelectedIndexChanged(object sender, EventArgs e)
        {
            Area area = areas.FirstOrDefault(a => a.Name == cbAreas.SelectedItem.ToString());
            areaDensity = area.Density;
            pnlField.BackColor = area.Color;
        }

        private void cbEntities_SelectedIndexChanged(object sender, EventArgs e)
        {
            Entity entity = entities.FirstOrDefault(a => a.Name == cbEntities.SelectedItem.ToString());
            itemDensity = entity.Density;
            pbItem.Image = Image.FromFile(entity.Image);
        }

        private void tbAddedVolume_Scroll(object sender, EventArgs e)
        {
            if (tbAddedVolume.Value > 30)
            {
                addedVolume = tbAddedVolume.Value;
                pbBalloon.Size = new Size((int)(addedVolume), (int)(addedVolume));
                pbBalloon.Show();
            }
            else
            {
                addedVolume = 0;
                pbBalloon.Hide();
            }
        }

        private void tbVolume_Scroll(object sender, EventArgs e)
        {
            item.Volume = tbVolume.Value;
            pbItem.Size = new Size((int)(item.Volume), (int)(item.Volume));
        }
        
        private void tbAddedMass_Scroll(object sender, EventArgs e)
        {
            if (tbAddedMass.Value > 30)
            {
                addedMass = tbAddedMass.Value;                
                pbWeight.Size = new Size((int)addedMass, (int)addedMass);
                pbWeight.Show();
            }
            else
            {
                addedMass = 0;
                pbWeight.Hide();
            }
        }

        private void btnStart_Click(object sender, EventArgs e) => timer.Start();

        private void btnStart_MouseEnter(object sender, EventArgs e) => btnStart.BackColor = Color.Green;

        private void btnStart_MouseLeave(object sender, EventArgs e) => btnStart.BackColor = Color.White;

        private void btnEnd_Click(object sender, EventArgs e) => this.Close();

        private void btnEnd_MouseEnter(object sender, EventArgs e) => btnEnd.BackColor = Color.Red;

        private void btnEnd_MouseLeave(object sender, EventArgs e) => btnEnd.BackColor = Color.White;

        private void cbAreas_MouseEnter(object sender, EventArgs e) => lblInfo.Text = "Area density";

        private void cbAreas_MouseLeave(object sender, EventArgs e) => lblInfo.Text = "";

        private void cbEntities_MouseEnter(object sender, EventArgs e) => lblInfo.Text = "Body density";

        private void pbBalloon_MouseEnter(object sender, EventArgs e) => lblInfo.Text = "Additional volume";

        private void pbBalloon_MouseLeave(object sender, EventArgs e) => lblInfo.Text = "";

        private void pbItem_MouseEnter(object sender, EventArgs e) => lblInfo.Text = "Body";

        private void pbItem_MouseLeave(object sender, EventArgs e) => lblInfo.Text = "";

        private void pbWeight_MouseEnter(object sender, EventArgs e) => lblInfo.Text = "Additional mass"; 

        private void pbWeight_MouseLeave(object sender, EventArgs e) => lblInfo.Text = "";

        private void cbEntities_MouseLeave(object sender, EventArgs e) => lblInfo.Text = "";

        private void cbPlanets_MouseEnter(object sender, EventArgs e) => lblInfo.Text = "Planet";

        private void cbPlanets_MouseLeave(object sender, EventArgs e) => lblInfo.Text = "";

        private void tbAddedVolume_MouseEnter(object sender, EventArgs e) => lblInfo.Text = "Additional volume";

        private void tbAddedVolume_MouseLeave(object sender, EventArgs e) => lblInfo.Text = "";

        private void tbVolume_MouseEnter(object sender, EventArgs e) => lblInfo.Text = "Body volume";

        private void tbVolume_MouseLeave(object sender, EventArgs e) => lblInfo.Text = "";

        private void tbAddedMass_MouseEnter(object sender, EventArgs e) => lblInfo.Text = "Additional mass";

        private void tbAddedMass_MouseLeave(object sender, EventArgs e) => lblInfo.Text = "";
    }
}