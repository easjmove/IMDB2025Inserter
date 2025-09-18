using IMDB2025Inserter;
using Microsoft.Data.SqlClient;

string connectionString = "Server=localhost;Database=IMDB;" +
    "integrated security=True;TrustServerCertificate=True;";
SqlConnection sqlConn = new SqlConnection(connectionString);
sqlConn.Open();
SqlTransaction sqlTrans = sqlConn.BeginTransaction();

Dictionary<string, int> TitleTypes = new Dictionary<string, int>();

string filename = "c:/temp/title.basics.tsv";
IEnumerable<string> imdbData = File.ReadAllLines(filename).Skip(1).Take(1000);
foreach (string titleString in imdbData)
{
    string[] values = titleString.Split('\t');
    if (values.Length == 9)
    {
        if (!TitleTypes.ContainsKey(values[1]))
        {
            AddTitleType(values[1], sqlConn, sqlTrans, TitleTypes);
        }

        try
        {
            Title title = new Title
            {
                Id = int.Parse(values[0].Substring(2)),
                TitleType = TitleTypes[values[1]],
                PrimaryTitle = values[2],
                OriginalTitle = values[3] == "\\N" ? null : values[3],
                IsAdult = values[4] == "1",
                StartYear = values[5] == "\\N" ? null : int.Parse(values[5]),
                EndYear = values[6] == "\\N" ? null : int.Parse(values[6]),
                RuntimeMinutes = values[7] == "\\N" ? null : int.Parse(values[7]),
                Genres = values[8] == "\\N" ? new List<string>() : values[8].Split(',').ToList()
            };

            SqlCommand sqlComm = new SqlCommand("", sqlConn, sqlTrans);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error parsing line: " + titleString);
            Console.WriteLine(ex.Message);
        }
    }
    else
    {
        Console.WriteLine("Not 9 values: " + titleString);
    }
}

sqlTrans.Rollback();
sqlConn.Close();

void AddTitleType(string titleType, SqlConnection sqlConn, SqlTransaction sqlTrans, Dictionary<string, int> TitleTypes)
{
    if (!TitleTypes.ContainsKey(titleType))
    {
        SqlCommand sqlComm = new SqlCommand(
            "INSERT INTO TitleTypes (TitleType) VALUES (" + titleType + "); " +
            "SELECT SCOPE_IDENTITY();", sqlConn, sqlTrans);
        int newId = Convert.ToInt32(sqlComm.ExecuteScalar());
        TitleTypes[titleType] = newId;
    }
}