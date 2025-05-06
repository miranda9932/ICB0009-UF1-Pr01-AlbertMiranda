using System;
using System.Text;
using System.Security.Cryptography;
using ClaveSimetricaClass;
using ClaveAsimetricaClass;

namespace SimuladorEnvioRecepcion
{
    class Program
    {   
        static string? UserName;
        static string? SecurePass;  
        static byte[]? Salt; // Variable para almacenar el salt
        static ClaveAsimetrica Emisor = new ClaveAsimetrica();
        static ClaveAsimetrica Receptor = new ClaveAsimetrica();
        static ClaveSimetrica ClaveSimetricaEmisor = new ClaveSimetrica();
        static ClaveSimetrica ClaveSimetricaReceptor = new ClaveSimetrica();

        static string TextoAEnviar = "Me he dado cuenta que incluso las personas que dicen que todo está predestinado y que no podemos hacer nada para cambiar nuestro destino igual miran antes de cruzar la calle. Stephen Hawking.";
        
        static void Main(string[] args)
        {

            /****PARTE 1****/
            //Login / Registro
            Console.WriteLine ("¿Deseas registrarte? (S/N)");
            string? registro = Console.ReadLine();

            if (registro == "S" || registro == "s")
            {
                //Realizar registro del cliente
                Registro();                
            }
            else
            {
                Console.WriteLine("Nota: No se ha realizado registro. Debes registrarte antes de hacer login.");
            }

            //Realizar login
            bool login = Login();

            /***FIN PARTE 1***/

            if (!login)
            {
                Console.WriteLine("No se pudo completar el login. El programa se cerrará.");
                return; // Salir del programa si el login falla
            }

            if (login)
            {                  
                byte[] TextoAEnviar_Bytes = Encoding.UTF8.GetBytes(TextoAEnviar); 
                Console.WriteLine("Texto a enviar bytes: {0}", BytesToStringHex(TextoAEnviar_Bytes));    
                
                //LADO EMISOR

                //Firmar mensaje


                //Cifrar mensaje con la clave simétrica


                //Cifrar clave simétrica con la clave pública del receptor

                //LADO RECEPTOR

                //Descifrar clave simétrica

                
                //Descifrar clave simétrica
 

                //Descifrar mensaje con la clave simétrica


                //Comprobar firma

            }
        }

        public static void Registro()
        {
            Console.WriteLine ("Indica tu nombre de usuario:");
            UserName = Console.ReadLine();
            //Una vez obtenido el nombre de usuario lo guardamos en la variable UserName y este ya no cambiará 

            Console.WriteLine ("Indica tu password:");
            string passwordRegister = Console.ReadLine();
            //Una vez obtenido el passoword de registro debemos tratarlo como es debido para almacenarlo correctamente a la variable SecurePass

            /***PARTE 1***/
            /*Añadir el código para poder almacenar el password de manera segura*/
            
            // Generamos un salt aleatorio
            Salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(Salt);
            }
            
            // Generamos el hash del password usando SHA512 y el salt
            using (var sha512 = SHA512.Create())
            {
                // Convertimos la contraseña a bytes
                byte[] passwordBytes = Encoding.UTF8.GetBytes(passwordRegister);
                
                // Combinamos el salt y la contraseña
                byte[] passwordWithSaltBytes = new byte[passwordBytes.Length + Salt.Length];
                Buffer.BlockCopy(passwordBytes, 0, passwordWithSaltBytes, 0, passwordBytes.Length);
                Buffer.BlockCopy(Salt, 0, passwordWithSaltBytes, passwordBytes.Length, Salt.Length);
                
                // Calculamos el hash
                byte[] hashBytes = sha512.ComputeHash(passwordWithSaltBytes);
                
                // Convertimos el hash a string y lo guardamos
                SecurePass = Convert.ToBase64String(hashBytes);
            }
            
            Console.WriteLine("==============================================");
            Console.WriteLine($"Usuario '{UserName}' registrado correctamente.");
            Console.WriteLine("El password ha sido almacenado de forma segura.");
            Console.WriteLine("==============================================");
        }


        public static bool Login()
        {
            bool auxlogin = false;

            // Verificar si hay un usuario registrado
            if (UserName == null)
            {
                Console.WriteLine("ERROR: No hay ningún usuario registrado. Debes registrarte primero.");
                return false;
            }

            do
            {
                Console.WriteLine ("Acceso a la aplicación");
                Console.WriteLine ("Usuario: ");
                string userName = Console.ReadLine();

                Console.WriteLine ("Password: ");
                string Password = Console.ReadLine();

                /***PARTE 1***/
                /*Modificar esta parte para que el login se haga teniendo en cuenta que el registro se realizó con SHA512 y salt*/
                
                // Verificar el nombre de usuario
                if (userName == UserName)
                {
                    // Verificar la contraseña usando el mismo proceso que en el registro
                    using (var sha512 = SHA512.Create())
                    {
                        // Convertimos la contraseña a bytes
                        byte[] passwordBytes = Encoding.UTF8.GetBytes(Password);
                        
                        // Combinamos con el salt que guardamos durante el registro
                        byte[] passwordWithSaltBytes = new byte[passwordBytes.Length + Salt.Length];
                        Buffer.BlockCopy(passwordBytes, 0, passwordWithSaltBytes, 0, passwordBytes.Length);
                        Buffer.BlockCopy(Salt, 0, passwordWithSaltBytes, passwordBytes.Length, Salt.Length);
                        
                        // Calculamos el hash
                        byte[] hashBytes = sha512.ComputeHash(passwordWithSaltBytes);
                        
                        // Convertimos a string para comparar
                        string hashPassword = Convert.ToBase64String(hashBytes);
                        
                        // Comparamos con el hash guardado
                        if (hashPassword == SecurePass)
                        {
                            Console.WriteLine("Login correcto.");
                            auxlogin = true;
                        }
                        else
                        {
                            Console.WriteLine("Password incorrecto. Inténtalo de nuevo.");
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Usuario incorrecto. El usuario registrado es '{UserName}'. Inténtalo de nuevo.");
                }

            }while (!auxlogin);

            return auxlogin;
        }

        static string BytesToStringHex (byte[] result)
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach (byte b in result)
                stringBuilder.AppendFormat("{0:x2}", b);

            return stringBuilder.ToString();
        }        
    }
}
