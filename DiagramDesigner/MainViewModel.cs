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
        public DataTable CurrentShapesTable => this.Model.CurrentShapes;
        public ProgramsSummaryTable CurrentProgramsDataTable { get;} = new ProgramsSummaryTable(); // for the pie chart
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
        public ICommand ClearGeometryCommand { set; get; }
        public ICommand ResolveProgramsCommand { get; set; }
        public ICommand AddNewProgramRequirementCommand { set; get; }

        public event PropertyChangedEventHandler PropertyChanged;

        private static readonly NLog.Logger Logger = NLog.LogManager.GetLogger("Default");

        public MainViewModel()
        {
            this.StartDrawingCommand = new DelegateCommand(ExecuteStartDrawing);
            this.EndDrawingCommand = new DelegateCommand(ExecuteEndDrawing);
            this.ClearGeometryCommand = new DelegateCommand(ExecuteClearGeometry);
            this.ResolveProgramsCommand = new DelegateCommand(ExecuteResolvePrograms);
            this.AddNewProgramRequirementCommand = new DelegateCommand(ExecuteAddNewRowToRequirementsTable);

            this.Model.ModelChanged += this.HandelGraphicsModified;
            this.Model.ModelChanged += this.HandelProgramsModified;
            this.RebuildGraphicsDataFromModel();

            Logger.Debug("MainViewModel initialized");
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

        private void RebuildProgramsTableFromModel()
		{
            this.CurrentProgramsDataTable.Clear();
            foreach (EnclosedProgram program in this.Model.Programs)
			{
                var name = program.Name;
                var area = program.Area;

				if (this.CurrentProgramsDataTable.Select($@"Name = '{ name }'").Count() == 0)
				{
					var newRow = this.CurrentProgramsDataTable.NewRow();
                    newRow["Name"] = name;
                    newRow["TotalArea"] = 0.0;
                    this.CurrentProgramsDataTable.Rows.Add(newRow);
                }
				
                var rowForTheProgram = this.CurrentProgramsDataTable.Select($@"Name = '{ name }'").First();
                rowForTheProgram["TotalArea"] = (double)rowForTheProgram["TotalArea"] + area;
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

        private void ExecuteClearGeometry(object obj)
		{
            this.ExecuteEndDrawing(obj);
            this.Model.RemoveAllWallsAndPrograms();
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
                    
                    var pointCloseBy = this.FindPointCloseBy(newPoint);
                    var pointOnLineCloseBy = this.FindPointOnLine(newPoint);
                    if (!(pointCloseBy is null))
					{
                        // prioritize pointCloseBy
                        newPoint = (WinPoint)pointCloseBy;
					} 
                    else if (!(pointOnLineCloseBy is null))
					{
                        newPoint = (WinPoint)pointOnLineCloseBy;
					}
                    
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

        /// <summary>
        /// Find a Windows Point close by measured by pixel. 
        /// This is intended for the UI feature that snap new points to existing points close by.
        /// </summary>
        /// <param name="wPoint"> The new point </param>
        /// <returns> A point in WallEntity on screen in close proximity, or null if no existing point qualifies </returns>
        private WinPoint? FindPointCloseBy(WinPoint wPoint)
        {
            const double maxDistance = 5;
            for (int i = 0; i < this.WallsToRender.Count; i++)
            {
                for (int j = 0; j < this.WallsToRender[i].Count; j++)
				{
                    var p = this.WallsToRender[i][j];
                    if (Utilities.DistanceBetweenWinPoints(wPoint, p) <= maxDistance)
					{
                        return p;
					}
				}
            }
            return null;
        }

        /// <summary>
        /// Find a Windows Point on a line close by measured by pixel. 
        /// This is intended for the UI feature that snap new points to existing lines close by. 
        /// </summary>
        /// <param name="wPoint"> The new point </param>
        /// <returns> A point on a line segment on screen in close proximity, or null if no line qualifies </returns>
        private WinPoint? FindPointOnLine(WinPoint wPoint)
		{
            const double maxDistance = 5;
            for (int i = 0; i < this.WallsToRender.Count; i++)
            {
                for (int j = 0; j < this.WallsToRender[i].Count-1; j++)
                {
                    var endPoint1 = this.WallsToRender[i][j];
                    var endPoint2 = this.WallsToRender[i][j+1];
                    var result = Utilities.DistanceFromWinPointToLine(wPoint, endPoint1, endPoint2);
                    if (!(result is null) && result.Item1 <= maxDistance)
                    {
                        return result.Item2;
                    }
                }
            }
            return null; // stub
		}

        private void HandelGraphicsModified(object sender, EventArgs e)
        {
            if (sender == this.Model)
			{
                this.RebuildGraphicsDataFromModel();
            }
            this.OnPropertyChanged("GraphicsToRender");
        }

        private void HandelProgramsModified(object sender, EventArgs e)
        {
            if (sender == this.Model)
            {
                this.RebuildProgramsTableFromModel();
            }
            this.OnPropertyChanged("ChartsToRender");
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
