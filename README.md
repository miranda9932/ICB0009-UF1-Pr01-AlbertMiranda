# Parte #1

El sistema de registro/login implementa un mecanismo de seguridad basado en hashing SHA512 con salt. Durante el registro, se genera un salt aleatorio que se combina con la contraseña antes de aplicar la función hash. En el login, se repite el mismo proceso con el salt almacenado y se compara el hash resultante con el guardado previamente. Este método evita almacenar contraseñas en texto plano y resiste ataques de diccionario y tablas rainbow.

# Parte #2

La simulación de envío y recepción segura implementa un proceso completo de comunicación cifrada entre emisor y receptor siguiendo estos pasos:

## Proceso detallado

### Lado Emisor:

1. **Firma del mensaje original**: 
   - Se toma el mensaje original en formato de bytes
   - Se utiliza la clave privada RSA del emisor para generar una firma digital con SHA512
   - La firma garantiza la autenticidad e integridad del mensaje
   - **Código implementado**: 

Convertimos el texto a bytes para poder firmarlo:

     byte[] TextoAEnviar_Bytes = Encoding.UTF8.GetBytes(TextoAEnviar);
     
    Utilizamos el método FirmarMensaje de la clase ClaveAsimetrica
    Este método utiliza internamente RSA.SignData con SHA512
     Firma = Emisor.FirmarMensaje(TextoAEnviar_Bytes);

   - **Código implementado**: El método `FirmarMensaje` utiliza la clave privada RSA del emisor (accesible solo para él) para generar un hash SHA512 del mensaje y luego cifrar ese hash con su clave privada. Solo alguien con acceso a la clave pública del emisor podrá verificar esta firma.

2. **Cifrado del mensaje con clave simétrica**:
   - Se cifra el mensaje original utilizando el algoritmo AES
   - Se usa la clave simétrica del emisor (Key + IV) para este proceso
   - **Código implementado**: 
Utilizamos el método CifrarMensaje de la clase ClaveSimetrica que recibe el texto en claro y devuelve un array de bytes cifrados
     TextoCifrado = ClaveSimetricaEmisor.CifrarMensaje(TextoAEnviar);

   - **Código implementado**: Este método utiliza el algoritmo AES en modo CBC con la clave (`Key`) y el vector de inicialización (`IV`) generados aleatoriamente en la creación del objeto `ClaveSimetricaEmisor`. Internamente crea un encriptador AES que transforma el texto plano en bytes cifrados mediante un `CryptoStream`.

3. **Cifrado de la clave simétrica**:
   - La clave simétrica (tanto Key como IV) se cifra utilizando la clave pública RSA del receptor
   - Este proceso asegura que solo el receptor pueda descifrar la clave simétrica
   - Se implementa así el intercambio seguro de claves sin necesidad de un canal seguro previo
   - **Código implementado**:

   Ciframos la clave (Key) con la clave pública del receptor para que solo él pueda descifrarla con su clave privada
     ClaveSimetricaKeyCifrada = Emisor.CifrarMensaje(ClaveSimetricaEmisor.Key, Receptor.PublicKey);
     
También ciframos el vector de inicialización (IV) necesario para descifrar
     ClaveSimetricaIVCifrada = Emisor.CifrarMensaje(ClaveSimetricaEmisor.IV, Receptor.PublicKey);

   - **Código implementado**: El método CifrarMensaje con dos parámetros utiliza la clave pública RSA del receptor para cifrar los bytes de la clave simétrica. Usa la función RSA.Encrypt para que solo quien posea la clave privada correspondiente pueda descifrar estos datos.

### Lado Receptor:

1. **Descifrado de la clave simétrica**:
   - El receptor utiliza su clave privada RSA para descifrar la clave simétrica (Key e IV)
   - Se recuperan los componentes originales de la clave simétrica del emisor
   - Se asignan estos valores a la instancia de clave simétrica del receptor
   - **Código implementado**:

Desciframos la Key usando la clave privada del receptor
     byte[] ClaveSimetricaKeyDescifrada = Receptor.DescifrarMensaje(ClaveSimetricaKeyCifrada);
     
Desciframos el IV usando la clave privada del receptor
     byte[] ClaveSimetricaIVDescifrada = Receptor.DescifrarMensaje(ClaveSimetricaIVCifrada);
     
 Asignamos estos valores al objeto de clave simétrica del receptor para poder descifrar el mensaje con los mismos parámetros que usó el emisor
     ClaveSimetricaReceptor.Key = ClaveSimetricaKeyDescifrada;
     ClaveSimetricaReceptor.IV = ClaveSimetricaIVDescifrada;

   - **Codigo utilizado**: El método `DescifrarMensaje` utiliza `RSA.Decrypt` con la clave privada del receptor para recuperar los bytes originales de la clave simétrica. Solo el receptor puede realizar esta operación, ya que es el único que posee la clave privada correspondiente a la clave pública que se usó para cifrar.

2. **Descifrado del mensaje**:
   - Con la clave simétrica recuperada, se descifra el mensaje utilizando AES
   - Se obtiene el mensaje original en texto plano
   - Este paso recupera el contenido confidencial del mensaje
   - **Código implementado**:

Desciframos el mensaje usando la clave simétrica recuperada, el método devuelve directamente el texto descifrado como string
     string MensajeDescifrado = ClaveSimetricaReceptor.DescifrarMensaje(TextoCifrado);
     
Convertimos el mensaje descifrado a bytes para poder verificar la firma
     byte[] MensajeDescifrado_Bytes = Encoding.UTF8.GetBytes(MensajeDescifrado);

   - **Código implementado**: El método `DescifrarMensaje` crea un descifrador AES con la Key e IV recuperados, luego utiliza un `CryptoStream` para transformar los bytes cifrados de vuelta al texto original.
3. **Verificación de la firma**:
   - Se utiliza la clave pública del emisor para verificar la firma
   - Se comprueba que el mensaje descifrado coincide con el mensaje que fue firmado
   - La verificación confirma que el mensaje es auténtico y no ha sido manipulado
   - **Código implementado**:

    Verificamos la firma usando la clave pública del emisor, esto garantiza que el mensaje fue firmado por el emisor y no ha sido alterado
     bool firmaValida = Receptor.ComprobarFirma(Firma, MensajeDescifrado_Bytes, Emisor.PublicKey);
     
Mostramos el resultado de la verificación y el mensaje solo si la firma es válida
     if (firmaValida) {
         Console.WriteLine("La firma es VÁLIDA. El mensaje es auténtico y no ha sido alterado.");
         Console.WriteLine("\nMensaje descifrado: " + MensajeDescifrado);
     } else {
         Console.WriteLine("La firma es INVÁLIDA. El mensaje podría haber sido alterado o no proviene del emisor esperado.");
     }
   - **Código implementado**: El método `ComprobarFirma` utiliza `RSA.VerifyData` con la clave pública del emisor para verificar que la firma corresponda al mensaje descifrado. Verifica que el hash SHA512 del mensaje coincida con el hash descifrado de la firma, lo que confirma la autenticidad e integridad.

# ¿Crees que alguno de los métodos programado en la clase asimétrica se podría eliminar por carecer de una utilidad real?

Tres métodos de la clase `ClaveAsimetrica`:

1. `FirmarMensaje(byte[], RSAParameters)`: Intenta firmar con clave pública, lo cual no es lo mas correcto.

2. `DescifrarMensaje(byte[], RSAParameters)`: Pretende descifrar con clave pública, una operación complicada. 

3. `CifrarMensaje(byte[])`: Cifra con la propia clave pública, cuando lo habitual es cifrar con la clave del destinatario.



