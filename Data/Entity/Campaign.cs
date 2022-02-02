using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Entity
{
    public class Campaign : BaseEntity
    {
        public string Name { get; set; }
        public int Duration { get; set; }
        public double PriceManipulationLimit { get; set; }
        public int TargetSalesCount { get; set; }
        public bool Status { get; set; }
        public int TotalSales { get; set; }
        public int Turnover { get; set; }
        public double AverageItemPrice { get; set; }
    }
}
