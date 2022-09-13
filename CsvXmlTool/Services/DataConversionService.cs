using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvXmlTool.Controllers;
using CsvXmlTool.Interfaces;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using OfficeOpenXml;

namespace CsvXmlTool.Services
{
    public class DataConversionService : IDataService
    {
        private static List<PersonPets> _persons;
        private readonly IDatabaseService _databaseService;

        public DataConversionService(IDatabaseService databaseService)
        {
            _persons = new List<PersonPets>();
            _databaseService = databaseService;
        }


        public string GetXmlFromDatabase()
        {
            //var databaseRecords = _databaseService.Get();

            var databaseRecords = _persons;

            var xmlString = "<?xml version=\"1.0\" encoding=\"UTF - 8\" ?>\n<people>\n";

            foreach (var person in databaseRecords)
            {
                xmlString+=new string(' ', 4);
                xmlString += $"<person name={person.PersonName} age={person.Age}>\n";
                xmlString += new string(' ', 8);
                xmlString += $"<pets>\n";

                xmlString = PetAppendHelper(xmlString, person.Pet1, person.Pet1Type);
                xmlString = PetAppendHelper(xmlString, person.Pet2, person.Pet2Type);
                xmlString = PetAppendHelper(xmlString, person.Pet3, person.Pet3Type);

                xmlString += new string(' ', 8);
                xmlString += $"</pets>\n";
                xmlString += new string(' ', 4);
                xmlString += $"</person>\n";
            }

            xmlString += "</people>";
            return xmlString;
        }


        public Tuple<bool, string> RecordCsvToDatabase(IFormFile formFile)
        {
            using (var fileStream = formFile.OpenReadStream())
            using (var reader = new StreamReader(fileStream))
            {
                string row;
                bool initial = true;
                while ((row = reader.ReadLine()) != null)
                {
                    var items = row.Split(',');

                    if (initial)
                    {
                        initial = false;
                        continue;
                    }

                    _persons.Add(new PersonPets
                    {
                        PersonName = items[0].ToString(),
                        Age = Convert.ToInt32(items[1]),
                        Pet1 = items[2].ToString(),
                        Pet1Type = items[3].ToString(),
                        Pet2 = items[4].ToString(),
                        Pet2Type = items[5].ToString(),
                        Pet3 = items[6].ToString(),
                        Pet3Type = items[7].ToString(),
                    });
                }

                //var insertResult = _databaseService.Insert(_persons);

                //if (!insertResult)
                //{
                //    return Tuple.Create(false, "Unexpected database error occured!");
                //}
            }

            return Tuple.Create(true, JsonConvert.SerializeObject(_persons, Formatting.Indented));
        }


        public Tuple<bool, string> RecordXlsxToDatabase(IFormFile formFile)
        {
            if (formFile.Length > 0)
            {
                var s = formFile.OpenReadStream();
                try
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    using (var p = new ExcelPackage(s))
                    {
                        var sheet = p.Workbook.Worksheets.First();
                        var count = sheet.Dimension.Rows;

                        for (int i = 2; i < count; i++)
                        {
                            _persons.Add(new PersonPets
                            {
                                PersonName = sheet.Cells[i, 1].Value?.ToString(),
                                Age = Convert.ToInt32(sheet.Cells[i, 2].Value?.ToString()),
                                Pet1 = sheet.Cells[i, 3].Value?.ToString(),
                                Pet1Type = sheet.Cells[i, 4].Value?.ToString(),
                                Pet2 = sheet.Cells[i, 5].Value?.ToString(),
                                Pet2Type = sheet.Cells[i, 6].Value?.ToString(),
                                Pet3 = sheet.Cells[i, 7].Value?.ToString(),
                                Pet3Type = sheet.Cells[i, 8].Value?.ToString(),
                            });
                        }
                        //var insertResult = _databaseService.Insert(_persons);

                        //if (!insertResult)
                        //{
                        //    return Tuple.Create(false, "Unexpected database error occured!");
                        //}
                    }
                    return Tuple.Create(true, JsonConvert.SerializeObject(_persons, Formatting.Indented));
                }
                catch (Exception ex)
                {
                    return Tuple.Create(false, $"Unexpected server error occured: {ex.Message}");
                }
            }

            return Tuple.Create(false, "Input file should have rows!");
        }

        private string PetAppendHelper(string xmlString, string pet, string petType)
        {
            if (Equals(pet.Trim(), "-") || Equals(petType.Trim(), "-"))
            {
                return xmlString;
            }

            xmlString += new string(' ', 12);
            xmlString += $"<pet name={pet} type={petType} />\n";

            return xmlString;
        }
    }
}
