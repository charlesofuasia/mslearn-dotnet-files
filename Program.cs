using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using Microsoft.VisualBasic;



var currentDirectory = Directory.GetCurrentDirectory();

var storeDirectory = Path.Combine(currentDirectory, "stores");

var salesTotalDir = Path.Combine(currentDirectory, "salesTotalDir");

Directory.CreateDirectory(salesTotalDir);
var salesFiles = FindFiles(storeDirectory);
var salesTotal = CalculateSalesTotal(salesFiles);

File.AppendAllText(Path.Combine(salesTotalDir, "totals.txt"), $"{salesTotal}{Environment.NewLine}");
var salesSummary = PrintSalesSummary(salesFiles);
//Console.WriteLine(salesSummary);
File.AppendAllText(Path.Combine(salesTotalDir, "salesSummary.txt"), salesSummary);


IEnumerable<string> FindFiles(string folderName)
{
    List<string> salesFiles = new List<string>();

    var foundFiles = Directory.EnumerateFiles(folderName, "*", SearchOption.AllDirectories);

    foreach (var file in foundFiles)
    {
        // The file name will contain the full path, so only check the end of it
        var extension = Path.GetExtension(file);
        if (extension == ".json")
        {
            salesFiles.Add(file);
        }
    }

    return salesFiles;
}

double CalculateSalesTotal(IEnumerable<string> salesFiles)
{
    double salesTotal = 0;
    //loop over each file path in salesFile
    foreach (var file in salesFiles)
    {
        //first read the file content
        string salesJson = File.ReadAllText(file);

        //then pass the contents as Json.
        SalesData? data = JsonConvert.DeserializeObject<SalesData?>(salesJson);

        // then add the amount found in the Total field to the salesTotal variable
        salesTotal += data?.Total ?? 0;
    }


    return salesTotal;
}

string PrintSalesSummary(IEnumerable<string> salesFiles)
{
    string sumTitle = "Sales Summary:\n";
    string lineCut = new string('-', 20);
    sumTitle += lineCut + "\n";
    sumTitle += $"Total Sales: --- {Math.Round(salesTotal, 2)}\n";
    sumTitle += lineCut + "\n";
    sumTitle += "Store Name, ----- Total\n";

    foreach (var file in salesFiles)
    {
        if (file.EndsWith("sales.json"))
        {
            string salesJson = File.ReadAllText(file);
            SalesData? data = JsonConvert.DeserializeObject<SalesData?>(salesJson);
            string storeName = Path.GetFileName(Path.GetDirectoryName(file) ?? string.Empty);
            sumTitle += $" {storeName}, ----- {data?.Total}\n";

        }
    }
    return sumTitle;
}
record SalesData(double Total);
