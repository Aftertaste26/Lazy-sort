using System;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;

enum Status
{
    setPath,
    startSorting,
    done
}

namespace file_sorter
{
    public partial class Form1 : Form
    {
        Status status;

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,     // x-coordinate of upper-left corner
            int nTopRect,      // y-coordinate of upper-left corner
            int nRightRect,    // x-coordinate of lower-right corner
            int nBottomRect,   // y-coordinate of lower-right corner
            int nWidthEllipse, // height of ellipse
            int nHeightEllipse // width of ellipse
        );


        public Form1()
        {
            InitializeComponent();
            status = Status.setPath;
            this.FormBorderStyle = FormBorderStyle.None;
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));

        }

        private void button1_Click(object sender, EventArgs e)
        {


            switch (status)
            {
                case Status.setPath:
                    if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                    {
                        pictureBox1.Image = file_sorter.Properties.Resources.content;
                        label1.Text = "Esc to change dir";
                        status = Status.startSorting;
                    }
                    break;
                case Status.startSorting:

                    Console.WriteLine("sorting");
                    Console.WriteLine(folderBrowserDialog1.SelectedPath);

                    string[] filePaths = Directory.GetFiles(folderBrowserDialog1.SelectedPath);

                    foreach (string file in filePaths)
                    {
                       File_Move(file);
                    }

                    status = Status.done;
                    
                    label1.Text = "Esc to sort another";
                    pictureBox1.Image = file_sorter.Properties.Resources.checklist;

                    break;

                case Status.done:
                    Console.WriteLine("done");
                    this.Close();
                    break;
            }
        }

        private void File_Move(string source)
        {
            Console.WriteLine("I'm about to move the file to destination");
            string[] soureceSplit = source.Split('\\');
            string filename = soureceSplit.Last();
            string directory  = soureceSplit[soureceSplit.Length-2];
            string destination = directory +"\\"+ filename.Split('.').Last();

            Console.WriteLine(source);
            Console.WriteLine(filename);
            Console.WriteLine(directory);
            Console.WriteLine(destination);

            if (Folder_IsValid(destination))
            {
                try
                {
                    File.Move(source, destination + "\\" + filename);
                    Console.WriteLine("Moved");
                }
                catch (IOException ex)
                {
                    label1.Text = "Error occured";
                    Console.WriteLine(ex);
                }
            }

        }

        private bool Folder_IsValid(string path)
        {

            try
            {
                // Determine whether the directory exists.
                if (Directory.Exists(path))
                {
                    Console.WriteLine("That path exists already.");
                }

                // Try to create the directory.
                DirectoryInfo di = Directory.CreateDirectory(path);
                Console.WriteLine("The directory was created successfully at {0}.", Directory.GetCreationTime(path));
                return true;
            }
            catch (Exception e)
            {
                label1.Text = "Error occured";
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
            finally { }

            return false;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            Console.WriteLine("{0} is pressed", e.KeyCode);

          if (e.KeyCode == Keys.Escape)
            {

                switch (status)
                {
                    case Status.setPath:
                        this.Close();
                        break;
                    default:
                        status = Status.setPath;
                        label1.Text = "Esc to exit";
                        pictureBox1.Image = file_sorter.Properties.Resources.folder;
                        break;
                }
                
            }
        }
    }
}
