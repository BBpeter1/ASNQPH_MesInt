
int[] machine = new int[] { 5, 10, 20 };
int[] job = new int[] { 10, 20, 50 };
for (int i = 0; i < machine.Length; i++)
{
    for (int j = 0; j < job.Length; j++)
    {
        futat(machine[i], job[j]);
    }
}


static void futat(int machines, int jobs)
{
    Random rnd = new Random();
    int[,] machandjob = new int[jobs, machines];
    for (int i = 0; i < machandjob.GetLength(0); i++)
    {
        for (int j = 0; j < machandjob.GetLength(1); j++)
        {
            machandjob[i, j] = rnd.Next(1, 10);
        }
    }
    int[] d = new int[machandjob.GetLength(0)];
    int[] jobsline = new int[machandjob.GetLength(0)];
    for (int i = 0; i < jobsline.Length; i++)
    {
        d[i] = rnd.Next(10, 30);
        jobsline[i] = i;
    }
    Console.WriteLine("Kezdeti táblázat");
    for (int i = 0; i < machandjob.GetLength(0); i++)
    {
        for (int j = 0; j < machandjob.GetLength(1); j++)
        {
            Console.Write($"{machandjob[i, j]},");
        }
        Console.WriteLine();
    }

    Console.WriteLine("A határidők:");
    foreach (var i in d)
    {
        Console.Write($"{i},");
    }
    Console.WriteLine();

    jobsline = jobsline.OrderBy(x => rnd.Next()).ToArray(); 
    int a = machandjob.GetLength(0) * machandjob.GetLength(1) * 100;
    tabu(a, jobsline, machandjob, d);

}



static List<int[]> szomszedok(int[] corline) 
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




static int[,] sorcsere(int[,] original, int[] lines) 
{

    int[,] ret = new int[original.GetLength(0), original.GetLength(1)];
    for (int i = 0; i < original.GetLength(0); i++)
    {
        for (int j = 0; j < original.GetLength(1); j++)
        {
            ret[i, j] = original[lines[i], j];
        }

    }
    return ret;
}
static int eredmeny(int[,] machandjob, int[] d, int[] jobline)

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
    for (int i = 0; i < machandjob.GetLength(0); i++)
    {
        end[i, end.GetLength(1) - 1] -= wait[i, wait.GetLength(1) - 1]; 
    }

    int[,] solutiontable = new int[machandjob.GetLength(0), machandjob.GetLength(1)];
    int b = 0;


    for (int i = 0; i < solutiontable.GetLength(0); i++)
    {
        solutiontable[i, 0] = end[i, 3] - d[i];
        if (solutiontable[i, 0] > 0) solutiontable[i, 1] = solutiontable[i, 0];
        else solutiontable[i, 1] = 0;
        b += solutiontable[i, 1];
    }

    return b; 



}

static bool keres2(int[] alap, List<int[]> tabuk)
{

    return tabuk.Contains(alap);
}
static void tabu(int iter, int[] joblines, int[,] machandjob, int[] d) 
{
    Console.WriteLine();
    //iter = 500;
    int[] mostanisor = joblines;
    List<int[]> tabu = new List<int[]>(); 
    List<int[]> szom = new List<int[]>(); 
                                          
    int tabumax = 35; 
    int max = 0;
    for (int i = 0; i < iter; i++)
    {
        List<int[]> szomszed = new List<int[]>();

        szom = szomszedok(mostanisor);
        for (int j = 0; j < szomszed.Count; j++)
        {
            if (keres2(szomszed[j], tabu))
            {

                szomszed.Remove(szomszed[j]); 
            }
        }
        max = eredmeny(machandjob, d, mostanisor);
        foreach (var j in szomszed)
        {
            int a = eredmeny(machandjob, d, j); 
            if (a < max)
            {
                max = a;
                mostanisor = j; 
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
    }
    Console.WriteLine("A legjobb megoldási sor");
    foreach (var i in mostanisor)
    {
        Console.Write($"{i},");
    }
    Console.WriteLine($"A legjobb megoldás értéke: {max}");
}
