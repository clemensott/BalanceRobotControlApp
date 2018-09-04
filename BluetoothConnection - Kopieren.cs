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
using Windows.Networking.Sockets;
using Windows.UI.Core;

namespace BalanceRobotControlApp
{
    class BluetoothConnection1
    {
        public DeviceInformation Device { get; private set; }

        public RfcommDeviceService Service { get; private set; }

        public StreamSocket Socket { get; private set; }

        public BinaryWriter Writer { get; private set; }

        public async Task Connect(DeviceInformation device)
        {
            RfcommDeviceService service = null;
            StreamSocket socket = null;
            BinaryWriter writer = null;

            Disconnect();

            try
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    try
                    {
                        service = await RfcommDeviceService.FromIdAsync(device.Id);
                    }
                    catch(Exception e)
                    {
                        await new Windows.UI.Popups.MessageDialog(e.Message).ShowAsync();
                    }
                });

                if (service == null)
                {
                    await new Windows.UI.Popups.MessageDialog("No Service found").ShowAsync();
                    return;
                }

                socket = new StreamSocket();

                await socket.ConnectAsync(service.ConnectionHostName, service.ConnectionServiceName,
                    SocketProtectionLevel.BluetoothEncryptionAllowNullAuthentication);
                writer = new BinaryWriter(socket.OutputStream.AsStreamForWrite());

                Service = service;
                Socket = socket;
                Writer = writer;
                Device = device;
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
            Device = null;
            Service = null;

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
