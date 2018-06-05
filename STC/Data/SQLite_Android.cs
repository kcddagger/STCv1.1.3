using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SQLite;


namespace STC.Data
{
    public class SQLite_Android
    {
        public SQLite_Android() { }

        //The call to this method allows the program to unpack the database file that is included 
        //and set it up in a safe location on the user's device.
        public static void NewDatabase()
        {
            string dbName = "db.sqlite";
            string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), dbName);

            if (!File.Exists(dbPath))
            {
                using (BinaryReader br = new BinaryReader(Android.App.Application.Context.Assets.Open("db.sqlite")))
                {
                    using (BinaryWriter bw = new BinaryWriter(new FileStream(dbPath, FileMode.Create)))
                    {
                        byte[] buffer = new byte[2048];
                        int len = 0;
                        while ((len = br.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            bw.Write(buffer, 0, len);
                        }
                    }
                }
            }
        }                
    }
}