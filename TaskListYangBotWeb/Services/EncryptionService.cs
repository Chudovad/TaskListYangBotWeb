using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TaskListYangBotWeb.Data.Interfaces;
using TaskListYangBotWeb.Models;

namespace TaskListYangBotWeb.Services
{
    class EncryptionService
    {
        public static byte[] EncryptStringToBytes(string plainText, string password)
        {
            // Use derived password to create an AES key.
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(password, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
            byte[] keyBytes = pdb.GetBytes(32);

            // Create an AES encryptor to perform the encryption.
            RijndaelManaged aesAlg = new RijndaelManaged();
            aesAlg.Key = keyBytes;
            aesAlg.IV = pdb.GetBytes(16);

            // Create a memory stream to hold the encrypted data.
            MemoryStream ms = new MemoryStream();

            // Create a CryptoStream to perform the encryption.
            CryptoStream cs = new CryptoStream(ms, aesAlg.CreateEncryptor(), CryptoStreamMode.Write);

            // Write the data and make it do the encryption.
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            cs.Write(plainTextBytes, 0, plainTextBytes.Length);

            // Pad the data with zeroes so it is a multiple of the block size.
            int padding = 16 - plainTextBytes.Length % 16;
            byte[] paddingBytes = new byte[padding];
            Array.Clear(paddingBytes, 0, paddingBytes.Length);
            cs.Write(paddingBytes, 0, paddingBytes.Length);

            // Close the CryptoStream to make the encryption take effect.
            cs.Close();

            // Convert the encrypted data to a byte array.
            byte[] cipherTextBytes = ms.ToArray();
            return cipherTextBytes;
        }

        public static string DecryptStringFromBytes(byte[] cipherText, string password)
        {
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(password, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
            byte[] keyBytes = pdb.GetBytes(32);

            // Create an AES encryptor to perform the decryption.
            RijndaelManaged aesAlg = new RijndaelManaged();
            aesAlg.Key = keyBytes;
            aesAlg.IV = pdb.GetBytes(16);
            aesAlg.Padding = PaddingMode.PKCS7;

            // Create a memory stream to hold the decrypted data.
            MemoryStream ms = new MemoryStream();

            // Create a CryptoStream to perform the decryption.
            CryptoStream cs = new CryptoStream(ms, aesAlg.CreateDecryptor(), CryptoStreamMode.Write);

            // Write the data and make it do the decryption.
            cs.Write(cipherText, 0, cipherText.Length);
            cs.Close();

            // Convert the decrypted data to a string.
            byte[] decryptedBytes = ms.ToArray();
            return Encoding.UTF8.GetString(decryptedBytes).Replace("\0", "");
        }

        public static string GenerateJwt(UserWeb userWeb, IConfiguration _configuration, out CookieOptions cookieOptions)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userWeb.Username),
                new Claim(ClaimTypes.Role, userWeb.Role.RoleName),
            };

            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials);
            cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            };

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
