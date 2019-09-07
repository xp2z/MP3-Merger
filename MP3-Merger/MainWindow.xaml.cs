using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MP3_Merger
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<string> filePaths = new List<string>();
        BackgroundWorker backgroundWorker = new BackgroundWorker();
        string path = "";
        string destination = "";

        public MainWindow()
        {
            InitializeComponent();
            backgroundWorker.WorkerSupportsCancellation = true;
            backgroundWorker.WorkerReportsProgress = true;

            backgroundWorker.ProgressChanged += backgroundworker_ProgressChanged;
            backgroundWorker.DoWork += backgroundworker_DoWork;


        }

        private void Btn_Navigate_Click(object sender, RoutedEventArgs e)
        {
            filePaths.Clear();
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.FilterIndex = 0;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.Filter = "MP3 files (*.mp3)|*.mp3|M4A files (*.m4a*)|*.m4a*";
            openFileDialog.Multiselect = true;

            if (openFileDialog.ShowDialog() == true)
            {
                txt_SourcePath.Text = openFileDialog.FileNames.Length.ToString() + " Files Selected";

                foreach(string path in openFileDialog.FileNames)
                {
                    filePaths.Add(path);
                }
            }
        }

        private void Btn_Start_Click(object sender, RoutedEventArgs e)
        {
            if(filePaths.Count > 0 && txt_DestinationFileName.Text != "")
            {

                path = System.IO.Path.GetDirectoryName(filePaths[1]);
                destination = txt_DestinationFileName.Text + System.IO.Path.GetExtension(filePaths[1]);

                ProgressBar.Maximum = filePaths.Count();
                ProgressBar.Value = 0;

                backgroundWorker.RunWorkerAsync();

              
            }
        }

        private void Merger()
        {
           

            using (var fileStream = File.OpenWrite(System.IO.Path.Combine(path, destination)))
            {
                int i = 0;
                foreach (var file in filePaths)
                {
                    var buffer = File.ReadAllBytes(System.IO.Path.Combine(path, file));
                    fileStream.Write(buffer, 0, buffer.Length);
                    i++;
                    backgroundWorker.ReportProgress(i);
                }

                fileStream.Flush();
            }

        }

        private void backgroundworker_ProgressChanged(object sender, ProgressChangedEventArgs eventArgs)
        {
            ProgressBar.Value = eventArgs.ProgressPercentage;
        }

        private void backgroundworker_DoWork(object sender, DoWorkEventArgs e)
        {
            Merger();
        }
    }
}
