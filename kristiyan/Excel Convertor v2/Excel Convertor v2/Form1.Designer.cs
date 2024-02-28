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
            pictureBox1 = new PictureBox();
            splitContainer1 = new SplitContainer();
            groupBox2 = new GroupBox();
            panel3 = new Panel();
            panel1 = new Panel();
            panel2 = new Panel();
            groupBox3 = new GroupBox();
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
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F);
            label1.Location = new Point(324, 71);
            label1.Name = "label1";
            label1.Size = new Size(0, 28);
            label1.TabIndex = 0;
            // 
            // button1
            // 
            button1.Location = new Point(30, 36);
            button1.Margin = new Padding(3, 4, 3, 4);
            button1.Name = "button1";
            button1.Size = new Size(250, 75);
            button1.TabIndex = 1;
            button1.Text = "Изберете входящ файл";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Location = new Point(30, 37);
            button2.Margin = new Padding(3, 4, 3, 4);
            button2.Name = "button2";
            button2.Size = new Size(250, 75);
            button2.TabIndex = 2;
            button2.Text = "Експорт";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // ItemToChooseListBox
            // 
            ItemToChooseListBox.Dock = DockStyle.Top;
            ItemToChooseListBox.FormattingEnabled = true;
            ItemToChooseListBox.Location = new Point(0, 0);
            ItemToChooseListBox.Name = "ItemToChooseListBox";
            ItemToChooseListBox.Size = new Size(364, 484);
            ItemToChooseListBox.TabIndex = 3;
            ItemToChooseListBox.SelectedIndexChanged += ItemToChooseListBox_SelectedIndexChanged;
            ItemToChooseListBox.MouseDoubleClick += ItemToChoose_MouseDoubleClick;
            // 
            // ChosenItemListBox
            // 
            ChosenItemListBox.Dock = DockStyle.Top;
            ChosenItemListBox.FormattingEnabled = true;
            ChosenItemListBox.Location = new Point(0, 0);
            ChosenItemListBox.Name = "ChosenItemListBox";
            ChosenItemListBox.Size = new Size(508, 484);
            ChosenItemListBox.TabIndex = 4;
            ChosenItemListBox.SelectedIndexChanged += ChosenItemListBox_SelectedIndexChanged;
            ChosenItemListBox.MouseDoubleClick += ChosenItemListBox_MouseDoubleClick;
            // 
            // button5
            // 
            button5.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            button5.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            button5.Location = new Point(24, 18);
            button5.Name = "button5";
            button5.Size = new Size(53, 113);
            button5.TabIndex = 7;
            button5.Text = "🡡";
            button5.UseVisualStyleBackColor = true;
            button5.Click += button5_Click;
            // 
            // button6
            // 
            button6.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            button6.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            button6.Location = new Point(24, 370);
            button6.Name = "button6";
            button6.Size = new Size(53, 113);
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
            label2.Location = new Point(95, 18);
            label2.Name = "label2";
            label2.Size = new Size(113, 20);
            label2.TabIndex = 9;
            label2.Text = "Всички колони";
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label3.AutoSize = true;
            label3.Location = new Point(590, 18);
            label3.Name = "label3";
            label3.Size = new Size(140, 20);
            label3.TabIndex = 10;
            label3.Text = "Колони за експорт";
            // 
            // autoOpenDirCheckBox
            // 
            autoOpenDirCheckBox.AutoSize = true;
            autoOpenDirCheckBox.Checked = true;
            autoOpenDirCheckBox.CheckState = CheckState.Checked;
            autoOpenDirCheckBox.Location = new Point(315, 37);
            autoOpenDirCheckBox.Margin = new Padding(3, 4, 3, 4);
            autoOpenDirCheckBox.Name = "autoOpenDirCheckBox";
            autoOpenDirCheckBox.Size = new Size(248, 24);
            autoOpenDirCheckBox.TabIndex = 11;
            autoOpenDirCheckBox.Text = "Авт. отваряне на директорията";
            autoOpenDirCheckBox.UseVisualStyleBackColor = true;
            autoOpenDirCheckBox.CheckedChanged += autoOpenDirCheckBox_CheckedChanged;
            // 
            // SaveFileDialogButtom
            // 
            SaveFileDialogButtom.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            SaveFileDialogButtom.Location = new Point(916, 73);
            SaveFileDialogButtom.Margin = new Padding(3, 4, 3, 4);
            SaveFileDialogButtom.Name = "SaveFileDialogButtom";
            SaveFileDialogButtom.Size = new Size(40, 39);
            SaveFileDialogButtom.TabIndex = 12;
            SaveFileDialogButtom.Text = "...";
            SaveFileDialogButtom.UseVisualStyleBackColor = true;
            SaveFileDialogButtom.Click += SaveFileDialogButtom_Click;
            // 
            // SavePathTextBox
            // 
            SavePathTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            SavePathTextBox.Font = new Font("Segoe UI", 12F);
            SavePathTextBox.Location = new Point(315, 73);
            SavePathTextBox.Margin = new Padding(3, 4, 3, 4);
            SavePathTextBox.Multiline = true;
            SavePathTextBox.Name = "SavePathTextBox";
            SavePathTextBox.Size = new Size(595, 39);
            SavePathTextBox.TabIndex = 13;
            // 
            // groupbox1
            // 
            groupbox1.Controls.Add(pictureBox1);
            groupbox1.Controls.Add(button1);
            groupbox1.Controls.Add(label1);
            groupbox1.Dock = DockStyle.Top;
            groupbox1.Location = new Point(0, 0);
            groupbox1.Name = "groupbox1";
            groupbox1.Size = new Size(982, 140);
            groupbox1.TabIndex = 14;
            groupbox1.TabStop = false;
            groupbox1.Text = "Стъпка 1 ";
            // 
            // pictureBox1
            // 
            pictureBox1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(684, 12);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(195, 122);
            pictureBox1.TabIndex = 2;
            pictureBox1.TabStop = false;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(ItemToChooseListBox);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(ChosenItemListBox);
            splitContainer1.Size = new Size(876, 635);
            splitContainer1.SplitterDistance = 364;
            splitContainer1.TabIndex = 15;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(panel3);
            groupBox2.Controls.Add(panel2);
            groupBox2.Dock = DockStyle.Fill;
            groupBox2.Location = new Point(0, 140);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(982, 713);
            groupBox2.TabIndex = 4;
            groupBox2.TabStop = false;
            groupBox2.Text = "Стъпка 2";
            // 
            // panel3
            // 
            panel3.Controls.Add(splitContainer1);
            panel3.Controls.Add(panel1);
            panel3.Dock = DockStyle.Fill;
            panel3.Location = new Point(3, 75);
            panel3.Name = "panel3";
            panel3.Size = new Size(976, 635);
            panel3.TabIndex = 18;
            // 
            // panel1
            // 
            panel1.Controls.Add(button6);
            panel1.Controls.Add(button5);
            panel1.Dock = DockStyle.Right;
            panel1.Location = new Point(876, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(100, 635);
            panel1.TabIndex = 16;
            // 
            // panel2
            // 
            panel2.Controls.Add(label2);
            panel2.Controls.Add(label3);
            panel2.Dock = DockStyle.Top;
            panel2.Location = new Point(3, 23);
            panel2.Name = "panel2";
            panel2.Size = new Size(976, 52);
            panel2.TabIndex = 17;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(button2);
            groupBox3.Controls.Add(SavePathTextBox);
            groupBox3.Controls.Add(SaveFileDialogButtom);
            groupBox3.Controls.Add(autoOpenDirCheckBox);
            groupBox3.Dock = DockStyle.Bottom;
            groupBox3.Location = new Point(0, 714);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(982, 139);
            groupBox3.TabIndex = 15;
            groupBox3.TabStop = false;
            groupBox3.Text = "Стъпка 3";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(982, 853);
            Controls.Add(groupBox3);
            Controls.Add(groupBox2);
            Controls.Add(groupbox1);
            Margin = new Padding(3, 4, 3, 4);
            MinimumSize = new Size(1000, 900);
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
    }
}
