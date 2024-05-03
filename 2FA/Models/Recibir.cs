namespace _2FA.Models
{
    public class Recibir
    {
        public int IdCuenta { get; set; }
        public string Nombre { get; set; }
        public string Contrasena { get; set; }

        public string SecretCode { get; set; }

        public string Correo { get; set; }
    }
}
