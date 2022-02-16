using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebexBot.Models
{
    public class ActionsPermissions
    {
        public int ID { get; set; }
        public int ActionID { get; set; }
        public int UserID { get; set; }
        public bool Enabled { get; set; }
        public string ActionDescription { get; set; }
    }
}
