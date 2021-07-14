using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Input;
using System.Runtime.CompilerServices;

namespace DiagramDesigner
{
    class MainViewModel : INotifyPropertyChanged
    {
        private List<DDPoint> defaultPointsToRender = new List<DDPoint> { new DDPoint(10, 10), new DDPoint(20, 30), new DDPoint(50, 45), new DDPoint(100, 100) };
        private List<DDPoint> _pointsToRender = null;
        public List<DDPoint> PointsToRender { // TODO: stub
            get { return this._pointsToRender == null ? this.defaultPointsToRender : this.PointsToRender;  }
            private set { this._pointsToRender = value; }
        }

        public ICommand TestCommand { protected set; get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel()
        {
            this.TestCommand = new DelegateCommand(ExecuteTestCommand);
        }
        
        public void testTranslate()
        {
            for (int i=0; i < this.PointsToRender.Count; i++)
            {
                var p = this.PointsToRender[i];
                p.coordinateX += 10;
                p.coordinateY += 10;
            }
            this.GeometriesChanged();
        }

        private void ExecuteTestCommand(object obj)
        {
            System.Diagnostics.Debug.WriteLine("test running");
            this.testTranslate();
        }

        private void GeometriesChanged()
        {
            this.OnPropertyChanged("PointsToRender");
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (object.Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
