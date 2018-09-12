using System;
using System.Collections;
using System.Text;

namespace PG.Servisi.resources.cs.ispis
{
    public class ByteCollection : CollectionBase
    {
        public ByteCollection()
        {
        }

        public ByteCollection(int capacity)
        {
            InnerList.Capacity = capacity;
        }

        public byte this[int index]
        {
            get
            {
                return (byte)List[index];
            }
            set
            {
                List[index] = value;
            }
        }

        public static ByteCollection operator +(ByteCollection bc1, ByteCollection bc2)
        {
            ByteCollection bc = new ByteCollection(bc1.Count + bc2.Count);
            bc.Add(bc1);
            bc.Add(bc2);
            return bc;
        }

        public static ByteCollection operator +(string bc1String, ByteCollection bc2)
        {
            ByteCollection bc1 = StringToByteCollection(bc1String);
            ByteCollection bc = new ByteCollection(bc1.Count + bc2.Count);
            bc.Add(bc1);
            bc.Add(bc2);
            return bc;
        }

        public static ByteCollection operator +(ByteCollection bc1, string bc2String)
        {
            ByteCollection bc2 = StringToByteCollection(bc2String);
            ByteCollection bc = new ByteCollection(bc1.Count + bc2.Count);
            bc.Add(bc1);
            bc.Add(bc2);
            return bc;
        }

        public void Add(ByteCollection bColl)
        {
            InnerList.AddRange(bColl);
            //Add(bColl.GetBytes());
        }

        public void Add(string text)
        {
            InnerList.AddRange(StringToByteCollection(text));
        }

        public void Add(byte[] bytes)
        {
            InnerList.AddRange(bytes);
        }

        public void Add(char c)
        {
            List.Add(Convert.ToByte(c));
        }

        public void Add(byte b)
        {
            List.Add(b);
        }

        public byte GetLast()
        {
            return GetLast(0);
        }

        public byte GetLast(int i)
        {
            byte ret = Byte.MinValue;
            if (List.Count > 0) ret = this[List.Count - Math.Abs(i) - 1];
            return ret;
        }

        public byte GetAndRemoveLast()
        {
            byte ret = Byte.MinValue;
            if (List.Count > 0)
            {
                ret = this[List.Count - 1];
                RemoveAt(List.Count - 1);
            }
            return ret;
        }

        public void RemoveLast()
        {
            RemoveLast(1);
        }

        public void RemoveLast(int i)
        {
            i = Math.Abs(i);
            while (i-- > 0)
            {
                if (List.Count > 0) RemoveAt(List.Count - 1);
                else break;
            }
        }

        public void Insert(int index, byte b)
        {
            List.Insert(index, b);
        }

        public byte[] GetBytes()
        {
            return (byte[])InnerList.ToArray(typeof(byte));
        }

        public static ByteCollection StringToByteCollection(string s) { return StringToByteCollection(s, 0, '_'); }
        public static ByteCollection StringToByteCollection(string s, int lenght) { return StringToByteCollection(s, lenght, '_'); }

        public static ByteCollection StringToByteCollection(string s, int length, char fill)
        {
            ByteCollection ret = new ByteCollection();
            if (length > 0 && length < s.Length) ret.Add(Encoding.Default.GetBytes(s.ToCharArray(), 0, length));
            else ret.Add(Encoding.Default.GetBytes(s));
            while (ret.Count < length) ret.Add(Convert.ToByte(fill));
            return ret;
        }

        public override string ToString()
        {
            return ToString(0, Count);
        }

        public string ToString(int start, int length)
        {
            length = Math.Min(length, Count - start);
            StringBuilder ret = new StringBuilder(length);
            for (int i = start; i < start + length; i++)
            {
                ret.Append(Convert.ToChar(this[i]));
            }
            return ret.ToString();
        }
    }
}