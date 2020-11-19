using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace KTSite.Areas.Warehouse.Views.OrderWarehouse
{
    public class CSVOrderImport
    {
        #region Attributes
        private List<CSVOrderLine> lines = new List<CSVOrderLine>();
        #endregion
        public void Import(string filename)
        {
            try
            {
                using (TextReader fs = new StreamReader(filename))
                {
                    // I just need this one line to load the records from the file in my List<CsvLine>
                    lines = new CsvHelper.CsvReader(fs,CultureInfo.InvariantCulture).GetRecords<CSVOrderLine>().ToList();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            PrintLines();
        }
        private void PrintLines()
        {
            Console.WriteLine(" - Import done: {0} lines imported!\r\n", lines.Count);
            Console.WriteLine(" - Showing the 1st three (3) rows:");

            // I know, I'm doing an 'extra' ToList there. It's just to make it a one-liner =)
            lines.Take(3).ToList().ForEach(l => Console.WriteLine("   - {0}", l));
        }
    }
}
