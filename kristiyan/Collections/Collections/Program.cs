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
        

        foreach(string s in str) {
            Console.WriteLine(s);
        }
    }
}