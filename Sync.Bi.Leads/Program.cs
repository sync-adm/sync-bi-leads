using ClosedXML.Excel;
using static Sync.Bi.Leads.Leads.LeadConst;
using Sync.Bi.Leads.Leads;
using Sync.Bi.Leads;
using DocumentFormat.OpenXml.Spreadsheet;
using Sync.Bi.Leads.Helpers;

class Program
{
    static async Task Main(string[] args)  // Mudança aqui
    {
        var url = "https://sync-bi-leads.s3.sa-east-1.amazonaws.com/raw/c2s/Martini_Motors_Leads_07072022_30042024.xlsx";

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
                Team = string.IsNullOrEmpty(row.Cell(1).Value.ToString()) ? "" : row.Cell(1).Value.ToString(),
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
