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
        SortedSet<string> colNames = new SortedSet<string>();
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
        private void AddCheckBoxes(SortedSet<string> checkboxNames)
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
            int? index = this.ItemToChooseListBox.IndexFromPoint(e.Location);
            if (index != null)
            {
                MessageBox.Show(index.ToString());
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
                    colNames = Read.ReadColTitles(fileName).Result;
                    AddCheckBoxes(Read.ReadColTitles(fileName).Result);
                }
                catch (Exception ex)
                {
                    Log.LogException(ex);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)//Convert button
        {
            if (fileName == "")
            {
                button1.Focus();
                MessageBox.Show("Моля изберете Excel файл за обработка", "Не сте избрали файл", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                if (ChosenItemListBox.Items.Count <= 0)
                {
                    button3.Focus();
                    label1.ForeColor = Color.Red;
                    MessageBox.Show("Изберете кои колони искате да добавите!", "Не сте избрали поле", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else
                {
                    try
                    {
                        chosenPropsToShowList = new List<string>();
                        ChosenItemListBox.Update();
                        foreach (var item in ChosenItemListBox.Items)
                        {
                            // Assuming the items are strings, you may need to adjust the type accordingly
                            chosenPropsToShowList.Add(item.ToString());
                        }
                        var rows = Read.ReadData(fileName,
                           chosenPropsToShowList);
                        if (!Directory.Exists("Outputs"))
                        {
                            // If not, create the directory
                            Directory.CreateDirectory("Outputs");
                        }
                        Write.WriteData($"Outputs\\Output_{DateTime.Now:dd_MM_yyyy_HH_mm_ss}.xlsx", rows);
                        //Log.LogExecutionTime()

                        MessageBox.Show("Вашият обработен Excel файл е готов!", "Output Excel File", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Нещо се обърка при конвертирането!", "Възникна грешка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Log.LogException(ex);
                    }
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
        private void button4_Click(object sender, EventArgs e)//Move Left
        {
            if (ChosenItemListBox.SelectedItem != null)
            {
                ItemToChooseListBox.Items.Add(ChosenItemListBox.SelectedItem);
                ChosenItemListBox.Items.Remove(ChosenItemListBox.SelectedItem);
                ItemToChooseListBox.Sorted = true;
            }
        }//Move Left
        private void button5_Click(object sender, EventArgs e)//Move Down
        {
            MoveItem(-1);
            ItemToChooseListBox.Sorted = true;
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

        private void ItemToChoose_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Clicks == 2)
            {
                // Get the ListBoxItem that was double-clicked
                if (ItemToChooseListBox.SelectedItem != null)
                {
                    ChosenItemListBox.Items.Add(ItemToChooseListBox.SelectedItem);
                    ItemToChooseListBox.Items.Remove(ItemToChooseListBox.SelectedItem);
                }

                // Do something with the double-clicked item
                //MessageBox.Show("You double-clicked: " + item.Content.ToString());
            }
            else
            {
                ItemToChooseListBox.SelectedItem = null;
            }
        }

        private void ChosenItemListBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Clicks == 2)
            {
                // Get the ListBoxItem that was double-clicked
                if (ChosenItemListBox.SelectedItem != null)
                {
                    ItemToChooseListBox.Items.Add(ChosenItemListBox.SelectedItem);
                    ChosenItemListBox.Items.Remove(ChosenItemListBox.SelectedItem);
                    ItemToChooseListBox.Sorted = true;
                }
                

                // Do something with the double-clicked item
                //MessageBox.Show("You double-clicked: " + item.Content.ToString());
            }
            else
            {
                ItemToChooseListBox.SelectedItem = null;
            }

        }
    }
}
