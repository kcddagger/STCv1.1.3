using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SQLite;

namespace STC
{
    /* This class sets up the database for access by setting out the information found in the table.
     * There can be multiple tables listed, each must be preceeded with [Table("tableName")].  However,
     * it is sometimes easier to give each its own class to prevent mixing up your own information both in your head and in the database.*/

    [Table("TaxRates")]
    public class TaxRates
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string TaxRegionName { get; set; }
        public string StateRate { get; set; }
        public string EstimatedCombinedRate { get; set; }
        public string EstimatedCountyRate { get; set; }
        public string EstimatedCityRate { get; set; }
        public string EstimatedSpecialRate { get; set; }
        public string RiskLevel { get; set; }
    }
}