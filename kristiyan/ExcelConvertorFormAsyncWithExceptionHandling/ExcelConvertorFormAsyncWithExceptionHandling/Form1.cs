namespace WinFormsApp1
{

    public partial class Form1 : Form
    {
        public string[] file;
        public Form1()
        {
            InitializeComponent();
            this.AllowDrop = true;
            this.DragEnter += new DragEventHandler(Form1_DragEnter);
            this.DragDrop += new DragEventHandler(Form1_DragDrop);
            label1.Text = "Моля пусни с мишката екселския файл, който искаш да конвертираш!";
            this.Text = "Ексел файл конвертор";
            label2.Text = "";
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            file = (string[])e.Data.GetData(DataFormats.FileDrop);

            ReadNWrite mp = new ReadNWrite();

            string res = mp.Main(file[0]).Result;

            if (res == "0")
            {
                label2.ForeColor = Color.Red;
                label2.Text = "Грешка при конвертирането, моля обърнете се към администратор!";
            }
        }
    }
}
