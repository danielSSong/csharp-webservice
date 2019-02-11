using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace NetCore.Utilities.Utils
{
    public static class Common
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="keyPath"></param>
        /// <param name="applicationName"></param>
        /// <param name="cryptotType"></param>
        public static void SetDataProtection(IServiceCollection services, string keyPath, string applicationName, Enum cryptotType)
        {
            var builder = services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(keyPath))
                .SetDefaultKeyLifetime(TimeSpan.FromDays(7))
                .SetApplicationName(applicationName);

            switch(cryptotType)
            {
                case Enums.CryptoType.Unmanaged:
                    //AES
                    //Advanced Encryption Standard
                    //Two-way : encrypt, decrypt
                    builder.UseCryptographicAlgorithms(
                        new AuthenticatedEncryptorConfiguration()
                        {
                            EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
                            //SHA
                            //Secure Hash Algorith
                            //One-way: Encrypt
                            ValidationAlgorithm = ValidationAlgorithm.HMACSHA512
                        });
                    break;
                case Enums.CryptoType.Managed:
                    builder.UseCustomCryptographicAlgorithms(
                        new ManagedAuthenticatedEncryptorConfiguration()
                        {
                            // A type that subclasses SymmetricAlgorithm
                            EncryptionAlgorithmType = typeof(Aes),

                            // Specified in bits
                            EncryptionAlgorithmKeySize = 256,

                            // A type that subclasses KeyedHashAlgorithm
                            ValidationAlgorithmType = typeof(HMACSHA512)
                        });
                    break;
                case Enums.CryptoType.CngCbc:
                    // CNG algorithm using CBC-mode encryption with HMAC validation
                    //CNG algorithm
                    // Cryptography API: Next Generation
                    //CBC-mode
                    // Cipher Block Chaining
                    builder.UseCustomCryptographicAlgorithms(
                        new CngCbcAuthenticatedEncryptorConfiguration()
                        {
                            // Passed to BCryptOpenAlgorithmProvider
                            EncryptionAlgorithm = "AES",
                            EncryptionAlgorithmProvider = null,

                            // Specified in bits
                            EncryptionAlgorithmKeySize = 256,

                            // Passed to BCryptOpenAlgorithmProvider
                            HashAlgorithm = "SHA512",
                            HashAlgorithmProvider = null
                        });
                    break;
                case Enums.CryptoType.CngGcm:
                    //CNG algorithm using Galois/Counter Mode encryption with validation
                    //Galois/Counter Mode 
                    //GCM
                    builder.UseCustomCryptographicAlgorithms(
                        new CngGcmAuthenticatedEncryptorConfiguration()
                        {
                            // Passed to BCryptOpenAlgorithmProvider
                            EncryptionAlgorithm = "AES",
                            EncryptionAlgorithmProvider = null,

                            // Specified in bits
                            EncryptionAlgorithmKeySize = 256
                        });
                    break;
            }
        }
    }
}
