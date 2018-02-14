using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace dal.models
{
    public class User
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string Login { get; set; }

        [Required]
        public string Password { get; set; }

        public bool TestPassword(string password)
        {
            return EncryptPassword(password, GetSalt()) == this.Password;
        }

        public void SetPassword(string password)
        {
            this.Password = EncryptPassword(password, GetSalt());
        }

        private string GetSalt()
        {
            return $"{Environment.GetEnvironmentVariable("DB_PASSWORD_SALT")}/{this.Login}/{this.Login.Length}";
        }

        public static string EncryptPassword(string password, string salt)
        {
            var hashMethod = SHA256.Create();

            var hash = hashMethod.ComputeHash(Encoding.UTF8.GetBytes(password + salt));
            return Convert.ToBase64String(hash);
        }
    }
}
