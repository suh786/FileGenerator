using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FileGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            bw = new BackgroundWorker {WorkerReportsProgress = true, WorkerSupportsCancellation = true};
            bw.DoWork += HandleDoWork;
            bw.ProgressChanged += HandleProgressChanged;
            bw.RunWorkerCompleted += RunWorkerCompleted;
            prgsBar.Value = 0;
        }

        private string _fileName;
        public string FileName
        {
            get { return _fileName; }
            set
            {
                _fileName = value;
                RaiseNotifyPropertyChanged("FileName");
            }
        }

        private int _numberOfRecords;
        public int NumberOfRecords
        {
            get { return _numberOfRecords; }
            set
            {
                _numberOfRecords = value;
                RaiseNotifyPropertyChanged("NumberOfRecords");
            }
        }

        private string _filePath;
        public string FilePath
        {
            get { return _filePath; }
            set
            {
                _filePath = value;
                RaiseNotifyPropertyChanged("FilePath");
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaiseNotifyPropertyChanged(string propertyName)
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        private BackgroundWorker bw;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new FolderBrowserDialog();

            if(folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                FilePath = folderDialog.SelectedPath;
                bw.RunWorkerAsync(FilePath + "\\" + FileName);
                btnGenerate.IsEnabled = false;
            }
            
        }

        private void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btnGenerate.IsEnabled = true;
        }

        private void HandleProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            prgsBar.Value = e.ProgressPercentage;
        }

        private void HandleDoWork(object sender, DoWorkEventArgs e)
        {
            using (var file = new StreamWriter(e.Argument.ToString()))
            {
                for (int i = 1; i <= NumberOfRecords; i++)
                {
                    if (bw.CancellationPending)
                    {
                        e.Cancel = true; 
                        return;
                    }

                    var counter = i.ToString();
                    var person = new Person
                                     {
                                         FirstName = "FirstName" + counter,
                                         MiddleName = "MiddleName" + counter,
                                         LastName = "LastName" + counter,
                                         Address = "Address" + counter,
                                         Age = i,
                                         FatherName = "FatherName" + counter,
                                         MotherName = "MotherName" + counter,
                                         OfficeName = "OfficeName" + counter,
                                         OfficeAddress = "OfficeAddress" + counter
                                     };
                    file.WriteLine(person.ToString());

                    if(i % 100 == 0)
                        bw.ReportProgress(i);
                }
            }
            
        }
    }

    public class Person
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public int Age { get; set; }
        public string FatherName { get; set; }
        public string MotherName { get; set; }
        public string OfficeName { get; set; }
        public string OfficeAddress { get; set; }

        public override string ToString()
        {

            return string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}{9}",
                                 ",",
                                 FirstName,
                                 MiddleName,
                                 LastName,
                                 Address,
                                 Age,
                                 FatherName,
                                 MotherName,
                                 OfficeName,
                                 OfficeAddress);
        }
    }
}
