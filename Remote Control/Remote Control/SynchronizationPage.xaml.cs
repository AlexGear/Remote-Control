using System;
using Microsoft.Phone.Controls;
using System.Net;
using ZXing;
using ZXing.QrCode;
using System.Windows.Threading;
using ZXing.Mobile;
using Microsoft.Devices;
using System.Windows.Navigation;
using ZXing.Common;
using SocketEx;
using System.IO;

namespace Remote_Control {
    public partial class SynchronizationPage : PhoneApplicationPage {
        public static IPEndPoint SynchronizationResult;

        private const string CHECKWORD = "RCSync";

        private readonly DispatcherTimer timer;
        private PhotoCameraLuminanceSource luminance;
        private QRCodeReader reader;
        private PhotoCamera photoCamera;
        private bool cameraInitialized;
        private bool closeAfterCameraInit;

        public SynchronizationPage() {
            InitializeComponent();
            cameraInitialized = false;
            closeAfterCameraInit = false;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(250);
            timer.Tick += (o, arg) => ScanPreviewBuffer();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            SynchronizationResult = null;

            photoCamera = new PhotoCamera();
            photoCamera.Initialized += OnPhotoCameraInitialized;
            previewVideo.SetSource(photoCamera);
            
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e) {
            if(!cameraInitialized) {
                closeAfterCameraInit = true;
                e.Cancel = true;
                return;
            }
            photoCamera?.Dispose();
            timer?.Stop();
            base.OnNavigatingFrom(e);
        }

        private void OnPhotoCameraInitialized(object sender, CameraOperationCompletedEventArgs e) {
            cameraInitialized = true;
            if(closeAfterCameraInit) {
                Dispatcher.BeginInvoke(GoBack);
                return;
            }
            int width = Convert.ToInt32(photoCamera.PreviewResolution.Width);
            int height = Convert.ToInt32(photoCamera.PreviewResolution.Height);

            luminance = new PhotoCameraLuminanceSource(width, height);
            reader = new QRCodeReader();

            Dispatcher.BeginInvoke(() => {
                previewTransform.Rotation = photoCamera.Orientation;
                timer.Start();
            });
        }

        private void GoBack() {
            if(NavigationService.CanGoBack)
                NavigationService.GoBack();
        }

        private void Synchronize(string text) {
            string[] splitted = text.Split('@');
            if(splitted?.Length == 2 && splitted?[0] == CHECKWORD) {
                string[] ipAndPort = splitted[1].Split(':');
                IPAddress ip;
                int port;
                if(IPAddress.TryParse(ipAndPort[0], out ip) &&
                   int.TryParse(ipAndPort[1], out port)) {
                    SynchronizationResult = new IPEndPoint(ip, port);
                    GoBack();
                }
            }
        }

        private void ScanPreviewBuffer() {
            try {
                photoCamera.GetPreviewBufferY(luminance.PreviewBufferY);
                var binarizer = new HybridBinarizer(luminance);
                var binBitmap = new BinaryBitmap(binarizer);
                var result = reader.decode(binBitmap);
                if(result != null)
                    Dispatcher.BeginInvoke(() => Synchronize(result.Text));
            }
            catch {
            }
        }

        private void CancelButtonClick(object sender, EventArgs e) {
            GoBack();
        }
    }
}