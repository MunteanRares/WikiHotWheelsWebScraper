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

        public string Series { get; set; }
        public int SerieNum { get; set; }

        public string PhotoURL { get; set; }
        
        public int YearProduced { get; set; }
        public int YearProducedNum { get; set; }
    }
}
