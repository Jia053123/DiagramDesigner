using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Input;
using System.Runtime.CompilerServices;
using DiagramDesignerEngine;
using System.Linq;
using WinPoint = System.Windows.Point;

namespace DiagramDesigner
{
    /// <summary>
    /// Manage events between the UI and the engine
    /// </summary>
    class MainViewModel : INotifyPropertyChanged
    {
        private double _displayUnitOverRealUnit = 2;
        public double DisplayUnitOverRealUnit
		{
            get { return _displayUnitOverRealUnit; }
            set { _displayUnitOverRealUnit = value; }
		}

        public ProgramRequirementsTable ProgramsTable = new ProgramRequirementsTable();

        private List<List<WinPoint>> _polylinesToRender = new List<List<WinPoint>>();
        public List<List<WinPoint>> PolylinesToRender 
        {
            get { return _polylinesToRender; }
            private set { _polylinesToRender = value; }
        }

        private (WinPoint startPoint, WinPoint endPoint) _newEdgePreview = ( new WinPoint(0, 0), new WinPoint(0, 0) );
        public (WinPoint startPoint, WinPoint endPoint) NewEdgePreview
		{
            get { return _newEdgePreview; }
            private set { _newEdgePreview = value; }
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
			this.PolylinesToRender.Add(new List<WinPoint>());
            this.IsInDrawingState = true;
        }

        private void ExecuteEndDrawing(object obj)
        {
            this.IsInDrawingState = false;
            this.NewEdgePreview = (new WinPoint(0, 0), new WinPoint(0, 0));
            this.GraphicsModified();
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
                if (this.PolylinesToRender.Count != 0 && this.PolylinesToRender.Last().Count != 0)
                {
                    this.NewEdgePreview = (this.NewEdgePreview.startPoint, new WinPoint(mea.LocationX, mea.LocationY));
                }
                this.GraphicsModified();
            }
        }

        public void HandleMouseLeftClickedEvent(object sender, EventArgs e)
        {
            if (this.IsInDrawingState)
            {
                var mea = (MouseEventArgs)e;
                if (this.PolylinesToRender != null)
                {
                    var newPoint = new WinPoint(mea.LocationX, mea.LocationY);
                    this.PolylinesToRender.Last().Add(newPoint);
                    this.NewEdgePreview = (newPoint, newPoint);
                }
                this.GraphicsModified();
            }
        }

        private void GraphicsModified()
        {
            this.OnPropertyChanged("GraphicsToRender");
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
