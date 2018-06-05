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
using System.IO;

namespace STC.Data
{
    class SalesTaxAPI
    {
        //A call to this method will pass the entered zipCode variable to a search of the database table named TaxRates.  
        //The return is the first or default record that gets pulled. 
        //(Given that each zip code is unique in this database it should return only one record for the user to see.)
        public static TaxRates GetSalesTax(string zipCode)
        {
            var dbName = "db.sqlite";
            string dbPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var path = Path.Combine(dbPath, dbName);
            var db = new SQLiteConnection(path);

            return db.Table<TaxRates>().FirstOrDefault(taxRate => taxRate.ZipCode == zipCode);

        }
    }
}