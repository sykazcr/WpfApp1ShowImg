using System;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Controls;
using System.IO;
using System.IO.MemoryMappedFiles;



namespace WpfApp1
{
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer dispatcherTimer;
        string [] m_imageUris = new string[8];
        int m_cnt = 0;
        byte[] _bytes;
        Mutex mutex;
        MemoryMappedFile mmf;
        MemoryMappedViewAccessor accessor;
        byte[] buffer = new byte[1024*20];

        /// <summary>
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            // Open the memory-mapped file
            //using (MemoryMappedFile mmf = MemoryMappedFile.OpenExisting("Global\\MySharedMemory"))
            {
                // Lock the mutex to ensure exclusive access
                mutex.WaitOne();

                try
                {
                    // Read from the shared memory
                    //using (MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor())
                    {
                        //byte[] buffer = new byte[1024];
                        accessor.ReadArray(0, buffer, 0, buffer.Length);
                        //string message = System.Text.Encoding.UTF8.GetString(buffer).TrimEnd('\0');
                        //Console.WriteLine("Message from Process 1: " + message);
                        heli.Source = ToBitmapImage();
                    }
                }
                finally
                {
                    // Release the mutex
                    mutex.ReleaseMutex();
                }
#if false // from local file version
            // assign new source to the Image
            timelabel.Content = DateTime.Now.ToLongTimeString();
            //Image image = sender as Image;

            m_imageUris[0] = "file:///e:/temp/0.png";
            m_imageUris[1] = "file:///e:/temp/1.png";
            m_imageUris[2] = "file:///e:/temp/2.png";
            m_imageUris[3] = "file:///e:/temp/3.png";
            m_imageUris[4] = "file:///e:/temp/4.png";
            m_imageUris[5] = "file:///e:/temp/5.png";
            m_imageUris[6] = "file:///e:/temp/6.png";
            m_imageUris[7] = "file:///e:/temp/7.png";

            heli.Source = new BitmapImage(new Uri(m_imageUris[m_cnt], UriKind.Absolute));
            m_cnt++;
            if (m_cnt > 7) m_cnt = 0;
#endif
            }
        }


        public MainWindow()
        {
            InitializeComponent();

            /*Mutex*/ mutex = new Mutex(false, "Global/MySharedMemoryMutex");
            /*MemoryMappedFile*/ mmf = MemoryMappedFile.OpenExisting("Global/MySharedMemory");
            /*MemoryMappedViewAccessor*/ accessor = mmf.CreateViewAccessor();

            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(100);//TimeSpan.FromSeconds(1);
            dispatcherTimer.Start();


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
        }

        public BitmapImage ToBitmapImage()
        {
            BitmapImage img = new BitmapImage();

            img.BeginInit();
            img.StreamSource = new MemoryStream(buffer);
            img.EndInit();

            return img;
        }
    }
}
