using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace DataCompression
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("\n\tCOMPRESSIONE LZ78 E Huffman\n\n");

            Console.WriteLine("Nota: Nella cartella files si trova un file log.txt. Al suo interno si trovano \n" + 
                            "i codici matlab per la stampa degli alberi risultato della compressione di Huffman. \n\n");

            Console.WriteLine("Nella cartella \"./files\" si trovano i seguenti file:");
            String[] files = Directory.GetFiles("./files");
            int i = 1;
            foreach(string item in files)
            {
                Console.WriteLine(i++ + " - " + item.Substring(8));
            }
            Console.Write("\nInserisci il numero del file da comprimere, 0 per saltare questo passaggio: ");
            String imm01 = "";
            int ans01 = -1;
            while(ans01 == -1)
            {
                imm01 = Console.ReadLine();
                if (Int32.TryParse(imm01, out ans01))
                {
                    if(ans01 < 0 || ans01 > i)
                    {
                        Console.WriteLine("Immissione non corretta");
                        Console.Write("\nInserisci il numero del file da comprimere, 0 per saltare questo passaggio: ");
                        ans01 = -1;
                    }
                }
                else
                {
                    Console.WriteLine("Immissione non corretta");
                    Console.Write("\nInserisci il numero del file da comprimere, 0 per saltare questo passaggio: ");
                    ans01 = -1;
                }
            }

            if(ans01 > 0)
            {
                Console.WriteLine("\nApertura file " + files[ans01 - 1].Substring(8) + "...\n");

                byte[] openedfile = Utils.ReadByteArray(files[ans01 - 1]);

                Console.WriteLine("Dimensione file: " + openedfile.Length + " byte(s).");

                double entropia = CODEC.ShannonEntropy(openedfile);

                Console.WriteLine("Entropia: " + entropia);

                Console.WriteLine("\nInserire il nome dei file in cui verranno salvati i risultati della compressione (rispettivamente \"nomefile\".hme per Huffman e \"nomefile\".lze per LZ78)");
                String nome = "./files/" + Console.ReadLine();

                Console.WriteLine("\nCompressione di Huffman");

                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                
                CODEC.HuffmanEncoding(openedfile, nome + ".hme");

                stopWatch.Stop();
                Console.WriteLine("\nFile compresso salvato in " + nome + ".hme");
                Console.WriteLine("Tempo impiegato per la compressione: " + stopWatch.ElapsedMilliseconds + "ms");
                byte[] tmp = Utils.ReadByteArray(nome + ".hme");
                Console.WriteLine("Dimensione finale: " + tmp.Length + " byte(s).\n");
                tmp = null;


                Console.WriteLine("\nCompressione Lempel-Ziv 78");
                Console.Write("\nInserisci la dimensione massima del dizionario (1-250): ");
                imm01 = "";
                int ans02 = -1;
                while(ans02 == -1)
                {
                    imm01 = Console.ReadLine();
                    if (Int32.TryParse(imm01, out ans02))
                    {
                        if(ans02 < 1 || ans02 > 250)
                        {
                            Console.WriteLine("Immissione non corretta");
                            Console.Write("\nInserisci la dimensione massima del dizionario (1-250): ");
                            ans02 = -1;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Immissione non corretta");
                        Console.Write("\nInserisci la dimensione massima del dizionario (1-250): ");
                        ans02 = -1;
                    }
                }

                stopWatch = new Stopwatch();
                stopWatch.Start();
                
                CODEC.LempelZiv78Encoding(openedfile, ans02, nome + ".lze");
                
                stopWatch.Stop();
                Console.WriteLine("\nFile compresso salvato in " + nome + ".lze");
                Console.WriteLine("Tempo impiegato per la compressione: " + stopWatch.ElapsedMilliseconds + "ms");
                tmp = Utils.ReadByteArray(nome + ".lze");
                Console.WriteLine("Dimensione finale: " + tmp.Length + " byte(s).\n");
                tmp = null;
            }

            Console.WriteLine("\nNella cartella \"./files\" si trovano i seguenti file:");
            files = Directory.GetFiles("./files");
            i = 1;
            foreach(string item in files)
            {
                Console.WriteLine(i++ + " - " + item.Substring(8));
            }
            Console.Write("\nInserisci il numero del file da decomprimere con Huffman, 0 per saltare questo passaggio: ");
            imm01 = "";
            ans01 = -1;
            while(ans01 == -1)
            {
                imm01 = Console.ReadLine();
                if (Int32.TryParse(imm01, out ans01))
                {
                    if(ans01 < 0 || ans01 > i)
                    {
                        Console.WriteLine("Immissione non corretta");
                        Console.Write("\nInserisci il numero del file da decomprimere con Huffman, 0 per saltare questo passaggio: ");
                        ans01 = -1;
                    }
                }
                else
                {
                    Console.WriteLine("Immissione non corretta");
                    Console.Write("\nInserisci il numero del file da decomprimere con Huffman, 0 per saltare questo passaggio: ");
                    ans01 = -1;
                }
            }
            if(ans01 > 0)
            {
                Console.WriteLine("\nApertura file " + files[ans01 - 1].Substring(8) + "...\n");

                byte[] hufffile = Utils.ReadByteArray(files[ans01 - 1]);

                Console.WriteLine("Dimensione file: " + hufffile.Length + " byte(s).");

                Console.WriteLine("\nInserire il nome dei file in cui verranno salvati i risultati della decompressione (con estensione)");
                String nome = "./files/" + Console.ReadLine();

                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                
                CODEC.HuffmanDecoding(hufffile, nome);

                stopWatch.Stop();
                Console.WriteLine("\nFile salvato in " + nome);
                Console.WriteLine("Tempo impiegato per la decompressione: " + stopWatch.ElapsedMilliseconds + "ms");
                byte[] tmp = Utils.ReadByteArray(nome);
                Console.WriteLine("Dimensione finale: " + tmp.Length + " byte(s).\n");
                tmp = null;
            }
            
            Console.WriteLine("\nNella cartella \"./files\" si trovano i seguenti file:");
            files = Directory.GetFiles("./files");
            i = 1;
            foreach(string item in files)
            {
                Console.WriteLine(i++ + " - " + item.Substring(8));
            }
            Console.Write("\nInserisci il numero del file da decomprimere con LZ78, 0 per saltare questo passaggio: ");
            imm01 = "";
            ans01 = -1;
            while(ans01 == -1)
            {
                imm01 = Console.ReadLine();
                if (Int32.TryParse(imm01, out ans01))
                {
                    if(ans01 < 0 || ans01 > i)
                    {
                        Console.WriteLine("Immissione non corretta");
                        Console.Write("\nInserisci il numero del file da decomprimere con LZ78, 0 per saltare questo passaggio: ");
                        ans01 = -1;
                    }
                }
                else
                {
                    Console.WriteLine("Immissione non corretta");
                    Console.Write("\nInserisci il numero del file da decomprimere con LZ78, 0 per saltare questo passaggio: ");
                    ans01 = -1;
                }
            }
            if(ans01 > 0)
            {
                Console.WriteLine("\nApertura file " + files[ans01 - 1].Substring(8) + "...\n");

                byte[] lzfile = Utils.ReadByteArray(files[ans01 - 1]);

                Console.WriteLine("Dimensione file: " + lzfile.Length + " byte(s).");

                Console.WriteLine("\nInserire il nome dei file in cui verranno salvati i risultati della decompressione (con estensione)");
                String nome = "./files/" + Console.ReadLine();

                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                
                CODEC.LempelZiv78Decoding(lzfile, nome);

                stopWatch.Stop();
                Console.WriteLine("\nFile salvato in " + nome);
                Console.WriteLine("Tempo impiegato per la decompressione: " + stopWatch.ElapsedMilliseconds + "ms");
                byte[] tmp = Utils.ReadByteArray(nome);
                Console.WriteLine("Dimensione finale: " + tmp.Length + " byte(s).\n");
                tmp = null;
            }
        }
        
    }
}
