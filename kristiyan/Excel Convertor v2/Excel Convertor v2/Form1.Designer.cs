namespace Excel_Convertor_v2
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            label1 = new Label();
            button1 = new Button();
            button2 = new Button();
            ItemToChooseListBox = new ListBox();
            ChosenItemListBox = new ListBox();
            button5 = new Button();
            button6 = new Button();
            timer1 = new System.Windows.Forms.Timer(components);
            timer2 = new System.Windows.Forms.Timer(components);
            label2 = new Label();
            label3 = new Label();
            autoOpenDirCheckBox = new CheckBox();
            SaveFileDialogButtom = new Button();
            SavePathTextBox = new TextBox();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 13F);
            label1.Location = new Point(6, 26);
            label1.Name = "label1";
            label1.Size = new Size(552, 25);
            label1.TabIndex = 0;
            label1.Text = "Моля изберете екселският файл, който искате да конвертирате ->";
            // 
            // button1
            // 
            button1.Location = new Point(611, 12);
            button1.Name = "button1";
            button1.Size = new Size(195, 56);
            button1.TabIndex = 1;
            button1.Text = "Избери Файл";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Location = new Point(611, 531);
            button2.Name = "button2";
            button2.Size = new Size(195, 56);
            button2.TabIndex = 2;
            button2.Text = "Конвертирай";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // ItemToChooseListBox
            // 
            ItemToChooseListBox.FormattingEnabled = true;
            ItemToChooseListBox.ItemHeight = 15;
            ItemToChooseListBox.Location = new Point(12, 164);
            ItemToChooseListBox.Margin = new Padding(3, 2, 3, 2);
            ItemToChooseListBox.Name = "ItemToChooseListBox";
            ItemToChooseListBox.Size = new Size(249, 424);
            ItemToChooseListBox.TabIndex = 3;
            ItemToChooseListBox.SelectedIndexChanged += ItemToChooseListBox_SelectedIndexChanged;
            ItemToChooseListBox.MouseDoubleClick += ItemToChoose_MouseDoubleClick;
            // 
            // ChosenItemListBox
            // 
            ChosenItemListBox.FormattingEnabled = true;
            ChosenItemListBox.ItemHeight = 15;
            ChosenItemListBox.Location = new Point(292, 164);
            ChosenItemListBox.Margin = new Padding(3, 2, 3, 2);
            ChosenItemListBox.Name = "ChosenItemListBox";
            ChosenItemListBox.Size = new Size(249, 424);
            ChosenItemListBox.TabIndex = 4;
            ChosenItemListBox.SelectedIndexChanged += ChosenItemListBox_SelectedIndexChanged;
            ChosenItemListBox.MouseDoubleClick += ChosenItemListBox_MouseDoubleClick;
            // 
            // button5
            // 
            button5.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            button5.Location = new Point(547, 164);
            button5.Margin = new Padding(3, 2, 3, 2);
            button5.Name = "button5";
            button5.Size = new Size(46, 85);
            button5.TabIndex = 7;
            button5.Text = "🡡";
            button5.UseVisualStyleBackColor = true;
            button5.Click += button5_Click;
            // 
            // button6
            // 
            button6.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            button6.Location = new Point(547, 502);
            button6.Margin = new Padding(3, 2, 3, 2);
            button6.Name = "button6";
            button6.Size = new Size(46, 85);
            button6.TabIndex = 8;
            button6.Text = "🡣";
            button6.UseVisualStyleBackColor = true;
            button6.Click += button6_Click;
            // 
            // timer1
            // 
            timer1.Interval = 350;
            timer1.Tick += timer1_Tick;
            // 
            // timer2
            // 
            timer2.Interval = 3000;
            timer2.Tick += timer2_Tick;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(13, 139);
            label2.Name = "label2";
            label2.Size = new Size(130, 15);
            label2.TabIndex = 9;
            label2.Text = "Колони в Input файла:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(292, 142);
            label3.Name = "label3";
            label3.Size = new Size(140, 15);
            label3.TabIndex = 10;
            label3.Text = "Колони в Output файла:";
            // 
            // autoOpenDirCheckBox
            // 
            autoOpenDirCheckBox.AutoSize = true;
            autoOpenDirCheckBox.Location = new Point(611, 506);
            autoOpenDirCheckBox.Name = "autoOpenDirCheckBox";
            autoOpenDirCheckBox.Size = new Size(195, 19);
            autoOpenDirCheckBox.TabIndex = 11;
            autoOpenDirCheckBox.Text = "Авт. отваряне на директорията";
            autoOpenDirCheckBox.UseVisualStyleBackColor = true;
            autoOpenDirCheckBox.CheckedChanged += autoOpenDirCheckBox_CheckedChanged;
            // 
            // SaveFileDialogButtom
            // 
            SaveFileDialogButtom.Location = new Point(611, 77);
            SaveFileDialogButtom.Name = "SaveFileDialogButtom";
            SaveFileDialogButtom.Size = new Size(195, 56);
            SaveFileDialogButtom.TabIndex = 12;
            SaveFileDialogButtom.Text = "Избери Output Директория";
            SaveFileDialogButtom.UseVisualStyleBackColor = true;
            SaveFileDialogButtom.Click += SaveFileDialogButtom_Click;
            // 
            // SavePathTextBox
            // 
            SavePathTextBox.Location = new Point(13, 77);
            SavePathTextBox.Multiline = true;
            SavePathTextBox.Name = "SavePathTextBox";
            SavePathTextBox.Size = new Size(529, 56);
            SavePathTextBox.TabIndex = 13;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(823, 599);
            Controls.Add(SavePathTextBox);
            Controls.Add(SaveFileDialogButtom);
            Controls.Add(autoOpenDirCheckBox);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(button6);
            Controls.Add(button5);
            Controls.Add(ChosenItemListBox);
            Controls.Add(ItemToChooseListBox);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(label1);
            Name = "Form1";
            Text = "Ексел Конвертор";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Button button1;
        private Button button2;
        private ListBox ItemToChooseListBox;
        private ListBox ChosenItemListBox;
        private Button button5;
        private Button button6;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Timer timer2;
        private Label label2;
        private Label label3;
        private CheckBox autoOpenDirCheckBox;
        private Button SaveFileDialogButtom;
        private TextBox SavePathTextBox;
    }
}
