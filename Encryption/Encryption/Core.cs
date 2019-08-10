using System;

namespace Infinitum.Encryption {
    public class Error {
        public void Message(string type, string message, string stacktrace = "") {
            Console.WriteLine(type + " Error - " + message);
            System.Environment.Exit(0);
        }
    }

    public class Core {
        private const string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*+=[]{}|~";

        public string generateString(int count = 16) {
            string _data = string.Empty;
            Random _ran = new Random();

            for(int i=0;i< count;i++) {
                _data += characters[_ran.Next(i, count)];
            }

            return _data;
        }
    }
}