
using System.Collections.Generic;
using System.Threading.Tasks;

using SwishMapper.Models;
using SwishMapper.Work;

namespace SwishMapper.Tests.TestHelpers
{
    public class CsvRowBuilder : ICsvNormalizer
    {
        private readonly List<CsvRow> rows = new List<CsvRow>();

        public CsvRow Current
        {
            get
            {
                return rows[rows.Count - 1];
            }
        }

        public string Path { get; set; }


        public CsvRowBuilder Row()
        {
            rows.Add(new CsvRow());
            return this;
        }


        public CsvRowBuilder EntityName(string name)
        {
            Current.ElementName = name;
            return this;
        }


        public CsvRowBuilder AttributeName(string name)
        {
            Current.AttributeName = name;
            return this;
        }


        public CsvRowBuilder ChildElementName(string name)
        {
            Current.ChildElementName = name;
            return this;
        }


        public CsvRowBuilder DataType(string type)
        {
            Current.DataType = type;
            return this;
        }


        public CsvRowBuilder MaxLength(string len)
        {
            Current.MaxLength = len;
            return this;
        }


        public CsvRowBuilder Required(string req)
        {
            Current.Required = req;
            return this;
        }


        public CsvRowBuilder Comment(string comment)
        {
            Current.Comment = comment;
            return this;
        }


        public CsvRowBuilder MinOccurs(string min)
        {
            Current.MinOccurs = min;
            return this;
        }


        public CsvRowBuilder MaxOccurs(string max)
        {
            Current.MaxOccurs = max;
            return this;
        }


        public Task<List<CsvRow>> RunAsync()
        {
            return Task.FromResult(rows);
        }


        public void Dump(PlanDumperContext context)
        {
        }
    }
}
