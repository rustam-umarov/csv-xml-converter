using System;
using Microsoft.AspNetCore.Http;

namespace CsvXmlTool.Interfaces
{
    public interface IDataService
    {
        public Tuple<bool, string> RecordCsvToDatabase(IFormFile formFile);
        public Tuple<bool, string> RecordXlsxToDatabase(IFormFile formFile);
        public string GetXmlFromDatabase();
    }
}
