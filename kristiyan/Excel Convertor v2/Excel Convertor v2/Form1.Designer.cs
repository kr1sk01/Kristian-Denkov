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
            label1 = new Label();
            button1 = new Button();
            button2 = new Button();
            LoggerLabel = new Label();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 16F);
            label1.Location = new Point(12, 22);
            label1.Name = "label1";
            label1.Size = new Size(661, 30);
            label1.TabIndex = 0;
            label1.Text = "Моля изберете екселският файл, който искате да конвертирате";
            label1.Click += label1_Click;
            // 
            // button1
            // 
            button1.Location = new Point(675, 12);
            button1.Name = "button1";
            button1.Size = new Size(131, 56);
            button1.TabIndex = 1;
            button1.Text = "Избери файл";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Location = new Point(675, 483);
            button2.Name = "button2";
            button2.Size = new Size(131, 56);
            button2.TabIndex = 2;
            button2.Text = "Конвертирай";
            button2.UseVisualStyleBackColor = true;
            // 
            // LoggerLabel
            // 
            LoggerLabel.AutoSize = true;
            LoggerLabel.Location = new Point(301, 255);
            LoggerLabel.Name = "LoggerLabel";
            LoggerLabel.Size = new Size(38, 15);
            LoggerLabel.TabIndex = 3;
            LoggerLabel.Text = "label2";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(818, 551);
            Controls.Add(LoggerLabel);
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
        private Label LoggerLabel;
    }
}
