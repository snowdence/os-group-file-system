using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FileManagementCore.Helper
{
    public static class OOHashHelper
    {
        private const int HASH_SIZE = 10;
        private const int ITERATIONS = 100000;
        private static byte[] salt = Encoding.ASCII.GetBytes("OS_PROJECT_Y2_S2");

        public static byte[] getBytes(string password)
        {
            Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, salt, ITERATIONS);
            return pbkdf2.GetBytes(HASH_SIZE);
        }

        public static string getString(string password)
        {
            byte[] bytes = getBytes(password);
            return Encoding.ASCII.GetString(bytes);
        }

        public static string getPasswordThenHash()
        {
            string pass = "";
            while (true)
            {
                ConsoleKeyInfo i = Console.ReadKey(true);
                if (i.Key == ConsoleKey.Enter)
                {
                    break;
                }
                else if (i.Key == ConsoleKey.Backspace)
                {
                    if (pass.Length > 0)
                    {
                        pass = pass.Remove(pass.Length - 1, 1);
                        Console.Write("\b \b");
                    }
                }
                else if (i.KeyChar != '\u0000')
                {
                    pass += i.KeyChar;
                    Console.Write("*");
                }
            }
            return getString(pass);
        }

        public static bool passwordValidation()
        {
            Console.Write("Enter your pass: ");
            string hashed1 = getPasswordThenHash();
            Console.WriteLine($"\nYour hashed: {hashed1} - Length {hashed1.Length}");

            Console.Write("\nReEnter your pass: ");
            string hashed2 = getPasswordThenHash();
            Console.WriteLine($"\nYour rehashed: {hashed2} - Length {hashed2.Length}");

            return hashed1 == hashed2;
        }
    }
}
