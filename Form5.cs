using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace TuioDemo
{
    public partial class Form5 : Form
    {
        string[] x;
        public Form5(string text)
        {
            InitializeComponent();
            x = text.Split(',');
            this.Load += Form5_Load;
            this.Paint += Form5_Paint;
        }

        private void Form5_Paint(object sender, PaintEventArgs e)
        {
            DrawScene(e.Graphics);
        }
        
        private void Form5_Load(object sender, EventArgs e)
        {
            label1.Text = x[0];
        }

        public void label1_Click(object sender, EventArgs e)
        {

        }
        void DrawScene(Graphics g)
        {
            int y = 50;
            g.Clear(Color.Cyan);
            for(int i =1; i<x.Length;i++)
            {
                g.DrawString(x[i], new Font("Arial", 18), Brushes.Black, 10 , 100 + y);
                y += 50;
            }
        }
    }
}

