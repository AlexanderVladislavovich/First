using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace First
{
    public partial class Form1 : Form
    {
        Bitmap image;
        public Form1()
        {
            InitializeComponent();
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image files | *.png; *.jpg; *.bmp | All Files ('*') | *.*";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                image = new Bitmap(dialog.FileName);
                pictureBox1.Image = image;
                pictureBox1.Refresh();  
            }
        }

        private void инверсияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InvertFilter filter = new InvertFilter(image);
            Bitmap resultImage = filter.processImage();
            pictureBox1.Image = resultImage;
            pictureBox1.Refresh();
        }

        private void линрастгистогрToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LinearHistogramStretch filter = new LinearHistogramStretch(image);
            Bitmap resultImage = filter.processImage();
            pictureBox1.Image = resultImage;
            pictureBox1.Refresh();
        }

        private void размытыеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new BlurFilter(image);
            Bitmap resultImage = filter.processImage();
            pictureBox1.Image = resultImage;
            pictureBox1.Refresh();
            
        }

        private void идеальныйОтражательToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new PerfectReflector(image);
            Bitmap resultImage = filter.processImage();
            pictureBox1.Image = resultImage;
            pictureBox1.Refresh();
        }

        private void резкостьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new Sharpness(image);
            Bitmap resultImage = filter.processImage();
            pictureBox1.Image = resultImage;
            pictureBox1.Refresh();
        }

        private void выделениеГраницToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new Bordering1(image);
            Bitmap resultImage = filter.processImage();
            pictureBox1.Image = resultImage;
            pictureBox1.Refresh();
            image = resultImage;
        }

        private void медианныйФильтр1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new MedianFilter1(image);
            Bitmap resultimage = filter.processImage();
            pictureBox1.Image = resultimage;
            pictureBox1.Refresh();
        }

        private void медианныйФильтр2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new MedianFilter2(image);
            Bitmap resultimage = filter.processImage();
            pictureBox1.Image = resultimage;
            pictureBox1.Refresh();
        }

        private void увеличениеЯркостиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new BrightnessFilter(image);
            Bitmap resultimage = filter.processImage();
            pictureBox1.Image = resultimage;
            pictureBox1.Refresh();
        }

        private void чБToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new GreyFilter(image);
            Bitmap resultimage = filter.processImage();
            pictureBox1.Image = resultimage;
            pictureBox1.Refresh();
        }

        private void стеклоToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new GlassFilter(image);
            Bitmap resultimage = filter.processImage();
            pictureBox1.Image = resultimage;
            pictureBox1.Refresh();
        }

        private void dilationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new DilationFilter(image);
            Bitmap resultimage = filter.processImage();
            pictureBox1.Image = resultimage;
            pictureBox1.Refresh();

        }

        private void erosionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new ErosionFilter(image);
            Bitmap resultimage = filter.processImage();
            pictureBox1.Image = resultimage;
            pictureBox1.Refresh();
        }

        private void blackHatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new BlackHatFilter(image);
            Bitmap resultimage = filter.processImage();
            pictureBox1.Image = resultimage;
            pictureBox1.Refresh();
        }

        private void topHatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new TopHatFilter(image);
            Bitmap resultimage = filter.processImage();
            pictureBox1.Image = resultimage;
            pictureBox1.Refresh();
        }

        private void gradToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new GradFilter(image);
            Bitmap resultimage = filter.processImage();
            pictureBox1.Image = resultimage;
            pictureBox1.Refresh();
        }
    }
}
