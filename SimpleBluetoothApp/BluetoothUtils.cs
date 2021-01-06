using System.Collections.Generic;

using Tizen.Network.Bluetooth;

namespace SimpleBluetoothApp.Bluetooth
{
    public class Scan
    {
        public delegate void StateChangeCallback(string msg);
        public delegate void DeviceFoundCallback(BluetoothDevice device);

        private void DiscoveryChanged(object sender, DiscoveryStateChangedEventArgs e)
        {
            switch (e.DiscoveryState)
            {
                case BluetoothDeviceDiscoveryState.Started:
                    OnStateChanged("Started scanning");
                    break;
                case BluetoothDeviceDiscoveryState.Found:
                    var device = e.DeviceFound;
                    devices.Add(e.DeviceFound);
                    OnDeviceFound(e.DeviceFound);
                    break;
                case BluetoothDeviceDiscoveryState.Finished:
                    OnStateChanged("Finished scanning");
                    break;
            }
        }

        public void Start()
        {
            devices.Clear();
            BluetoothAdapter.DiscoveryStateChanged += DiscoveryChanged;
            BluetoothAdapter.StartDiscovery();
        }

        public void Stop()
        {
            BluetoothAdapter.StopDiscovery();
            BluetoothAdapter.DiscoveryStateChanged -= DiscoveryChanged;
        }

        public IReadOnlyCollection<BluetoothDevice> Devices() => devices.AsReadOnly();

        public event StateChangeCallback OnStateChanged;
        public event DeviceFoundCallback OnDeviceFound;

        private readonly List<BluetoothDevice> devices = new List<BluetoothDevice> { };
    }

    public class Pair
    {
        private Pair() { }

        public delegate void BondCreated(Pair pair);

        public static void With(BluetoothDevice device, BondCreated onSuccess)
        {
            device.BondCreated += (s, e) =>
            {
                if (e.Result == BluetoothError.None)
                {
                    var pair = new Pair
                    {
                        Device = e.Device,
                    };
                    onSuccess(pair);
                }
            };
            device.CreateBond();
        }

        public BluetoothDevice Device { get; set; }
    }
}