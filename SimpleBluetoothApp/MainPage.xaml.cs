using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Tizen.Network.Bluetooth;

using SimpleBluetoothApp.Bluetooth;

namespace SimpleBluetoothApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        public string BluetoothState
        {
            get { return BluetoothAdapter.IsBluetoothEnabled ? "ON" : "OFF"; }
        }

        private string connection = null;
        public string BluetoothConnectionState
        {
            get
            {
                if (connection == null)
                {
                    return "No device connected";
                }
                return connection;
            }
            set
            {
                connection = value;

                OnPropertyChanged("BluetoothConnectionState");
            }
        }


        private void ScanClicked(object sender, EventArgs e)
        {
            BluetoothConnectionState = "Searching...";

            scan = new Scan();

            scan.OnDeviceFound += (BluetoothDevice device) =>
            {
                BluetoothConnectionState = string.Format("Found {0} devices", scan.Devices().Count);
            };
            scan.OnStateChanged += (string msg) =>
            {
                BluetoothConnectionState = msg;
            };

            try
            {
                scan.Start();
            }
            catch (InvalidOperationException ex)
            {
                BluetoothConnectionState = ex.Message;
            }
}

        private void StopClicked(object sender, System.EventArgs e)
        {
            BluetoothConnectionState = "Stopping...";
            scan.Stop();
            BluetoothConnectionState = string.Format("Finished ({0} devices found)", scan.Devices().Count);

            RefreshDeviceList();
        }

        private void RefreshDeviceList()
        {
            var devices = scan.Devices();

            InnerLayout.Children.Clear();
            InnerLayout.Children.Add(new Label
            {
                Text = devices.Count == 0
                       ? "No devices found"
                       : "Devices:",
                HorizontalOptions = LayoutOptions.Center,
            });

            foreach (var device in devices)
            {
                AddDeviceItem(device);
            }
        }

        private void OnDevicePair(Pair pair)
        {
            this.pair = pair;
            BluetoothConnectionState = string.Format("Paired with {0}", pair.Device.Name);
        }

        private void AddDeviceItem(BluetoothDevice device)
        {

            var deviceInfo = string.Format("{0} ({1})", device.Name, device.Address);

            var pairButton = new Button()
            {
                Text = "Pair",
            };
            pairButton.Clicked += (s, e) =>
            {
                Pair.With(device, OnDevicePair);
            };

            InnerLayout.Children.Add(new StackLayout {
                Orientation = StackOrientation.Vertical,
                Children = {
                    new Label
                    {
                        Text = deviceInfo,
                        HorizontalOptions = LayoutOptions.Center,
                    },
                    pairButton,
                },
            });
        }

        private Pair pair = null;
        private Scan scan = null;
    }
}