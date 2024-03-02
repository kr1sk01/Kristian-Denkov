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
using System.Diagnostics;
using AuditLogProcessor.Services;
namespace Excel_Convertor_v2
{
    public partial class Form1 : Form
    {
        List<string> chosenPropsToShowList = new List<string>();
        Dictionary<string, string> colNames = new Dictionary<string, string>();
        private TextBox jsonColumnNamesTextBox;

        //Default save directory (excluding outputfile name )
        string defaultDir = Directory.GetCurrentDirectory().ToString() + "\\Outputs";
        //FileName ( full Path including file name and its extension ) for Input file
        string fullFileNamePath = "";
        //Save file path ( only folder )
        string savePathFolder = "";
        //Save file path full path ( including file name and its extension )
        string fullSavePath = $"{Directory.GetCurrentDirectory().ToString()}\\Output_{DateTime.Now:dd_MM_yyyy_HH_mm_ss}.xlsx";

        bool openFinalPathFolder = false;


        public List<string>? jsonColNames = null;

        public Form1()
        {
            InitializeComponent();
            savePathFolder = defaultDir;
            SavePathTextBox.BackColor = Color.White;
            SavePathTextBox.ReadOnly = true;
            SavePathTextBox.Multiline = true;
            SavePathTextBox.Text = Directory.GetCurrentDirectory().ToString() + "\\Output";
        }
        private void AddCheckBoxes(List<string> checkboxNames)
        {
            foreach (string name in checkboxNames)
            {
                ItemToChooseListBox.Items.Add(name);
            }
        }
        private void SelectInputFileButton_Click(object sender, EventArgs e)
        {
            InputFileFullPathLabel.Text = "";
            ItemToChooseListBox.Items.Clear();
            ChosenItemListBox.Items.Clear();
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "xlsx files (*.xlsx)|*.xlsx";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                fullFileNamePath = openFileDialog1.FileName;
                try
                {
                    jsonColNames = UserInput.ConvertStringToList(jsonColumnNamesTextBox.Text);
                    colNames = Read.ReadColTitles(fullFileNamePath, ref jsonColNames);
                    var addCheckBoxesParam = colNames.Keys.ToList();
                    addCheckBoxesParam.Sort();
                    AddCheckBoxes(addCheckBoxesParam);
                    InputFileFullPathLabel.Text = fullFileNamePath;
                    pictureBox1.Hide();
                }
                catch (Exception ex)
                {
                    Log.LogException(ex);
                }
            }
        }
        private void ConvertFileButton_Click(object sender, EventArgs e)//Convert button
        {
            if (fullFileNamePath == "")
            {
                SelectInputFileButton.Focus();
                MessageBox.Show("Моля изберете Excel файл за обработка", "Не сте избрали файл", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                if (ChosenItemListBox.Items.Count <= 0)
                {
                    SelectInputFileButton.Focus();
                    //label1.ForeColor = Color.Red;
                    MessageBox.Show("Изберете кои колони искате да добавите!", "Не сте избрали поле", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else
                {
                    try
                    {
                        fullSavePath = $"{savePathFolder}\\Output_{DateTime.Now:dd_MM_yyyy_HH_mm_ss}.xlsx";
                        chosenPropsToShowList = new List<string>();
                        foreach (var item in ChosenItemListBox.Items)
                        {
                            // Assuming the items are strings, you may need to adjust the type accordingly
                            chosenPropsToShowList.Add(item.ToString());
                        }
                        var rows = Read.ReadData(fullFileNamePath, chosenPropsToShowList, jsonColNames, colNames);

                        if (!Directory.Exists(defaultDir))
                        {
                            // If not, create the directory
                            Directory.CreateDirectory(defaultDir);
                        }
                        Write.WriteData(fullSavePath, rows);
                        
                        openFinalPathFolder = autoOpenDirCheckBox.Checked;
                        if (openFinalPathFolder)
                        {
                            Process.Start("explorer.exe", savePathFolder);
                        }
                        else
                        {
                            MessageBox.Show("Одит лог файла е успешно експортиран в " + savePathFolder, "Успешен експорт", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Нещо се обърка при конвертирането!", "Възникна грешка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Log.LogException(ex);
                    }
                }
            }

        }
        private void button5_Click(object sender, EventArgs e)//Move Down
        {
            MoveItem(-1);
            ItemToChooseListBox.Sorted = true;
            timer2.Stop();
            timer2.Start();
        }
        private void button6_Click(object sender, EventArgs e)//Move Up
        {
            MoveItem(1);
            timer2.Stop();
            timer2.Start();
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
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            ItemToChooseListBox.SelectedItem = null;
            timer1.Stop();
        }
        private void ItemToChooseListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (timer1.Enabled) { timer1.Stop(); }
            timer1.Start();
        }
        private void ChosenItemListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (timer2.Enabled) { timer1.Stop(); }
            timer2.Start();
        }
        private void timer2_Tick(object sender, EventArgs e)
        {
            ChosenItemListBox.SelectedItem = null;
            timer2.Stop();
        }
        private void autoOpenDirCheckBox_CheckedChanged(object sender, EventArgs e)
        {

        }
        private void SaveFileDialogButtom_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();

            // Set the initial directory (optional)
            // saveFileDialog.InitialDirectory = "C:\\";

            // Show the save file dialog
            var result = folderBrowserDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                ;
                // Return the selected directory
                savePathFolder = folderBrowserDialog.SelectedPath.ToString();
                ;

            }
            SavePathTextBox.Text = savePathFolder;
        }
    }
}
