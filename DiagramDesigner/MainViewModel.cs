using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Input;
using System.Runtime.CompilerServices;
using DiagramDesignerEngine;

namespace DiagramDesigner
{
    /// <summary>
    /// Manage events between the UI and the engine
    /// </summary>
    class MainViewModel : INotifyPropertyChanged
    {
        private List<Point> DefaultPointsToRender = new List<Point> { new Point(10, 10), new Point(20, 30), new Point(50, 45), new Point(100, 100) };
        private List<Point> _pointsToRender = null;
        public List<Point> PointsToRender { // TODO: stub
            get { return this._pointsToRender == null ? this.DefaultPointsToRender : this.PointsToRender; }
            private set { this._pointsToRender = value; }
        }

        public ICommand TestCommand { protected set; get; }
        public ICommand HandleMouseClick { protected set; get; }       

        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel()
        {
            this.TestCommand = new DelegateCommand(ExecuteTestCommand);
            this.HandleMouseClick = new DelegateCommand(ExecuteHandleMouseClick);
        }
        private void ExecuteTestCommand(object obj)
        {
            System.Diagnostics.Debug.WriteLine("test running");
            this.testTranslate();
        }

        private void ExecuteHandleMouseClick(object obj)
        {
            var location = (System.Windows.Point)obj;
            if (this.PointsToRender != null) {
                this.PointsToRender[0].coordinateX = location.X;
                this.PointsToRender[0].coordinateY = location.Y;
            }
            this.PointsToRenderChanged();
        }

        public void testTranslate()
        {
            for (int i=0; i < this.PointsToRender.Count; i++)
            {
                var p = this.PointsToRender[i];
                p.coordinateX += 10;
                p.coordinateY += 10;
            }
            this.PointsToRenderChanged();
        }

        private void PointsToRenderChanged()
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
