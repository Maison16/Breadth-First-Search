using System;
using System.Collections.Generic;
using System.IO;


class Program
{
    static void Main()
    {
        // StreamReader sr = new StreamReader("IzdebskiDuzyPlik2.txt");
        StreamReader sr = new StreamReader("Izdebski2.txt");
        string bufor = sr.ReadLine();
        string[] dane = bufor.Split(' ');
        int liczbaMiast = int.Parse(dane[0]);
        int liczbaPolaczen = int.Parse(dane[1]);
        int start = int.Parse(dane[2]);
        int kosztBiletuKolejowego = int.Parse(dane[3]);
        int kosztBiletuAutobusowego = int.Parse(dane[4]);
        List<int>[] trasy = new List<int>[liczbaMiast + 1];
        for (int i = 0; i <= liczbaMiast; i++)
        {
            trasy[i] = new List<int>();
        }
        while ((bufor = sr.ReadLine()) != null)
        {
            dane = bufor.Split(' ');
            trasy[int.Parse(dane[0])].Add(int.Parse(dane[1])); //wszystkie ścieżki polaczenie grafie są obustronne
            trasy[int.Parse(dane[1])].Add(int.Parse(dane[0])); //wszystkie ścieżki polaczenie grafie są obustronne
        }
        sr.Close();
        Console.ForegroundColor = ConsoleColor.Blue;
        for (int i = 1; i <= liczbaMiast; i++)
        {
            Console.Write($"Połączenia z miasta {i}: ");
            foreach (int trasa in trasy[i])
            {
                Console.Write($"{trasa} ");
            }
            Console.WriteLine("");
        }
        BFS odpowiedz = new BFS();
        odpowiedz.Znajdz(liczbaMiast, start, kosztBiletuKolejowego, kosztBiletuAutobusowego, trasy);
        odpowiedz.PokazOdpowiedz();
        Console.ReadKey();
    }
}

class BFS
{
    static int licznik = 0;
    int[] koszt, poprzednicy;
    bool[] odwiedzone;
    bool[] odwiedzoneAutobusowo;
    Queue<int> fifo;

    public void Znajdz(int liczbaMiast, int start, int kosztBiletuKolejowego, int kosztBiletuAutobusowego, List<int>[] trasy)
    {
        koszt = new int[liczbaMiast + 1]; //tabela kosztów
        poprzednicy = new int[liczbaMiast + 1]; //tabela poprzedników
        odwiedzone = new bool[liczbaMiast + 1]; //tabela odwiedzone. Używam, aby nie wracać do już odwiedzonych wierzchołków
        odwiedzoneAutobusowo = new bool[liczbaMiast + 1]; //tabela odwiedzonych autobusowo. Możemy jechać autobusem do miasta w którym NIE byliśmy
        fifo = new Queue<int>();
        for (int i = 1; i <= liczbaMiast; i++)
        {
            odwiedzone[i] = false;
            koszt[i] = int.MaxValue;
        }
        odwiedzone[start] = true; //punkt początkowy oznaczamy jako odzwiedzony
        koszt[start] = 0; // długość drogi od punktu do tego samego punktu to 0 
        fifo.Enqueue(start); //wrzucamy punkt startowy na koniec kolejki
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"miasto: {start} poprzednik:{poprzednicy[start]} koszt:{koszt[start]}");
        while (fifo.Count != 0)
        {
            int v = fifo.Dequeue(); //zdjęty element z kolejki
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"Zdjęty z kolejki element: {v}");
            foreach (int polaczenie in trasy[v])
            {
                licznik++;
                if (!odwiedzone[polaczenie])
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    fifo.Enqueue(polaczenie);
                    odwiedzone[polaczenie] = true;
                    koszt[polaczenie] = koszt[v] + kosztBiletuKolejowego;
                    poprzednicy[polaczenie] = v;
                    Console.WriteLine($"miasto: {polaczenie} poprzednik:{poprzednicy[polaczenie]} koszt:{koszt[polaczenie]}");
                }
            }
        }
        Console.WriteLine("**************************************");
        Console.WriteLine($"Złożoność: {licznik} V+K");
        Console.ForegroundColor = ConsoleColor.Cyan;
        for (int i = 1; i <= liczbaMiast; i++)
        {
            licznik++;
            if (!odwiedzoneAutobusowo[poprzednicy[i]]) //jeżeli poprzednik NIE został odwiedzony autobusowo
            {
                int poprzednikPoprzednika = poprzednicy[poprzednicy[i]];
                if (poprzednikPoprzednika != 0) //jeżeli poprzednikPoprzednika nie jest punktem startowym
                {
                    if (koszt[i] > koszt[poprzednikPoprzednika] + kosztBiletuAutobusowego) //jeżeli poprzednik poprzednika z dodanym biletem autobusowym jest bardziej optymalny
                    {
                        koszt[i] = koszt[poprzednikPoprzednika] + kosztBiletuAutobusowego;
                        poprzednicy[i] = poprzednikPoprzednika;
                        odwiedzoneAutobusowo[i] = true;
                        Console.WriteLine($"miasto: {i} poprzednik:{poprzednicy[i]} koszt:{koszt[i]}\t czy odwiedzone Autobusem: {odwiedzoneAutobusowo[i]}");
                    }
                }
            }
            else
            {
                if (koszt[i] > koszt[poprzednicy[i]] + kosztBiletuKolejowego)
                {
                    koszt[i] = koszt[poprzednicy[i]] + kosztBiletuKolejowego;
                    Console.WriteLine($"miasto: {i} poprzednik:{poprzednicy[i]} koszt:{koszt[i]}\t czy odwiedzone Autobusem: {odwiedzoneAutobusowo[i]}");
                }
            }
        }
        Console.WriteLine($"Złożoność: {licznik} V+K+V (to całość)");
    }


    public void PokazOdpowiedz()
    {
        Console.ForegroundColor = ConsoleColor.Red;
        int n = koszt.Length;
        for (int i = 1; i < n; i++)
        {
            Console.Write("miasto: ");
            Console.Write($"*{i}* \t");
            Console.Write("koszt: ");
            Console.Write($"{koszt[i]}\t");
            Console.Write("poprzednik: ");
            Console.Write($"{poprzednicy[i]}");
            Console.WriteLine();
        }
        Console.ResetColor();
    }
}