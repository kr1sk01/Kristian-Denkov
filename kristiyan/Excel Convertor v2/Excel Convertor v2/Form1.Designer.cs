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
            contextMenuStrip1 = new ContextMenuStrip(components);
            ItemToChooseListBox = new ListBox();
            ChosenItemListBox = new ListBox();
            button3 = new Button();
            button4 = new Button();
            button5 = new Button();
            button6 = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 14F);
            label1.Location = new Point(14, 29);
            label1.Name = "label1";
            label1.Size = new Size(721, 32);
            label1.TabIndex = 0;
            label1.Text = "Моля изберете екселският файл, който искате да конвертирате";
            label1.Click += label1_Click;
            // 
            // button1
            // 
            button1.Location = new Point(735, 16);
            button1.Margin = new Padding(3, 4, 3, 4);
            button1.Name = "button1";
            button1.Size = new Size(186, 75);
            button1.TabIndex = 1;
            button1.Text = "Избери файл";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Location = new Point(735, 644);
            button2.Margin = new Padding(3, 4, 3, 4);
            button2.Name = "button2";
            button2.Size = new Size(186, 75);
            button2.TabIndex = 2;
            button2.Text = "Конвертирай";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.ImageScalingSize = new Size(20, 20);
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(61, 4);
            // 
            // ItemToChooseListBox
            // 
            ItemToChooseListBox.FormattingEnabled = true;
            ItemToChooseListBox.Location = new Point(12, 98);
            ItemToChooseListBox.Name = "ItemToChooseListBox";
            ItemToChooseListBox.Size = new Size(284, 404);
            ItemToChooseListBox.TabIndex = 3;
            ItemToChooseListBox.SelectedIndexChanged += ItemToChooseListBox_SelectedIndexChanged;
            // 
            // ChosenItemListBox
            // 
            ChosenItemListBox.FormattingEnabled = true;
            ChosenItemListBox.Location = new Point(361, 98);
            ChosenItemListBox.Name = "ChosenItemListBox";
            ChosenItemListBox.Size = new Size(284, 404);
            ChosenItemListBox.TabIndex = 4;
            ChosenItemListBox.SelectedIndexChanged += ChosenItemListBox_SelectedIndexChanged;
            // 
            // button3
            // 
            button3.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            button3.Location = new Point(302, 117);
            button3.Name = "button3";
            button3.Size = new Size(53, 48);
            button3.TabIndex = 5;
            button3.Text = ">>";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // button4
            // 
            button4.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            button4.Location = new Point(302, 434);
            button4.Name = "button4";
            button4.Size = new Size(53, 48);
            button4.TabIndex = 6;
            button4.Text = "<<";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // button5
            // 
            button5.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            button5.Location = new Point(651, 117);
            button5.Name = "button5";
            button5.Size = new Size(53, 48);
            button5.TabIndex = 7;
            button5.Text = "🡡";
            button5.UseVisualStyleBackColor = true;
            button5.Click += button5_Click;
            // 
            // button6
            // 
            button6.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            button6.Location = new Point(651, 434);
            button6.Name = "button6";
            button6.Size = new Size(53, 48);
            button6.TabIndex = 8;
            button6.Text = "🡣";
            button6.UseVisualStyleBackColor = true;
            button6.Click += button6_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(935, 735);
            Controls.Add(button6);
            Controls.Add(button5);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(ChosenItemListBox);
            Controls.Add(ItemToChooseListBox);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(label1);
            Margin = new Padding(3, 4, 3, 4);
            Name = "Form1";
            Text = "Ексел Конвертор";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Button button1;
        private Button button2;
        private ContextMenuStrip contextMenuStrip1;
        private ListBox ItemToChooseListBox;
        private ListBox ChosenItemListBox;
        private Button button3;
        private Button button4;
        private Button button5;
        private Button button6;
    }
}
