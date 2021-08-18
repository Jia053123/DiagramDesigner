using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Input;
using System.Runtime.CompilerServices;
using DiagramDesignerEngine;
using System.Linq;
using WinPoint = System.Windows.Point;
using System.Data;
using DiagramDesignerModel;

namespace DiagramDesigner
{
    /// <summary>
    /// Manage events between the UI and the engine
    /// </summary>
    class MainViewModel : INotifyPropertyChanged
    {
        private DDModel Model = new DDModel();

        public double DisplayUnitOverRealUnit { get; set; } = 5;
        public DataTable ProgramRequirementsDataTable => this.Model.ProgramRequirements; 
        public List<List<WinPoint>> WallsToRender { get; private set; }
        public List<ProgramToRender> ProgramsToRender { get; private set; }

        private readonly (WinPoint startPoint, WinPoint endPoint) NewEdgePreviewDefault = (new WinPoint(0, 0), new WinPoint(0, 0));
        public (WinPoint startPoint, WinPoint endPoint) NewEdgePreview => NewEdgePreviewData is null ? NewEdgePreviewDefault : (NewEdgePreviewData.StartPoint, NewEdgePreviewData.EndPoint);
        private DirectedLine NewEdgePreviewData { get; set; } = null; 

        private bool _isInDrawingState = false;
        public bool IsInDrawingState
        {
            private set { SetProperty(ref _isInDrawingState, value); }
            get { return this._isInDrawingState; }
        }

        public ICommand StartDrawingCommand { set; get; }
        public ICommand EndDrawingCommand { set; get; }
        public ICommand ResolveProgramsCommand { get; set; }
        public ICommand AddNewProgramRequirementCommand { set; get; }

        public event PropertyChangedEventHandler PropertyChanged;

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public MainViewModel()
        {
            this.StartDrawingCommand = new DelegateCommand(ExecuteStartDrawing);
            this.EndDrawingCommand = new DelegateCommand(ExecuteEndDrawing);
            this.ResolveProgramsCommand = new DelegateCommand(ExecuteResolvePrograms);
            this.AddNewProgramRequirementCommand = new DelegateCommand(ExecuteAddNewRowToRequirementsTable);

            this.Model.ModelChanged += this.HandelGraphicsModified;
            this.RebuildGraphicsDataFromModel();
        }

        private void RebuildGraphicsDataFromModel()
		{
            // Walls
            this.WallsToRender = new List<List<WinPoint>>();
            foreach (WallEntity we in this.Model.WallEntities)
			{
                this.WallsToRender.Add(new List<WinPoint>());
                foreach (Point p in we.Geometry.PathsDefinedByPoints)
				{
                    this.WallsToRender.Last().Add(Utilities.ConvertPointToWindowsPoint(p, this.DisplayUnitOverRealUnit));
				}
			}

            // Programs
            this.ProgramsToRender = new List<ProgramToRender>();
            foreach (EnclosedProgram ep in this.Model.Programs)
			{
                var perimeter = new List<WinPoint>();
                foreach (Point p in ep.Perimeter)
				{
                    perimeter.Add(Utilities.ConvertPointToWindowsPoint(p, DisplayUnitOverRealUnit));
				}

                var innerPerimeters = new List<List<WinPoint>>();
                foreach (List<Point> innerPerimeter in ep.InnerPerimeters)
				{
                    innerPerimeters.Add(new List<WinPoint>());
                    foreach (Point p in innerPerimeter)
					{
                        innerPerimeters.Last().Add(Utilities.ConvertPointToWindowsPoint(p, DisplayUnitOverRealUnit));
					}
				}

                ProgramsToRender.Add(new ProgramToRender(perimeter, innerPerimeters, ep.Name, ep.Area));
			}
		}

        private void ExecuteStartDrawing(object obj)
        {
            this.Model.CreateNewWallEntity();
            this.IsInDrawingState = true;
        }

        private void ExecuteEndDrawing(object obj)
        {
            this.IsInDrawingState = false;
            this.NewEdgePreviewData = null; 
            this.HandelGraphicsModified(this, null);
        }

        private void ExecuteResolvePrograms(object obj)
		{
            this.Model.ResolvePrograms();
		}

        private void ExecuteAddNewRowToRequirementsTable(object obj)
        {
            try
			{
                this.ProgramRequirementsDataTable.Rows.Add(this.ProgramRequirementsDataTable.NewRow());
            }
            catch (System.Data.ConstraintException ex)
			{
                Logger.Error(ex, "Program Requirement Table Constraint Failed");
			}
        }

        public void HandleMouseMovedEvent(object sender, EventArgs e)
        {
            if (this.IsInDrawingState)
            {
                var mea = (MouseEventArgs)e;
                if (this.NewEdgePreviewData != null)
                {
                    this.NewEdgePreviewData.EndPoint = new WinPoint(mea.LocationX, mea.LocationY);
                    this.HandelGraphicsModified(this, null);
                }
            }
        }

        public void HandleMouseLeftClickedEvent(object sender, EventArgs e)
        {
            if (this.IsInDrawingState)
            {
                var mea = (MouseEventArgs)e;
                if (this.WallsToRender != null)
                {
                    var newPoint = new WinPoint(mea.LocationX, mea.LocationY);
                    this.Model.AddPointToWallEntityAtIndex(Utilities.ConvertWindowsPointToPoint(newPoint, this.DisplayUnitOverRealUnit), this.Model.WallEntities.Count-1);
                    if (this.NewEdgePreviewData is null)
					{
                        this.NewEdgePreviewData = new DirectedLine(newPoint, newPoint);
					} 
                    else
					{
                        this.NewEdgePreviewData.StartPoint = newPoint;
                    }
                }
            }
        }

        private void HandelGraphicsModified(object sender, EventArgs e)
        {
            if (sender == this.Model)
			{
                this.RebuildGraphicsDataFromModel();
            }
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
