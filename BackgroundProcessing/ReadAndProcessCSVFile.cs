using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace BackgroundProcessing
{
    public class ReadAndProcessCSVFile
    {
        private readonly ILogger<ReadAndProcessCSVFile> _logger;

        public ReadAndProcessCSVFile(ILogger<ReadAndProcessCSVFile> logger)
        {
            _logger = logger;
        }

        [Function(nameof(ReadAndProcessCSVFile))]
        public async Task Run([BlobTrigger("csv-files/{name}", Connection = "AzureWebJobsStorage")] Stream stream, string name)
        {
            using var blobStreamReader = new StreamReader(stream);
            var content = await blobStreamReader.ReadToEndAsync();
            _logger.LogInformation($"C# Blob trigger function Processed blob\n Name: {name} \n Data: {content}");
        }
    }
}
