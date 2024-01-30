namespace MyProject;
class Program
{



    


    static void Main(string[] args)
    {
        string[] collect = { "Kisi pisi", "tedikis", "zevscheto" };


        List<string> str = new List<string>(5);

        str.AddRange(collect);

        str.Add("Aide nashte");

        str.RemoveAt(0);

        str.Remove("tedikis");


        str.Sort();

        List<string> newstr = str.FindAll(x => x.Length > 5);

        PrintList(newstr);
        
    }
    static void PrintList(List<string> list) 
    {
        foreach (string s in list)
        {
            Console.WriteLine(s);
        }
    }
}