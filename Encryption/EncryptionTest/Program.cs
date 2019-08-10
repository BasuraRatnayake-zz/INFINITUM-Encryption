using Infinitum.Encryption;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncryptionTest {
    class Program {
        private static Error error = new Error();

        static void Main(string[] args) {
            if (!BitConverter.IsLittleEndian)
                error.Message("System", "Actually it's made just for Little Endian -> Little Endian so far");

            string password = "uKl[egT|so";

            RSA rsa = new RSA(password, new SettingData() {
                ViewLimit = "1000", DateExpire = "UU/UU/UUUU"
            });

            MemoryStream encryptData = rsa.Encrypt("316qi2t.jpg");

            rsa.writeToFile("316qi2t_e.jpg", encryptData);


            MemoryStream decryptData = rsa.Decrypt("316qi2t_e.jpg", password);

            rsa.writeToFile("316qi2t_d.jpg", decryptData);

            Console.WriteLine();
        }
    }
}
