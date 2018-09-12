using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OpenAlprApi.Api;
using OpenAlprApi.Client;
using OpenAlprApi.Model;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Drawing;

namespace PG.Servisi.RESTApi
{
    public static class OpenALRPApi
    {
        public static Tuple<string, string> Recognize(string base64Image, bool prepoznajVozilo, ref Rectangle plateRect, out JObject vozilo)
        {

            String reg ="", drz="";
            var apiInstance = new DefaultApi("https://api.openalpr.com/v2"); //new DefaultApi();
            //var imageBytes = base64image;  // string | The image file that you wish to analyze encoded in base64 
            var secretKey = "sk_b1572dbc0c19945677074b5e";  // string | The secret key used to authenticate your account.  You can view your  secret key by visiting  https://cloud.openalpr.com/ 
            var country = "eu";  // string | Defines the training data used by OpenALPR.  \"us\" analyzes  North-American style plates.  \"eu\" analyzes European-style plates.  This field is required if using the \"plate\" task  You may use multiple datasets by using commas between the country  codes.  For example, 'au,auwide' would analyze using both the  Australian plate styles.  A full list of supported country codes  can be found here https://github.com/openalpr/openalpr/tree/master/runtime_data/config 
            var recognizeVehicle = (prepoznajVozilo)? 1:0;  // int? | If set to 1, the vehicle will also be recognized in the image This requires an additional credit per request  (optional)  (default to 0)
            string state = null;  // string | Corresponds to a US state or EU country code used by OpenALPR pattern  recognition.  For example, using \"md\" matches US plates against the  Maryland plate patterns.  Using \"fr\" matches European plates against  the French plate patterns.  (optional)  (default to )
            var returnImage = 0;  // int? | If set to 1, the image you uploaded will be encoded in base64 and  sent back along with the response  (optional)  (default to 0)
            var topn = 12;  // int? | The number of results you would like to be returned for plate  candidates and vehicle classifications  (optional)  (default to 10)
            string prewarp = null;  // string | Prewarp configuration is used to calibrate the analyses for the  angle of a particular camera.  More information is available here http://doc.openalpr.com/accuracy_improvements.html#calibration  (optional)  (default to )

            vozilo = new JObject();

            byte[] mbmp = Convert.FromBase64String(base64Image);
            try
            {

                InlineResponse200 result = apiInstance.RecognizeFile(new MemoryStream(mbmp), secretKey, country, recognizeVehicle, state, returnImage, topn, prewarp);
                if (result != null && result.Results.Count > 0)
                {
                    reg = result.Results[0].Plate;
                    drz = result.Results[0].Region;
                    if (result.Results[0].Vehicle != null)
                    {
                        vozilo.Add(new JProperty("marka", result.Results[0].Vehicle.Make[0].Name));
                        vozilo.Add(new JProperty("boja", result.Results[0].Vehicle.Color[0].Name));
                    }
                    if(plateRect != null)
                    {
                        var coord = result.Results[0].Coordinates;
                        try
                        {
                            int left = -1, top = -1, right = -1, bottom = -1;
                            foreach(Coordinate c in coord)
                            {
                                if (left < 0 || left > c.X) left = (int) c.X;
                                if (right < 0 || right < c.X) right = (int)c.X;
                                if (top < 0 || top > c.Y) top = (int)c.Y;
                                if (bottom < 0 || bottom < c.Y) bottom = (int)c.Y;

                            }
                            if(left >= 0 && right >= 0 && top >= 0 && bottom >= 0)
                            {
                                plateRect.X = left;
                                plateRect.Y = top;
                                plateRect.Width = right - left;
                                plateRect.Height = bottom - top;
                            }

                            //if (coord.Count == 4)
                            //{
                            //    int x = Math.Min((int)coord[0].X, (int)coord[3].X);
                            //    int y = Math.Min((int)coord[0].Y, (int)coord[1].Y);
                            //    int w = Math.Max((int)coord[1].X, (int)coord[2].X) - x;
                            //    int h = Math.Max((int)coord[2].Y, (int)coord[3].Y) - y;
                            //    plateRect.X = x;
                            //    plateRect.Y = y;
                            //    plateRect.Width = w;
                            //    plateRect.Height = h;
                            //}
                        }catch(Exception ex)
                        {
                            Methods.SpremiGresku("", ex, "OPENNALPR RECOGNIZE - koordinate tablice...");
                        }
                    }
                }
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Methods.SpremiGresku("", e, "OPENNALPR RECOGNIZE");
            }
            return new Tuple<string, string>(reg, drz);
        }
    }
}