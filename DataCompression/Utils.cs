using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace DataCompression
{
    static class Utils
    {
        public static String GetInfo()
        {
            return "Utils";
        }

        public static String[] ReadStringArray(String path)
        {
            String line;
            List<String> res = new List<String>();
            try
            {
                StreamReader sr = new StreamReader(path);
                
                do
                {
                    line = sr.ReadLine();
                    res.Add(line); 
                } while(line != null);
                
                res.RemoveAt(res.Count - 1);
                sr.Close();
            }
            catch(Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            return res.ToArray();
        }

        public static byte[] ReadByteArray(String path)
        {
            List<byte> res = new List<byte>();
            int ch;
            try
            {
                StreamReader sr = new StreamReader(path);
                do
                {
                    ch = sr.Read();
                    res.Add((byte)ch);
                } while(ch != -1);
                res.RemoveAt(res.Count - 1);
                sr.Close();
            }
            catch(Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            return res.ToArray();
        }

        public static void WriteByteArray(String path, byte[] data)
        {
            try
            {
                File.Delete(path);
                FileStream sb = new FileStream(path, FileMode.OpenOrCreate);
                StreamWriter sw = new StreamWriter(sb);
                for(int i = 0; i < data.Length; sw.Write((char)data[i++]));
                sw.Close();
            }
            catch(Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
        }

        public static void AppendLog(String logpath, String data)
        {
            if (!File.Exists(logpath))
            {
                using (StreamWriter sw = File.CreateText(logpath))
                {
                    sw.WriteLine("LOG\n");
                }	
            }

            using (StreamWriter sw = File.AppendText(logpath))
            {
                sw.WriteLine("[" + DateTime.Now.ToString() + "] \t" + data); 
                sw.Close();
            }	
        }

        public static byte[] StringArrayToByteArray(String[] text)
        {
            List<byte> res = new List<byte>();
            for(int i = 0; i < text.Length; i++)
            {
                byte[] temp = Encoding.ASCII.GetBytes(text[i]);
                for(int j = 0; j < temp.Length; j++)
                {
                    res.Add(temp[j]);
                }
            }
            return res.ToArray();
        }

        public static BitArray ByteArrayToBitArray(byte[] bytes)
        {
            return new BitArray(bytes);
        }

        public static byte[] BitArrayToByteArray(BitArray bits)
        {
            byte[] ret = new byte[(bits.Length - 1) / 8 + 1];
            bits.CopyTo(ret, 0);
            return ret;
        }

        public static int Contains(byte[] data, byte element)
        {
            for(int i = 0; i < data.Length; i++)
            {
                if(data[i] == element) return i;
            }
            return -1;
        }

        // Print and Output methods

        public static void PrintValues(IEnumerable myList, int myWidth)  
        {
            int i = myWidth;
            foreach (Object obj in myList) 
            {
                if (i <= 0)  {
                    i = myWidth;
                    Console.WriteLine();
                }
                i--;
                Console.Write("{0,8}", obj);
            }
            Console.WriteLine();
        }

        public static void PrintBits(BitArray myList, int myWidth)  
        {
            int i = myWidth;
            foreach (Object obj in myList) 
            {
                if (i <= 0)  {
                    i = myWidth;
                    Console.WriteLine();
                }
                i--;
                Console.Write((bool)obj ? '1' : '0');
            }
            Console.WriteLine();
        }
    }
}