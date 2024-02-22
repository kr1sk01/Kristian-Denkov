using System.Security;
using System.Windows.Forms;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Text.Json;
using System.Text.Encodings;

namespace Excel_Convertor_v2
{
    public partial class Form1 : Form
    {
        public List<CheckBox> checkBoxList { get; set; }
        private Button selectButton;
        private TextBox textBox1;

        public Form1()
        {
            checkBoxList = new List<CheckBox>();
            InitializeComponent();

        }
        private void SetText(string text)
        {
            textBox1.Text = text;
        }
        private void AddCheckBoxes(HashSet<string> checkboxNames)
        {

            Panel panel = new Panel();
            panel.Location = new Point(10, 100);
            panel.Width = 350;
            panel.Height = 450;
            panel.AutoScroll = true;
            panel.Width = 500;

            // Add checkboxes to the Panel
            

            foreach (string name in checkboxNames)
            {

                CheckBox checkBox = new CheckBox();
                checkBox.Name = name;
                checkBox.Text = name;
                checkBox.AutoSize = true;
                checkBox.Location = new System.Drawing.Point(20, 20 + (checkBox.Height + 5) * panel.Controls.Count);
                panel.Controls.Add(checkBox);
                checkBoxList.Add(checkBox);
            }
            //panel.Controls.AddRange(checkBoxList.ToArray());
            // Add Panel to the form
            Controls.Add(panel);
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "xlsx files (*.xlsx)|*.xlsx";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string fileName = openFileDialog1.FileName;
                try
                {
                    ReadAndWrite rw = new ReadAndWrite();
                    AddCheckBoxes(rw.ReadColTitles(fileName).Result);
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void textBoxLogger_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            var checkBoxChecked = checkBoxList.Where((x) => x.CheckState == CheckState.Checked).Select((x)=>x.Text).ToList();//testa sudurja vsichki checknati boxove
            if (checkBoxChecked.Count == 0)
            {
                MessageBox.Show("Изберете кои колони искате да добавите!", "Не сте избрали поле", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
                ;
        }
    }
}
