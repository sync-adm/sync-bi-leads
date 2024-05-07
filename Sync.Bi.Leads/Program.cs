using ClosedXML.Excel;
using static Sync.Bi.Leads.Leads.LeadConst;
using Sync.Bi.Leads.Leads;
using Sync.Bi.Leads;
using DocumentFormat.OpenXml.Spreadsheet;
using Sync.Bi.Leads.Helpers;
using Microsoft.IdentityModel.Tokens;
using Sync.Bi.Leads.FacebookCsv;
using System.Formats.Asn1;
using System.Globalization;
using CsvHelper;
using DocumentFormat.OpenXml.Drawing.Diagrams;

class Program
{
    static async Task Main(string[] args)  // Mudança aqui
    {
          await CreateCSV();

       // await InsertIntoBaseData();

    }
    private static async Task CreateCSV()
    {
        using var db = new LeadDbContext();
        var leads = db.Leads.ToList();
        
      
        var facebookInCsv = new List<FacebookCSVModel>();
        foreach (var lead in leads)
        {            
            var nameSplited  = lead.Name.Split(' ').ToList();           
            var copyName = new List<string>();
            copyName.AddRange(nameSplited);
            copyName.RemoveAt(0);
            var lastName = string.Join(" ", copyName);

            var leadCsv = new FacebookCSVModel
            {
                email = lead.Email,
                phone = lead.Phone,
                ct = lead.City,
                fn = nameSplited[0],
                ln = lastName,
                country = "BR"
            };

            facebookInCsv.Add(leadCsv);
        }

        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string filePath = Path.Combine(desktopPath, "FacebookLeadsTest.csv");

        // Criando e escrevendo no arquivo CSV
        using (var writer = new StreamWriter(filePath)) // Cria o arquivo na área de trabalho
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(facebookInCsv); // Escreve os registros da lista no arquivo
        }

    }

    private static async Task InsertIntoBaseData()
    {
        var url = "https://sync-bi-leads.s3.sa-east-1.amazonaws.com/raw/c2s/Martini_Motors_Leads_07072022_30042024.xlsx"; //Substituir com a Url  no parametro 

        using var client = new HttpClient();
        using var response = client.GetAsync(url).Result;
        response.EnsureSuccessStatusCode(); // Lança exceção se a resposta não for bem-sucedida.


        using var stream = await response.Content.ReadAsStreamAsync();
        using var workbook = new XLWorkbook(stream);
        var worksheet = workbook.Worksheet(1);
        var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // Pula o cabeçalho
        var ListRows = rows.ToList();
        int count = 0;
        using var db = new LeadDbContext();
        for (int i = 0; i < ListRows.Count(); i++) // valor do i 
        {

            var row = ListRows[i]; // Corrigido o uso de 'i' em vez de 'row'

            var test = row.Cell(4).Value;
            var priceInString = row.Cell(13).Value.ToString();
            priceInString = priceInString.OnlyNumber();
            decimal? priceDecimal = !string.IsNullOrEmpty(priceInString) ? Convert.ToDecimal(priceInString) : (decimal?)null;

            var dataDoChegada = row.Cell(2).Value.ToString();
            var DataValue = DateTime.Parse(dataDoChegada);

            var lead = new Lead
            {                
                Date = DataValue,
                Source = string.IsNullOrEmpty(row.Cell(4).Value.ToString()) ? "" : row.Cell(4).Value.ToString(),
                Title = string.IsNullOrEmpty(row.Cell(12).Value.ToString()) ? "" : row.Cell(12).Value.ToString(),
                Price = Convert.ToDecimal(priceDecimal ?? 0),
                Name = string.IsNullOrEmpty(row.Cell(14).Value.ToString()) ? "" : row.Cell(14).Value.ToString(),
                Email = string.IsNullOrEmpty(row.Cell(15).Value.ToString()) ? "" : row.Cell(15).Value.ToString(),
                Phone = string.IsNullOrEmpty(row.Cell(17).Value.ToString()) ? "" : row.Cell(17).Value.ToString(),
                City = string.IsNullOrEmpty(row.Cell(18).Value.ToString()) ? "" : row.Cell(18).Value.ToString(),
                CompanyId = 1
            };

            db.Leads.Add(lead);
            db.SaveChanges();
            count++;
        }

        Console.WriteLine($"Ultimo possição que foi atualizada no banco {count}");
    }
}

