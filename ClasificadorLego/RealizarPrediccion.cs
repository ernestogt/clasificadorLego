using Keras.PreProcessing.Image;
using Numpy;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClasificadorLego
{
    class RealizarPrediccion
    {
        public static string Prediccion(string path, string modelPath)
        {

            Image imagen = Image.FromFile(path);
            var img = ImageUtil.LoadImg(path, target_size: (224, 224));
            var x = ImageUtil.ImageToArray(img)/255.0;
            //Console.WriteLine(x);
            x = np.expand_dims(x, axis: 0);
            //Console.WriteLine(x);
            var model = Keras.Models.Model.LoadModel(modelPath);
            var preds = model.Predict(x);
            //Console.WriteLine(preds);

            //Console.WriteLine(np.argmax(preds));


            String[] labels = {"lego_1x1","lego_1x1_Circular","lego_1x2", "lego_1x2_Pendiente", "lego_1x3", "lego_2x2", "lego_2x2_L","lego_2x2_Pendiente",
            "lego_2x3", "lego_2x3_Plato","lego_2x3_Pendiente", "lego_2x4", "lego2x4_Plato","lego_2x4_Pendiente"};

            string aux = labels[(int)np.argmax(preds)];
            return aux;
        }
    }
}
