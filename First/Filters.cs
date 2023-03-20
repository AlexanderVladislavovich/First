using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace First
{
    abstract class Filters
    {
        public Bitmap sourceImage;

        public int Clamp (int value, int min, int max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        protected abstract Color calculateNewPixelColor(int x, int y);

        public Bitmap processImage()
        {
            Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);
            for (int i = 0; i < sourceImage.Width; i++)
            {
                for (int j = 0; j < sourceImage.Height; j++)
                {
                    resultImage.SetPixel(i, j, calculateNewPixelColor(i, j)); 
                }
            }
            return resultImage;

        }
    }
    class InvertFilter : Filters
    {
        public InvertFilter(Bitmap _sourceImage)
        {
            sourceImage = _sourceImage;
        }
        protected override Color calculateNewPixelColor(int x, int y)
        {
            Color sourceColor  = sourceImage.GetPixel(x, y);
            Color resutColor = Color.FromArgb(255 - sourceColor.R, 255 - sourceColor.G, 255 - sourceColor.B);
            return resutColor;
        }
    }

    class GlassFilter : Filters
    { 
        public GlassFilter(Bitmap _sourceimage)
        {
            sourceImage = _sourceimage;
        }
        protected override Color calculateNewPixelColor(int x, int y)
        {
            Random r1 = new Random();
            Random r2 = new Random();
            int rand1 = (int)((r1.NextDouble() - 0.5) * 10);
            int rand2 = (int)((r2.NextDouble() - 0.5) * 10);
            int coordx = Clamp(x + rand1, 0, sourceImage.Width - 1);
            int coordy = Clamp(y + rand2, 0, sourceImage.Height - 1);
            return sourceImage.GetPixel(coordx, coordy);
        }

    }


    class BrightnessFilter : Filters
    {
        public BrightnessFilter(Bitmap _sourceimage)
        {
            sourceImage = _sourceimage;
        }
        protected override Color calculateNewPixelColor(int x, int y)
        {
            int sourceR = (int)(sourceImage.GetPixel(x, y).R * 1.3);
            int sourceG = (int)(sourceImage.GetPixel(x, y).G * 1.3);
            int sourceB = (int)(sourceImage.GetPixel(x, y).B * 1.3);

            return Color.FromArgb(Clamp(sourceR , 0, 255), Clamp(sourceG, 0 ,255), Clamp(sourceB, 0, 255));
        }
    }

    class GreyFilter : Filters
    {
        public GreyFilter(Bitmap _sourceimage)
        {
            sourceImage = _sourceimage;
        }

        protected override Color calculateNewPixelColor(int x, int y)
        {
            double avg = Math.Sqrt(Math.Pow(sourceImage.GetPixel(x, y).R, 2) +Math.Pow(sourceImage.GetPixel(x, y).G, 2) + Math.Pow(sourceImage.GetPixel(x, y).B, 2 ));
            return Color.FromArgb((int)(avg / 1.73), (int)(avg / 1.73), (int)(avg / 1.73));
        }

    }


    class LinearHistogramStretch : Filters
    {
        int[] histo = new int[256];
        int Ymin = 256, Ymax = 0;

        public LinearHistogramStretch(Bitmap _sourceImage)
        {
            sourceImage = _sourceImage;
            histo = get_histo();
        }

        int[] get_histo()
        {
            int[] histo = new int[256];
            double Y;
            Color pix;
            Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);
            for (int i = 0; i < sourceImage.Width; i++)
            {
                for (int j = 0; j < sourceImage.Height; j++)
                {
                    pix = sourceImage.GetPixel(i, j);
                    Y = pix.R * 0.3 + pix.G * 0.59 + pix.B * 0.11;
                    histo[(int)Y]++;
                    Ymax = Math.Max((int)Y, Ymax);
                    Ymin = Math.Min((int)Y, Ymin);
                }
            }
            return histo;
        }

        protected override Color calculateNewPixelColor(int i, int j)
        {
            Color pix = sourceImage.GetPixel(i, j);
            double y = pix.R * 0.3 + pix.G * 0.59 + pix.B * 0.11;
            double x = (y - Ymin) * (255/(Ymax - Ymin));
            Color resutColor;
            if (x > y)
            {
                int temp = (int)(x - y);
                resutColor = Color.FromArgb(Clamp(pix.R + temp, 0, 255), Clamp(pix.G + temp, 0, 255), Clamp(pix.B + temp, 0, 255));
            }
            else
            {
                int temp = (int)(y - x);
                resutColor = Color.FromArgb(Clamp(pix.R - temp, 0, 255), Clamp(pix.G - temp, 0, 255), Clamp(pix.B - temp, 0, 255));
            }
            return resutColor;
        }
    }

    class PerfectReflector : Filters
    {
        int Rmax = 0, Gmax = 0, Bmax = 0;
        public PerfectReflector(Bitmap _sourceImage)
        {
            sourceImage = _sourceImage;
            Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);
            Color pix;
            for (int i = 0; i < sourceImage.Width; i++)
            {
                for (int j = 0; j < sourceImage.Height; j++)
                {
                    pix = sourceImage.GetPixel(i, j);
                    Rmax = Math.Max(Rmax, pix.R);
                    Gmax = Math.Max(Gmax, pix.G);
                    Bmax = Math.Max(Bmax, pix.B);
                }
            }
        }

        protected override Color calculateNewPixelColor(int i, int j)
        {
            Color pix = sourceImage.GetPixel(i, j);
            Color resutColor = Color.FromArgb(Clamp(pix.R*255/Rmax, 0, 255), Clamp(pix.G * 255/Gmax, 0, 255), Clamp(pix.B*255/Bmax, 0, 255));
            return resutColor;
        }
    }



    class MatrixFilter : Filters
    {
        protected float[,] kernel = null;
        protected MatrixFilter() { }
        public MatrixFilter(float[,] kernel)
        {
            this.kernel = kernel;
        }
        protected override Color calculateNewPixelColor(int x, int y)
        {
            int radiusX = kernel.GetLength(0) / 2;
            int radiusY = kernel.GetLength(1) / 2;
            float resultR = 0;
            float resultG = 0;
            float resultB = 0;
            for (int l = -radiusY; l <= radiusY; l++)
            {
                for (int k = -radiusX; k <= radiusX; k++)
                {
                    int idX = Clamp(x + k, 0, sourceImage.Width - 1);
                    int idY = Clamp(y + l, 0, sourceImage.Height - 1);
                    Color neighborColor = sourceImage.GetPixel(idX, idY);
                    resultR += neighborColor.R * kernel[k + radiusX, l + radiusY];
                    resultG += neighborColor.G * kernel[k + radiusX, l + radiusY];
                    resultB += neighborColor.B * kernel[k + radiusX, l + radiusY];
                }
            }
            return Color.FromArgb(
                Clamp((int)resultR, 0, 255),
                Clamp((int)resultG, 0, 255),
                Clamp((int)resultB, 0, 255)
                );
        }
    }

    class BlurFilter : MatrixFilter
    {
        public BlurFilter(Bitmap _sourceimage)
        {
            sourceImage = _sourceimage;
            int sizeX = 3;
            int sizeY = 3;
            kernel = new float[sizeX , sizeY];
            for(int i=0;i<sizeX;i++)
            {
                for(int j=0;j<sizeY;j++)
                {
                    kernel[i, j] = 1.0f / (float)(sizeX * sizeY);
                }
            }
        }
    }

    class Sharpness : MatrixFilter
    {
        public Sharpness(Bitmap _sourceimage)
        {
            sourceImage = _sourceimage;
            int sizeX = 3;
            int sizeY = 3;
            kernel = new float[sizeX, sizeY];
            for (int i = 0; i < sizeX; i++)
            {
                for (int j = 0; j < sizeY; j++)
                {
                    kernel[i, j] = -1.0f;
                }
            }
            kernel[1, 1] = 9.0f;
        }
    }

    class Bordering1 : MatrixFilter
    {
        public Bordering1(Bitmap _sourceimage)
        {
            sourceImage = _sourceimage;
            int sizeX = 3;
            int sizeY = 3;
            kernel = new float[sizeX, sizeY];
            kernel[0, 0] = 3.0f;
            kernel[0, 1] = 10.0f;
            kernel[0, 2] = 3.0f;
            kernel[1, 0] = 0.0f;
            kernel[1, 1] = 0.0f;
            kernel[1, 2] = 0.0f;
            kernel[2, 0] = -3.0f;
            kernel[2, 1] = -10.0f;
            kernel[2, 2] = -3.0f;
        }
    }

    class DilationFilter : Filters  //расширение
    {

        int[,] structelem =
        {
            { 1, 1, 1 },
            { 1, 1, 1 },
            { 1, 1, 1 }
        };
        public DilationFilter(Bitmap _sourceimage)
        {
            sourceImage = _sourceimage;
        }

        protected override Color calculateNewPixelColor(int x, int y)
        {
            int maxR = 0;
            int maxG = 0;
            int maxB = 0;
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    int coordx = x + i;
                    int coordy = y + j;
                    
                    if ((coordx >= 0 && coordy >= 0) && (coordx < sourceImage.Width) && (coordy < sourceImage.Height) && (structelem[i + 1, j + 1] == 1))
                    {
                        Color col = sourceImage.GetPixel(coordx, coordy);
                        maxR = Math.Max(maxR, col.R);
                        maxG = Math.Max(maxG, col.G);
                        maxB = Math.Max(maxB, col.B);
                    }
                }
            }
            return Color.FromArgb(maxR, maxG, maxB);

        }
    }

    class ErosionFilter : Filters  //сжатие
    {

        int[,] structelem =
        {
            { 1, 1, 1 },
            { 1, 1, 1 },
            { 1, 1, 1 }
        };
        public ErosionFilter(Bitmap _sourceimage)
        {
            sourceImage = _sourceimage;
        }

        protected override Color calculateNewPixelColor(int x, int y)
        {
            int minR = 255;
            int minG = 255;
            int minB = 255;
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {   
                    int coordx = x + i;
                    int coordy = y + j;
                   
                    if ((coordx >= 0 && coordy >= 0) && (coordx < sourceImage.Width) && (coordy < sourceImage.Height) && (structelem[i + 1, j + 1] == 1) )
                    {
                        Color col = sourceImage.GetPixel(coordx, coordy);
                        minR = Math.Min(minR, col.R);
                        minG = Math.Min(minG, col.G);
                        minB = Math.Min(minB, col.B);
                    }
                }
            }
            return Color.FromArgb(minR, minG, minB);
        }
    }

    class TopHatFilter : Filters
    {

        protected Bitmap openedImage;
        public TopHatFilter(Bitmap _sourceimage)
        {
            sourceImage = _sourceimage;
            Filters eros = new ErosionFilter(_sourceimage);
            openedImage = eros.processImage();
            Filters eros2 = new ErosionFilter(openedImage);
            openedImage = eros2.processImage();
            Filters dilat = new DilationFilter(openedImage);
            openedImage = dilat.processImage();
        }

        protected override Color calculateNewPixelColor(int x, int y)
        {
            Color col1 = sourceImage.GetPixel(x, y);
            Color col2 = openedImage.GetPixel(x, y);
            return Color.FromArgb(
                Clamp(col1.R - col2.R, 0, 255),
                 Clamp(col1.G - col2.G, 0, 255),
                 Clamp(col1.B - col2.B, 0, 255)
                );
           
        }

    }

    class BlackHatFilter : Filters
    {

        protected Bitmap closedImage;
        public BlackHatFilter(Bitmap _sourceimage)
        {
            sourceImage = _sourceimage;
            Filters dilat = new DilationFilter(_sourceimage);
            closedImage = dilat.processImage();
            Filters dilat2 = new DilationFilter(closedImage);
            closedImage = dilat2.processImage();
            Filters eros = new ErosionFilter(closedImage);
            closedImage = eros.processImage();
        }

        protected override Color calculateNewPixelColor(int x, int y)
        {
            Color col1 = sourceImage.GetPixel(x, y);
            Color col2 = closedImage.GetPixel(x, y);
            return Color.FromArgb(
                Clamp(col2.R - col1.R, 0, 255),
                 Clamp(col2.G - col1.G, 0, 255),
                 Clamp(col2.B - col1.B, 0, 255)
                );
        }

    }

    class GradFilter : Filters
    {
        Bitmap erosImage;
        Bitmap dilatImage;

        public GradFilter( Bitmap _sourceimage)
        {
            sourceImage = _sourceimage;
            Filters eros = new ErosionFilter(_sourceimage);
            erosImage = eros.processImage();
            Filters dilat = new DilationFilter(_sourceimage);
            dilatImage = dilat.processImage();
        }

        protected override Color calculateNewPixelColor(int x, int y)
        {
            
            Color col1 = erosImage.GetPixel(x, y);
            Color col2 = dilatImage.GetPixel(x, y);
            return Color.FromArgb(
                Clamp(col2.R - col1.R, 0, 255),
                Clamp(col2.G - col1.G, 0, 255),
                Clamp(col2.B - col1.B, 0, 255)
                );

        }

    }



    class MedianFilter1 : MatrixFilter //медианный
    { 
        public MedianFilter1(Bitmap _sourceimage)
        {
            sourceImage = _sourceimage;

        }

        protected override Color calculateNewPixelColor(int x, int y)
        {
            Color resultcolor;
            List<int> arrR = new List<int>();
            List<int> arrG = new List<int>();
            List<int> arrB = new List<int>();

            int coordx, coordy;
            for (int i = -1;i <= 1; i++)
            {
                for(int j = -1; j <= 1; j++)
                {
                    coordx = x + i; coordy = y + j;
                    if (coordy >=0 && coordy < sourceImage.Height && coordx >= 0 && coordx < sourceImage.Width)
                    {
                        arrR.Add(sourceImage.GetPixel(coordx, coordy).R);
                        arrG.Add(sourceImage.GetPixel(coordx, coordy).G);
                        arrB.Add(sourceImage.GetPixel(coordx, coordy).B);
                    }
                }
            }
            arrR.Sort();int newred = arrR[arrR.Count() / 2];
            arrG.Sort(); int newgreen = arrG[arrG.Count() / 2];
            arrB.Sort(); int newblue = arrB[arrB.Count() / 2];
            resultcolor = Color.FromArgb(newred, newgreen, newblue);
            return resultcolor;
        }


    }

    class MedianFilter2 : MatrixFilter   //медианный2
    {
        public MedianFilter2(Bitmap __sourceimage)
        {
            sourceImage = __sourceimage;

        }

        protected override Color calculateNewPixelColor(int x, int y)
        {
            Color resultcolor;
            List<int> arrR = new List<int>();
            List<int> arrG = new List<int>();
            List<int> arrB = new List<int>();

            int coordx, coordy;
            for (int i = -2; i <= 2; i++)
            {
                for (int j = -2; j <= 2; j++)
                {
                    coordx = x + i; coordy = y + j;
                    if (coordy >= 0 && coordy < sourceImage.Height && coordx >= 0 && coordx < sourceImage.Width)
                    {
                        arrR.Add(sourceImage.GetPixel(coordx, coordy).R);
                        arrG.Add(sourceImage.GetPixel(coordx, coordy).G);
                        arrB.Add(sourceImage.GetPixel(coordx, coordy).B);
                    }
                }
            }
            arrR.Sort(); int newred = arrR[arrR.Count() / 2];
            arrG.Sort(); int newgreen = arrG[arrG.Count() / 2];
            arrB.Sort(); int newblue = arrB[arrB.Count() / 2];
            resultcolor = Color.FromArgb(newred, newgreen, newblue);
            return resultcolor;
        }


    }
    //    class Bordering2 : MatrixFilter
    //    {
    //        public Bordering2(Bitmap _sourceimage)
    //        {
    //            sourceImage = _sourceimage;
    //            int sizeX = 3;
    //            int sizeY = 3;
    //            kernel = new float[sizeX, sizeY];
    //            kernel[0, 0] = 3.0f;
    //            kernel[0, 1] = 0.0f;
    //            kernel[0, 2] = -3.0f;
    //            kernel[1, 0] = 10.0f;
    //            kernel[1, 1] = 0.0f;
    //            kernel[1, 2] = -10.0f;
    //            kernel[2, 0] = 3.0f;
    //            kernel[2, 1] = 0.0f;
    //            kernel[2, 2] = -3.0f;

    //        }
    //    }
}

//точечный глобальный локальный