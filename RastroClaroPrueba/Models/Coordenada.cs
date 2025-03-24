using System;

namespace RastroClaroPrueba.Models
{
    public class Coordenada
    {
        public int Id { get; set; } 
        public double Latitude { get; set; } 
        public double Longitude { get; set; } 
        public int PacienteId { get; set; } 
        public string Fecha { get; set; } 
    }
}