using System.Runtime.CompilerServices;


int[] a= new int[] { 20,50,100}; //Munkak szama

for (int k = 0; k < a.Length; k++) //Futtato for ciklus
{
    int munk = a[k];

    int[] sor = new int[munk];
    Random rnd = new Random();
    List<int[]> v = new List<int[]>();
    for (int i = 0; i < munk; i++)  //Kezdeti matrix
    {
        List<int> atmen = new List<int>();
        for (int j = 0; j < 2; j++)
        {
            atmen.Add(rnd.Next(5, 10));
        }
        v.Add(atmen.ToArray());
    }
    int[,] matrix = new int[munk, 2];
    for (int i = 0; i < munk; i++)
    {
        matrix[i, 0] = v[i][0];
        matrix[i, 1] = v[i][1];
        Console.WriteLine($"{matrix[i, 0]} {matrix[i, 1]}  ");
    }


    int couter = munk - 1;
    int counter2 = 0;
    for (int i = 0; i < munk; i++) //Minimum kereses
    {
        int[] kell = min(v);
        if (kell[1] == 0)
        {
            sor[counter2] = kell[0] + 1;
            v[kell[0]][1] = int.MaxValue;
            v[kell[0]][0] = int.MaxValue;
            counter2++;
        }
        else
        {
            sor[couter] = kell[0] + 1;
            v[kell[0]][1] = int.MaxValue;
            v[kell[0]][0] = int.MaxValue;
            couter--;
        }
    }
    foreach (var i in sor)
    {
        Console.Write($" {i},"); // Kiiratas
    }

    int[] jobsline = new int[munk];
    for (int i = 0; i < jobsline.Length; i++)
    {
        jobsline[i] = i;
    }
    jobsline = jobsline.OrderBy(x => rnd.Next()).ToArray(); //Rendezo
    Console.WriteLine();

    tabu(500, 35, jobsline, matrix, sor);
    tabu(500, 20, jobsline, matrix, sor);
}

static int[] min(List<int[]> p) //Kivalogatas
{
    int minimum = int.MaxValue;
    int index = 0;
    int index2 = 0;
    for (int i = 0; i < p.Count; i++)
    {
        for (int j = 0; j < p[i].Length; j++)
        {
            if (p[i][j] < minimum)
            {

                minimum = p[i][j];
                index = i;
                index2 = j;
            }
        }
    }
    return new int[] { index, index2 };
}
static int[,] sorcsere(int[,] original, int[] lines) //Elemek rendszerezese az eredeti matrixbol
{

    int[,] ret = new int[original.GetLength(0), original.GetLength(1)];

    for (int i = 0; i < original.GetLength(0); i++)
    {
        for (int j = 0; j < original.GetLength(1); j++)
        {
            if (lines.Max() >= original.GetLength(0))
            {
                if (lines[i] == 0) ret[i, j] = original[0, j];
                else ret[i, j] = original[lines[i] - 1, j];
            }
            else ret[i, j] = original[lines[i], j];
        }
    }
    return ret;
}
static int eredmeny(int[,] machandjob, int[] jobline) //Eredmeny meghatarozasa tabukereseshez
{
    machandjob = sorcsere(machandjob, jobline);
    int[,] end = new int[machandjob.GetLength(0), machandjob.GetLength(1)];
    int[,] wait = new int[machandjob.GetLength(0), machandjob.GetLength(1)];

    for (int i = 0; i < machandjob.GetLength(1); i++)
    {
        for (int j = 0; j < machandjob.GetLength(0); j++)
        {
            if (i == 0)
            {
                int osz = 0;
                for (int k = j; k >= 0; k--)
                {
                    osz += machandjob[k, i];
                }
                end[j, i] = osz;
            }
            else
            {
                int osz = end[0, i - 1];
                int osz2 = 0;
                int waiteLocal = 0;
                int a = 0;
                for (int k = 0; k <= j; k++)
                {
                    osz += machandjob[k, i];
                    if (k != machandjob.GetLength(0) - 1 && j != 0)
                    {
                        a = end[k + 1, i - 1];
                        osz2 = osz;
                        waiteLocal = end[k + 1, i - 1] - osz;
                        if (end[k + 1, i - 1] - wait[k + 1, i - 1] > osz)
                        {
                            osz = end[k + 1, i - 1];
                        }
                    }
                }
                if (osz == a) wait[j, i] = a - osz2;
                end[j, i] = osz;
            }
        }
    }
    return end[end.GetLength(0) - 1, end.GetLength(1) - 1];
}
static List<int[]> szomszedok(int[] corline) //Szomszedekok keresese
{
    List<int[]> szom = new List<int[]>();
    for (int i = 0; i < corline.Length; i++)
    {
        int a;
        int[] bnsIUDBFHIASJN = corline;
        for (int j = i + 1; j < corline.Length; j++)
        {
            a = corline[i];
            corline[i] = corline[j];
            corline[j] = a;
            szom.Add(corline.ToArray());
            a = corline[i];
            corline[i] = corline[j];
            corline[j] = a;
        }
    }
    return szom;

}

static bool keres2(int[] alap, List<int[]> tabuk) //Kereses a tabulistaban
{
    return tabuk.Contains(alap);
}
static void tabu(int iter, int tabumax, int[] joblines, int[,] machandjob, int[] sor)  //Tabukereses
{

    int[] mostanisor = joblines;
    List<int[]> tabu = new List<int[]>();
    List<int[]> szom;

    int max = int.MaxValue;

    for (int i = 0; i < iter; i++)
    {
        szom = szomszedok(mostanisor);

        for (int j = 0; j < szom.Count; j++)
        {
            if (keres2(szom[j], tabu))
            {

                szom.Remove(szom[j]);
            }
        }
        max = eredmeny(machandjob, mostanisor);

        foreach (var j in szom)
        {
            int a = eredmeny(machandjob, j);
            if (a < max)
            {
                max = a;
                mostanisor = j.ToArray();
            }
        }
        tabu.Add(mostanisor.ToArray());
        if (tabu.Count > tabumax)
        {
            for (int j = 0; j < tabumax; j++)
            {
                tabu.Remove(tabu[0]);
            }
        }
        if (max == eredmeny(machandjob, sor))
        {
            Console.WriteLine($"Az iterációk amibe került: {i}"); ;
            break;
        }
    }
}
