using _2FA.Models;

namespace _2FA.Infraestructura
{
    public interface ICuentasServices
    {
        Task Borrar(int IdCuenta);
        Task Crear(Cuentas cuentas);
        Task<string> GetSecret(string Correo);
        Task SetSecret(Cuentas cuentas);
        Task<IEnumerable<Cuentas>> TodosLosRegistros();

        Task<bool> ValidarConContrasena(Cuentas cuentas);
    }
}
