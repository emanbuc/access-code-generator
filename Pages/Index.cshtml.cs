using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ReclutamentoSondaggio.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    private const string CodesFilePath = "codes.txt";
    private const string linkPrefix = "https://example.com?code=";

    private static readonly object fileLock = new object();
    private List<string> AvailableCodes { get; set; }

    public string Link { get; private set; }

    public void OnGet()
    {
        PopAccessCodeFromListWithLock();
    }

    private void PopAccessCodeFromListWithLock()
    {
        //log the thread id
        _logger.LogInformation($"PopAccessCodeFromListWithLock. Thread id: {Thread.CurrentThread.ManagedThreadId}");
        lock (fileLock)
        {
            AvailableCodes = ReadCodesFromFile();
            string code = AvailableCodes[0];            
            AvailableCodes.RemoveAt(0);
            UpdateCodesFile();
            Link = GenerateLinkWithCode(code);            
        }
        
    }

    private List<string> ReadCodesFromFile()
    {
        var codes = new List<string>();

        using (var reader = new StreamReader(CodesFilePath))
        {
            
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                codes.Add(line.Trim());
            }
        }

        return codes;
    }

    private string GenerateLinkWithCode(string code)
    {                
        return  $"{linkPrefix}{code}";
        
    }

    private void UpdateCodesFile()
    {
        using (var writer = new StreamWriter(CodesFilePath))
        {
            foreach (var code in AvailableCodes)
            {
                writer.WriteLine(code);
            }
        }
    }    
}
