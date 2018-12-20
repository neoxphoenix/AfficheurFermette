using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ShowableData
{
    /// <summary>
    /// héritable permettant de suivre les changements de prpriétés
    /// </summary>
    public class ObservableData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string PropertyName)
        {
            PropertyChangedEventHandler Handler = PropertyChanged;
            if (Handler != null)
                Handler(this, new PropertyChangedEventArgs(PropertyName));
        }
    }
}
