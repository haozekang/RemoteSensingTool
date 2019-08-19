using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace RemoteSensingTool
{
    public class nFile : INotifyPropertyChanged
    {
        public string filepath { get; set; }
        public string filename { get; set; }

        private string _state;

        public string state
        {
            get
            {
                return _state;
            }

            set
            {
                _state = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("state"));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
