using System.Security;
using System.Windows.Forms;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Text.Json;
using System.Text.Encodings;
using Excel_Convertor_v2.Services;
using Microsoft.VisualBasic;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace Excel_Convertor_v2
{
    public partial class Form1 : Form
    {
        List<string> chosenPropsToShowList = new List<string>();

        private TextBox textBox1;

        string fileName = "";
        public Form1()
        {
            InitializeComponent();
        }
        private void SetText(string text)
        {
            textBox1.Text = text;
        }
        private void AddCheckBoxes(HashSet<string> checkboxNames)
        {
            foreach (string name in checkboxNames)
            {

                ItemToChooseListBox.Items.Add(name);
            }
            label1.Text = "Моля изберете колоните които искате да включите в експорта: ";
        }
        private void ItemToChooseListBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // Check if the double click occurred on an item
            int index = ItemToChooseListBox.IndexFromPoint(e.Location);
            if (index != ListBox.NoMatches)
            {
                // Handle the double click event on the item
                MessageBox.Show($"Double clicked on item: {ItemToChooseListBox.Items[index]}");
            }
        }
        private void textBoxLogger_TextChanged(object sender, EventArgs e)
        {

        }
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "xlsx files (*.xlsx)|*.xlsx";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                fileName = openFileDialog1.FileName;
                try
                {
                    AddCheckBoxes(Read.ReadColTitles(fileName).Result);
                }
                catch (Exception ex)
                {

                }
            }
        }
        private void button2_Click(object sender, EventArgs e)//Convert button
        {
            if (ChosenItemListBox.Items.Count <= 0)
            {
                label1.ForeColor = Color.Red;
                MessageBox.Show("Изберете кои колони искате да добавите!", "Не сте избрали поле", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else
            {
                foreach (object item in ChosenItemListBox.Items)
                {
                    chosenPropsToShowList.Add(item.ToString());
                }
            }
        }
        private void button3_Click(object sender, EventArgs e)//Move Right
        {
            if (ItemToChooseListBox.SelectedItem != null)
            {
                ChosenItemListBox.Items.Add(ItemToChooseListBox.SelectedItem);
                ItemToChooseListBox.Items.Remove(ItemToChooseListBox.SelectedItem);
            }

        }
        private void button4_Click(object sender, EventArgs e)
        {
            if (ChosenItemListBox.SelectedItem != null)
            {
                ItemToChooseListBox.Items.Add(ChosenItemListBox.SelectedItem);
                ChosenItemListBox.Items.Remove(ChosenItemListBox.SelectedItem);
            }
        }//Move Left
        private void button5_Click(object sender, EventArgs e)//Move Down
        {
            MoveItem(-1);
        }
        private void button6_Click(object sender, EventArgs e)//Move Up
        {
            MoveItem(1);
        }
        private void MoveItem(int direction)
        {
            // Check if an item is selected and if it can be moved
            if (ChosenItemListBox.SelectedItem == null || ChosenItemListBox.SelectedIndex + direction < 0 ||
                ChosenItemListBox.SelectedIndex + direction >= ChosenItemListBox.Items.Count)
                return;

            // Swap the selected item with the item above or below it
            int newIndex = ChosenItemListBox.SelectedIndex + direction;
            object selectedItem = ChosenItemListBox.SelectedItem;
            ChosenItemListBox.Items.RemoveAt(ChosenItemListBox.SelectedIndex);
            ChosenItemListBox.Items.Insert(newIndex, selectedItem);
            ChosenItemListBox.SelectedIndex = newIndex;
        }//Actual move void
    }
}
