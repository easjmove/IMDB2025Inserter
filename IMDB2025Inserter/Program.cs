
string filename = "c:/temp/title.basics.tsv";
IEnumerable<string> imdbData = File.ReadAllLines(filename).Skip(1).Take(1000);
foreach (string title in imdbData)
{
    string[] values = title.Split('\t');
    if (values.Length == 9)
    {
        
    }
    else
    {
        Console.WriteLine("Not 9 values: " + title);
    }
}