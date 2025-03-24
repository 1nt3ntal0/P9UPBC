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
        public string nombre { get; set; }
        public DateTime fecha { get; set; }
        public int sangre { get; set; }
        public string religion { get; set; }
        public int Grado { get; set; }
        public int Extra { get; set; }
        public long Telefono { get; set; }
        

    }
}
