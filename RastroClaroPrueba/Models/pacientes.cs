using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RastroClaroPrueba.Models
{
    public class pacientes
    {
        public int FkID { get; set; }
        public string name { get; set; }
        public int age { get; set; } 
        public int blood_type { get; set; }
        public string religion { get; set; }
        public int grade { get; set; }
        public int extra { get; set; }
        public string phone { get; set; } 
    }
}
