using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.Proximity;
using Windows.Phone.UI.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Die Elementvorlage "Leere Seite" ist unter http://go.microsoft.com/fwlink/?LinkId=391641 dokumentiert.

namespace BalanceRobotControlApp
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet werden kann oder auf die innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private bool isConnecting, sentStand;
        private DispatcherTimer sendTimer, connectTimer;
        private BluetoothConnection connection;
        private Settings settings;

        public MainPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;

            connection = new BluetoothConnection();
            settings = new Settings();

            sendTimer = new DispatcherTimer();
            sendTimer.Interval = TimeSpan.FromMilliseconds(100);
            sendTimer.Tick += SendTimer_Tick;
            sendTimer.Start();

            connectTimer = new DispatcherTimer();
            connectTimer.Interval = TimeSpan.FromSeconds(2);
            connectTimer.Tick += ConnectTimer_Tick;
            //connectTimer.Start();

            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            if (Frame.BackStackDepth > 0) Frame.GoBack();
            else Application.Current.Exit();
        }

        private void SendTimer_Tick(object sender, object e)
        {
            if (connection.Peer == null) return;

            bool isStanding = joyStick.Value.X == 0 && joyStick.Value.Y == 0;
            if (isStanding&&sentStand) return;

            connection.Writer.Write((byte)7);
            connection.Writer.Write((float)(joyStick.Value.Y * settings.MaxValueY));
            connection.Writer.Write((byte)11);
            connection.Writer.Write((float)(joyStick.Value.X * settings.MaxValueX));

            connection.Writer.Flush();

            sentStand = isStanding;
        }

        private void ConnectTimer_Tick(object sender, object e)
        {
            if (connection.Peer == null) Connect(false);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            Connect(true);
        }

        private async void Connect(bool notify)
        {
            if (isConnecting) return;
            isConnecting = true;

            try
            {
                PeerFinder.AlternateIdentities["Bluetooth:Paired"] = "";
                var peers = await PeerFinder.FindAllPeersAsync();
                System.Diagnostics.Debug.WriteLine(string.Join("\n", peers.Select(d => d.DisplayName)));
                var peer = peers.FirstOrDefault(d => d.DisplayName == settings.BluetoothName) ?? peers.FirstOrDefault();
                System.Diagnostics.Debug.WriteLine("Connect: " + (peer?.DisplayName ?? "Null"));

                if (peer != null) await connection.Connect(peer);
            }
            catch (Exception exc)
            {
                if (notify) await new MessageDialog(exc.Message).ShowAsync();
            }

            isConnecting = false;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (connection.Peer == null) Connect(true);
        }

        private void abbSettings_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SettingsPage), settings);
        }
    }
}
