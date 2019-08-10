using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Encrypto {
    public partial class StartUp :Form {
        public StartUp() {
            InitializeComponent();
        }

        private void btnEncrypt_Click(object sender, EventArgs e) {
            new Encrypt().ShowDialog();
        }

        private void btnDecrypt_Click(object sender, EventArgs e) {
            new Decrypt().ShowDialog();
        }
    }
}
