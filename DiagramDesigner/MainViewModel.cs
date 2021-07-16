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

        private bool _isInDrawingState = false;
        public bool IsInDrawingState
        {
            private set { SetProperty(ref _isInDrawingState, value); }
            get { return this._isInDrawingState; }
        }

        public ICommand StartDrawingCommand { set; get; }
        public ICommand EndDrawingCommand { set; get; }
        public ICommand HandleMouseClickCommand { set; get; }       

        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel()
        {
            this.StartDrawingCommand = new DelegateCommand(ExecuteStartDrawing);
            this.EndDrawingCommand = new DelegateCommand(ExecuteEndDrawing);
            this.HandleMouseClickCommand = new DelegateCommand(ExecuteHandleMouseClick);
        }
        private void ExecuteStartDrawing(object obj)
        {
            this.IsInDrawingState = true;
        }

        private void ExecuteEndDrawing(object obj)
        {
            this.IsInDrawingState = false;
        }

        private void ExecuteHandleMouseClick(object obj)
        {
            if (this.IsInDrawingState)
            {
                var location = (System.Windows.Point)obj;
                if (this.PointsToRender != null) {
                    this.PointsToRender[0].coordinateX = location.X;
                    this.PointsToRender[0].coordinateY = location.Y;
                }
                this.PointsToRenderChanged();
            }
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
