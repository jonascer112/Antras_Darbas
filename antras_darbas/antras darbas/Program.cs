using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

class Program
{
    static string Encrypt(string plaintext, byte[] secret_key, CipherMode mode)
    {
        int pad = 16 - plaintext.Length % 16;
        plaintext = plaintext + new string((char)pad, pad);

        using (Aes aes = Aes.Create())
        {
            aes.Key = secret_key;
            aes.Mode = mode;

            ICryptoTransform encryptor = aes.CreateEncryptor();
            byte[] encrypted = encryptor.TransformFinalBlock(Encoding.UTF8.GetBytes(plaintext), 0, plaintext.Length);

            return Convert.ToBase64String(encrypted);
        }
    }

    static string Decrypt(string ciphertext, byte[] secret_key, CipherMode mode)
    {
        byte[] decoded = Convert.FromBase64String(ciphertext);

        using (Aes aes = Aes.Create())
        {
            aes.Key = secret_key;
            aes.Mode = mode;

            ICryptoTransform decryptor = aes.CreateDecryptor();
            byte[] decrypted = decryptor.TransformFinalBlock(decoded, 0, decoded.Length);

            return Encoding.UTF8.GetString(decrypted).TrimEnd('\0');
        }
    }

    static void Main(string[] args)
    {
        Console.Write("Enter plaintext: ");
        string plaintext = Console.ReadLine();

        Console.Write("Enter secret key: ");
        byte[] secret_key = Encoding.UTF8.GetBytes(Console.ReadLine());

        Console.Write("Select mode (ECB, CBC, CFB): ");
        string mode_str = Console.ReadLine().ToUpper();
        CipherMode mode;
        switch (mode_str)
        {
            case "ECB":
                mode = CipherMode.ECB;
                break;
            case "CBC":
                mode = CipherMode.CBC;
                break;
            case "CFB":
                mode = CipherMode.CFB;
                break;
            default:
                Console.WriteLine("Invalid mode");
                return;
        }

        Console.Write("Encrypt or decrypt (E/D): ");
        string encrypt_or_decrypt_str = Console.ReadLine().ToUpper();

        if (encrypt_or_decrypt_str == "E")
        {
            string ciphertext = Encrypt(plaintext, secret_key, mode);

            Console.Write("Save to file (Y/N): ");
            string save_str = Console.ReadLine().ToUpper();
            if (save_str == "Y")
            {
                Console.Write("Enter file path: ");
                string file_path = Console.ReadLine();

                File.WriteAllText(file_path, ciphertext);
            }

            Console.WriteLine("Ciphertext: " + ciphertext);
        }
        else if (encrypt_or_decrypt_str == "D")
        {
            Console.Write("Read from file (Y/N): ");
            string read_str = Console.ReadLine().ToUpper();
            string ciphertext;
            if (read_str == "Y")
            {
                Console.Write("Enter file path: ");
                string file_path = Console.ReadLine();

                ciphertext = File.ReadAllText(file_path);
            }
            else
            {
                Console.Write("Enter ciphertext: ");
                ciphertext = Console.ReadLine();
            }

            string plaintext2 = Decrypt(ciphertext, secret_key, mode);
            Console.WriteLine("Plaintext: " + plaintext2);
        }
        else
        {
            Console.WriteLine("Invalid choice");
        }
    }
}