using _2FA.Infraestructura;
using _2FA.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace _2FA.Services
{
    public class CuentasServices: ICuentasServices
    {
        private readonly string connectionString;

        public CuentasServices(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<Cuentas>> TodosLosRegistros()
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Cuentas>(@"SELECT * FROM Cuentas", new { });
        }

        public async Task Crear(Cuentas cuentas)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection
                .QueryFirstOrDefaultAsync<int>(@"INSERT INTO Cuentas (Nombre, Contrasena)
                 Values(@Nombre, @Contrasena, @Correo)
                  SELECT SCOPE_IDENTITY();", cuentas);
            cuentas.IdCuenta = id;
        }


        public async Task SetSecret(Cuentas cuentas)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"UPDATE Cuentas SET
            SecretCode=@SecretCode WHERE Correo=@Correo", cuentas);
        }


        public async Task<string> GetSecret(string Correo)
        {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryFirstOrDefaultAsync<string>(@"SELECT SecretCode FROM Cuentas
                Where Correo= @Correo", new {Correo});

        }
    }
}

