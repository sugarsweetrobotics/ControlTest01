using System;
using System.ComponentModel;

namespace ControlTest01
{


    /// <summary>
    /// NumericUpDown とTrackBar にバインドされるデータ
    /// </summary>
    public class Data : INotifyPropertyChanged
    {
        /// <summary>
        /// INotifyPropertyChanged から継承したイベントデリゲート
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// イベント通知
        /// </summary>
        /// <param name="info"></param>
        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        int _value;
        public int Value
        {
            get { return _value; }
            set
            {
                if (value != _value)
                {
                    _value = value;
                    // このプロパティ名を渡してイベント通知
                    NotifyPropertyChanged("Value");
                }
            }
        }
    }
}
