using Microsoft.Win32;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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
            openFileDialog.Filter = "MP3 files (*.mp3)|*.mp3";
            openFileDialog.Multiselect = true;

            if (openFileDialog.ShowDialog() == true)
            {
                txt_SourcePath.Text = openFileDialog.FileNames.Length.ToString() + " Files Selected";

                foreach (string path in openFileDialog.FileNames)
                {
                    filePaths.Add(path);
                }
            }
        }

        private void Btn_Start_Click(object sender, RoutedEventArgs e)
        {
            if (filePaths.Count > 0 && txt_DestinationFileName.Text != "")
            {

                path = System.IO.Path.GetDirectoryName(filePaths[1]);
                destination = txt_DestinationFileName.Text + System.IO.Path.GetExtension(filePaths[1]);

                ProgressBar.Maximum = filePaths.Count();
                ProgressBar.Value = 0;

                backgroundWorker.RunWorkerAsync();


            }
            else
            {
                MessageBox.Show("Missing Source or Destination Filename", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Merger()
        {
            try
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

                    MessageBox.Show("Merging Successfull!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                    System.Diagnostics.Process.Start("explorer.exe", path);

                }
            }
            catch {

                MessageBox.Show("Merging Failed! \n Please try again", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
