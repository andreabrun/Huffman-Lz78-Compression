using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;

namespace DataCompression
{
    /// <summary>Classe statica <c>CODEC</c> fornisce i metodi per la codifica e decodifica di un file. Si 
    /// interfaccia alle classi Huffman e LempelZiv78. </summary>
    static class CODEC
    {
        private static HuffmanNode[] CreateHuffmanNodes(byte[] data)
        {
            List<HuffmanNode> tmp = new List<HuffmanNode>();

            for(int i = 0; i < data.Length; i++)
            {
                int index = HuffmanNode.Occurred(tmp.ToArray(), new HuffmanNode(data[i]));
                if(index >= 0)
                {
                    tmp.ElementAt(index).Frequence += 1;
                }
                else
                {
                    tmp.Add(new HuffmanNode(data[i], 1));
                }
            }
            return tmp.ToArray();
        }

        public static double ShannonEntropy(byte[] data)
        {
            double res = 0;
            HuffmanNode[] huffmanNodes = CreateHuffmanNodes(data);
            for(int i = 0; i < huffmanNodes.Length; i++)
            {
                double pi = ((double)huffmanNodes[i].Frequence) / ((double)data.Length);
                res -= pi * Math.Log2(pi);
            }
            return res;
        }

        public static void LempelZiv78Encoding(byte[] data, int maxDictionaryLength, String path)
        {
            LempelZiv78 l = new LempelZiv78(data, maxDictionaryLength);
            l.Encode();
            byte[] comp = l.EncodedData;
            Utils.WriteByteArray(path, comp);
        }

        public static void LempelZiv78Decoding(byte[] data, String path)
        {
            LempelZiv78 l = new LempelZiv78(data);
            l.Decode();
            byte[] uncomp = l.Data;
            Utils.WriteByteArray(path, uncomp);
        }

        public static void HuffmanEncoding(byte[] data, String path)
        {
            Huffman h = new Huffman(data, false);
            String[] res = h.Encode();
            byte[] comp = h.EncodedData;
            Utils.WriteByteArray(path, comp);
            foreach(String item in res)
            {
                Utils.AppendLog("./files/log.txt", item);
            }
        }

        public static void HuffmanDecoding(byte[] data, String path)
        {
            Huffman h = new Huffman(data, true);
            String res = h.Decode();
            byte[] comp = h.Data;
            Utils.WriteByteArray(path, comp);
            Utils.AppendLog("./files/log.txt", res);
        }

        public static String GetInfo()
        {
            return "Compression Tools";
        }
    }
}