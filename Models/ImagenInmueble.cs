namespace inmobiliariaFUNES.Models
{
    public class ImagenInmueble
    {
        public int IdImagen { get; set; }
        public int IdInmueble { get; set; }
        public string Url { get; set; } = "";
        public bool EsPortada { get; set; }
    }
}