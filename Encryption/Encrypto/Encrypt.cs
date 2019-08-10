using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Infinitum.Encryption;
using System.IO;

namespace Encrypto {
    public partial class Encrypt :Form {

        private RSA rsa;

        public Encrypt() {
            InitializeComponent();
            if(!BitConverter.IsLittleEndian)
                MessageBox.Show("Actually it's made just for Little Endian -> Little Endian so far", "System", MessageBoxButtons.OK, MessageBoxIcon.Error);

            rsa = new RSA("a", new SettingData() {
                ViewLimit = "1000", DateExpire = "UU/UU/UUUU"
            });
        }

        private void btnEncrypt_Click(object sender, EventArgs e) {
            try {
                if (string.IsNullOrWhiteSpace(file_output) && string.IsNullOrWhiteSpace(file_location) && string.IsNullOrWhiteSpace(password)) {
                    MessageBox.Show("Please fill all the required fields", "System", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                rsa = new RSA(password, new SettingData() {
                    ViewLimit = "1000", DateExpire = "UU/UU/UUUU"
                });

                MemoryStream encryptData = rsa.Encrypt(file_location);

                string new_file = Path.GetFileName(file_location);
                new_file = Path.Combine(file_output, new_file);

                rsa.writeToFile(new_file, encryptData);

                MessageBox.Show("File Encrypted and Saved in Output Location", "System", MessageBoxButtons.OK, MessageBoxIcon.Information);
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "System", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string file_location = string.Empty;
        private string file_output = string.Empty;
        private string password = string.Empty;

        private void btnFile_Click(object sender, EventArgs e) {
            if (ofd.ShowDialog() == DialogResult.OK) {
                file_location = ofd.FileName;
                txtFile.Text = file_location;
            }
        }

        private void btnOutput_Click(object sender, EventArgs e) {
            if(fbd.ShowDialog() == DialogResult.OK) {
                file_output = fbd.SelectedPath;
                txtOutput.Text = file_output;
            }
        }
    }
}
