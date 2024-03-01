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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
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
            groupbox1 = new GroupBox();
            jsonColumnNamesTextBox = new TextBox();
            jsonColumnNamesLabel = new Label();
            pictureBox1 = new PictureBox();
            splitContainer1 = new SplitContainer();
            groupBox2 = new GroupBox();
            panel3 = new Panel();
            panel1 = new Panel();
            panel2 = new Panel();
            groupBox3 = new GroupBox();
            panel4 = new Panel();
            panel5 = new Panel();
            panel6 = new Panel();
            groupbox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            groupBox2.SuspendLayout();
            panel3.SuspendLayout();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            groupBox3.SuspendLayout();
            panel4.SuspendLayout();
            panel5.SuspendLayout();
            panel6.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F);
            label1.Location = new Point(284, 53);
            label1.Name = "label1";
            label1.Size = new Size(0, 21);
            label1.TabIndex = 0;
            // 
            // button1
            // 
            button1.Location = new Point(12, 65);
            button1.Name = "button1";
            button1.Size = new Size(219, 56);
            button1.TabIndex = 1;
            button1.Text = "Изберете входящ файл";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Location = new Point(26, 28);
            button2.Name = "button2";
            button2.Size = new Size(219, 56);
            button2.TabIndex = 2;
            button2.Text = "Експорт";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // ItemToChooseListBox
            // 
            ItemToChooseListBox.Dock = DockStyle.Fill;
            ItemToChooseListBox.FormattingEnabled = true;
            ItemToChooseListBox.ItemHeight = 15;
            ItemToChooseListBox.Location = new Point(0, 0);
            ItemToChooseListBox.Margin = new Padding(3, 2, 3, 2);
            ItemToChooseListBox.Name = "ItemToChooseListBox";
            ItemToChooseListBox.Size = new Size(317, 355);
            ItemToChooseListBox.TabIndex = 3;
            ItemToChooseListBox.SelectedIndexChanged += ItemToChooseListBox_SelectedIndexChanged;
            ItemToChooseListBox.MouseDoubleClick += ItemToChoose_MouseDoubleClick;
            // 
            // ChosenItemListBox
            // 
            ChosenItemListBox.Dock = DockStyle.Fill;
            ChosenItemListBox.FormattingEnabled = true;
            ChosenItemListBox.ItemHeight = 15;
            ChosenItemListBox.Location = new Point(0, 0);
            ChosenItemListBox.Margin = new Padding(3, 2, 3, 2);
            ChosenItemListBox.Name = "ChosenItemListBox";
            ChosenItemListBox.Size = new Size(444, 355);
            ChosenItemListBox.TabIndex = 4;
            ChosenItemListBox.SelectedIndexChanged += ChosenItemListBox_SelectedIndexChanged;
            ChosenItemListBox.MouseDoubleClick += ChosenItemListBox_MouseDoubleClick;
            // 
            // button5
            // 
            button5.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button5.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            button5.Location = new Point(10, 0);
            button5.Margin = new Padding(3, 2, 3, 2);
            button5.Name = "button5";
            button5.Size = new Size(70, 105);
            button5.TabIndex = 7;
            button5.Text = "🡡";
            button5.UseVisualStyleBackColor = true;
            button5.Click += button5_Click;
            // 
            // button6
            // 
            button6.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            button6.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            button6.Location = new Point(10, 240);
            button6.Margin = new Padding(3, 2, 3, 2);
            button6.Name = "button6";
            button6.Size = new Size(70, 105);
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
            label2.Location = new Point(83, 14);
            label2.Name = "label2";
            label2.Size = new Size(91, 15);
            label2.TabIndex = 9;
            label2.Text = "Всички колони";
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label3.AutoSize = true;
            label3.Location = new Point(517, 14);
            label3.Name = "label3";
            label3.Size = new Size(110, 15);
            label3.TabIndex = 10;
            label3.Text = "Колони за експорт";
            // 
            // autoOpenDirCheckBox
            // 
            autoOpenDirCheckBox.AutoSize = true;
            autoOpenDirCheckBox.Checked = true;
            autoOpenDirCheckBox.CheckState = CheckState.Checked;
            autoOpenDirCheckBox.Location = new Point(276, 28);
            autoOpenDirCheckBox.Name = "autoOpenDirCheckBox";
            autoOpenDirCheckBox.Size = new Size(195, 19);
            autoOpenDirCheckBox.TabIndex = 11;
            autoOpenDirCheckBox.Text = "Авт. отваряне на директорията";
            autoOpenDirCheckBox.UseVisualStyleBackColor = true;
            autoOpenDirCheckBox.CheckedChanged += autoOpenDirCheckBox_CheckedChanged;
            // 
            // SaveFileDialogButtom
            // 
            SaveFileDialogButtom.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            SaveFileDialogButtom.Location = new Point(804, 55);
            SaveFileDialogButtom.Name = "SaveFileDialogButtom";
            SaveFileDialogButtom.Size = new Size(35, 29);
            SaveFileDialogButtom.TabIndex = 12;
            SaveFileDialogButtom.Text = "...";
            SaveFileDialogButtom.UseVisualStyleBackColor = true;
            SaveFileDialogButtom.Click += SaveFileDialogButtom_Click;
            // 
            // SavePathTextBox
            // 
            SavePathTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            SavePathTextBox.Font = new Font("Segoe UI", 12F);
            SavePathTextBox.Location = new Point(276, 55);
            SavePathTextBox.Multiline = true;
            SavePathTextBox.Name = "SavePathTextBox";
            SavePathTextBox.Size = new Size(523, 30);
            SavePathTextBox.TabIndex = 13;
            // 
            // groupbox1
            // 
            groupbox1.Controls.Add(jsonColumnNamesTextBox);
            groupbox1.Controls.Add(jsonColumnNamesLabel);
            groupbox1.Controls.Add(pictureBox1);
            groupbox1.Controls.Add(button1);
            groupbox1.Controls.Add(label1);
            groupbox1.Dock = DockStyle.Fill;
            groupbox1.Location = new Point(0, 0);
            groupbox1.Margin = new Padding(3, 2, 3, 2);
            groupbox1.Name = "groupbox1";
            groupbox1.Padding = new Padding(3, 2, 3, 2);
            groupbox1.Size = new Size(861, 134);
            groupbox1.TabIndex = 14;
            groupbox1.TabStop = false;
            groupbox1.Text = "Стъпка 1 ";
            // 
            // jsonColumnNamesTextBox
            // 
            jsonColumnNamesTextBox.Location = new Point(12, 38);
            jsonColumnNamesTextBox.Name = "jsonColumnNamesTextBox";
            jsonColumnNamesTextBox.Size = new Size(660, 23);
            jsonColumnNamesTextBox.TabIndex = 4;
            // 
            // jsonColumnNamesLabel
            // 
            jsonColumnNamesLabel.AutoSize = true;
            jsonColumnNamesLabel.Location = new Point(12, 20);
            jsonColumnNamesLabel.Name = "jsonColumnNamesLabel";
            jsonColumnNamesLabel.Size = new Size(416, 15);
            jsonColumnNamesLabel.TabIndex = 3;
            jsonColumnNamesLabel.Text = "Моля въведете json, като ги разделите със запетая ПРИМЕР: col1,col2,col3 ";
            // 
            // pictureBox1
            // 
            pictureBox1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(684, 20);
            pictureBox1.Margin = new Padding(3, 2, 3, 2);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(165, 110);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 2;
            pictureBox1.TabStop = false;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Margin = new Padding(3, 2, 3, 2);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(ItemToChooseListBox);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(ChosenItemListBox);
            splitContainer1.Size = new Size(765, 355);
            splitContainer1.SplitterDistance = 317;
            splitContainer1.TabIndex = 15;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(panel3);
            groupBox2.Controls.Add(panel1);
            groupBox2.Controls.Add(panel2);
            groupBox2.Dock = DockStyle.Fill;
            groupBox2.Location = new Point(0, 0);
            groupBox2.Margin = new Padding(3, 2, 3, 2);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new Padding(3, 2, 3, 2);
            groupBox2.Size = new Size(861, 414);
            groupBox2.TabIndex = 4;
            groupBox2.TabStop = false;
            groupBox2.Text = "Стъпка 2";
            // 
            // panel3
            // 
            panel3.Controls.Add(splitContainer1);
            panel3.Dock = DockStyle.Fill;
            panel3.Location = new Point(3, 57);
            panel3.Margin = new Padding(3, 2, 3, 2);
            panel3.Name = "panel3";
            panel3.Size = new Size(765, 355);
            panel3.TabIndex = 18;
            // 
            // panel1
            // 
            panel1.Controls.Add(button6);
            panel1.Controls.Add(button5);
            panel1.Dock = DockStyle.Right;
            panel1.Location = new Point(768, 57);
            panel1.Margin = new Padding(3, 2, 3, 2);
            panel1.Name = "panel1";
            panel1.Size = new Size(90, 355);
            panel1.TabIndex = 16;
            // 
            // panel2
            // 
            panel2.Controls.Add(label2);
            panel2.Controls.Add(label3);
            panel2.Dock = DockStyle.Top;
            panel2.Location = new Point(3, 18);
            panel2.Margin = new Padding(3, 2, 3, 2);
            panel2.Name = "panel2";
            panel2.Size = new Size(855, 39);
            panel2.TabIndex = 17;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(button2);
            groupBox3.Controls.Add(SavePathTextBox);
            groupBox3.Controls.Add(SaveFileDialogButtom);
            groupBox3.Controls.Add(autoOpenDirCheckBox);
            groupBox3.Dock = DockStyle.Fill;
            groupBox3.Location = new Point(0, 0);
            groupBox3.Margin = new Padding(3, 2, 3, 2);
            groupBox3.Name = "groupBox3";
            groupBox3.Padding = new Padding(3, 2, 3, 2);
            groupBox3.Size = new Size(861, 98);
            groupBox3.TabIndex = 15;
            groupBox3.TabStop = false;
            groupBox3.Text = "Стъпка 3";
            // 
            // panel4
            // 
            panel4.Controls.Add(groupbox1);
            panel4.Dock = DockStyle.Top;
            panel4.Location = new Point(0, 0);
            panel4.Margin = new Padding(3, 2, 3, 2);
            panel4.Name = "panel4";
            panel4.Size = new Size(861, 134);
            panel4.TabIndex = 16;
            // 
            // panel5
            // 
            panel5.Controls.Add(groupBox3);
            panel5.Dock = DockStyle.Bottom;
            panel5.Location = new Point(0, 548);
            panel5.Margin = new Padding(3, 2, 3, 2);
            panel5.Name = "panel5";
            panel5.Size = new Size(861, 98);
            panel5.TabIndex = 17;
            // 
            // panel6
            // 
            panel6.Controls.Add(groupBox2);
            panel6.Dock = DockStyle.Fill;
            panel6.Location = new Point(0, 134);
            panel6.Margin = new Padding(3, 2, 3, 2);
            panel6.Name = "panel6";
            panel6.Size = new Size(861, 414);
            panel6.TabIndex = 18;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(861, 646);
            Controls.Add(panel6);
            Controls.Add(panel5);
            Controls.Add(panel4);
            MinimumSize = new Size(877, 685);
            Name = "Form1";
            Text = "Одит лог процесор";
            groupbox1.ResumeLayout(false);
            groupbox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            panel3.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            panel4.ResumeLayout(false);
            panel5.ResumeLayout(false);
            panel6.ResumeLayout(false);
            ResumeLayout(false);
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
        private GroupBox groupbox1;
        private SplitContainer splitContainer1;
        private GroupBox groupBox2;
        private Panel panel2;
        private Panel panel1;
        private GroupBox groupBox3;
        private Panel panel3;
        private PictureBox pictureBox1;
        private Panel panel4;
        private Panel panel5;
        private Panel panel6;
        private Label jsonColumnNamesLabel;
    }
}
