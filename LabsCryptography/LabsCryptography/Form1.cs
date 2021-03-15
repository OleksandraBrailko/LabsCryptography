using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LabsCryptography
{
    public partial class Form1 : MaterialForm
    {
        static string textFromOriginalFile;
        static string key;
        public Form1()
        {
            InitializeComponent();
            //key = textBox1.Text;
            MaterialSkinManager materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;

            // Configure color schema
            materialSkinManager.ColorScheme = new ColorScheme(
                Primary.Indigo500, Primary.Indigo600,
                Primary.Indigo600, Accent.Indigo200,
                TextShade.WHITE
                );
         
        }

        private void materialFlatButton1_Click(object sender, EventArgs e)
        {          
            DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                string file = openFileDialog1.FileName;

                textFromOriginalFile = File.ReadAllText(file);
            }
        }



        private void materialRaisedButton1_Click(object sender, EventArgs e)
        {
            key = materialSingleLineTextField1.Text;
            if (key!= null && textFromOriginalFile!= null)
            {
               
                string executableLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var alph = new Alphabet();
                var ciphered = "";
                var obj1 = new OneTimePad(textFromOriginalFile, key);
                var obj2 = new Vigener(textFromOriginalFile, key, alph);
                var obj3 = new AffineCipher(textFromOriginalFile, alph);

                var rsaKeys = new RSAKeys();

                var obj4 = new RSA(textFromOriginalFile, rsaKeys.GetRSAKeys[0]);

                //var rabinKeys = new RabinKeys();

                //var obj5 = new RabinCipher(textFromOriginalFile, rabinKeys.GetRabinKeys[0]);

                var elHamalKeys = new ElHamalKey();

                var obj6 = new ElHamalCipher(textFromOriginalFile, elHamalKeys.GetElHamalKeys[0]);


                if (materialRadioButton1.Checked == true)
                {
                    ciphered = obj1.Encrypt();
                    materialSingleLineTextField1.Enabled = true;
                    materialLabel1.Visible = false;
                }
                else if (materialRadioButton2.Checked == true)
                {
                    ciphered = obj2.Encrypt();
                    materialSingleLineTextField1.Enabled = true;
                    materialLabel1.Visible = false;
                }
                else if (materialRadioButton3.Checked == true)
                {
                    ciphered = obj3.Encrypt();
                    materialLabel1.Text = "Ключ("+obj3.GetKey.Key+";"+obj3.GetKey.Value+")";
                    materialSingleLineTextField1.Enabled = false;
                }
                else if (materialRadioButton4.Checked == true)
                {
                    ciphered = obj4.Encrypt();

                    File.WriteAllText(Path.Combine(executableLocation, "RSAkeys.txt"), string.Join("\n", rsaKeys.GetRSAKeys.Select(x => x)));

                    //var a = File.ReadAllText(Path.Combine(executableLocation, "RSAkeys.txt")).Trim(']', '[').Split('\n').Select(x => x.Trim(']', '[')).ToList();

               
                    materialLabel1.Text = "Ключ(" + obj4.GetKey.Key + ";" + obj4.GetKey.Value + ")";
                    materialSingleLineTextField1.Enabled = false;
                }
                else if (materialRadioButton5.Checked == true)
                {
                    ciphered = obj6.Encrypt();

                    File.WriteAllText(Path.Combine(executableLocation, "ElhamalKeys.txt"), string.Join("\n", elHamalKeys.GetElHamalKeys.Select(x => string.Join(";", x.ToArray()))));



                    materialLabel1.Text = "Ключ(" + obj6.GetKey[0] + ";" + obj6.GetKey[1] +";"+ obj6.GetKey[2] +")";
                    materialSingleLineTextField1.Enabled = false;
                }


                textBox2.Text = textFromOriginalFile;
                materialLabel2.Text = "Початковий текст";

                File.WriteAllText(Path.Combine(executableLocation, "CipheredText.txt"), ciphered);
                textBox1.Text = File.ReadAllText(Path.Combine(executableLocation, "CipheredText.txt"));
                materialLabel3.Text = "Зашифрований текст";
            }
            else 
            {
                MessageBox.Show("Введіть ключ та оберіть файл для шифрування даних");
            }
        }

        private void materialRaisedButton2_Click(object sender, EventArgs e)
        {

            key = materialSingleLineTextField1.Text;
            string executableLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var alph = new Alphabet();
            var textFromCipheredFile = File.ReadAllText((Path.Combine(executableLocation, "CipheredText.txt")));
            materialLabel2.Text = "Зашифрований текст";
            textBox2.Text = textFromCipheredFile;

            var obj1 = new OneTimePad(textFromCipheredFile, key);
            var obj2 = new Vigener(textFromCipheredFile, key, alph);
            var obj3 = new AffineCipher(textFromCipheredFile, alph);

            var temp1 = File.ReadAllText(Path.Combine(executableLocation, "RSAkeys.txt")).Trim(']', '[').Split('\n').Select(x => x.Trim(']', '[').Split(',')).ToList();
            var obj4 = new RSA(textFromCipheredFile, new KeyValuePair<int, int>(Convert.ToInt32(temp1[1][0]), Convert.ToInt32(temp1[1][1])));

            ////var temp2 = File.ReadAllText(Path.Combine(executableLocation, "Rabinkeys.txt")).Trim(']', '[').Split('\n').Select(x => x.Trim(']', '[').Split(',')).ToList();
            ////var obj5 = new RabinCipher(textFromCipheredFile, new KeyValuePair<int, int>(Convert.ToInt32(temp2[1][0]), Convert.ToInt32(temp2[1][1])));

            var temp3 = File.ReadAllText(Path.Combine(executableLocation, "ElhamalKeys.txt")).Split('\n').ToList().Select(x => x.Split(';').ToList()).ToList()
                .Select(x => x.Select(y => int.Parse(y)).ToList()).ToList();
            var obj6 = new ElHamalCipher(textFromCipheredFile, temp3[1]);

            var decrypted = "";

            if (materialRadioButton1.Checked == true)
            {
                decrypted = obj1.Decrypt();
            }
            else if (materialRadioButton2.Checked == true)
            {
                decrypted = obj2.Decrypt();
            }
            else if (materialRadioButton3.Checked == true)
            {
                decrypted = obj3.Decrypt();
            }

            else if (materialRadioButton4.Checked == true)
            {
                decrypted = obj4.Decrypt();
                materialLabel1.Text = "Ключ(" + obj4.GetKey.Key + ";" + obj4.GetKey.Value + ")";
            }

            else if (materialRadioButton5.Checked == true)
            {
                decrypted = obj6.Decrypt(temp3[0]);
            }

            File.WriteAllText(Path.Combine(executableLocation, "DecryptedText.txt"), decrypted);
            textBox1.Text = File.ReadAllText(Path.Combine(executableLocation, "DecryptedText.txt"));
            materialLabel3.Text = "Розшифрований текст";
        }
    }
}
