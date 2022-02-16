using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebexBot.Models
{
    public class Users
    {
        public int ID { get; set; }
        public string ClientID { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public bool Enabled { get; set; }
    }
}
