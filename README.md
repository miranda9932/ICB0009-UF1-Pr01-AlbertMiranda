# Proyecto de Simulación de Cifrado

Este proyecto implementa diferentes mecanismos de seguridad para la simulación de envío y recepción de mensajes cifrados, incluyendo:

## Características principales

- Registro y login de usuarios con contraseñas seguras (SHA512 + salt)
- Cifrado simétrico (AES) para el contenido de los mensajes
- Cifrado asimétrico (RSA) para intercambio de claves
- Firma digital para verificar la autenticidad de los mensajes
- Simulación completa del proceso de envío y recepción segura

## Estructura del proyecto

- `SimulacionEnvioRecepcion`: Programa principal 
- `ClaveSimetricaClass`: Biblioteca para cifrado simétrico
- `ClaveAsimetricaClass`: Biblioteca para cifrado asimétrico y firma digital

Proyecto desarrollado como práctica para el curso ICB0009-UF1.