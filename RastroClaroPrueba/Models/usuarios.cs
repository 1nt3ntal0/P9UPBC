using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RastroClaroPrueba.Models
{
    public class usuarios
    {
        public int Id { get; set; } 
        public string Username { get; set; } 
        public string Password { get; set; }
        public pacientes Paciente { get; set; }
    }
}
