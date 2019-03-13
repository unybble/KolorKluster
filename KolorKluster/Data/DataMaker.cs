using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace KolorKluster.Data
{
    public static class DataMaker
    {
        public static List<RGBColor> GetColors(int numberOfColors)
        {
            Random rand = new Random();
            List<RGBColor> colors = new List<RGBColor>();

            for (int i = 0; i < numberOfColors; i++)
                colors.Add(new RGBColor(rand.Next(1, 255), rand.Next(1, 255), rand.Next(1, 255)));

            return colors;
        }

       
    }

    public class RGBColor
    {
        public int R {get;set;}
        public int G { get; set; }
        public int B { get; set; }
        public int RG { get; set; }
        public int GB { get; set; }
        public int BR { get; set; }

        public int Cluster { get; set; }

        public RGBColor(int r, int g, int b)
        {
            R = r;
            G = g;
            B = b;
            RG = r - g;
            GB = g - b;
            BR = b - r;
            Cluster = 0;

        }


    }
    public class Cluster
    {
        public List<RGBColor> colors { get; set; }
        public int ID { get; set; }

    }


}
