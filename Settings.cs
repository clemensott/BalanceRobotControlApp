using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalanceRobotControlApp
{
    class Settings : INotifyPropertyChanged
    {
        private string bluetoothName;
        private double maxValueX, maxValueY;

        public string BluetoothName
        {
            get { return bluetoothName; }
            set
            {
                if (value == bluetoothName) return;

                bluetoothName = value;
                OnPropertyChanged("BluetoothName");
            }
        }

        public double MaxValueX
        {
            get { return maxValueX; }
            set
            {
                if (value == maxValueX || value > 1 || value <= 0) return;

                maxValueX = value;
                OnPropertyChanged("MaxValueX");
            }
        }

        public double MaxValueY
        {
            get { return maxValueY; }
            set
            {
                if (value == maxValueY || value > 1 || value <= 0) return;

                maxValueY = value;
                OnPropertyChanged("MaxValueY");
            }
        }

        public Settings()
        {
            bluetoothName = "RNBT-6DD3";
            MaxValueX = 0.5;
            MaxValueY = 0.5;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
