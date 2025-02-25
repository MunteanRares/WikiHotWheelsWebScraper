using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WikiHotWheelsWebScraper.Models
{
    public class HotWheelsModel
    {
        public string ModelName { get; set; }
        public string SeriesName { get; set; }
        public string SeriesNum { get; set; }
        public string PhotoURL { get; set; }
        public string YearProduced { get; set; }
        public string YearProducedNum { get; set; }
        public string ToyNum { get; set; }
    }
}
