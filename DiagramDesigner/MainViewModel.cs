using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Input;
using System.Runtime.CompilerServices;
using DiagramDesignerEngine;
using System.Linq;

namespace DiagramDesigner
{
    /// <summary>
    /// Manage events between the UI and the engine
    /// </summary>
    class MainViewModel : INotifyPropertyChanged
    {
        public ProgramRequirementsTable ProgramsTable = new ProgramRequirementsTable();

        private List<List<Point>> _pointsToRender = new List<List<Point>>();


        public List<List<Point>> PolylinesToRender {
            get { return _pointsToRender; }
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

        public ICommand AddNewProgramRequirementCommand { set; get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel()
        {
            this.StartDrawingCommand = new DelegateCommand(ExecuteStartDrawing);
            this.EndDrawingCommand = new DelegateCommand(ExecuteEndDrawing);
            this.AddNewProgramRequirementCommand = new DelegateCommand(ExecuteAddNewRowToRequirementsTable);
        }
        private void ExecuteStartDrawing(object obj)
        {
            this.IsInDrawingState = true;
        }

        private void ExecuteEndDrawing(object obj)
        {
            this.IsInDrawingState = false;
        }

        private void ExecuteAddNewRowToRequirementsTable(object obj)
        {
            this.ProgramsTable.Rows.Add(this.ProgramsTable.NewRow());
        }

        public void HandleMouseMovedEvent(object sender, EventArgs e) // TODO: should view model care about this? 
        {
            if (this.IsInDrawingState)
            {
                var mea = (MouseEventArgs)e;
                if (this.PolylinesToRender.Count != 0 && this.PolylinesToRender.First().Count != 0)
                {
                    this.PolylinesToRender.First().Last().coordinateX = mea.LocationX;
                    this.PolylinesToRender.First().Last().coordinateY = mea.LocationY;
                }
                this.PointsToRenderChanged();
            }
        }

        public void HandleMouseLeftClickedEvent(object sender, EventArgs e)
        {
            if (this.IsInDrawingState)
            {
                var mea = (MouseEventArgs)e;
                if (this.PolylinesToRender != null)
                {
                    var newPoint = new Point(mea.LocationX, mea.LocationY);

                    if (this.PolylinesToRender.Count == 0)
					{
                        this.PolylinesToRender.Add(new List<Point>());
					}

                    this.PolylinesToRender.First().Add(newPoint);
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
