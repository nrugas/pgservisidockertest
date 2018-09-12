using iTextSharp.text.pdf;
using PG.Servisi.resources.cs.ispis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace PG.Servisi.RESTApi
{

    public static class BixolonUtils
    {

        private static int[][][] VALIDATION_TABLE;

        static BixolonUtils()
        {
                mIsOverFlow = false;
        VALIDATION_TABLE = new int[][][]{
           new int[][]  {
               new int[] {41, 25, 17},
               new int[] {34, 20, 14},
               new int[]  {27, 16, 11},
               new int[] {17, 10, 7}},
           new int[][] {
               new int[] {77, 47, 32},
               new int[] {63, 38, 26},
               new int[] {48, 29, 20},
               new int[] {34, 20, 14}},
           new int[][] {
               new int[] {127, 77, 53},
               new int[] {101, 61, 42},
               new int[] {77, 47, 32},
               new int[] {58, 35, 24}},
           new int[][] {
               new int[] {187, 114, 78},
               new int[] {149, 90, 62},
               new int[] {111, 67, 46},
               new int[] {82, 50, 34}},
           new int[][] {
               new int[] {255, 154, 106},
               new int[] {202, 122, 84},
               new int[] {144, 87, 60},
               new int[] {106, 64, 44}},
           new int[][] {
               new int[] {322, 195, 134},
               new int[] {255, 154, 106},
               new int[] {178, 108, 74},
               new int[] {139, 84, 58}},
           new int[][] {
               new int[] {370, 224, 154},
               new int[] {293, 178, 122},
               new int[] {207, 125, 86},
               new int[] {154, 93, 64}},
           new int[][] {
               new int[] {461, 279, 192},
               new int[] {365, 221, 152},
               new int[] {259, 157, 108},
               new int[] {202, 122, 84}},
           new int[][] {
               new int[] {552, 335, 230},
               new int[] {432, 262, 180},
               new int[] {312, 189, 130},
               new int[] {235, 143, 98}},
           new int[][] {
               new int[] {652, 395, 271},
               new int[] {513, 311, 213},
               new int[] {364, 221, 151},
               new int[] {288, 174, 119}}, 
           new int[][] { new int[] {772, 468, 321}, new int[] {604, 366, 251}, new int[] {427, 259, 177}, new int[] {331, 200, 137}}, 
           new int[][] { new int[] {883, 535, 367}, new int[] {691, 419, 287}, new int[] {489, 296, 203}, new int[] {374, 227, 155}}, 
           new int[][] { new int[] {1022, 619, 425}, new int[] {796, 483, 331}, new int[] {580, 352, 241}, new int[] {427, 259, 177}}, 
           new int[][] { new int[] {1101, 667, 458}, new int[] {871, 528, 362}, new int[] {621, 376, 258}, new int[] {468, 283, 194}}, 
           new int[][] { new int[] {1250, 758, 520}, new int[] {991, 600, 412}, new int[] {703, 426, 292}, new int[] {530, 321, 220}}, 
           new int[][] { new int[] {1408, 854, 586}, new int[] {1082, 656, 450}, new int[] {775, 470, 322}, new int[] {602, 365, 250}}, 
           new int[][] { new int[] {1548, 938, 644}, new int[] {1212, 734, 504}, new int[] {876, 531, 364}, new int[] {674, 408, 280}}, 
           new int[][] { new int[] {1725, 1046, 718}, new int[] {1346, 816, 560}, new int[] {948, 574, 394}, new int[] {746, 452, 310}}, 
           new int[][] { new int[] {1903, 1153, 792}, new int[] {1500, 909, 624}, new int[] {1063, 644, 422}, new int[] {813, 493, 338}}, 
           new int[][] { new int[] {2061, 1249, 858}, new int[] {1600, 970, 666}, new int[] {1159, 702, 482}, new int[] {919, 557, 382}}, 
           new int[][] { new int[] {2232, 1352, 929}, new int[] {1708, 1035, 771}, new int[] {1224, 742, 509}, new int[] {969, 587, 403}}, 
           new int[][] { new int[] {2409, 1460, 1003}, new int[] {1872, 1134, 779}, new int[] {1358, 823, 565}, new int[] {1056, 640, 439}}, 
           new int[][] { new int[] {2620, 1588, 1091}, new int[] {2059, 1248, 857}, new int[] {1468, 890, 611}, new int[] {1108, 672, 461}}, 
           new int[][] { new int[] {2812, 1704, 1171}, new int[] {2188, 1326, 911}, new int[] {1588, 963, 661}, new int[] {1228, 744, 511}}, 
           new int[][] { new int[] {3057, 1853, 1273}, new int[] {2395, 1451, 997}, new int[] {1718, 1041, 715}, new int[] {1286, 779, 535}}, 
           new int[][] { new int[] {3283, 1990, 1367}, new int[] {2544, 1542, 1059}, new int[] {1804, 1094, 751}, new int[] {1425, 864, 593}}, 
           new int[][] { new int[] {3514, 2132, 1465}, new int[] {2701, 1637, 1125}, new int[] {1933, 1172, 805}, new int[] {1501, 910, 625}}, 
           new int[][] { new int[] {3669, 2223, 1528}, new int[] {2857, 1732, 1190}, new int[] {2085, 1263, 868}, new int[] {1581, 958, 658}}, 
           new int[][] { new int[] {3909, 2369, 1628}, new int[] {3035, 1839, 1264}, new int[] {2181, 1322, 908}, new int[] {1677, 1016, 698}}, 
           new int[][] { new int[] {4158, 2520, 1732}, new int[] {3289, 1994, 1370}, new int[] {2358, 1429, 982}, new int[] {1782, 1080, 742}}, 
           new int[][] { new int[] {4417, 2677, 1840}, new int[] {3486, 2113, 1452}, new int[] {2473, 1499, 1030}, new int[] {1897, 1150, 790}}, 
           new int[][] { new int[] {4686, 2840, 1952}, new int[] {3693, 2238, 1538}, new int[] {2670, 1618, 1112}, new int[] {2022, 1226, 842}}, 
           new int[][] { new int[] {4965, 3009, 2068}, new int[] {3909, 2369, 1628}, new int[] {2805, 1700, 1168}, new int[] {2157, 1307, 898}}, 
           new int[][] { new int[] {5253, 3183, 2188}, new int[] {4134, 2506, 1722}, new int[] {2949, 1787, 1228}, new int[] {2301, 1394, 958}}, 
           new int[][] { new int[] {5529, 3351, 2303}, new int[] {4343, 2632, 1809}, new int[] {3081, 1867, 1283}, new int[] {2361, 1431, 983}}, 
           new int[][] { new int[] {5836, 3537, 2431}, new int[] {4588, 2780, 1911}, new int[] {3244, 1966, 1351}, new int[] {2524, 1530, 1051}}, 
           new int[][] { new int[] {6153, 3729, 2563}, new int[] {4775, 2894, 1989}, new int[] {3417, 2071, 1423}, new int[] {2625, 1591, 1093}}, 
           new int[][] { new int[] {6479, 3927, 2699}, new int[] {5039, 3054, 2099}, new int[] {3599, 2181, 1499}, new int[] {2735, 1658, 1139}}, 
           new int[][] { new int[] {6743, 4087, 2809}, new int[] {5313, 3220, 2213}, new int[] {3791, 2298, 1579}, new int[] {2927, 1774, 1219}}, 
           new int[][] { new int[] {7089, 4296, 2953}, new int[] {5596, 3391, 2331}, new int[] {3993, 2420, 1663}, new int[] {3057, 1852, 1273}}};
    }


        public static byte[] PrintText(String text, int alignment, int attribute, int size)
        {
            return PrintText(text, alignment, attribute, size, true);
        }
        public static byte[] PrintText(String text, int alignment, int attribute, int size, bool addCRLF)
        {
            byte[] bytes = new byte[0];
            if (!string.IsNullOrEmpty(text))
            {
                text = ObradjivanjePodataka.SkidanjeKvacica(text);
                if ((attribute & 64) == 64)
                {
                    char[] charArray = text.ToCharArray();
                    Array.Reverse(charArray);
                    text = new string(charArray);
                }

                bytes = Encoding.Default.GetBytes(text);
            }
            byte[] securityCode = null;
            int capacity = BixolonCommands.ALIGNMENT_LEFT.Length + BixolonCommands.DEVICE_FONT_A.Length + BixolonCommands.UNDERLINE_OFF.Length + BixolonCommands.EMPHASIZED_OFF.Length + BixolonCommands.REVERSE_OFF.Length + BixolonCommands.CHARACTER_SIZE.Length + 1 + bytes.Length;
            //if (!this.mIsPageMode)
            //{
            //    securityCode = SecurityManager.getSecurityCode();
            //    capacity += securityCode.length;
            //}

            ByteCollection buffer = new ByteCollection(capacity);
            //if (!this.mIsPageMode)
            //{
            //    buffer.Add(securityCode);
            //}

            if (alignment == 1)
            {
                buffer.Add(BixolonCommands.ALIGNMENT_CENTER);
            }
            else if (alignment == 2)
            {
                buffer.Add(BixolonCommands.ALIGNMENT_RIGHT);
            }
            else
            {
                buffer.Add(BixolonCommands.ALIGNMENT_LEFT);
            }

            if ((attribute & 1) == 1)
            {
                buffer.Add(BixolonCommands.DEVICE_FONT_B);
            }
            else if ((attribute & 2) == 2)
            {
                buffer.Add(BixolonCommands.DEVICE_FONT_C);
            }
            else
            {
                buffer.Add(BixolonCommands.DEVICE_FONT_A);
            }

            if ((attribute & 4) == 4)
            {
                buffer.Add(BixolonCommands.UNDERLINE_1DOT_THICK);
            }
            else if ((attribute & 8) == 8)
            {
                buffer.Add(BixolonCommands.UNDERLINE_2DOT_THICK);
            }
            else
            {
                buffer.Add(BixolonCommands.UNDERLINE_OFF);
            }

            if ((attribute & 16) == 16)
            {
                buffer.Add(BixolonCommands.EMPHASIZED_ON);
            }
            else
            {
                buffer.Add(BixolonCommands.EMPHASIZED_OFF);
            }

            if ((attribute & 32) == 32)
            {
                buffer.Add(BixolonCommands.REVERSE_ON);
            }
            else
            {
                buffer.Add(BixolonCommands.REVERSE_OFF);
            }

            buffer.Add(BixolonCommands.CHARACTER_SIZE);
            buffer.Add((byte)size);
            buffer.Add(bytes);
            if (addCRLF && bytes.Length > 0) buffer.Add("\r\n");
            return buffer.GetBytes();
        }

        static bool mIsOverFlow = false;
        public static byte[] getQrCode(byte[] data, int model, int size, int errorCorrectionLevel, int version)
        {
            
            mIsOverFlow = false;
            if (size <= 0 || size >= 9)
            {
                size = 3;
            }

           
            if (data != null && data.Length != 0 && (model == 49 || model == 50) && errorCorrectionLevel >= 48 && errorCorrectionLevel <= 51)
            {
                int capacity = BixolonCommands.QR_CODE_MODEL2.Length + BixolonCommands.QR_CODE_SIZE.Length + 1 + BixolonCommands.QR_CODE_ERROR_CORRECTION_LEVEL.Length + 1 + BixolonCommands.QR_CODE_STORE.Length + 2 + BixolonCommands.QR_CODE_STORE_PARAMETER.Length + data.Length + BixolonCommands.QR_CODE_PRINT.Length;
                if (version > 0)
                {
                    if (version < 1 || version > 40)
                    {
                        return null;
                    }

                    int type = 0;

                    int maxLength;
                    for (maxLength = 0; maxLength < data.Length; ++maxLength)
                    {
                        int character = data[maxLength] & 255;
                        if ((character < 65 || character > 90) && (character < 97 || character > 122) && character != 32 && character != 36 && character != 37 && character != 42 && character != 43 && character != 45 && character != 46 && character != 47 && character != 58)
                        {
                            if (character < 48 || character > 57)
                            {
                                type = 2;
                                break;
                            }
                        }
                        else
                        {
                            type = 1;
                        }
                    }

                   

                    maxLength = VALIDATION_TABLE[version - 1][errorCorrectionLevel - 48][type];
                    mIsOverFlow = data.Length > maxLength;
                    if (mIsOverFlow)
                    {
                       

                        //ByteCollection buffer = new ByteCollection(maxLength);
                        //buffer.Add(data, 0, maxLength);
                        //data = buffer.array();
                        capacity = BixolonCommands.QR_CODE_VERSION.Length + 1 + BixolonCommands.QR_CODE_MODEL2.Length + BixolonCommands.QR_CODE_SIZE.Length + 1 + BixolonCommands.QR_CODE_ERROR_CORRECTION_LEVEL.Length + 1 + BixolonCommands.QR_CODE_STORE.Length + 2 + BixolonCommands.QR_CODE_STORE_PARAMETER.Length + 
                            maxLength + BixolonCommands.QR_CODE_PRINT.Length;
                    }
                    else
                    {
                        

                        capacity += BixolonCommands.QR_CODE_VERSION.Length + 1;
                    }
                }
                
                ByteCollection buffer = new ByteCollection(capacity+ BixolonCommands.ALIGNMENT_CENTER.Length);
                buffer.Add(BixolonCommands.ALIGNMENT_CENTER);
                if (version > 0)
                {
                    if (version >= 20)
                    {
                        version = 6;
                    }

                    buffer.Add(BixolonCommands.QR_CODE_VERSION);
                    buffer.Add((byte)version);
                }

                if (model == 49)
                {
                    buffer.Add(BixolonCommands.QR_CODE_MODEL1);
                }
                else
                {
                    buffer.Add(BixolonCommands.QR_CODE_MODEL2);
                }
                
                buffer.Add(BixolonCommands.QR_CODE_SIZE);
                buffer.Add((byte)size);
                buffer.Add(BixolonCommands.QR_CODE_ERROR_CORRECTION_LEVEL);
                buffer.Add((byte)errorCorrectionLevel);
                buffer.Add(BixolonCommands.QR_CODE_STORE);
                buffer.Add((byte)((data.Length + 3) % 256));
                buffer.Add((byte)((data.Length + 3) / 256));
                buffer.Add(BixolonCommands.QR_CODE_STORE_PARAMETER);
                buffer.Add(data);
                buffer.Add(BixolonCommands.QR_CODE_PRINT);
                return buffer.GetBytes();
            }
            else
            {
                return new byte[0];
            }
        }

        public static byte[] getMaxiCode(byte[] data, int mode)
        {
            int capacity = BixolonCommands.MAXI_CODE_MODE2.Length + BixolonCommands.MAXI_CODE_STORE.Length + 2 + BixolonCommands.MAXI_CODE_STORE_PARAMETER.Length + data.Length + BixolonCommands.MAXI_CODE_PRINT.Length;
            ByteCollection buffer = new ByteCollection(capacity);
            switch (mode)
            {
                case 51:
                    buffer.Add(BixolonCommands.MAXI_CODE_MODE3);
                    break;
                case 52:
                    buffer.Add(BixolonCommands.MAXI_CODE_MODE4);
                    break;
                default:
                    buffer.Add(BixolonCommands.MAXI_CODE_MODE2);
                    break;
            }

            buffer.Add(BixolonCommands.MAXI_CODE_STORE);
            buffer.Add((byte)((data.Length + 3) % 256));
            buffer.Add((byte)((data.Length + 3) / 256));
            buffer.Add(BixolonCommands.MAXI_CODE_STORE_PARAMETER);
            buffer.Add(data);
            buffer.Add(BixolonCommands.MAXI_CODE_PRINT);
            return buffer.GetBytes();
        }

        public static byte[] getPdf417(byte[] data, int width, int height)
        {
            if (width < 2 || width > 3)
            {
                width = 3;
            }

            if (height < 2 || height > 8)
            {
                height = 3;
            }

            int capacity = BixolonCommands.PDF417_COLUMN_NUMBER_AUTO.Length + BixolonCommands.PDF417_ROW_NUMBER_AUTO.Length + BixolonCommands.PDF417_WIDTH.Length + 1 + BixolonCommands.PDF417_HEIGHT.Length + 1 + BixolonCommands.PDF417_STORE.Length + 2 + BixolonCommands.PDF417_STORE_PARAMETER.Length + data.Length + BixolonCommands.PDF417_PRINT.Length;
            ByteCollection buffer = new ByteCollection(capacity);
            buffer.Add(BixolonCommands.PDF417_COLUMN_NUMBER_AUTO);
            buffer.Add(BixolonCommands.PDF417_ROW_NUMBER_AUTO);
            buffer.Add(BixolonCommands.PDF417_WIDTH);
            buffer.Add((byte)width);
            buffer.Add(BixolonCommands.PDF417_HEIGHT);
            buffer.Add((byte)height);
            buffer.Add(BixolonCommands.PDF417_STORE);
            buffer.Add((byte)((data.Length + 3) % 256));
            buffer.Add((byte)((data.Length + 3) / 256));
            buffer.Add(BixolonCommands.PDF417_STORE_PARAMETER);
            buffer.Add(data);
            buffer.Add(BixolonCommands.PDF417_PRINT);
            return buffer.GetBytes();
        }


    }

    public static class BixolonCommands
    {
            public static byte[] REAL_TIME_OFF_LINE_STATUS = new byte[] { 16, 4, 2 };
            public static byte COVER_OPEN = 4;
            public static byte PAPER_FED = 8;
            public static byte PRINTING_STOPPED = 32;
            public static byte ERROR_OCCURRED = 64;
            public static byte[] REAL_TIME_PAPER_ROLL_SENSOR_STATUS = new byte[] { 16, 4, 4 };
            public static byte PAPER_NEAR_END = 12;
            public static byte PAPER_NOT_PRESENT = 96;
            public static byte[] REAL_TIME_TPH_POWER_STATUS = new byte[] { 16, 4, 5 };
            public static byte TPH_OVER_HEATING = 4;
            public static byte SMPS_MODE = 64;
            public static byte LOW_VOLTAGE = 32;
            public static byte[] REAL_TIME_RECEIVE_BUFFER_DATA_SIZE = new byte[] { 16, 4, 6 };
            public static byte[] HORIZONTAL_TAB = new byte[] { 9 };
            public static byte[] LINE_FEED = new byte[] { 10 };
            public static byte[] FORM_FEED = new byte[] { 12 };
            public static byte[] CCARRIAGE_RETURN = new byte[] { 13 };
            public static byte[] CANCEL = new byte[] { 24 };
            public static byte[] PRINT_IN_PAGE_MODE = new byte[] { 27, 12 };
            public static byte[] PRINT_MODE = new byte[] { 27, 33 };
            public static byte PRINT_MODE_FONT_B = 1;
            public static byte PRINT_MODE_EMPHASIZED = 8;
            public static byte PRINT_MODE_DOUBLE_HEIGHT = 16;
            public static byte PRINT_MODE_DOUBLE_WIDTH = 32;
            public static byte[] ABSOLUTE_POSITION = new byte[] { 27, 36 };
            public static byte[] SPECIFY_BIT_IMAGE_8DOT_SINGLE_DENSITY = new byte[] { 27, 42, 0 };
            public static byte[] SPECIFY_BIT_IMAGE_8DOT_DOUBLE_DENSITY = new byte[] { 27, 42, 1 };
            public static byte[] SPECIFY_BIT_IMAGE_24DOT_SINGLE_DENSITY = new byte[] { 27, 42, 32 };
            public static byte[] SPECIFY_BIT_IMAGE_24DOT_DOUBLE_DENSITY = new byte[] { 27, 42, 33 };
            public static byte[] PRINT_AND_FEED = new byte[] { 27, 74 };
            public static byte[] UNDERLINE_OFF = new byte[] { 27, 45, 48 };
            public static byte[] UNDERLINE_1DOT_THICK = new byte[] { 27, 45, 49 };
            public static byte[] UNDERLINE_2DOT_THICK = new byte[] { 27, 45, 50 };
            public static byte[] EMPHASIZED_OFF = new byte[] { 27, 69, 0 };
            public static byte[] EMPHASIZED_ON = new byte[] { 27, 69, 1 };
            public static byte[] REVERSE_OFF = new byte[] { 29, 66, 0 };
            public static byte[] REVERSE_ON = new byte[] { 29, 66, 1 };
            public static byte[] DEVICE_FONT_A = new byte[] { 8, 77, 0, 65 };
            public static byte[] DEVICE_FONT_B = new byte[] { 8, 77, 0, 66 };
            public static byte[] DEVICE_FONT_C = new byte[] { 8, 77, 0, 67 };
            public static byte[] INITIALIZATION = new byte[] { 27, 64 };
            public static byte[] PAGE_MODE = new byte[] { 27, 76 };
            public static byte[] STANDARD_MODE = new byte[] { 27, 83 };
            public static byte[] PRINT_DIRECION_LEFT_TO_RIGHT = new byte[] { 27, 84, 0 };
            public static byte[] PRINT_DIRECION_BOTTOM_TO_TOP = new byte[] { 27, 84, 1 };
            public static byte[] PRINT_DIRECION_RIGHT_TO_LEFT = new byte[] { 27, 84, 2 };
            public static byte[] PRINT_DIRECION_TOP_TO_BOTTOM = new byte[] { 27, 84, 3 };
            public static byte[] PRINT_AREA = new byte[] { 27, 87 };
            public static byte[] RELATIVE_PRINT_POSITION = new byte[] { 27, 92, 0, 0 };
            public static byte[] ABSOLUTE_VERTICAL_POSITION = new byte[] { 29, 36 };
            public static byte[] MSR_SET_123TRACK_READER_MODE = new byte[] { 27, 77, 66 };
            public static byte[] MSR_CANCEL_READER_MODE = new byte[] { 27, 77, 99 };
            public static byte[] MSR_SETTING_VALUE = new byte[] { 27, 77, 73 };
            public static byte[] INTERNATIONAL_CHARACTER_SET = new byte[] { 27, 82, 0 };
            public static byte INTERNATIONAL_CHARACTER_SET_USA = 0;
            public static byte INTERNATIONAL_CHARACTER_SET_FRANCE = 1;
            public static byte INTERNATIONAL_CHARACTER_SET_GERMANY = 2;
            public static byte INTERNATIONAL_CHARACTER_SET_UK = 3;
            public static byte INTERNATIONAL_CHARACTER_SET_DENMARK1 = 4;
            public static byte INTERNATIONAL_CHARACTER_SET_SWEDEN = 5;
            public static byte INTERNATIONAL_CHARACTER_SET_ITALY = 6;
            public static byte INTERNATIONAL_CHARACTER_SET_SPAIN1 = 7;
            public static byte INTERNATIONAL_CHARACTER_SET_NORWAY = 9;
            public static byte INTERNATIONAL_CHARACTER_SET_DENMARK2 = 10;
            public static byte INTERNATIONAL_CHARACTER_SET_SPAIN2 = 11;
            public static byte INTERNATIONAL_CHARACTER_SET_LATIN_AMERICA = 12;
            public static byte INTERNATIONAL_CHARACTER_SET_KOREA = 13;
            public static byte[] ALIGNMENT_LEFT = new byte[] { 27, 97, 0 };
            public static byte[] ALIGNMENT_CENTER = new byte[] { 27, 97, 1 };
            public static byte[] ALIGNMENT_RIGHT = new byte[] { 27, 97, 2 };
            public static byte[] FEED_N = new byte[] { 27, 100, 0 };
            public static byte[] CHARACTER_CODE_PAGE = new byte[] { 27, 116 };
            public static byte CP_437_USA = 0;
            public static byte CP_KATAKANA = 1;
            public static byte CP_850_MULTILINGUAL = 2;
            public static byte CP_860_PORTUGUESE = 3;
            public static byte CP_863_CANADIAN_FRENCH = 4;
            public static byte CP_865_NORDIC = 5;
            public static byte CP_1252_LATIN1 = 16;
            public static byte CP_866_CYRILLIC2 = 17;
            public static byte CP_852_LATIN2 = 18;
            public static byte CP_858_EURO = 19;
            public static byte CP_862_HEBREW_DOS_CODE = 21;
            public static byte CP_864_ARABIC = 22;
            public static byte CP_THAI42 = 23;
            public static byte CP_1253_GREEK = 24;
            public static byte CP_1254_TURKISH = 25;
            public static byte CP_1257_BALTIC = 26;
            public static byte CP_FARSI = 27;
            public static byte CP_1251_CYRILLIC = 28;
            public static byte CP_737_GREEK = 29;
            public static byte CP_775_BALTIC = 30;
            public static byte CP_THAI14 = 31;
            public static byte CP_1255_HEBREW_NEW_CODE = 33;
            public static byte CP_THAI11 = 34;
            public static byte CP_THAI18 = 35;
            public static byte CP_855_CYRILLIC = 36;
            public static byte CP_857_TURKISH = 37;
            public static byte CP_928_GREEK = 38;
            public static byte CP_THAI16 = 39;
            public static byte CP_1256_ARABIC = 40;
            public static byte CP_1258_VIETNAM = 41;
            public static byte CP_KHMER_CAMBODIA = 42;
            public static byte CP_1250_CZECH = 47;
            public static sbyte USER_CODE_PAGE = -1;
            public static sbyte CP_TCVN3 = -2;
            public static byte[] UPSIDE_DOWN_OFF = new byte[] { 27, 123, 0 };
            public static byte[] UPSIDE_DOWN_ON = new byte[] { 27, 123, 1 };
            public static byte[] KANJI_CHATACTER_ON = new byte[] { 28, 38 };
            public static byte[] KANJI_CHATACTER_OFF = new byte[] { 28, 46 };
            public static byte[] CHARACTER_SIZE = new byte[] { 29, 33 };
            public static byte HORIZONTAL_1TIME = 0;
            public static byte HORIZONTAL_2TIMES = 16;
            public static byte HORIZONTAL_3TIMES = 32;
            public static byte HORIZONTAL_4TIMES = 48;
            public static byte HORIZONTAL_5TIMES = 64;
            public static byte HORIZONTAL_6TIMES = 80;
            public static byte HORIZONTAL_7TIMES = 96;
            public static byte HORIZONTAL_8TIMES = 112;
            public static byte VERTICCAL_1TIME = 0;
            public static byte VERTICCAL_2TIMES = 1;
            public static byte VERTICCAL_3TIMES = 2;
            public static byte VERTICCAL_4TIMES = 3;
            public static byte VERTICCAL_5TIMES = 4;
            public static byte VERTICCAL_6TIMES = 5;
            public static byte VERTICCAL_7TIMES = 6;
            public static byte VERTICCAL_8TIMES = 7;
            public static byte[] SELF_TEST = new byte[] { 29, 40, 65, 2, 0, 1, 2 };
            public static byte[] ADJUSTMENT_STARTING_POSITION = new byte[] { 29, 40, 70, 4, 0, 1, 0, 0, 0 };
            public static byte[] ADJUSTMENT_CUTTING_POSITION = new byte[] { 29, 40, 70, 4, 0, 2, 0, 0, 0 };
            public static byte[] BLACK_MARK_PAPER_FORMAT = new byte[] { 29, 40, 70, 5, 0, 112, 0, 0, 0 };
            public static byte[] PRINTER_ID_MODEL_ID = new byte[] { 29, 73, 1 };
            public static byte[] PRINTER_ID_TYPE_ID = new byte[] { 29, 73, 2 };
            public static byte[] PRINTER_ID_FEATURE_ID = new byte[] { 29, 73, 3 };
            public static byte[] PRINTER_ID_FIRMWARE_VERSION = new byte[] { 29, 73, 65 };
            public static byte[] PRINTER_ID_MANUFACTURER = new byte[] { 29, 73, 66 };
            public static byte[] PRINTER_ID_PRINTER_MODEL = new byte[] { 29, 73, 67 };
            public static byte[] PRINTER_ID_CODE_PAGE = new byte[] { 29, 73, 69 };
            public static byte[] PRINTER_ID_PRODUCT_SERIAL = new byte[] { 29, 73, 68 };
            public static byte[] BATTERY_STATUS = new byte[] { 29, 73, 98 };
            public static byte FULL = 48;
            public static byte HIGH = 49;
            public static byte MIDDLE = 50;
            public static byte LOW = 51;
            public static byte[] ENABLE_ASB = new byte[] { 29, 97, 1 };
            public static byte[] DISABLE_ASB = new byte[] { 29, 97, 0 };
            public static byte ASB_OFF_LINE = 8;
            public static byte ASB_COVER_OPEN = 32;
            public static byte ASB_PAPER_FED = 64;
            public static byte ASB_BATTERY_LOW = 1;
            public static byte ASB_UNRECOVERABLE_ERROR = 32;
            public static byte ASB_AUTOMATICALLY_RECOVERABLE_ERROR = 64;
            public static byte ASB_NO_PAPER = 12;
            public static byte ASB_FIXED = 15;
            public static byte[] BAR_CODE_HEIGHT = new byte[] { 29, 104 };
            public static byte[] BAR_CODE_WIDTH = new byte[] { 29, 119 };
            public static byte[] BAR_CODE_UPC_A = new byte[] { 29, 107, 65 };
            public static byte[] BAR_CODE_UPC_E = new byte[] { 29, 107, 66 };
            public static byte[] BAR_CODE_EAN13 = new byte[] { 29, 107, 67 };
            public static byte[] BAR_CODE_EAN8 = new byte[] { 29, 107, 68 };
            public static byte[] BAR_CODE_CODE39 = new byte[] { 29, 107, 69 };
            public static byte[] BAR_CODE_ITF = new byte[] { 29, 107, 70 };
            public static byte[] BAR_CODE_CODABAR = new byte[] { 29, 107, 71 };
            public static byte[] BAR_CODE_CODE93 = new byte[] { 29, 107, 72 };
            public static byte[] BAR_CODE_CODE128 = new byte[] { 29, 107, 73 };
            public static byte[] HRI_CHARACTERS_NOT_PRINTED = new byte[] { 29, 72, 0 };
            public static byte[] HRI_CHARACTERS_ABOVE_BAR_CODE = new byte[] { 29, 72, 1 };
            public static byte[] HRI_CHARACTERS_BELOW_BAR_CODE = new byte[] { 29, 72, 2 };
            public static byte[] HRI_CHARACTERS_ABOVE_AND_BELOW_BAR_CODE = new byte[] { 29, 72, 3 };
            public static byte[] PDF417_COLUMN_NUMBER_AUTO = new byte[] { 29, 40, 107, 3, 0, 48, 65, 0 };
            public static byte[] PDF417_ROW_NUMBER_AUTO = new byte[] { 29, 40, 107, 3, 0, 48, 66, 0 };
            public static byte[] PDF417_WIDTH = new byte[] { 29, 40, 107, 3, 0, 48, 67 };
            public static byte[] PDF417_HEIGHT = new byte[] { 29, 40, 107, 3, 0, 48, 68 };
            public static byte[] PDF417_STORE = new byte[] { 29, 40, 107 };
            public static byte[] PDF417_STORE_PARAMETER = new byte[] { 48, 80, 48 };
            public static byte[] PDF417_PRINT = new byte[] { 29, 40, 107, 3, 0, 48, 81, 48 };
            public static byte[] QR_CODE_MODEL1 = new byte[] { 29, 40, 107, 4, 0, 49, 65, 49, 0 };
            public static byte[] QR_CODE_MODEL2 = new byte[] { 29, 40, 107, 4, 0, 49, 65, 50, 0 };
            public static byte[] QR_CODE_SIZE = new byte[] { 29, 40, 107, 3, 0, 49, 67 };
            public static byte[] QR_CODE_ERROR_CORRECTION_LEVEL = new byte[] { 29, 40, 107, 3, 0, 49, 69 };
            public static byte[] QR_CODE_STORE = new byte[] { 29, 40, 107 };
            public static byte[] QR_CODE_STORE_PARAMETER = new byte[] { 49, 80, 48 };
            public static byte[] QR_CODE_PRINT = new byte[] { 29, 40, 107, 3, 0, 49, 81, 48 };
            public static byte[] QR_CODE_VERSION = new byte[] { 31, 31, 98 };
            public static byte[] MAXI_CODE_MODE2 = new byte[] { 29, 40, 107, 3, 0, 50, 65, 50 };
            public static byte[] MAXI_CODE_MODE3 = new byte[] { 29, 40, 107, 3, 0, 50, 65, 51 };
            public static byte[] MAXI_CODE_MODE4 = new byte[] { 29, 40, 107, 3, 0, 50, 65, 52 };
            public static byte[] MAXI_CODE_STORE = new byte[] { 29, 40, 107 };
            public static byte[] MAXI_CODE_STORE_PARAMETER = new byte[] { 50, 80, 48 };
            public static byte[] MAXI_CODE_PRINT = new byte[] { 29, 40, 107, 3, 0, 50, 81, 48 };
            public static byte[] DATA_MATRIX_SIZE = new byte[] { 29, 40, 107, 3, 0, 51, 67 };
            public static byte[] DATA_MATRIX_STORE = new byte[] { 29, 40, 107 };
            public static byte[] DATA_MATRIX_STORE_PARAMETER = new byte[] { 51, 80, 48 };
            public static byte[] DATA_MATRIX_PRINT = new byte[] { 29, 40, 107, 3, 0, 51, 81, 48 };
            public static byte[] DRAWER_CONNECTOR_STATUS = new byte[] { 29, 114, 2 };
            public static byte DRAWER_PIN3_HIGH = 1;
            public static byte[] RASTER_BIT_IMAGE_NORMAL = new byte[] { 29, 118, 48, 0 };
            public static byte[] RASTER_BIT_IMAGE_DOUBLE_WIDTH = new byte[] { 29, 118, 48, 1 };
            public static byte[] RASTER_BIT_IMAGE_DOUBLE_HEIGHT = new byte[] { 29, 118, 48, 2 };
            public static byte[] RASTER_BIT_IMAGE_QUADRUPLE = new byte[] { 29, 118, 48, 3 };
            public static byte[] AUTOMATIC_CALIBRATION = new byte[] { 8, 76, 65 };
            public static byte[] LABEL_MODE = new byte[] { 8, 76, 76 };
            public static byte[] RECEIPT_MODE = new byte[] { 8, 76, 82 };
            public static byte[] MEMORY_SWITCH_SETTING_FUNCTION1 = new byte[] { 29, 40, 69, 3, 0, 1, 73, 78 };
            public static byte[] MEMORY_SWITCH_SETTING_FUNCTION2 = new byte[] { 29, 40, 69, 4, 0, 2, 79, 85, 84 };
            public static byte[] MEMORY_SWITCH_SETTING_FUNCTION3 = new byte[] { 29, 40, 69, 10, 0, 3 };
            public static byte[] DOUBLE_BYTE_FONT = new byte[] { 2, 48, 48, 48, 48, 48, 48, 48, 49 };
            public static byte[] KS5601 = new byte[] { 12, 48, 48, 48, 48, 48, 48, 48, 49 };
            public static byte[] BIG5_GB2312 = new byte[] { 12, 48, 48, 48, 48, 48, 48, 49, 48 };
            public static byte[] SHIFT_JIS = new byte[] { 12, 48, 48, 48, 48, 48, 48, 49, 49 };
            public static byte[] SINGLE_BYTE_FONT = new byte[] { 12, 48, 48, 48, 48, 48, 48, 48, 48 };
            public static byte[] CODE_PAGE_437_USA = new byte[] { 2, 48, 48, 48, 48, 48, 48, 48, 48 };
            public static byte[] CODE_PAGE_KATAKANA = new byte[] { 2, 48, 48, 48, 48, 49, 48, 48, 48 };
            public static byte[] CODE_PAGE_850_MULTILINGUAL = new byte[] { 2, 48, 48, 48, 49, 48, 48, 48, 48 };
            public static byte[] CODE_PAGE_860_PORTUGUESE = new byte[] { 2, 48, 48, 48, 49, 49, 48, 48, 48 };
            public static byte[] CODE_PAGE_863_CCANADIAN_FRENCH = new byte[] { 2, 48, 48, 49, 48, 48, 48, 48, 48 };
            public static byte[] CODE_PAGE_865_NORDIC = new byte[] { 2, 48, 48, 49, 48, 49, 48, 48, 48 };
            public static byte[] CODE_PAGE_1252_LATIN1 = new byte[] { 2, 48, 48, 49, 49, 48, 48, 48, 48 };
            public static byte[] CODE_PAGE_866_CYRILLIC2 = new byte[] { 2, 48, 48, 49, 49, 49, 48, 48, 48 };
            public static byte[] CODE_PAGE_852_LATIN2 = new byte[] { 2, 48, 49, 48, 48, 48, 48, 48, 48 };
            public static byte[] CODE_PAGE_858_EURO = new byte[] { 2, 48, 49, 48, 48, 49, 48, 48, 48 };
            public static byte[] CODE_PAGE_862_HEBREW_DOS_CODE = new byte[] { 2, 48, 49, 48, 49, 48, 48, 48, 48 };
            public static byte[] CODE_PAGE_864_ARABIC = new byte[] { 2, 48, 49, 48, 49, 49, 48, 48, 48 };
            public static byte[] CODE_PAGE_THAI42 = new byte[] { 2, 48, 49, 49, 48, 48, 48, 48, 48 };
            public static byte[] CODE_PAGE_1253_GREEK = new byte[] { 2, 48, 49, 49, 48, 49, 48, 48, 48 };
            public static byte[] CODE_PAGE_1254_TURKISH = new byte[] { 2, 48, 49, 49, 49, 48, 48, 48, 48 };
            public static byte[] CODE_PAGE_1257_BALTIC = new byte[] { 2, 48, 49, 49, 49, 49, 48, 48, 48 };
            public static byte[] CODE_PAGE_FARSI = new byte[] { 2, 49, 48, 48, 48, 48, 48, 48, 48 };
            public static byte[] CODE_PAGE_1251_CYRILLIC = new byte[] { 2, 49, 48, 48, 48, 49, 48, 48, 48 };
            public static byte[] CODE_PAGE_737_GREEK = new byte[] { 2, 49, 48, 48, 49, 48, 48, 48, 48 };
            public static byte[] CODE_PAGE_775_BALTIC = new byte[] { 2, 49, 48, 48, 49, 49, 48, 48, 48 };
            public static byte[] CODE_PAGE_THAI14 = new byte[] { 2, 49, 48, 49, 48, 48, 48, 48, 48 };
            public static byte[] CODE_PAGE_HEBREW_OLD_CODE = new byte[] { 2, 49, 48, 49, 48, 49, 48, 48, 48 };
            public static byte[] CODE_PAGE_1255_HEBREW_NEW_CODE = new byte[] { 2, 49, 48, 49, 49, 48, 48, 48, 48 };
            public static byte[] CODE_PAGE_THAI11 = new byte[] { 2, 49, 48, 49, 49, 49, 48, 48, 48 };
            public static byte[] CODE_PAGE_THAI18 = new byte[] { 2, 49, 49, 48, 48, 48, 48, 48, 48 };
            public static byte[] CODE_PAGE_855_CYRILLIC = new byte[] { 2, 49, 49, 48, 48, 49, 48, 48, 48 };
            public static byte[] CODE_PAGE_857_TURKISH = new byte[] { 2, 49, 49, 48, 49, 48, 48, 48, 48 };
            public static byte[] CODE_PAGE_928_GREEK = new byte[] { 2, 49, 49, 48, 49, 49, 48, 48, 48 };
            public static byte[] CODE_PAGE_THAI16 = new byte[] { 2, 49, 49, 49, 48, 48, 48, 48, 48 };
            public static byte[] CODE_PAGE_1256_ARB = new byte[] { 2, 49, 49, 49, 48, 49, 48, 48, 48 };
            public static byte[] CODE_PAGE_1258_VIETNAM = new byte[] { 2, 49, 49, 49, 49, 48, 48, 48, 48 };
            public static byte[] CODE_PAGE_KHMER_CAMBODIA = new byte[] { 2, 49, 49, 49, 49, 49, 48, 48, 48 };
            public static byte[] CODE_PAGE_1250_CZECH = new byte[] { 2, 48, 48, 49, 48, 48, 49, 48, 48 };
            public static byte[] MEMORY_SWITCH_SETTING_FUNCTION4 = new byte[] { 29, 40, 69, 2, 0, 4 };
            public static byte PRINT_SPEED_DENSITY = 1;
            public static byte SELECT_SINGLE_BYTE_FONT = 2;
            public static byte SELECT_DOUBLE_BYTE_FONT = 12;
            public static byte[] ENABLE_PRINTER = new byte[] { 27, 61, 1 };
            public static byte[] DISABLE_PRINTER = new byte[] { 27, 61, 2 };
            public static byte[] NV_IMAGE_REMAINING_CAPACITY = new byte[] { 29, 40, 76, 2, 0, 48, 51 };
            public static byte[] NV_IMAGE_KEY_CODE_LIST = new byte[] { 29, 40, 76, 4, 0, 48, 64, 75, 67 };
            public static byte NV_IMAGE_KEY_CODE_LIST_ACK = 6;
            public static byte[] NV_IMAGE_ALL_REMOVAL = new byte[] { 29, 40, 76, 5, 0, 48, 65, 67, 76, 82 };
            public static byte[] NV_IMAGE_REMOVAL = new byte[] { 29, 40, 76, 4, 0, 48, 66 };
            public static byte[] NV_IMAGE_PRINT = new byte[] { 29, 40, 76, 6, 0, 48, 69 };
            public static byte[] NV_IMAGE_PRINT_FOOTER = new byte[] { 1, 1 };
            public static byte[] NV_IMAGE_DEFINITION = new byte[] { 29, 40, 76 };
            public static byte[] NV_IMAGE_DEFINITION_FUNCTION = new byte[] { 48, 67, 48 };
            public static byte NV_IMAGE_DEFINITION_COLORS_NUMBER = 1;
            public static byte NV_IMAGE_DEFINITION_COLOR1 = 49;
            public static byte[] DISABLE_POWER_SAVING_TIME = new byte[] { 8, 94, 80, 48, 0, 0 };
            public static byte[] ENABLE_POWER_SAVING_TIME = new byte[] { 8, 94, 80, 48, 1 };
            public static byte[] GET_POWER_SAVING_TIME_STATUS = new byte[] { 8, 94, 80, 49 };
            public static byte[] PARTIAL_CUT = new byte[] { 29, 86, 65 };
            public static byte[] PARTIAL_CUT_NO_FEED = new byte[] { 27, 105 };
            public static byte[] BS_MEMORY_SWITCH_SETTING_FUNCTION1 = new byte[] { 8, 94, 69, 1, 0, 1 };
            public static byte[] BS_MEMORY_SWITCH_SETTING_FUNCTION2 = new byte[] { 8, 94, 69, 1, 0, 2 };
            public static byte[] BS_MEMORY_SWITCH_SETTING_FUNCTION3 = new byte[] { 8, 94, 69, 17, 0, 3 };
            public static byte[] BS_MEMORY_SWITCH_SETTING_FUNCTION4_2 = new byte[] { 8, 94, 69, 2, 0, 4, 2 };
            public static byte[] BS_MEMORY_SWITCH_SETTING_FUNCTION4_8 = new byte[] { 8, 94, 69, 2, 0, 4, 8 };
            public static byte[] PRINT_COLOR_BLACK = new byte[] { 27, 114, 48 };
            public static byte[] PRINT_COLOR_RED = new byte[] { 27, 114, 49 };
            public static byte[] DRAWER_CONNECTOR_PIN2_REALTIME = new byte[] { 16, 20, 1, 0 };
            public static byte[] DRAWER_CONNECTOR_PIN5_REALTIME = new byte[] { 16, 20, 1, 1 };
            public static byte[] DRAWER_CONNECTOR_PIN2 = new byte[] { 27, 112, 0, 127, 127 };
            public static byte[] DRAWER_CONNECTOR_PIN5 = new byte[] { 27, 112, 1, 127, 127 };
            public static byte[] FS_PRINT_NV_IMAGE_NORMAL = new byte[] { 28, 112, 1, 48 };
            public static byte[] FS_PRINT_NV_IMAGE_DOUBLE_WIDTH = new byte[] { 28, 112, 1, 49 };
            public static byte[] FS_PRINT_NV_IMAGE_DOUBLE_HEIGHT = new byte[] { 28, 112, 1, 50 };
            public static byte[] FS_PRINT_NV_IMAGE_QUADRUPLE = new byte[] { 28, 112, 1, 51 };
            public static byte[] FS_DEFINE_NV_IMAGE = new byte[] { 28, 113, 1 };
            public static byte[] BPP_100_FIRMWARE_DOWNLOAD_OLD = new byte[] { 8, 35, 109, 116, 109, 115 };
            public static byte[] BPP_100_FIRMWARE_DOWNLOAD_NEW = new byte[] { 8, 35, 109, 80, 79, 83, 48, 48, 50 };
            public static byte[] PRINT_LINE = new byte[] { 28, 68, 76, 73, 78, 69 };
            public static byte[] PRINT_BOX = new byte[] { 28, 68, 66, 79, 88 };
            public static byte[] SMART_CARD_POWER_DOWN = new byte[] { 31, 83, 17 };
            public static byte[] SMART_CARD_POWER_UP = new byte[] { 31, 83, 18 };
            public static byte[] SMART_CARD_EXCHANGE_APDU = new byte[] { 31, 83, 21 };
            public static byte[] SMART_CARD_SET_APDU_MODE = new byte[] { 31, 83, 22, 0 };
            public static byte[] SMART_CARD_SET_TPDU_MODE = new byte[] { 31, 83, 22, 1 };
            public static byte[] SMART_CARD_STATUS = new byte[] { 31, 83, 23 };
            public static byte[] SMART_CARD_SELECT_SMART_CARD = new byte[] { 31, 83, 32, 48 };
            public static byte[] SMART_CARD_SELECT_SAM1 = new byte[] { 31, 83, 32, 49 };
            public static byte[] SMART_CARD_SELECT_SAM2 = new byte[] { 31, 83, 32, 50 };
            //public static byte SMART_CARD_RESPONSE_HEADER1 = 2;
            //public static byte SMART_CARD_RESPONSE_HEADER2 = -128;
            //public static byte SMART_CARD_RESPONSE_FOOTER = -1;
            
    }


    public static class BixolonESC
    {
        public static int MESSAGE_STATE_CHANGE = 1;
        public static int MESSAGE_READ = 2;
        public static int MESSAGE_WRITE = 3;
        public static int MESSAGE_DEVICE_NAME = 4;
        public static int MESSAGE_TOAST = 5;
        public static int MESSAGE_LOG = 6;
        public static int MESSAGE_BLUETOOTH_DEVICE_SET = 7;
        public static int MESSAGE_PRINT_COMPLETE = 8;
        public static int MESSAGE_COMPLETE_PROCESS_BITMAP = 9;
        public static int MESSAGE_USB_DEVICE_SET = 10;
        public static int MESSAGE_USB_SERIAL_SET = 11;
        public static int MESSAGE_NETWORK_DEVICE_SET = 12;
        public static int MESSAGE_ERROR_INVALID_ARGUMENT = 13;
        public static int MESSAGE_ERROR_OUT_OF_MEMORY = 14;
        public static int MESSAGE_ERROR_NV_MEMORY_CAPACITY = 15;
        public static int MESSAGE_ERROR_CLASSES_NOT_FOUND = 16;
        public static int STATE_NONE = 0;
        public static int STATE_CONNECTING = 1;
        public static int STATE_CONNECTED = 2;
        public static String KEY_STRING_DEVICE_NAME = "device_name";
    public static String KEY_STRING_TOAST = "toast";
    public static String KEY_STRING_MSR_TRACK1 = "msr_track1";
    public static String KEY_STRING_MSR_TRACK2 = "msr_track2";
    public static String KEY_STRING_MSR_TRACK3 = "msr_track3";
    public static String KEY_STRING_PRINTER_ID = "printer_id";
    public static String KEY_STRING_DIRECT_IO = "direct_io";
    public static String KEY_STRING_CODE_PAGE = "code_page";
    public static String KEY_STRING_MONO_PIXELS = "mono_pixels";
    public static int MSR_MODE_TRACK123_COMMAND = 65;
        public static int MSR_MODE_TRACK1_AUTO = 66;
        public static int MSR_MODE_TRACK2_AUTO = 67;
        public static int MSR_MODE_TRACK3_AUTO = 68;
        public static int MSR_MODE_TRACK12_AUTO = 69;
        public static int MSR_MODE_TRACK23_AUTO = 70;
        public static int MSR_MODE_TRACK123_AUTO = 71;
        public static int MSR_MODE_NOT_USED = 72;
        public static String NV_IMAGE_KEY_CODES = "nv_image_key_codes";
    public static String EXECUTE_DIRECT_IO = "EXECUTE_DIRECT_IO";
    public static int ALIGNMENT_LEFT = 0;
        public static int ALIGNMENT_CENTER = 1;
        public static int ALIGNMENT_RIGHT = 2;
        public static int TEXT_ATTRIBUTE_FONT_A = 0;
        public static int TEXT_ATTRIBUTE_FONT_B = 1;
        public static int TEXT_ATTRIBUTE_FONT_C = 2;
        public static int TEXT_ATTRIBUTE_UNDERLINE1 = 4;
        public static int TEXT_ATTRIBUTE_UNDERLINE2 = 8;
        public static int TEXT_ATTRIBUTE_EMPHASIZED = 16;
        public static int TEXT_ATTRIBUTE_REVERSE = 32;
        public static int TEXT_ATTRIBUTE_REVERSE_ORDER = 64;
        public static int TEXT_SIZE_HORIZONTAL1 = 0;
        public static int TEXT_SIZE_HORIZONTAL2 = 16;
        public static int TEXT_SIZE_HORIZONTAL3 = 32;
        public static int TEXT_SIZE_HORIZONTAL4 = 48;
        public static int TEXT_SIZE_HORIZONTAL5 = 64;
        public static int TEXT_SIZE_HORIZONTAL6 = 80;
        public static int TEXT_SIZE_HORIZONTAL7 = 96;
        public static int TEXT_SIZE_HORIZONTAL8 = 112;
        public static int TEXT_SIZE_VERTICAL1 = 0;
        public static int TEXT_SIZE_VERTICAL2 = 1;
        public static int TEXT_SIZE_VERTICAL3 = 2;
        public static int TEXT_SIZE_VERTICAL4 = 3;
        public static int TEXT_SIZE_VERTICAL5 = 4;
        public static int TEXT_SIZE_VERTICAL6 = 5;
        public static int TEXT_SIZE_VERTICAL7 = 6;
        public static int TEXT_SIZE_VERTICAL8 = 7;
        public static byte CODE_PAGE_437_USA = 0;
        public static byte CODE_PAGE_KATAKANA = 1;
        public static byte CODE_PAGE_850_MULTILINGUAL = 2;
        public static byte CODE_PAGE_860_PORTUGUESE = 3;
        public static byte CODE_PAGE_863_CANADIAN_FRENCH = 4;
        public static byte CODE_PAGE_865_NORDIC = 5;
        public static byte CODE_PAGE_1252_LATIN1 = 16;
        public static byte CODE_PAGE_866_CYRILLIC2 = 17;
        public static byte CODE_PAGE_852_LATIN2 = 18;
        public static byte CODE_PAGE_858_EURO = 19;
        public static byte CODE_PAGE_862_HEBREW_DOS_CODE = 21;
        public static byte CODE_PAGE_864_ARABIC = 22;
        public static byte CODE_PAGE_THAI42 = 23;
        public static byte CODE_PAGE_1253_GREEK = 24;
        public static byte CODE_PAGE_1254_TURKISH = 25;
        public static byte CODE_PAGE_1257_BALTIC = 26;
        public static byte CODE_PAGE_FARSI = 27;
        public static byte CODE_PAGE_1251_CYRILLIC = 28;
        public static byte CODE_PAGE_737_GREEK = 29;
        public static byte CODE_PAGE_775_BALTIC = 30;
        public static byte CODE_PAGE_THAI14 = 31;
        public static byte CODE_PAGE_1255_HEBREW_NEW_CODE = 33;
        public static byte CODE_PAGE_THAI11 = 34;
        public static byte CODE_PAGE_THAI18 = 35;
        public static byte CODE_PAGE_855_CYRILLIC = 36;
        public static byte CODE_PAGE_857_TURKISH = 37;
        public static byte CODE_PAGE_928_GREEK = 38;
        public static byte CODE_PAGE_THAI16 = 39;
        public static byte CODE_PAGE_1256_ARABIC = 40;
        public static byte CODE_PAGE_1258_VIETNAM = 41;
        public static byte CODE_PAGE_KHMER_CAMBODIA = 42;
        public static byte CODE_PAGE_1250_CZECH = 47;
        public static sbyte CODE_PAGE_TCVN3 = -2;
        public static byte DOUBLE_BYTE_FONT_GB18030 = 123;
        public static byte DOUBLE_BYTE_FONT_KS5601 = 124;
        public static byte DOUBLE_BYTE_FONT_BIG5 = 125;
        public static byte DOUBLE_BYTE_FONT_GB2312 = 126;
        public static byte DOUBLE_BYTE_FONT_SHIFT_JIS = 127;
        public static int BAR_CODE_UPC_A = 65;
        public static int BAR_CODE_UPC_E = 66;
        public static int BAR_CODE_EAN13 = 67;
        public static int BAR_CODE_EAN8 = 68;
        public static int BAR_CODE_CODE39 = 69;
        public static int BAR_CODE_ITF = 70;
        public static int BAR_CODE_CODABAR = 71;
        public static int BAR_CODE_CODE93 = 72;
        public static int BAR_CODE_CODE128 = 73;
        public static int HRI_CHARACTER_NOT_PRINTED = 0;
        public static int HRI_CHARACTERS_ABOVE_BAR_CODE = 1;
        public static int HRI_CHARACTERS_BELOW_BAR_CODE = 2;
        public static int HRI_CHARACTERS_ABOVE_AND_BELOW_BAR_CODE = 3;
        public static int QR_CODE_MODEL1 = 49;
        public static int QR_CODE_MODEL2 = 50;
        public static int QR_CODE_ERROR_CORRECTION_LEVEL_L = 48;
        public static int QR_CODE_ERROR_CORRECTION_LEVEL_M = 49;
        public static int QR_CODE_ERROR_CORRECTION_LEVEL_Q = 50;
        public static int QR_CODE_ERROR_CORRECTION_LEVEL_H = 51;
        public static int MAXI_CODE_MODE2 = 50;
        public static int MAXI_CODE_MODE3 = 51;
        public static int MAXI_CODE_MODE4 = 52;
        public static int PRINTER_ID_MODEL_ID = 1;
        public static int PRINTER_ID_TYPE_ID = 2;
        public static int PRINTER_ID_FEATURE_ID = 3;
        public static int PRINTER_ID_FIRMWARE_VERSION = 65;
        public static int PRINTER_ID_MANUFACTURER = 66;
        public static int PRINTER_ID_PRINTER_MODEL = 67;
        public static int PRINTER_ID_CODE_PAGE = 69;
        public static int PRINTER_ID_PRODUCT_SERIAL = 68;
        public static int STATUS_NORMAL = 0;
        public static int STATUS_COVER_OPEN = 4;
        public static int STATUS_PAPER_FED = 8;
        public static int STATUS_PRINTING_STOPPED = 32;
        public static int STATUS_ERROR_OCCURRED = 64;
        public static int STATUS_PAPER_NEAR_END = 12;
        public static int STATUS_PAPER_NOT_PRESENT = 96;
        public static int STATUS_TPH_OVER_HEATING = 4;
        public static int STATUS_SMPS_MODE = 64;
        public static int STATUS_BATTERY_LOW_VOLTAGE = 32;
        public static int STATUS_BATTERY_FULL = 48;
        public static int STATUS_BATTERY_HIGH = 49;
        public static int STATUS_BATTERY_MIDDLE = 50;
        public static int STATUS_BATTERY_LOW = 51;
        public static int BITMAP_WIDTH_FULL = -1;
        public static int BITMAP_WIDTH_NONE = 0;
        public static int AUTO_STATUS_OFF_LINE = 8;
        public static int AUTO_STATUS_COVER_OPEN = 32;
        public static int AUTO_STATUS_PAPER_FED = 64;
        public static int AUTO_STATUS_BATTERY_LOW = 1;
        public static int AUTO_STATUS_UNRECOVERABLE_ERROR = 2;
        public static int AUTO_STATUS_AUTO_RECOVERABLE_ERROR = 4;
        public static int AUTO_STATUS_NO_PAPER = 12;
        public static int DIRECTION_0_DEGREE_ROTATION = 0;
        public static int DIRECTION_90_DEGREE_ROTATION = 1;
        public static int DIRECTION_180_DEGREE_ROTATION = 2;
        public static int DIRECTION_270_DEGREE_ROTATION = 3;
        public static int DRAWER_CONNECTOR_PIN2 = 0;
        public static int DRAWER_CONNECTOR_PIN5 = 1;
        public static int PROCESS_NONE = 0;
        public static int PROCESS_RESPONSE = 1;
        public static int PROCESS_GET_STATUS = 2;
        public static int PROCESS_GET_STATUS1 = 3;
        public static int PROCESS_GET_STATUS2 = 4;
        public static int PROCESS_GET_PRINTER_ID = 5;
        public static int PROCESS_AUTO_STATUS_BACK = 6;
        public static int PROCESS_GET_TPH_THEMISTOR_STATUS = 7;
        public static int PROCESS_GET_POWER_MODE = 8;
        public static int PROCESS_EXECUTE_DIRECT_IO = 9;
        public static int PROCESS_SET_SINGLE_BYTE_FONT = 10;
        public static int PROCESS_SET_DOUBLE_BYTE_FONT = 11;
        public static int PROCESS_GET_DOUBLE_BYTE_FONT = 13;
        public static int PROCESS_GET_PRINT_SPEED = 14;
        public static int PROCESS_SET_PRINT_SPEED = 15;
        public static int PROCESS_GET_PRINT_DENSITY = 16;
        public static int PROCESS_SET_PRINT_DENSITY = 17;
        public static int PROCESS_GET_NV_IMAGE_KEY_CODES = 18;
        public static int PROCESS_DEFINE_NV_IMAGE = 19;
        public static int PROCESS_REMOVE_NV_IMAGE = 20;
        public static int PROCESS_UPDATE_FIRMWARE = 21;
        public static int PROCESS_CONNECTED = 22;
        public static int PROCESS_MSR_TRACK = 23;
        public static int PROCESS_GET_POWER_SAVING_MODE = 24;
        public static int PROCESS_GET_BATTERY_VOLTAGE_STATUS = 25;
        public static int PROCESS_GET_BS_CODE_PAGE = 26;
        public static int PROCESS_SET_BS_CODE_PAGE_START = 27;
        public static int PROCESS_SET_BS_CODE_PAGE = 28;
        public static int PROCESS_GET_BATTERY_STATUS = 29;
        public static int PROCESS_GET_MSR_MODE = 30;
        public static int PROCESS_GET_RECEIVE_BUFFER_DATA_SIZE = 31;
        public static int PROCESS_KICK_OUT_CASH_DRAWER = 32;
        public static int PROCESS_SMART_CARD_POWER_DOWN = 33;
        public static int PROCESS_SMART_CARD_POWER_UP = 34;
        public static int PROCESS_SMART_CARD_EXCHANGE_APDU = 35;
        public static int PROCESS_SMART_CARD_CHANGE_MODE = 36;
        public static int PROCESS_SMART_CARD_STATUS = 37;
        public static int PROCESS_SMART_CARD_SELECT = 38;
        public static int SMART_CARD_MODE_APDU = 0;
        public static int SMART_CARD_MODE_TPDU = 1;
        public static int SMART_CARD_SELECT_SMART_CARD = 48;
        public static int SMART_CARD_SELECT_SAM1 = 49;
        public static int SMART_CARD_SELECT_SAM2 = 50;
        public static int SMART_CARD_STATUS_CODE_COMMAND_SUCCESSFUL = 0;
        public static int SMART_CARD_STATUS_CODE_WRONG_COMMAND_LENGTH = 1;
        public static int SMART_CARD_STATUS_CODE_EXCESSIVE_CURRENT = 2;
        public static int SMART_CARD_STATUS_CODE_DEFECTIVE_VOLTAGE = 3;
        public static int[] SMART_CARD_STATUS_CODE_INVALID_BYTE = new int[] { 7, 8, 9, 10, 21 };
        public static int SMART_CARD_STATUS_CODE_SHORT_CIRCUITING = 162;
        public static int SMART_CARD_STATUS_CODE_ATR_TOO_LONG = 163;
        public static int SMART_CARD_STATUS_CODE_CARD_TOO_LONG = 176;
        public static int SMART_CARD_STATUS_CODE_EMV_PROTOCOL_ERROR = 187;
        public static int SMART_CARD_STATUS_CODE_CARD_PROTOCOL_ERROR = 189;
        public static int SMART_CARD_STATUS_CODE_APDU_COMMAND_LENGTH_WRONG = 190;
        public static int SMART_CARD_STATUS_CODE_ATR_TCK_INAVALID = 247;
        public static int SMART_CARD_STATUS_CODE_ATR_TS_INVALID = 248;
        public static int SMART_CARD_STATUS_CODE_PARITY_ERROR = 253;
        public static int SMART_CARD_STATUS_CODE_CARD_NOT_PRESENT = 254;
        public static byte SMART_CARD_STATUS_BIT_POWER_5V = 1;
        public static byte SMART_CARD_STATUS_BIT_POWER_3V = 2;
        public static byte SMART_CARD_STATUS_BIT_POWER_18V = 3;
        public static byte SMART_CARD_STATUS_BIT_POWERED = 4;
        public static byte SMART_CARD_STATUS_BIT_INSERTED = 8;
        public static int PRINT_SPEED_LOW = 0;
        public static int PRINT_SPEED_MEDIUM = 1;
        public static int PRINT_SPEED_HIGH = 2;
        public static int PRINT_DENSITY_LIGHT = 0;
        public static int PRINT_DENSITY_DEFAULT = 1;
        public static int PRINT_DENSITY_DARK = 2;
        public static int COLOR_BLACK = 0;
        public static int COLOR_RED = 1;
        private static int MAX_BITMAP_SIZE = 196480;
        private static int MAX_NV_GRAPHIC_SIZE = 65535;
        private static int MAX_BITMAP_HEIGHT = 1662;
        private static int MAX_BITMAP_HEIGHT_4inch = 80;
    }
}