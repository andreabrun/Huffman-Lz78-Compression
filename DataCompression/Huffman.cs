using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;

namespace DataCompression
{

    /// <summary>Classe <c>HuffmanNode</c> modella un nodo dell'albero di Huffman 
    /// etichettato con (carattere, numero di occorrenze).</summary>
    public class HuffmanNode
    {
        int freq;

        byte data;

        public int Frequence
        {
            get => freq;
            set => freq = value;
        }

        public byte Data
        {
            get => data;
        }

        public HuffmanNode()
        {
            freq = 0;
            data = 0;
        }

        public HuffmanNode(byte data)
        {
            this.freq = 0;
            this.data = data;
        }

        public HuffmanNode(byte data, int freq)
        {
            this.freq = freq;
            this.data = data;
        }

        public static bool operator == (HuffmanNode A, HuffmanNode B)
            => A.data == B.data; 

        public static bool operator != (HuffmanNode A, HuffmanNode B)
            => A.data != B.data;


        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator > (HuffmanNode A, HuffmanNode B)
            => A.freq > B.freq; 

        public static bool operator < (HuffmanNode A, HuffmanNode B)
            => A.freq < B.freq; 
        
        public static bool operator >= (HuffmanNode A, HuffmanNode B)
            => A.freq >= B.freq; 
        
        public static bool operator <= (HuffmanNode A, HuffmanNode B)
            => A.freq <= B.freq; 


        public static void Swap(ref HuffmanNode A, ref HuffmanNode B)
        {
            HuffmanNode tmp = new HuffmanNode();
            tmp = A;
            A = B;
            B = tmp;
        }

        public override string ToString()
        {
            return "(" + ((char)data == '\n' ? "\\n" : ((char)data)) + "," + Frequence + ")";
        }

        public static int Occurred(HuffmanNode[] data, HuffmanNode el)
        {
            for(int i = 0; i < data.Length; i++)
            {
                if(data[i] == el) return i;
            }
            return -1;
        }
    }

    /// <summary>Classe <c>HuffmanCodedNode</c> modella un nodo dell'albero di Huffman 
    /// etichettato con (carattere, codice).
    /// Questi oggetti saranno Serializzati per salvarli nel file compresso. </summary>
    class HuffmanCodedNode : Serializable
    {
        private byte data;
        private String code;

        public HuffmanCodedNode(byte data, String code)
        {
            this.data = data;
            this.code = code;
        }

        public byte Data
        {
            get => data;
        }

        public String Code
        {
            get => code;
        }

        public BitArray CodeBits
        {
            get => CodeToBitArray();
        }

        public byte[] Serialize()
        {
            List<byte> res = new List<byte>();
            res.Add((byte)code.Length);
            res.Add(data);
            BitArray tmp = CodeToBitArray();
            byte[] conversion = Utils.BitArrayToByteArray(tmp);
            for(int i = 0; i < conversion.Length; i++) res.Add(conversion[i]);
            return res.ToArray();
        }

        private BitArray CodeToBitArray()
        {
            bool[] res = new bool[code.Length];
            for(int i = 0; i < code.Length; i++)
            {
                res[i] = (code.ElementAt(i) == '1');
            }
            return new BitArray(res);
        }

        public override String ToString()
        {
            return " (" + ((char)data == '\n' ? "\\n" : ((char)data)) + "," + code + ")";
        }

        public static HuffmanCodedNode Deserialize(byte[] ser)
        {
            List<byte> tmp = new List<byte>();
            for(int k = 0; k < ((ser[0] - 1) / 8) + 1; tmp.Add(ser[2 + k++]));
            BitArray restmp = Utils.ByteArrayToBitArray(tmp.ToArray());
            List<char> res = new List<char>();
            for(int i = 0; i < ser[0]; res.Add((bool)restmp[i++] ? '1' : '0'));
            return new HuffmanCodedNode(ser[1], new String(res.ToArray()));
        }

        private static bool EqualBitArrays(BitArray A, BitArray B)
        {
            if(A.Count != B.Count) return false;
            for(int i = 0; i < A.Count; i++)
            {
                if(A[i] != B[i]) return false;
            }
            return true;
        }

        /// <summary>Funzione CreateTreeFromLeafs crea l'albero
        /// partendo dal vettore delle foglie. Il Codice del carattere in
        /// questo caso di usa per stabilire dove il nodo si trova nell'albero </summary>
        public static Tree CreateTreeFromLeafs(HuffmanCodedNode[] data)
        {
            Node root = new Node(new HuffmanCodedNode(0, ""));
            Tree T = new Tree(root);
            for(int i = 0; i < data.Length; i++)
            {
                bool end = false;
                String path = "";
                while(!end)
                {
                    if(root.IsLeaf() && path != data[i].Code)
                    {
                        if(data[i].Code.ElementAt(path.Length) == '0')
                        {
                            path += "0";
                            root.AddChild(new Node(new HuffmanCodedNode(0, path)));
                            root = root.Children.ElementAt(0);
                        }
                        else if(data[i].Code.ElementAt(path.Length) == '1')
                        {
                            root.AddChild(new Node(new HuffmanCodedNode(0, path + "0")));
                            path += "1";
                            root.AddChild(new Node(new HuffmanCodedNode(0, path)));
                            root = root.Children.ElementAt(1);
                        }
                    }
                    else if(!root.IsLeaf() && path != data[i].Code)
                    {
                        List<Node> children = root.Children;
                        if(data[i].Code.ElementAt(path.Length) == '0')
                        {
                            path += "0";
                            root = children.ElementAt(0);
                        }
                        else if(data[i].Code.ElementAt(path.Length) == '1')
                        {
                            path += "1";
                            if(root.Children.Count < 2) root.AddChild(new Node(new HuffmanCodedNode(0, path)));
                            root = children.ElementAt(1);
                        }
                    }
                    else 
                    { 
                        root.Data = data[i];
                        end = true;
                        path = "";
                        root = T.Root;
                    }
                }
            }
            return T;
        }
    }

    /// <summary>Classe <c>Huffman</c> fornisce i metodi per la codifica e decodifica di un file utilizzando
    /// l'algoritmo di Huffman. Può venire istanziata con i dati originali o con i dati compressi, in relazione al
    /// flag compressed, passato come argomento al costruttore. </summary>
    public class Huffman
    {
        byte[] uncompressedData;
        byte[] compressedData;

        bool compressed;

        HuffmanNode[] huffmanNodes;

        HuffmanCodedNode[] huffmancodednodes;

        public Huffman(byte[] data, bool compressed)
        {
            this.compressed = compressed;
            if(!compressed)
            {
                this.uncompressedData = data;
                this.compressedData =  null;
                this.CreateHuffmanNodes();
                huffmancodednodes = null;
            }
            else
            {
                this.compressedData = data;
                this.uncompressedData = null;
                this.huffmanNodes = null;
                this.huffmancodednodes = null;
            }
        }

        public byte[] EncodedData
        {
            get => compressedData;
        }

        public byte[] Data
        {
            get => uncompressedData;
        }

        /// <summary>Metodo CreateHuffmanNodes crea i nodi di tipo HuffmanNode relativi ad uncompressedData. </summary>
        private void CreateHuffmanNodes()
        {
            List<HuffmanNode> tmp = new List<HuffmanNode>();

            for(int i = 0; i < uncompressedData.Length; i++)
            {
                int index = HuffmanNode.Occurred(tmp.ToArray(), new HuffmanNode(uncompressedData[i]));
                if(index >= 0)
                {
                    tmp.ElementAt(index).Frequence += 1;
                }
                else
                {
                    tmp.Add(new HuffmanNode(uncompressedData[i], 1));
                }
            }
            huffmanNodes = tmp.ToArray();
        }

        private int IndexOf(byte el)
        {
            for(int i = 0; i < huffmancodednodes.Length; i++)
            {
                if(huffmancodednodes[i].Data == el) return i;
            }
            return -1;
        }

        /// <summary>Metodo EncodeContent codifica il contenuto del file in uncompressedData utilizzando
        /// la i nodi codificati in huffmancodednodes. </summary>
        private BitArray EncodeContent()
        {
            List<bool> res = new List<bool>();
            for(int i = 0; i < uncompressedData.Length; i++)
            {
                int index = IndexOf(uncompressedData[i]);
                BitArray tmp = huffmancodednodes[index].CodeBits;
                for(int j = 0; j < tmp.Length; j++)
                {
                    res.Add(tmp[j]);
                }
            }
            return new BitArray(res.ToArray());
        }

        /// <summary>Metodo Encode codifica il contenuto del file in uncompressedData. 
        /// Ritorna due stringhe, corrispondenti al codice matlab per la stampa degli alberi. </summary>
        public String[] Encode()
        {
            String[] matlabres = new String[2];
            if(!compressed)
            {
                // Crea l'albero con i Nodi HuffmanNode
                Tree huffmanTree = Huffman.HuffmanTree(huffmanNodes);
                matlabres[0] = huffmanTree.ToMatlab();

                // Crea l'albero con i Nodi HuffmanCodedNode
                Tree encodingtree = Huffman.EncodingTree(huffmanTree);
                matlabres[1] = encodingtree.ToMatlab();

                // Salva in huffmancodednodes le foglie dell'albero corrispondenti
                Node[] encodingleafs = encodingtree.GetLeafs();
                huffmancodednodes = new HuffmanCodedNode[encodingleafs.Length];

                List<byte> res = new List<byte>();

                res.Add((byte)huffmancodednodes.Length);

                // Serializza gli huffmancodednodes
                for(int i = 0; i < encodingleafs.Length; i++)
                {
                    huffmancodednodes[i] = (HuffmanCodedNode)encodingleafs[i].Data;
                    byte[] tmpdict = huffmancodednodes[i].Serialize();
                    for(int h = 0; h < tmpdict.Length; h++) res.Add(tmpdict[h]);
                }
                
                // Salva serializza gli Codifica il contenuto
                BitArray prova = this.EncodeContent();

                byte[] content = Utils.BitArrayToByteArray(prova);

                for(int i = 0; i < content.Length; i++) res.Add(content[i]);

                // Salva il risultato della compressione in compressedData

                compressedData = res.ToArray();

                compressed = true;
            }
            return matlabres;
        }

        /// <summary>Metodo Decode decodifica il contenuto del file in compressedData. </summary>
        public String Decode()
        {
            String stringres = "";
            if(compressed && uncompressedData == null)
            {
                // Legge i primi byte e deserializza gli huffmancodednodes all'inizio del file.
                List<HuffmanCodedNode> tmpnodes = new List<HuffmanCodedNode>();
                byte dictlength = compressedData[0];
                int j = 1;
                for(byte i = 0; i < dictlength; i++)
                {
                    List<byte> tmp = new List<byte>();
                    tmp.Add(compressedData[j]);
                    tmp.Add(compressedData[j+1]);
                    int k = 0;
                    for(; k < (compressedData[j] - 1) / 8 + 1; tmp.Add(compressedData[j + 2 + k++]));

                    j = j + 3 + (compressedData[j] - 1) / 8;

                    HuffmanCodedNode hcn = HuffmanCodedNode.Deserialize(tmp.ToArray());
                    tmpnodes.Add(hcn);
                }

                this.huffmancodednodes = tmpnodes.ToArray();

                // Legge il contenuto del file (dopo le codifiche delle foglie) e lo salva in content.
                byte[] content = new byte[compressedData.Length - j];
                for(int i = 0; i < compressedData.Length - j; content[i] = compressedData[j + i++]);

                BitArray bits = Utils.ByteArrayToBitArray(content);

                // Crea l'albero partendo dagli huffmancodednodes letti dal file
                Tree T = HuffmanCodedNode.CreateTreeFromLeafs(huffmancodednodes);
                stringres = T.ToMatlab();
                
                // Decomprime il contenuto del file effettuando ricerche sull'albero
                List<byte> res = new List<byte>();
                Node node = T.Root;

                for(int i = 0; i < bits.Length; i++)
                {
                    if(!bits[i])
                    {
                        node = node.Children.ElementAt(0);
                    }
                    else
                    {
                        node = node.Children.ElementAt(1);
                    }

                    if(node.IsLeaf())
                    {
                        res.Add(((HuffmanCodedNode)node.Data).Data);
                        node = T.Root;
                    }
                }

                uncompressedData = res.ToArray();
            }
            return stringres;
        }

        // STATIC METHODS
        public static String GetInfo()
        {
            return "Huffman Encoding";
        }

        private static HuffmanNode[] SimpleSort(HuffmanNode[] nodes)
        {
            int n = nodes.Length;

            HuffmanNode[] res = new HuffmanNode[n];
            for(int i = 0; i < n; res[i] = nodes[i++]);

            for(int i = 0; i < n-1; i++)
            {
                for(int j = 1; j < n; j++)
                {
                    if(res[j-1].Frequence < res[j].Frequence)
                    {
                        HuffmanNode.Swap(ref res[j-1],ref res[j]);
                    }
                }
            }
            return res;
        }

        private static Tree HuffmanTree(HuffmanNode[] nodes)
        {
            HuffmanNode[] huffnodes = Huffman.SimpleSort(nodes);
            List<Node> stack = new List<Node>();
            for(int i = 0; i < huffnodes.Length; i++)
            {
                stack.Add(new Node(huffnodes[i]));
            }

            while(stack.Count > 1)
            {
                Node t1 = stack.ElementAt(stack.Count - 1);
                Node t2 = stack.ElementAt(stack.Count - 2);
                stack.RemoveRange(stack.Count - 2, 2);
                Node t = new Node(new HuffmanNode(0, ((HuffmanNode)(t1.Data)).Frequence + ((HuffmanNode)(t2.Data)).Frequence));
                t.AddChild(t1);
                t.AddChild(t2);

                int i = 0;
                while(i < stack.Count && ((HuffmanNode)(stack.ElementAt(stack.Count - i - 1).Data)).Frequence < ((HuffmanNode)(t.Data)).Frequence)
                {
                    i++;
                }

                stack.Insert(stack.Count - i,t);
            }

            
            return new Tree(stack.ElementAt(0));
        }

        private static Tree EncodingTree(Tree huffmanTree)
        {
            Tree res = new Tree(huffmanTree.Root);
            List<Node> nodes = new List<Node>();
            res.Root.Data = new HuffmanCodedNode(0, "");
            nodes.Add(res.Root);
            while(nodes.Count > 0)
            {
                int i = 0;
                foreach(Node item in nodes.ElementAt(0).Children)
                {
                    item.Data = new HuffmanCodedNode(((HuffmanNode)item.Data).Data, ((HuffmanCodedNode)(nodes.ElementAt(0).Data)).Code + i);
                    i++;
                    if(!item.IsLeaf())
                        nodes.Add(item);
                }
                nodes.RemoveAt(0);
            }
            return res;
        }
    }
}