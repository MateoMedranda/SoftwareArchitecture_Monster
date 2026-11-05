using System;
using System.Text;

namespace CLICON_Facturacion.Config
{
    public static class AutenticacionMock
    {
        public const string Token = "MONSTER-DEMO-TOKEN";

        public static bool SolicitarInicioSesion()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("╔════════════════════════════════════════════╗");
                Console.WriteLine("║        CELULARES MONSTER · ACCESO          ║");
                Console.WriteLine("╚════════════════════════════════════════════╝");
                Console.WriteLine();
                Console.Write("Usuario: ");
                var usuario = Console.ReadLine();
                Console.Write("Contraseña: ");
                var contrasena = LeerContrasena();
                Console.WriteLine();

                if (usuario == "MONSTER" && contrasena == "MONSTER9") return true;
                Console.WriteLine();
                Console.WriteLine("Credenciales incorrectas. Presione una tecla para intentar nuevamente.");
                Console.ReadKey(true);
            }
        }

        private static string LeerContrasena()
        {
            var texto = new StringBuilder();
            ConsoleKeyInfo tecla;
            while ((tecla = Console.ReadKey(true)).Key != ConsoleKey.Enter)
            {
                if (tecla.Key == ConsoleKey.Backspace && texto.Length > 0)
                {
                    texto.Length--;
                    Console.Write("\b \b");
                }
                else if (!char.IsControl(tecla.KeyChar))
                {
                    texto.Append(tecla.KeyChar);
                    Console.Write("●");
                }
            }
            return texto.ToString();
        }
    }
}
