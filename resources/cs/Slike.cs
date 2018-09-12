using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using ExifLib;
using PG.Servisi.resources.podaci.upiti;

namespace PG.Servisi.resources.cs
{
    public class Slike
    {

        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public static void Metadata(byte[] slika)
        {
            Stream stream = new MemoryStream(slika);

            using (ExifReader reader = new ExifReader(stream))
            {
                // Extract the tag data using the ExifTags enumeration
                DateTime datePictureTaken;
                if (reader.GetTagValue(ExifTags.DateTimeDigitized, out datePictureTaken))
                {
                    // Do whatever is required with the extracted information
                  
                }

                int orientation;
                reader.GetTagValue(ExifTags.Orientation, out orientation);
            }
        }

        public static void SpremiNaDisk(string putanja, byte[] slika)
        {
            try
            {
                using (FileStream fs = new FileStream(putanja, FileMode.Create, FileAccess.Write))
                {
                    using (BinaryWriter bw = new BinaryWriter(fs))
                    {
                        bw.Write(slika);
                        bw.Close();
                        bw.Dispose();
                        fs.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                Sustav.SpremiGresku("", ex, 1, putanja);
            }
        }

        public static byte[] TimeStamp(string putanja, DateTime picTime, string ulica, string uid)
        {
            Bitmap bmp = new Bitmap(putanja);
            return TimeStamp(bmp, picTime, ulica, uid);
        }

        public static byte[] TimeStamp(byte[] slika, DateTime picTime, string ulica, string uid)
        {
            MemoryStream ms = new MemoryStream(slika);
            Bitmap bmp = new Bitmap(ms);
            return TimeStamp(bmp, picTime, ulica, uid);
        }

        public static byte[] TimeStamp(Bitmap bmp, DateTime picTime, string ulica, string uid)
        {
            MemoryStream ms = new MemoryStream();
            //Bitmap bmp = null;
            Random rnd = new Random();

            try
            {
                //bmp = new Bitmap(putanja);

                Graphics g = Graphics.FromImage(bmp);
                Font f = new Font(System.Drawing.FontFamily.GenericMonospace, 10);
                string text = string.Format("{0:dd.MM.yyyy u HH:mm:ss} ({2})\r\n{1}", picTime, ulica, uid);

                SizeF s = g.MeasureString(text, f, bmp.Width);
                RectangleF rec = new RectangleF(new PointF(0, bmp.Height - s.Height - 1), s);

                int i, crgb = 0;
                for (i = 0; i < Convert.ToInt32(rec.Width * rec.Height / 10); i++)
                {
                    System.Drawing.Color c = bmp.GetPixel(rnd.Next((int)rec.Width),
                        bmp.Height - 1 - rnd.Next((int)s.Height));
                    crgb += (c.R + c.G + c.B) / 3;
                }

                crgb = crgb / i;

                g.FillRectangle(new SolidBrush(Color.FromArgb(120, crgb, crgb, crgb)), rec);
                g.DrawString(text, f,
                    new SolidBrush(crgb >= 128 ? Color.Black : Color.White), rec);
                g.Dispose();

                bmp.Save(ms, ImageFormat.Jpeg);
                // TODO provjeriti s Pajom da li je ovo bitno zbog nekog drugog...
                return ms.ToArray(); // ResizeImage(ms.ToArray(), new Size(800, 600));
            }
            catch
            {
                return null;
            }
            finally
            {
                ms.Dispose();
                bmp.Dispose();
            }
        }

        public static byte[] ResizeImage(byte[] img, Size size)
        {
            Bitmap bmp;

            using (MemoryStream ms = new MemoryStream(img)) bmp = new Bitmap(ms);

            double f = Math.Min((double)size.Height / bmp.Height, (double)size.Width / bmp.Width);
            bmp = new Bitmap(bmp, Convert.ToInt32(bmp.Width * f), Convert.ToInt32(bmp.Height * f));
            return ImageToByte(bmp);
        }

        public static byte[] ImageToByte(Image img)
        {
            byte[] byteArray;
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, ImageFormat.Jpeg);
                stream.Close();

                byteArray = stream.ToArray();
            }

            return byteArray;
        }
    }
}