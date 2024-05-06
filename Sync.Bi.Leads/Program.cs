using ClosedXML.Excel;
using static Sync.Bi.Leads.Leads.LeadConst;
using Sync.Bi.Leads.Leads;
using Sync.Bi.Leads;
using DocumentFormat.OpenXml.Spreadsheet;

class Program
{
    static async Task Main(string[] args)  // Mudança aqui
    {
        var url = "https://sync-bi-leads.s3.sa-east-1.amazonaws.com/raw/c2s/00027164000102%20-%20Martini%20Veiculos.xlsx";

        using var client = new HttpClient();
        using var response = client.GetAsync(url).Result;
        response.EnsureSuccessStatusCode(); // Lança exceção se a resposta não for bem-sucedida.

        
        using var stream = await response.Content.ReadAsStreamAsync();
        using var workbook = new XLWorkbook(stream);
        var worksheet = workbook.Worksheet(1);
        var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // Pula o cabeçalho
        
        using var db = new LeadDbContext(); // Certifique-se de que LeadDbContext está corretamente configurado
        foreach (var row in rows)
        {
            var aaaa = row.Cell(2).Value.ToString();
          var a =  DateTime.Parse(aaaa);

            Console.WriteLine($"Lendo linha {row.RowNumber()}:, {row.Cell(1).Value.ToString()},{aaaa}");
            var lead = new Lead
            {

                IntegrationType = IntegrationType.C2S,
                Team = row.Cell(1).GetValue<string>(),
                Date = row.Cell(2).GetValue<DateTime>(),
                Source = row.Cell(3).GetValue<string>(),
                Title = row.Cell(4).GetValue<string>(),
                Price = row.Cell(5).GetValue<decimal>(),
                Name = row.Cell(6).GetValue<string>(),
                Email = row.Cell(7).GetValue<string>(),
                Phone = row.Cell(8).GetValue<string>(),
                City = row.Cell(9).GetValue<string>()
            };
            db.Leads.Add(lead);
        }
        db.SaveChanges();
    }
}
