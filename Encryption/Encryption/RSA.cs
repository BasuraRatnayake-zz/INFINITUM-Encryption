using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Infinitum.Encryption {
    public class RSA {
        protected const string initVector = "D1dfc@D4e5F#g7H8";           // Must be 16 bytes        
        private string hashAlgorithm = "SHA1";              // Can also be "MD5", "SHA1" is stronger
        private int passwordIterations = 4;                 // Can be any number, usually 1 or 2       
        private const int keySize = 256;                          // Allowed values: 192, 128 or 256

        private string passPhrase = string.Empty;               // Any string
        private const string saltValue = "W#]3+oefVyogPb^Z7p+29&S&0[w#fl";               // Any string
        private string randomKeyText = string.Empty;

        private Error error = new Error();

        public Settings appSettings = new Settings();
        public SettingData settingData = new SettingData();

        public RSA(string passPhrase, SettingData settingData, int passwordIterations = 2, string hashAlgorithm = "SHA1") {
            this.passPhrase = passPhrase;
            this.passwordIterations = passwordIterations;
            this.hashAlgorithm = hashAlgorithm;
            this.settingData = settingData;

            //initVector = core.generateString();
        }
        
        private string GetString(byte[] bytes) {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        private byte[] GetBytes(string randomKeyText) {
            byte[] bytes = new byte[randomKeyText.Length * sizeof(char)];
            System.Buffer.BlockCopy(randomKeyText.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public string GetRandomKeyText(string passsword, string salt) {
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(salt);

            PasswordDeriveBytes password = new PasswordDeriveBytes(passsword, saltValueBytes, hashAlgorithm, passwordIterations);
            byte[] keyBytes = password.GetBytes(keySize / 8);

            return GetString(keyBytes);
        }

        public MemoryStream Encrypt(string fileName) {

            FileStream readStream = new FileStream(fileName, FileMode.Open);
            byte[] plainTextBytes = new byte[readStream.Length];
            readStream.Read(plainTextBytes, 0, plainTextBytes.Length);

            /* Handle Settings - Start */
            byte[] dataWithSettings = appSettings.writeSettings(settingData, plainTextBytes);
            /* Handle Settings - End */

            byte[] initVectorBytes = Encoding.UTF8.GetBytes(initVector);
            byte[] saltValueBytes = Encoding.UTF8.GetBytes(saltValue);

            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, saltValueBytes, hashAlgorithm, passwordIterations);

            byte[] keyBytes = password.GetBytes(keySize / 8);
            randomKeyText = GetString(keyBytes);

            RijndaelManaged symmetricKey = new RijndaelManaged();

            symmetricKey.Mode = CipherMode.CBC;

            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);

            MemoryStream memoryStream = new MemoryStream();

            CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);

            cryptoStream.Write(dataWithSettings, 0, dataWithSettings.Length);
            cryptoStream.FlushFinalBlock();

            memoryStream.Seek(0, SeekOrigin.Begin);

            return memoryStream;
        }

        public void writeToFile(string outputFile, MemoryStream memoryStream) {
            using (FileStream fs = new FileStream(outputFile, FileMode.OpenOrCreate)) {
                memoryStream.CopyTo(fs);
                fs.Flush();
            }
            memoryStream.Close();
        }

        public MemoryStream Decrypt(string fileName, string password) {
            FileStream readStream = new FileStream(fileName, FileMode.Open);
            byte[] cipherTextBytes = new byte[readStream.Length];
            readStream.Read(cipherTextBytes, 0, cipherTextBytes.Length);

            string randomKeyText = GetRandomKeyText(password, saltValue);

            byte[] initVectorBytes = Encoding.UTF8.GetBytes(initVector);

            RijndaelManaged symmetricKey = new RijndaelManaged();

            symmetricKey.Mode = CipherMode.CBC;

            byte[] keyBytes = GetBytes(randomKeyText);
            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);

            MemoryStream memoryStream = new MemoryStream(cipherTextBytes);

            CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);

            byte[] plainTextBytes = new byte[cipherTextBytes.Length];

            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);

            /* Settings - Start */
            byte[] dataWithoutSettings = appSettings.readSettings(plainTextBytes);
            /* Settings - End */

            return new MemoryStream(dataWithoutSettings);
        }
    }
}