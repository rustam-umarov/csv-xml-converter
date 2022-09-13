using System;
using System.Collections.Generic;
using CsvXmlTool.Controllers;

namespace CsvXmlTool.Interfaces
{
    public interface IDatabaseService
    {
        public bool Insert(List<PersonPets> personPets);
        public List<PersonPets> Get();
    }
}
