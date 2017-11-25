using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
using System.Windows.Forms;
using System.ComponentModel;
using System.Threading;
using System.Timers;
using System.Net.Http;
using System.Net;

namespace RecordingDeviceSimulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private int secondsDelay = 5;
        private FileInfo[] fileList;
        private bool stopFlag= true;
        System.Timers.Timer myTimer = new System.Timers.Timer();
        int fileCounter = 0;
        string pauseText = "Pause";
        string resumeText = "Resume";

        private string filePath;
        public string FilePath
        {
            get { return filePath; }
            set
            {
                filePath = value;
                OnPropertyChanged("FilePath");
            }
        }

        private string pauseResumeLabel;
        public string PauseResumeLabel
        {
            get { return pauseResumeLabel; }
            set
            {
                pauseResumeLabel = value;
                OnPropertyChanged("PauseResumeLabel");
            }
        }

        private ImageSource imageSource;
        public ImageSource ImageSource
        {
            get { return imageSource; }
            set
            {
                imageSource = value;
                OnPropertyChanged("ImageSource");
            }
        }

        private string servicePath;
        public string ServicePath
        {
            get { return servicePath; }
            set
            {
                servicePath = value;
                OnPropertyChanged("ServicePath");
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            this.speedBox.Text = secondsDelay.ToString();
            this.DataContext = this;
            PauseResumeLabel = pauseText;
        }

        private void OpenPathDialog(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog openFolderDialog = new FolderBrowserDialog();
            var dialogResult = openFolderDialog.ShowDialog();

            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                FilePath = openFolderDialog.SelectedPath;
            }
        }

        private void IncreaseSpeed(object sender, RoutedEventArgs e)
        {
            secondsDelay++;
            this.speedBox.Text = secondsDelay.ToString();
            myTimer.Interval = secondsDelay * 1000;
        }

        private void DecreaseSpeed(object sender, RoutedEventArgs e)
        {
            secondsDelay--;
            this.speedBox.Text = secondsDelay.ToString();
            myTimer.Interval = secondsDelay * 1000;
        }

        private void SendPictureStream(object source, ElapsedEventArgs e)
        {
            //ImageSource imageSource;
            if (fileCounter < fileList.Count())
            {
                var currentFile = fileList[fileCounter];

                WebClient myWebClient = new WebClient();
                byte[] responseArray = myWebClient.UploadFile(ServicePath, currentFile.FullName);
                //byte[] paramFileBytes = File.ReadAllBytes(currentFile.FullName);//ReadImageFile(currentFile);

                //HttpContent bytesContent = new ByteArrayContent(paramFileBytes);
                //using (var client = new HttpClient())
                //{
                //    using (var formData = new MultipartFormDataContent())
                //    {
                //        //formData.Add(stringContent, "param1", "param1");
                //        //formData.Add(fileStreamContent, "file", f.Name);
                //        formData.Add(bytesContent, "image", currentFile.Name);
                //        var response = client.PostAsync(ServicePath, formData).Result;
                //        if (!response.IsSuccessStatusCode)
                //        {
                //            return;
                //        }
                //        //return response.Content.ReadAsStreamAsync().Result;
                //    }
                //}

                fileCounter++;
            }
            else
            {
                myTimer.Stop();
            }
        }

        //public static byte[] ReadImageFile(FileInfo currentFile)
        //{
        //    byte[] imageData = null;
        //    long imageFileLength = currentFile.Length;
        //    using (var fs = new FileStream(currentFile.FullName, FileMode.Open, FileAccess.Read))
        //    {
        //        BinaryReader br = new BinaryReader(fs);
        //        imageData = br.ReadBytes((int)imageFileLength);
        //    }
        //    return imageData;
        //}

        private void Stop(object sender, RoutedEventArgs e)
        {
            stopFlag = true;
            fileCounter = 0;
            myTimer.Stop();
        }

        private void Start(object sender, RoutedEventArgs e)
        {
            stopFlag = false;
            if (FilePath != string.Empty || FilePath != "...")
            {
                DirectoryInfo dirInfo = new DirectoryInfo(FilePath);
                fileList = dirInfo.GetFiles("*.*");
            }

            myTimer.Elapsed += new ElapsedEventHandler(SendPictureStream);
            myTimer.Interval = secondsDelay*1000; // 1000 ms is one second
            myTimer.Start();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, e);
        }

        private void Pause(object sender, RoutedEventArgs e)
        {
            if (myTimer.Enabled)
            {
                PauseResumeLabel = resumeText;
                myTimer.Stop();
            }
            else
            {
                PauseResumeLabel = pauseText;
                myTimer.Start();
            }
        }
    }
}
