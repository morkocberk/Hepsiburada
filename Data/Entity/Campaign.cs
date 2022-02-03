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
        public STATUS Status { get; set; }
        public int TotalSales { get; set; }
        public double Turnover { get; set; }
        public double AverageItemPrice { get; set; }
    }
    public enum STATUS
    {
        ACTIVE,
        PASSIVE
    }
}
