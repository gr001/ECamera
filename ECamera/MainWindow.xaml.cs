using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
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

namespace ECamera
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        OpenCvSharp.UserInterface.PictureBoxIpl m_cameraPictureBox;
        object m_sync = new object();
        System.ComponentModel.BackgroundWorker m_videoCaptureThread;

        public IEnumerable<ImageAction> ImageActions { get { return m_imageActions; } }
        ObservableCollection<ImageAction> m_imageActions = new ObservableCollection<ImageAction>();

        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += OnLoaded;

            m_cameraPictureBox = new OpenCvSharp.UserInterface.PictureBoxIpl();
            Part_Form.Child = m_cameraPictureBox;

            System.Windows.Data.BindingOperations.EnableCollectionSynchronization(m_imageActions, m_sync);

            m_imageActions.Add(new CameraSource());
            m_imageActions.Add(new ConvertToGrayscale());
            m_imageActions.Add(new Flipping());
            m_imageActions.Add(new HistogramEqualization());
            m_imageActions.Add(new Clahe());
            m_imageActions.Add(new ContrastStretching());
            m_imageActions.Add(new UnsharpMask());
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            m_videoCaptureThread = new System.ComponentModel.BackgroundWorker();
            m_videoCaptureThread.DoWork += OnVideoCaptureThread;
            m_videoCaptureThread.ProgressChanged += OnSnapshotCaptured;
            m_videoCaptureThread.WorkerReportsProgress = true;
            m_videoCaptureThread.WorkerSupportsCancellation = true;

            m_videoCaptureThread.RunWorkerAsync();
        }

        private void OnVideoCaptureThread(object sender, DoWorkEventArgs e)
        {
            int sleepTime = 10;// (int)Math.Round(1000 / m_videoCapture.Fps);

            using (OpenCvSharp.Mat image = new OpenCvSharp.Mat()) // Frame image buffer
            {
                // When the movie playback reaches end, Mat.data becomes NULL.
                while (!e.Cancel)
                {
                    OpenCvSharp.Mat inputOutputMat = image;
                    //lock (m_sync)
                    {
                        
                        foreach (var imageAction in m_imageActions)
                        {
                            inputOutputMat = imageAction.Execute(inputOutputMat);
                        }
                    }

                    m_videoCaptureThread.ReportProgress(0, inputOutputMat);
                    System.Threading.Thread.Sleep(sleepTime);
                }
            }
        }

        private void OnSnapshotCaptured(object sender, ProgressChangedEventArgs e)
        {
            var mat = e.UserState as OpenCvSharp.Mat;

            m_cameraPictureBox.ImageIpl = mat;

            //if (m_cameraPictureBox.ImageIpl != null && (m_cameraPictureBox.Width != mat.Width || m_cameraPictureBox.Height != mat.Height))
            //{
            //    //m_cameraPictureBox = new OpenCvSharp.UserInterface.PictureBoxIpl();
            //    m_cameraPictureBox.Width = mat.Width;
            //    m_cameraPictureBox.Height = mat.Height;
            //    m_cameraPictureBox.ImageIpl = mat;
            //}
        }
    }
}
