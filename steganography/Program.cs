using System;
using System.IO;
using System.Threading;

namespace steganography
{
    class Program
    {
        static string BitesToString(string bites) {
            string t = "";
            string norm = "";
            t += bites[0];
            for (int i = 1; i <= bites.Length; i++) {
                if (i % 8 == 0) {
                    norm += ((char)Convert.ToInt16(t, 2)).ToString();
                    t = "";
                }
                else {
                    t += bites[i];
                }
            }
            return norm;
        }

        static string StringToBites(string inform) {
            int a = 4 - inform.Length % 4;
            for (int i = 0; i < a; i++) 
                inform += " ";
            inform += "    ";
            string temp;
            string inform_bites = "";
            foreach (var item in inform) {
                temp = "";
                for (int j = 0; j < 8 - Convert.ToString(((byte)item), 2).Length; j++)
                    temp += "0";
                inform_bites += temp + Convert.ToString(((byte)item), 2);
            }
            return inform_bites;
        }

        static void SetSteg(string sourcefile, string newfile, string information)
        {
            FileStream sourceFile = new FileStream(sourcefile, FileMode.Open);
            FileStream newFile = new FileStream(newfile, FileMode.Create);
            string bites = StringToBites(information);
            int i = 0;
            int j = 0;
            int t = sourceFile.ReadByte();
            newFile.WriteByte((byte)t);
            Console.WriteLine("Процесс записи начат");
            while (t != -1) {
                ++i;
                t = sourceFile.ReadByte();
                if (i > 54 & j < bites.Length) {
                    if (t % 2 != Convert.ToInt16(bites[j]) % 2) {
                        if (t + 1 <= 255)
                            ++t;
                        else
                            --t;
                    }
                    ++j;
                }
                newFile.WriteByte((byte)t);
            }
            sourceFile.Close();
            newFile.Close();
            Console.WriteLine("Информация записана");
        }

        static string GetSteg(string sourcefile) {
            FileStream sourceFile = new FileStream(sourcefile, FileMode.Open);
            int t = sourceFile.ReadByte();
            int i = 0;
            int j = 0;
            string msg = "";
            string bites = "";
            string temp = "";
            while (t != -1) {
                ++i;
                t = sourceFile.ReadByte();
                if (i > 54) {
                    if (j > 31) {
                        j = 0;
                        temp = BitesToString(bites);
                        if (temp != "    ")
                            msg += temp;
                        else {
                            Console.WriteLine("Информация считана");
                            return msg;
                        }
                        bites = "";
                    }
                    ++j;
                    bites += (t % 2 == 0) ? "0": "1";
                }
            }
            sourceFile.Close();
            Console.WriteLine("Предупреждение: при чтении встречен конец файла, часть информации могла быть потеряна при записи!");
            return msg;
        }

        static void Main(string[] args) {
            string information = "Hello, Universe!";
            string fileName = "Just_a_picture.bmp";
            string tofile = "Its_still_just_a_picture.bmp";
            SetSteg(fileName, tofile, information);
            Console.WriteLine(GetSteg(tofile));
        }
    }
}
