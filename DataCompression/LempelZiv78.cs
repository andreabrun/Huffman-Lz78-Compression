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

    /// <summary>Classe <c>LZCodedNode</c> rappresenta le coppie del tipo (indice, mismatch),
    /// risultato della compressione LZ78. Implementa l'interfaccia Serializable in quanto
    /// le istanze verranno codificare in byte, e decodificate nel processo di decoding. </summary>
    public class LZCodedNode : Serializable
    {
        byte index;
        byte mismatch;

        public LZCodedNode(byte index, byte mismatch)
        {
            this.index = index;
            this.mismatch = mismatch;
        }

        public byte Index
        {
            get => index;
        }

        public byte Mismatch
        {
            get => mismatch;
        }
        
        public byte[] Serialize()
        {
            return new byte[2] {index, mismatch};
        }

        public static LZCodedNode Deserialize(byte[] data)
        {
            return new LZCodedNode(data[0], data[1]);
        }

        public override string ToString()
        {
            return "(" + index + "; " + ((char)mismatch == '\n' ? "\\n" : ((char)(mismatch))) + ")";
        }
    }

    /// <summary>Classe <c>LZDictionary</c> il dizionario dinamico che si costruisce durante la compressione LZ78. 
    /// Implementa l'interfaccia Serializable in quanto verrà codificato in byte e scritto nel file di output,
    ///  e decodificato nel processo di decoding. </summary>
    public class LZDictionary : Serializable
    {
        List<String> data;

        public LZDictionary()
        {
            this.data = new List<String>();
        }

        public LZDictionary(List<String> data)
        {
            this.data = data;
        }

        public bool Contains(String el)
        {
            return data.Contains(el);
        }

        public void Add(String el)
        {
            this.data.Add(el);
        }

        public byte IndexOf(String el)
        {
            return (byte)data.IndexOf(el);
        }

        public String ElementAt(byte el)
        {
            return data.ElementAt((int)el);
        }

        public byte Count
        {
            get => (byte)data.Count;
        }

        public byte[] Serialize()
        {
            List<byte> res = new List<Byte>();
            res.Add(this.Count);
            for(int i = 0; i < this.Count; i++)
            {
                byte j = (byte)data.ElementAt(i).Length;
                res.Add(j);
                for(int k = 0; k < j; k++)
                {
                    res.Add((byte)data.ElementAt(i).ElementAt(k));
                }

            }
            return res.ToArray();
        }

        public static LZDictionary Deserialize(ref byte[] data)
        {
            List<String> res = new List<String>();

            int j = 1;
            for(byte i = 0; i < data[0]; i++)
            {
                List<char> tmp = new List<char>();
                for(int k = 1; k <= data[j]; k++)
                {
                    tmp.Add((char)data[j+k]);
                }
                j = (int)(j + data[j] + 1);
                res.Add(new String(tmp.ToArray()));
            }
            for(int i = 0; i < j-1; data[i++] = 0);
            data[j-1] = 255;

            return new LZDictionary(res);
        }

        public override string ToString()
        {
            String res = "";
            foreach(String item in data)
            {
                res += item + "; "; 
            }
            return res;
        }
    }

    /// <summary>Classe <c>LempelZiv78</c> fornisce i metodi per la codifica e decodifica di un file utilizzando
    /// l'algoritmo LZ78. Può venire istanziata con i dati originali o con i dati compressi.</summary>
    public class LempelZiv78
    {

        byte[] uncompressedData;
        byte[] compressedData;

        LZDictionary dictionary;

        LZCodedNode[] codingnodes;

        bool compressed;

        int maxdictionary;

        public LempelZiv78(byte[] data, int maxdictionary)
        {
            this.uncompressedData = data;
            dictionary = new LZDictionary();
            dictionary.Add("");
            codingnodes = null;
            compressedData = null;
            compressed = false;
            this.maxdictionary = maxdictionary;
        }

        public LempelZiv78(byte[] encoded)
        {
            this.compressedData = encoded;
            this.dictionary = null;
            this.uncompressedData = null;
            compressed = true;
        }

        /// <summary>Metodo Encode crea una lista di LZCodedNode per la codifica del testo.
        /// Dopodiche serializza il dizionario, serializza i LZCodedNodes e il risultato
        /// in compressedData. </summary>
        public void Encode()
        {
            // Creazione della lista di LZCodedNode per la codifica del testo.
            List<LZCodedNode> lzcn = new List<LZCodedNode>();
            String w = "";
            for(int i = 0; i < uncompressedData.Length; i++)
            {
                if(dictionary.Contains(w + (char)uncompressedData[i]))
                {
                    w = w + (char)uncompressedData[i];
                }
                else
                {
                    lzcn.Add(new LZCodedNode((byte)dictionary.IndexOf(w), uncompressedData[i]));
                    if(dictionary.Count < maxdictionary) 
                        dictionary.Add(w + (char)uncompressedData[i]);
                    w = "";
                }
            }

            codingnodes = lzcn.ToArray();

            // Serializzazione Dizionario
            byte[] dict = dictionary.Serialize();

            // Salvataggio in compressedData
            compressedData = new byte[dict.Length + codingnodes.Length * 2];

            for(int g = 0; g < dict.Length; compressedData[g] = dict[g++]);

            for(int g = 0; g < codingnodes.Length; g++)
            {
                byte[] tmp = codingnodes[g].Serialize();
                compressedData[dict.Length + 2*g] = tmp[0];
                compressedData[dict.Length + 2*g + 1] = tmp[1];
            }

            compressed = true;
        }

        /// <summary>Metodo Decode decomprime il vettore di byte 
        /// compressedData e salva il risultato del processo in uncompressedData.</summary>
        public void Decode()
        {
            if(compressed && dictionary == null)
            {
                // Creazione del dizionario
                dictionary = LZDictionary.Deserialize(ref compressedData);

                int enddict;
                for(enddict = 0; compressedData[enddict] != 255; enddict++);

                List<String> res = new List<String>();
                List<LZCodedNode> lzcnlist = new List<LZCodedNode>();

                // Deserializzazione dei nodi
                for(int j = enddict + 1; j < compressedData.Length; j += 2)
                {
                    LZCodedNode lztmp = LZCodedNode.Deserialize(new byte[] {compressedData[j], compressedData[j+1]});
                    lzcnlist.Add(lztmp);
                }

                codingnodes = lzcnlist.ToArray();

                for(int i = 0; i < codingnodes.Length; i++)
                {
                    res.Add(dictionary.ElementAt(codingnodes[i].Index) + (char)codingnodes[i].Mismatch);
                }
                // Salvataggio in uncompressedData
                this.uncompressedData = Utils.StringArrayToByteArray(res.ToArray());
            }
        }

        public byte[] EncodedData
        {
            get => this.compressedData;
        }

        public byte[] Data
        {
            get => this.uncompressedData;
        }
    }
}