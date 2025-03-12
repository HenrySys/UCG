using System;
using System.Security.Cryptography;
using System.Text;


namespace UCG.Services
{
    public class HashingService
    {
        private const int SaltSize = 128 / 8; //tamaño en bytes del salt
        private const int KeySize = 256 / 8; //tamaño en bytes de la llave
        private const int Iterations = 10000; //número de iteraciones


        public string GenerateHash(string password)
        {
            // se genera un salt aleatorio
            var salt = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // se genera el hash usando el password y el salt
            using (var rfc2898 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
            {
                var hash = rfc2898.GetBytes(KeySize);
                // se combinal el salt y el hash en un solo array
                var hashBytes = new byte[SaltSize + KeySize];
                Array.Copy(salt, 0, hashBytes, 0, SaltSize);
                Array.Copy(hash, 0, hashBytes, SaltSize, KeySize);

                // se convierte a base64 para almacenar en la base de datos
                return Convert.ToBase64String(hashBytes);
            }
        }

        // método para verificar si el password ingresado es correcto
        public bool VeryfyHash(string password, string storedHash)
        {
            var hashBytes = Convert.FromBase64String(storedHash);

            // se extrae el salt del hash almacenado
            var salt = new byte[SaltSize];
            Array.Copy(hashBytes, 0, salt, 0, SaltSize);

            // se genera el hash con el password ingresado y el salt almacenado
            using (var rfc2898 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
            {
                var hash = rfc2898.GetBytes(KeySize);

                // se compara el hash generado con el hash almacenado
                for (int i = 0; i < KeySize; i++)
                {
                    if (hashBytes[i + SaltSize] != hash[i])
                    {
                        return false;
                    }
                }
                return true;
            }
        }
    }
}
