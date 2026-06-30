using System;
using System.Threading.Tasks;
using MySqlConnector;
using System.Text;
using System.Data;
using System.ComponentModel;

namespace Progra3Card.Administrativo
{
    class Program
    {
        private static string connectionString = "Server=localhost;Database=mi_banco_db;Uid=root;Pwd=;";
        private static Dictionary<string,int> prefijosBancos = new Dictionary<string, int>
        {
            {"banco nacion", 4512},
            {"banco provincia", 4513},
            {"banco galicia", 4514},
            {"banco santander", 4515},
            {"banco bbva", 4516},
            {"banco macro", 4517}
        };

        static async Task Main(string[] args)
        {
            bool salir = false;
            while (!salir)
            {
                //Console.Clear();
                Console.WriteLine("========================================");
                Console.WriteLine("    SISTEMA ADMINISTRATIVO PROGRA3CARD   ");
                Console.WriteLine("========================================");
                Console.WriteLine("1. Emitir Nuevo Usuario (Alta de Usuario)");
                Console.WriteLine("2. Emitir Nueva Tarjeta (Alta de Cliente)");
                Console.WriteLine("3. Listar Tarjetas");
                Console.WriteLine("4. Ver Detalle de una Tarjeta / Cliente");
                Console.WriteLine("5. Eliminar Tarjeta (Baja de Sistema)");
                Console.WriteLine("6. Emitir Nueva Liquidación Mensual");
                Console.WriteLine("7. Salir");
                Console.WriteLine("========================================");
                Console.Write("Seleccione una opción: ");

                switch (Console.ReadLine())
                {
                    case "1": await MenuEmitirUsuario(); break;
                    case "2": await MenuEmitirTarjeta(); break;
                    case "3": await MenuListarTarjetas(); break;
                    case "4": await MenuVerDetalleTarjeta(); break;
                    case "5": await MenuEliminarTarjeta(); break;
                    case "6": await MenuEmitirLiquidacion(); break;
                    case "7": salir = true; break;
                    default:
                        Console.WriteLine("Opción no válida. Presione una tecla para continuar...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        // Funciones a completar:

        static async Task MenuEmitirUsuario()
        {
            try{
                Console.WriteLine("Ingrese numero de documento.");
                string documento = Console.ReadLine();
                Console.WriteLine("Ingrese tipo de documento.");
                string tipo_doc = Console.ReadLine();
                Console.WriteLine("Ingrese el nombre del nuevo usuario");
                string nombre = Console.ReadLine();
                Console.WriteLine("Ingrese el apellido del nuevo usuario");
                string apellido = Console.ReadLine();
                Console.WriteLine("Ingrese fecha de nacimiento del nuevo usuario (formato: YYYY-MM-DD).");
                string fecha_nacimiento = Console.ReadLine();
                Console.WriteLine("Ingrese el email del nuevo usuario.");
                string email = Console.ReadLine();
                string sql = "INSERT INTO usuarios(documento,tipo_doc,nombre,apellido,fecha_nacimiento,email,usuario,password)" +
                            "VALUES(@documento,@tipo_doc,@nombre,@apellido,@fecha_nacimiento,@email, NULL, NULL);";
                using var connection = new MySqlConnection(connectionString);
                using var command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@documento",documento);
                command.Parameters.AddWithValue("@tipo_doc",tipo_doc);
                command.Parameters.AddWithValue("@nombre",nombre);
                command.Parameters.AddWithValue("@apellido",apellido);
                command.Parameters.AddWithValue("@fecha_nacimiento",fecha_nacimiento);
                command.Parameters.AddWithValue("@email",email);
                await connection.OpenAsync();
                int rows = command.ExecuteNonQuery();
                if (rows > 0)
                {
                    Console.WriteLine("Se creo el usuario");
                }
                else
                {
                    Console.WriteLine("no se creo el usuario");
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Error en la base de datos: {ex.Message}");
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error al intentar crear el usuario: {ex.Message}");
            }

        }

        static async Task MenuEmitirTarjeta()
        {
            try{
                Console.Clear();
                Console.WriteLine("--- EMITIR TARJETA ---");
                Console.WriteLine("Ingrese DNI titular");
                string dni = Console.ReadLine();
                Console.WriteLine($"{dni}");
                Console.WriteLine("Ingrese banco emisor: ");
                string banco = Console.ReadLine();
                Console.WriteLine($"{banco}");
                int resultado = await EmitirTarjeta(dni, banco);
                if (resultado > 0)
                {
                    Console.WriteLine("Tarjeta creada con exito");
                }
                else
                {
                    Console.WriteLine("No se pudo crear la tarjeta");
                }
            }catch(Exception ex)
            {
                Console.WriteLine($"Error en menu de emitir tarjeta: {ex.Message}");
            }
        }

        static async Task MenuListarTarjetas()
        {
            Console.Clear();
            Console.WriteLine("--- LISTADO GENERAL DE TARJETAS ---");
            Console.WriteLine("{0,-12} {1,-18} {2,-20} {3,-15}", "Nro Cuenta", "Nro Tarjeta", "Banco Emisor", "DNI Titular");
            Console.WriteLine("----------------------------------------------------------------------");

            // === A realizar ===
            // Aquí deben implementar un SELECT sobre la tabla 'tarjetas'
            // para recorrer las filas e imprimirlas en la consola.
            
            await ObtenerYMostrarTarjetas();

            Console.WriteLine("\nPresione una tecla para volver al menú...");
            Console.ReadKey();
        }

        static async Task MenuVerDetalleTarjeta()
        {
            Console.Clear();
            Console.WriteLine("--- DETALLE DE TARJETA Y CLIENTE ---");
            Console.Write("Ingrese el Número de Cuenta a consultar: ");
            //int numCuenta = Convert.ToInt32(Console.ReadLine());
            if(int.TryParse(Console.ReadLine(), out int numCuenta))
            {
                try
                {
                    await MostrarDetalleCompleto(numCuenta);
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Error al consultar: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Numero de cuenta invalido");
            }

            // === A realizar ===
            // Aquí deben realizar un SELECT con un JOIN entre 'tarjetas' y 'usuarios' 
            // filtrando por el numCuenta para traer todos los campos (Nombre, Apellido, Email, Saldo, etc.)
            
            //await MostrarDetalleCompleto(numCuenta);

            Console.WriteLine("\nPresione una tecla para volver al menú...");
            Console.ReadKey();
        }

        static async Task MenuEliminarTarjeta()
        {
            Console.Clear();
            Console.WriteLine("--- ELIMINAR TARJETA DEL SISTEMA ---");
            Console.Write("Ingrese el Número de Cuenta de la tarjeta a dar de baja: ");
            int numCuenta = Convert.ToInt32(Console.ReadLine());

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n⚠️ ADVERTENCIA: Se eliminará la tarjeta, sus liquidaciones y los datos de acceso web vinculados.");
            Console.ResetColor();
            Console.Write("¿Está seguro de continuar? (S/N): ");
            
            if (Console.ReadLine().ToUpper() == "S")
            {
                // === A realizar ===
                // Aquí deben ejecutar un DELETE sobre la tabla 'tarjetas' donde num_cuenta = numCuenta.
                // Como definimos ON DELETE CASCADE en la base de datos, las liquidaciones se borrarán solas.
                // Opcional: Evaluar si también eliminan al usuario de la tabla 'usuarios' o si lo mantienen.
                
                bool exito = await DarDeBajaTarjeta(numCuenta);

                if (exito)
                    Console.WriteLine("\nTarjeta eliminada correctamente del sistema.");
                else
                    Console.WriteLine("\nError al intentar eliminar la tarjeta. Verifique el número de cuenta.");
            }
            else
            {
                Console.WriteLine("\nOperación cancelada.");
            }

            Console.WriteLine("\nPresione una tecla para volver al menú...");
            Console.ReadKey();
        }

        static async Task MenuEmitirLiquidacion()
        {
            try{
            /*CREATE TABLE IF NOT EXISTS liquidaciones (
                id_liquidacion INT AUTO_INCREMENT PRIMARY KEY,
                num_cuenta INT NOT NULL,
                periodo VARCHAR(7) NOT NULL, -- Formato: 'YYYY-MM'
                fecha_vencimiento DATE NOT NULL,
                total_a_pagar DECIMAL(10,2) NOT NULL,
                pago_minimo DECIMAL(10,2) NOT NULL,
                FOREIGN KEY (num_cuenta) REFERENCES tarjetas(num_cuenta) ON DELETE CASCADE
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
            */ 
            Console.WriteLine("Ingrese numero de cuenta de la tarjeta:\n");
            long num_cuenta = long.Parse(Console.ReadLine());
            Console.WriteLine("Ingrese periodo: (formato: 'YYYY-MM')\n");
            string periodo = Console.ReadLine();
            Console.WriteLine("Ingrese fecha vencimiento: (formato: 'YYYY-MM-DD')\n");
            string fecha_vencimiento = Console.ReadLine();
            Console.WriteLine("Ingrese total a pagar:\n");
            decimal total_a_pagar = decimal.Parse(Console.ReadLine());
            Console.WriteLine("Ingrese pago minimo\n");
            decimal pago_minimo = decimal.Parse(Console.ReadLine());

            string sql = "INSERT INTO liquidaciones(num_cuenta,periodo,fecha_vencimiento,total_a_pagar,pago_minimo)" +
                         "VALUES(@num_cuenta,@periodo,@fecha_vencimiento,@total_a_pagar,@pago_minimo)";
            using var connection = new MySqlConnection(connectionString);
            using var command = new MySqlCommand(sql, connection);
            await connection.OpenAsync();

            command.Parameters.AddWithValue("@num_cuenta",num_cuenta);
            command.Parameters.AddWithValue("@periodo",periodo);
            command.Parameters.AddWithValue("@fecha_vencimiento",fecha_vencimiento);
            command.Parameters.AddWithValue("@total_a_pagar",total_a_pagar);
            command.Parameters.AddWithValue("@pago_minimo",pago_minimo);
            int rows = await command.ExecuteNonQueryAsync();
            if (rows > 0)
            {
                Console.WriteLine("Se creo la liquidacion con exito");
            }
            else
            {
                Console.WriteLine("No se pudo crear la liquidacion");
                }
            }
            catch(MySqlException ex)
            {
                Console.WriteLine($"Error en la base de datos: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear la liquidacion: {ex.Message}");
            }
        }

        // =========================================================================
        // MÉTODOS BASE QUE DEBEN COMPLETAR CON LA LÓGICA 
        // =========================================================================

        static async Task<int> EmitirTarjeta(string dni, string banco)
        {
            try{
                Console.WriteLine("Estoy dentro de emitirtarjeta");
                //string binDelBanco = prefijosBancos[banco.ToLower()].ToString();
                if (!prefijosBancos.TryGetValue(banco.ToLower(), out int binDelBanco))
                {
                    throw new ArgumentException($"El banco '{banco}' no esta registrado");
                }

                string identificadorDeCuenta = generarIdentificadorDeCuenta();
                string binEIdentificador = $"{binDelBanco}{identificadorDeCuenta}";
                string numeroTarjetaCreada = calcularAlgoritmoDeLuhn(binEIdentificador);
                string sql = "INSERT INTO tarjetas (numero_tarjeta,banco_emisor,dni_titular) VALUES(@numeroTarjetaCreada,@banco,@dni)";
                using var connection = new MySqlConnection(connectionString);
                using var command = new MySqlCommand(sql, connection);
                await connection.OpenAsync();
                command.Parameters.AddWithValue("@numeroTarjetaCreada",numeroTarjetaCreada);
                command.Parameters.AddWithValue("@banco",banco);
                command.Parameters.AddWithValue("@dni",dni);
                int resultado = await command.ExecuteNonQueryAsync();
                return resultado;
            }catch(MySqlException ex)
            {
                Console.WriteLine($"Error en la base de datos: {ex.Message}");
                return 0;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error al emitir la tarjeta: {ex.Message}");
                return 0;
            }
        }
        static async Task ObtenerYMostrarTarjetas()
        {
            // Completar 
            // Ejemplo de impresión dentro del bucle: 
            // Console.WriteLine("{0,-12} {1,-18} {2,-20} {3,-15}", reader["num_cuenta"], reader["numero_tarjeta"], ...);
            const string sql = "SELECT * FROM tarjetas;";
            using var connection = new MySqlConnection(connectionString);
            using var command = new MySqlCommand(sql, connection);

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                Console.WriteLine($"\nNumero de cuenta: {reader["num_cuenta"]}\n"+
                                  $"Numero de tarjeta: {reader["numero_tarjeta"]}\n"+
                                  $"Banco emisor: {reader["banco_emisor"]}\n"+
                                  $"Estado: {reader["estado"]}\n"+
                                  $"Saldo: {reader["saldo"]}\n"+
                                  $"Dni titular: {reader["dni_titular"]}\n"
                                  );
            }

        }

        static async Task MostrarDetalleCompleto(int cuenta)
        {
            // Completar haciendo un SELECT con JOIN de usuarios y tarjetas WHERE num_cuenta = @cuenta
                        // === A realizar ===
            // Aquí deben realizar un SELECT con un JOIN entre 'tarjetas' y 'usuarios' 
            // filtrando por el numCuenta para traer todos los campos (Nombre, Apellido, Email, Saldo, etc.)
            const string sql = "SELECT * FROM tarjetas t " + 
                               "JOIN usuarios u " +
                               "ON t.dni_titular = u.documento " +
                               "WHERE t.num_cuenta = @cuenta";
            
            using var connection = new MySqlConnection(connectionString);
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@cuenta", cuenta);
            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                Console.WriteLine("\nDATOS DE USUARIO\n");
                Console.WriteLine($"{reader["tipo_doc"]}: {reader["documento"]}\n" +
                                  $"Nombre: {reader["nombre"]}\n" +
                                  $"Apellido: {reader["apellido"]}\n" +
                                  $"Fecha de nacimiento: {reader["fecha_nacimiento"]}\n" +
                                  $"Email: {reader["email"]}\n" +
                                  $"Usuario: {reader["usuario"]}\n" +
                                  $"Password: {reader["password"]}\n" +
                                  $"Creado el: {reader["creado_el"]}\n");
                Console.WriteLine("DATOS DE TARJETA\n");
                Console.WriteLine($"Numero de cuenta: {reader["num_cuenta"]}\n" +
                                  $"Numero de tarjeta: {reader["numero_tarjeta"]}\n" +
                                  $"Banco emisor: {reader["banco_emisor"]}\n" +
                                  $"Estado: {reader["estado"]}\n" +
                                  $"Saldo: {reader["saldo"]}\n" +
                                  $"DNI titular: {reader["dni_titular"]}");
            }
            else
            {
                Console.WriteLine("NO SE ENCONTRARON COINCIDENCIAS");
            }

        }

        static async Task<bool> DarDeBajaTarjeta(int cuenta)
        {
            // === A realizar ===
                // Aquí deben ejecutar un DELETE sobre la tabla 'tarjetas' donde num_cuenta = numCuenta.
                // Como definimos ON DELETE CASCADE en la base de datos, las liquidaciones se borrarán solas.
                // Opcional: Evaluar si también eliminan al usuario de la tabla 'usuarios' o si lo mantienen.
            // Completar usando un DELETE FROM tarjetas WHERE num_cuenta = @cuenta
            string sql = "DELETE FROM tarjetas WHERE num_cuenta = @cuenta";
            using var connection = new MySqlConnection(connectionString);
            using var command = new MySqlCommand(sql,connection);
            command.Parameters.AddWithValue("@cuenta", cuenta);

            await connection.OpenAsync();

            int filasAfectadas = await command.ExecuteNonQueryAsync();

            return filasAfectadas > 0;
        }

        // =========================================================================
        // MÉTODOS UTILS
        // =========================================================================
        private static string generarIdentificadorDeCuenta(int longitud = 11)
        {
            var constructorTexto = new StringBuilder(longitud);
            for (int i = 0; i < longitud; i++)
            {
                int digitoAleatorio = Random.Shared.Next(0,10);
                constructorTexto.Append(digitoAleatorio);
            }
            return constructorTexto.ToString();
        }
        private static string calcularAlgoritmoDeLuhn(string binEIdentificador)
        {
            int acumulador = 0;
            int aux = 0;
            int multiplo = 0;
            int multiplicador = 0;
            int luhn;
            string numeroTarjeta;
            for(int i = binEIdentificador.Length - 1; i >= 0; i--)
            {
                if ( i % 2 == 0 )
                {
                    aux = acumulador + (int.Parse(binEIdentificador[i].ToString()) * 2);
                    if (aux > 9)
                    {
                        aux = aux - 9;
                        acumulador += aux;
                    }
                    else
                    {
                        acumulador += aux;
                    }
                }
                else
                {
                    acumulador += int.Parse(binEIdentificador[i].ToString());
                }
            }
            while(acumulador > multiplo)
            {
                multiplicador++;
                multiplo = 10 * multiplicador;
            }
            luhn = multiplo - acumulador;
            numeroTarjeta = $"{binEIdentificador}{luhn.ToString()}";
            return numeroTarjeta;
        }
    }
}