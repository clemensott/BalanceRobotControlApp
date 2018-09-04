using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using Windows.Networking.Proximity;
using Windows.Networking.Sockets;
using Windows.UI.Core;

namespace BalanceRobotControlApp
{
    class BluetoothConnection
    {
        public PeerInformation Peer { get; private set; }

        public StreamSocket Socket { get; private set; }

        public BinaryWriter Writer { get; private set; }

        public async Task Connect(PeerInformation peer)
        {
            StreamSocket socket = null;
            BinaryWriter writer = null;

            Disconnect();

            try
            {
                socket = new StreamSocket();

                await socket.ConnectAsync(peer.HostName, "{00001101-0000-1000-8000-00805f9b34fb}");
                writer = new BinaryWriter(socket.OutputStream.AsStreamForWrite());

                Socket = socket;
                Writer = writer;
                Peer = peer;
            }
            catch (Exception e)
            {
                writer?.Dispose();
                writer = null;

                socket?.Dispose();
                socket = null;

                throw;
            }
        }

        public void Disconnect()
        {
            Peer = null;

            Writer?.Dispose();
            Writer = null;

            Socket?.Dispose();
            Socket = null;
        }

        public async static Task<DeviceInformationCollection> GetSerialDevices()
        {
            string deviceSelector = RfcommDeviceService.GetDeviceSelector(RfcommServiceId.SerialPort);
            return await DeviceInformation.FindAllAsync(BluetoothDevice.GetDeviceSelector());
        }
    }
}
