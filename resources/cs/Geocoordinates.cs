using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using PG.Servisi.resources.podaci.upiti;
using Point = System.Windows.Point;

namespace PG.Servisi.resources.cs
{
    public static class Geocoordinates
    {
        public static string PozicijaPrekrsajaOS(string lat, string lng)
        {
            return string.Format("http://staticmap.openstreetmap.de/staticmap.php?center={0},{1}&zoom=17&size=210x230&markers={0},{1},red-pushpin", lat, lng);
        }

        public static string PozicijaPrekrsajaGM(string lat, string lng)
        {
            return string.Format("http://maps.google.com/maps/api/staticmap?center={0},{1}&zoom=17&size=210x230&sensor=false&maptype=hybrid&markers=color:red|{0},{1}", lat, lng);
        }

        public static double distance(double lat1, double lon1, double lat2, double lon2, char unit)
        {
            double theta = lon1 - lon2;
            double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) +
                          Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
            dist = Math.Acos(dist);
            dist = rad2deg(dist);
            dist = dist * 60 * 1.1515;

            if (unit == 'K')
            {
                dist = dist * 1.609344;
            }
            else if (unit == 'N')
            {
                dist = dist * 0.8684;
            }

            return (dist);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //::  This function converts decimal degrees to radians             :::
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        private static double deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //::  This function converts radians to decimal degrees             :::
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        private static double rad2deg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }


        /// <summary>
        /// Pretvara decimalne koordinate u stupnjeve, minute i sekunde
        /// </summary>
        /// <param name="coordinate">decimalna kordinata</param>
        /// <param name="type">da li je širina ili dužina</param>
        /// <returns>string koordinatu u stupnjevima</returns>
        public static string DDtoDMS(double coordinate, CoordinateType type)
        {
            // Set flag if number is negative
            bool neg = coordinate < 0d;

            // Work with a positive number
            coordinate = Math.Abs(coordinate);

            // Get d/m/s components
            double d = Math.Floor(coordinate);
            coordinate -= d;
            coordinate *= 60;
            double m = Math.Floor(coordinate);
            coordinate -= m;
            coordinate *= 60;
            double s = Math.Round(coordinate);

            // Create padding character
            char pad;
            char.TryParse("0", out pad);

            // Create d/m/s strings
            string dd = d.ToString();
            string mm = m.ToString().PadLeft(2, pad);
            string ss = s.ToString().PadLeft(2, pad);

            // Append d/m/s
            string dms = string.Format("{0}°{1}'{2}\"", dd, mm, ss);

            // Append compass heading
            switch (type)
            {
                case CoordinateType.longitude:
                    dms += neg ? "W" : "E";
                    break;
                case CoordinateType.latitude:
                    dms += neg ? "S" : "N";
                    break;
            }

            // Return formated string
            return dms;
        }

        /// <summary>
        /// Koja kordinata se pretvara
        /// </summary>
        public enum CoordinateType { longitude, latitude };



        public static bool PointInPolygon(Point currentPoint, List<Point> polygon)
        {
            //Ray-cast algorithm is here onward
            int k, j = polygon.Count - 1;
            bool oddNodes = false; //to check whether number of intersections is odd
            for (k = 0; k < polygon.Count; k++)
            {
                //fetch adjucent points of the polygon
                Point polyK = polygon[k];
                Point polyJ = polygon[j];

                //check the intersections
                if (((polyK.Y > currentPoint.Y) != (polyJ.Y > currentPoint.Y)) &&
                 (currentPoint.X < (polyJ.X - polyK.X) * (currentPoint.Y - polyK.Y) / (polyJ.Y - polyK.Y) + polyK.X))
                    oddNodes = !oddNodes; //switch between odd and even
                j = k;
            }

            //if odd number of intersections
            if (oddNodes)
            {
                //mouse point is inside the polygon
                return true;
            }

            //mouse point is outside the polygon so deselect the polygon
            return false;
        }

        public static List<_DetaljiLokacije> DohvatiAdresu(double lat, double lon)
        {
            if (!lat.ToString().Contains(".") && !lat.ToString().Contains(","))
            {
                lat = lat / Math.Pow(10, lat.ToString().Length - 2);
            }

            if (!lon.ToString().Contains(".") && !lon.ToString().Contains(","))
            {
                lon = lon / Math.Pow(10, lon.ToString().Length - 2);
            }

            string webAdresa = "http://maps.google.com/maps/api/geocode/json?latlng=" + lat.ToString().Replace(",", ".") + "," +
                               lon.ToString().Replace(",", ".") + "&sensor=false";

            //string webAdresa = "https://maps.google.com/maps/api/geocode/json?latlng=" + lat.ToString().Replace(",", ".") + "," +
            //                   lon.ToString().Replace(",", ".") + "&sensor=false&key=AIzaSyAeGf2P7OxpUAK6LW86Wpcf8ay--zQjZN4";
            //string webAdresa = "http://maps.google.com/maps/geo?q=" + lat.ToString().Replace(",", ".") + "," + lon.ToString().Replace(",", ".") +
            //                "&output=json&sensor=true&key=AIzaSyAeGf2P7OxpUAK6LW86Wpcf8ay--zQjZN4";
            try
            {
                StringBuilder sb = new StringBuilder();
                byte[] buf = new byte[8192];
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(webAdresa);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream resStream = response.GetResponseStream();

                int count;

                do
                {
                    // fill the buffer with data
                    count = resStream.Read(buf, 0, buf.Length);

                    // make sure we read some data
                    if (count != 0)
                    {
                        // translate from bytes to ASCII text
                        string tempString = Encoding.UTF8.GetString(buf, 0, count);

                        // continue building the string
                        sb.Append(tempString);
                    }
                } while (count > 0); // any more data to read?

                return Pronadji(sb.ToString());
            }
            catch
            {
                //Sustav.SpremiGresku("", ex, 1, "GEOCODING: " + webAdresa);
                return new List<_DetaljiLokacije>();
            }
        }

        private static List<_DetaljiLokacije> Pronadji(string tekst)
        {
            var root = JsonConvert.DeserializeObject<RootObject>(tekst);

            if (root.status != "OK")
            {
                return new List<_DetaljiLokacije>();
            }

            List<_DetaljiLokacije> Adresa = new List<_DetaljiLokacije>();

            var a = root.results.First();

            Adresa.Add(new _DetaljiLokacije(
                a.address_components.First(i => i.types.Contains("route")).long_name,
                a.address_components.First(i => i.types.Contains("street_number")).long_name,
                a.address_components.First(i => i.types.Contains("locality")).long_name,
                a.address_components.First(i => i.types.Contains("administrative_area_level_1")).long_name,
                a.address_components.First(i => i.types.Contains("country")).short_name, 6)); //ako je manje od 6 draško ne prikazuje

            return Adresa;
        }
    }

    public class AddressComponent
    {
        public string long_name { get; set; }
        public string short_name { get; set; }
        public List<string> types { get; set; }
    }

    public class Bounds
    {
        public Location northeast { get; set; }
        public Location southwest { get; set; }
    }

    public class Location
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Geometry
    {
        public Bounds bounds { get; set; }
        public Location location { get; set; }
        public string location_type { get; set; }
        public Bounds viewport { get; set; }
    }

    public class Result
    {
        public List<AddressComponent> address_components { get; set; }
        public string formatted_address { get; set; }
        public Geometry geometry { get; set; }
        public bool partial_match { get; set; }
        public List<string> types { get; set; }
    }

    public class RootObject
    {
        public List<Result> results { get; set; }
        public string status { get; set; }
    }
}
