using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqliteBasic
{
    public class Registration_class
    {
        [PrimaryKey, AutoIncrement]
        public string name { get; set; }
        public int numOfSeat { get; set; }
        public string password { get; set; }
    }
}
