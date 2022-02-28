using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ClasificadorLego
{
    class Program
    {
        static void Main(string[] args)
        {
            string imagePath = @"C:\Users\ernes\Documents\Visual Studio 2019\Projects\ClasificadorLego\ClasificadorLego\images73.jpg";
            string modelPath = @"C:\Users\ernes\Documents\Visual Studio 2019\Projects\ClasificadorLego\ClasificadorLego\model7.h5";
            string name = "";
            int[] coordenadas = ObtenerCentro.CalcularCentro(imagePath);
            string prediccion = RealizarPrediccion.Prediccion(imagePath, modelPath);
            Image imagen = Image.FromFile(imagePath);
            Bitmap img = (Bitmap)imagen;
            int x = coordenadas[0];
            int y = coordenadas[1];

            Color color = Color.FromArgb(img.GetPixel(x, y).R, img.GetPixel(x, y).G, img.GetPixel(x, y).B);
            Console.WriteLine(color);
            string a = Convert.ToString(ObtenerColor(color));
            char[] aux = { '[', ']' };
            string colorR = a.Split(aux)[1];

            var colorLookup = typeof(Color)
                .GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Select(f => (Color)f.GetValue(null, null))
                .Where(c => c.IsNamedColor)
                .ToLookup(c => c.ToArgb());

            
            foreach (var namedColor in colorLookup[color.ToArgb()])
            {
                Console.WriteLine(namedColor.Name);
                name=namedColor.Name;
            }
            if (name!="")
            {
                Console.WriteLine(name);

            }
            else
            {

                
                Console.WriteLine("El color resultante de la clasificación del lego es "+colorR);
            }

            Console.WriteLine(GetColorName(color));




            string[] resultado = prediccion.Split('_');

            if (resultado.Length==2)
            {
                Console.WriteLine("El resultado del modelo fue un lego rectangular con factor de: "+resultado[1]);
            }
            else
            {
                Console.WriteLine("El resultado del modelo fue un lego con factor de: "+resultado[1]);
                Console.WriteLine("La caracteristica del lego es: "+resultado[2]);

            }
            Console.ReadLine();

        }
        private static int DiferenciaColor(Color c1, Color c2)
        {
            int distance;
            if (c2.GetSaturation() < 0.1)
            {
                // Near 0 is grayscale
                distance = (int)Math.Abs(c1.GetSaturation()-c2.GetSaturation());
            }
            else
            {
                distance = (int)Math.Abs(c1.GetHue()- c2.GetHue());
            }
            return distance;
        }
        public static Color ObtenerColor(Color colorBase)
        {
            Color[] colores =
            {
                Color.Red,
                Color.Black,
                Color.White,
                Color.Blue,
                Color.Yellow,
                Color.Green,
                Color.Gray,
                Color.Brown,
                Color.SkyBlue,
                Color.Orange
            };

            var colors = colores.Select(x => new { Value = x, Diff = DiferenciaColor(x, colorBase) }).ToList();
            var min = colors.Min(x => x.Diff);
            return colors.Find(x => x.Diff == min).Value;
        }

    }
}

