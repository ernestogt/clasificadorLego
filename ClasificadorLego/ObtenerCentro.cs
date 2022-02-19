using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClasificadorLego
{
    class ObtenerCentro
    {
        public static int[] CalcularCentro(string ImagePath)
        {

            int cx = 0, cy = 0;
            int[] centro = { 0, 0 };
            Mat image = Cv2.ImRead(ImagePath);
            image = image.CvtColor(ColorConversionCodes.BGR2GRAY);
            Mat blurred = new Mat();
            Cv2.GaussianBlur(image, blurred, new OpenCvSharp.Size(5, 5), 0);
            Mat canny = new Mat();
            Cv2.Canny(blurred, canny, 30, 300);
            Point[][] contour;
            HierarchyIndex[] hierarchyIndexes;
            Cv2.FindContours(canny, out contour, out hierarchyIndexes, RetrievalModes.External, ContourApproximationModes.ApproxSimple);
            foreach (var c in contour)
            {
                Moments M = Cv2.Moments(c);
                if (M.M00!=0)
                {
                    cx=(int)(M.M10/M.M00);
                    cy=(int)(M.M01/M.M00);
                }
                else
                {
                    cx=0;
                    cy=0;
                }
            }
            centro[0]=cx;
            centro[1]=cy;
            return centro;

        }
    }
}
