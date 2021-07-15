using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TAMS.Models
{
    public class ScreeningToolData
    {
        public int Id { get; set; }
        public string BarCode { get; set; }
        public int Action { get; set; }
        public string Remarks { get; set; }
        public DateTime CreatedDate { get; set; }

    }
}
