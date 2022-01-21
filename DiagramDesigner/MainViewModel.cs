using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using System.Runtime.CompilerServices;
using System.Linq;
using WinPoint = System.Windows.Point;
using System.Data;
using DiagramDesignerModel;
using System.Diagnostics;
using BasicGeometries;
using ShapeGrammarEngine;

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
        public DataTable GrammarRulesDataTable => this.Model.CurrentRules;
        public DataTable LayersDataTable { get; } = new LayersDataTable(); // TODO: should this be stored here? 
        public ProgramsSummaryTable CurrentProgramsDataTable { get;} = new ProgramsSummaryTable(); // for the pie chart
        public List<List<WinPoint>> WallsToRender { get; private set; }

        /// <summary>
        /// Walls to be highlighted as the context. The three integers represent the index of the geometry from WallsToRender, and
        /// the two consecutive indexes in ascending order of the points representing the line on the geometry
        /// </summary>
        public List<Tuple<int, int, int>> WallsToHighlightAsContext { get; private set; } = new List<Tuple<int, int, int>>();
        /// <summary>
        /// Walls to be highlighted as additions in the rule creation phase. The three integers represent the index of the geometry from WallsToRender, and
        /// the two consecutive indexes in ascending order of the points representing the line on the geometry
        /// </summary>
        public List<Tuple<int, int, int>> WallsToHighlightAsAdditions { get; private set; } = new List<Tuple<int, int, int>>();
        public List<ProgramToRender> ProgramsToRender { get; private set; }

        private readonly (WinPoint startPoint, WinPoint endPoint) NewEdgePreviewDefault = (new WinPoint(0, 0), new WinPoint(0, 0));
        public (WinPoint startPoint, WinPoint endPoint) NewEdgePreview => NewEdgePreviewData is null ? NewEdgePreviewDefault : (NewEdgePreviewData.StartPoint, NewEdgePreviewData.EndPoint);
        private DirectedLine NewEdgePreviewData { get; set; } = null;

        private WinPoint? LastAddedPointInEditingState = null;

        private MainViewModelState _state = MainViewModelState.ViewingState;
        public MainViewModelState State
        {
            private set 
            {
                this.DoesAcceptChangeInOrthogonalityOption = value != MainViewModelState.ViewingState;
                SetProperty(ref _state, value); 
            }
            get { return this._state; }
        }

        private DraftingManager ConstrainsApplier;

        public bool IsDrawingOrthogonally => this.ConstrainsApplier.IsDrawingOrthogonally;

        private bool _doesAcceptChangeInOrthogonalityOption = true;
        public bool DoesAcceptChangeInOrthogonalityOption
		{
            private set { SetProperty(ref _doesAcceptChangeInOrthogonalityOption, value); }
            get { return this._doesAcceptChangeInOrthogonalityOption; }
        }

        public ICommand AddNewLayerCommand { set; get; }
        public ICommand StartDrawingCommand { set; get; }
        public ICommand EndDrawingCommand { set; get; }
        public ICommand AddNewRuleCommand { set; get; }
        public ICommand DonePickingContextCommand { set; get; }
        public ICommand DoneAddingRuleCommand { set; get; }
        public ICommand ClearGeometryCommand { set; get; }
        public ICommand ResolveProgramsCommand { get; set; }
        public ICommand AddNewProgramRequirementCommand { set; get; }
        public ICommand ToggleOrthogonalDrawingCommand { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private static readonly NLog.Logger Logger = NLog.LogManager.GetLogger("Default");

        public MainViewModel()
        {
            this.AddNewLayerCommand = new DelegateCommand(ExecuteAddNewLayer);
            this.StartDrawingCommand = new DelegateCommand(ExecuteStartDrawing);
            this.EndDrawingCommand = new DelegateCommand(ExecuteEndDrawing);
            this.AddNewRuleCommand = new DelegateCommand(ExecuteAddNewRule);
            this.DonePickingContextCommand = new DelegateCommand(ExecuteDonePickingContext);
            this.DoneAddingRuleCommand = new DelegateCommand(ExecuteDoneAddingRule);
            this.ClearGeometryCommand = new DelegateCommand(ExecuteClearGeometry);
            this.ResolveProgramsCommand = new DelegateCommand(ExecuteResolvePrograms);
            this.AddNewProgramRequirementCommand = new DelegateCommand(ExecuteAddNewRowToRequirementsTable);
            this.ToggleOrthogonalDrawingCommand = new DelegateCommand(ExecuteToggleOrthogonalDrawing);

            this.Model.ModelChanged += this.HandelGraphicsModified;
            this.Model.ModelChanged += this.HandelProgramsModified;
            this.RebuildGraphicsDataFromModel();

            this.ConstrainsApplier = new DraftingManager(this.WallsToRender);
            this.ConstrainsApplier.IsDrawingOrthogonally = false;

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
                    this.WallsToRender.Last().Add(MathUtilities.ConvertRealScaledPointToWindowsPointOnScreen(p, this.DisplayUnitOverRealUnit));
				}
			}

            // Programs
            this.ProgramsToRender = new List<ProgramToRender>();
            foreach (EnclosedProgram ep in this.Model.Programs)
			{
                var perimeter = new List<WinPoint>();
                foreach (Point p in ep.Perimeter)
				{
                    perimeter.Add(MathUtilities.ConvertRealScaledPointToWindowsPointOnScreen(p, DisplayUnitOverRealUnit));
				}

                var innerPerimeters = new List<List<WinPoint>>();
                foreach (List<Point> innerPerimeter in ep.InnerPerimeters)
				{
                    innerPerimeters.Add(new List<WinPoint>());
                    foreach (Point p in innerPerimeter)
					{
                        innerPerimeters.Last().Add(MathUtilities.ConvertRealScaledPointToWindowsPointOnScreen(p, DisplayUnitOverRealUnit));
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

        private void CleanUpTempDataForDrawing()
		{
            this.NewEdgePreviewData = null;
            this.LastAddedPointInEditingState = null;
            this.WallsToHighlightAsContext.Clear();
            this.WallsToHighlightAsAdditions.Clear();
            this.HandelGraphicsModified(this, null);
        }

        private void ExecuteAddNewLayer(object obj)
		{
            try
            {
                this.LayersDataTable.Rows.Add(this.LayersDataTable.NewRow());
            }
            catch (System.Data.ConstraintException ex)
            {
                Logger.Error(ex, "Layers Table Constraint Failed");
            }
        }

        private void ExecuteStartDrawing(object obj)
		{
            this.Model.CreateNewWallEntity();
            this.State = MainViewModelState.NormalEditingState;
        }

        private void ExecuteEndDrawing(object obj)
		{
            this.State = MainViewModelState.ViewingState;
            this.CleanUpTempDataForDrawing();
        }

        private void ExecuteAddNewRule(object obj)
        {
            this.Model.CreateNewWallEntity();
            this.State = MainViewModelState.ContextPickingState;
        }

        private void ExecuteDonePickingContext(object obj)
		{
            // convert selection to connections of points in wall entities
            



            this.State = MainViewModelState.RuleCreationEditingState;
		}

        private void ExecuteDoneAddingRule(object obj)
        {
            // create the rule
            // TODO: stub
            try
            {
                this.GrammarRulesDataTable.Rows.Add(this.GrammarRulesDataTable.NewRow());
            }
            catch (System.Data.ConstraintException ex)
            {
                Logger.Error(ex, "Grammar Table Constraint Failed");
            }


            this.State = MainViewModelState.ViewingState;
            this.CleanUpTempDataForDrawing();
        }

        private void ExecuteClearGeometry(object obj)
		{
            this.ExecuteDoneAddingRule(obj);
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

        private void ExecuteToggleOrthogonalDrawing(object obj)
		{
            if (this.DoesAcceptChangeInOrthogonalityOption)
			{
                bool isOrthogonal = (bool)obj;
                this.ConstrainsApplier.IsDrawingOrthogonally = isOrthogonal;
            }
		}

        public void HandleMouseMovedEvent(object sender, EventArgs e)
        {
            var mea = (MouseEventArgs)e;
            switch (this.State)
            {
                case MainViewModelState.NormalEditingState:
                case MainViewModelState.RuleCreationEditingState: 
					if (this.NewEdgePreviewData != null)
					{
						this.NewEdgePreviewData.EndPoint = new WinPoint(mea.LocationX, mea.LocationY);
						this.HandelGraphicsModified(this, null);
					}
					break;
                default:
                    break;
            }
        }

        public void HandleMouseLeftClickedEvent(object sender, EventArgs e)
        {
            var mea = (MouseEventArgs)e;
            switch (this.State)
            {
                case MainViewModelState.NormalEditingState:
                    this.MouseLeftClickedInNormalEditingState(mea);
                    break;
                case MainViewModelState.RuleCreationEditingState:
                    this.MouseLeftClickedInRuleCreationEditingState(mea);
					break;
                case MainViewModelState.ContextPickingState:
                    this.MouseLeftClickedInContextPickingState(mea);
                    break;
                default:
                    break;
            }
        }

        private void MouseLeftClickedInRuleCreationEditingState(MouseEventArgs mea)
		{
            if (this.WallsToRender != null)
            {
                var newPoint = new WinPoint(mea.LocationX, mea.LocationY);

                // handle orthogonal restrictions
                if (this.IsDrawingOrthogonally)
                {
                    if (!(this.LastAddedPointInEditingState is null))
                    {
                        newPoint = MathUtilities.PointOrthogonal((WinPoint)this.LastAddedPointInEditingState, newPoint);
                    }
                }

                // snap to point or line nearby
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

                this.Model.AddPointToWallEntityAtIndex(MathUtilities.ConvertWindowsPointOnScreenToRealScalePoint(newPoint, this.DisplayUnitOverRealUnit), this.Model.WallEntities.Count - 1);
                this.WallsToHighlightAsAdditions.Add(new Tuple<int, int, int>(this.WallsToRender.Count - 1, this.WallsToRender.Last().Count - 2, this.WallsToRender.Last().Count - 1));

                this.LastAddedPointInEditingState = newPoint;
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

        private void MouseLeftClickedInNormalEditingState(MouseEventArgs mea)
		{
            if (this.WallsToRender != null)
            {
                var newPoint = new WinPoint(mea.LocationX, mea.LocationY);

                // handle orthogonal restrictions
                if (this.IsDrawingOrthogonally)
                {
                    if (!(this.LastAddedPointInEditingState is null))
                    {
                        newPoint = MathUtilities.PointOrthogonal((WinPoint)this.LastAddedPointInEditingState, newPoint);
                    }
                }

                // snap to point or line nearby
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

                this.Model.AddPointToWallEntityAtIndex(MathUtilities.ConvertWindowsPointOnScreenToRealScalePoint(newPoint, this.DisplayUnitOverRealUnit), this.Model.WallEntities.Count - 1);
                this.LastAddedPointInEditingState = newPoint;
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

        private void MouseLeftClickedInContextPickingState(MouseEventArgs mea)
		{
            var result = this.FindLineClicked(new WinPoint(mea.LocationX, mea.LocationY));
            if (!(result is null)) 
            {
                if (!this.WallsToHighlightAsContext.Contains(result))
				{
                    this.WallsToHighlightAsContext.Add(result);
                }
                else
				{
                    var s = this.WallsToHighlightAsContext.Remove(result);
                    Debug.Assert(s);
				}
                this.HandelGraphicsModified(this, null);
            } 
		}

        /// <summary>
        /// Find the line segment on screen clicked
        /// </summary>
        /// <param name="clickLocation"> location of the click </param>
        /// <returns> a tuple containing the index of the geometry, 
        /// the two consecutive indexes in ascending order of the points representing the line on the geometry, 
        /// or null if no line is clicked </returns>
        private Tuple<int, int, int> FindLineClicked(WinPoint clickLocation)
		{
            const double tolerance = 2;
            for (int i = 0; i < this.WallsToRender.Count; i++)
            {
                for (int j = 0; j < this.WallsToRender[i].Count - 1; j++)
                {
                    var endPoint1 = this.WallsToRender[i][j];
                    var endPoint2 = this.WallsToRender[i][j + 1];
                    var result = MathUtilities.DistanceFromWinPointToLine(clickLocation, endPoint1, endPoint2);
                    if (!(result is null) && result.Item1 <= tolerance)
                    {
                        Debug.Assert(j + 1 < this.WallsToRender[i].Count);
                        return new Tuple<int, int, int>(i, j, j + 1);
                    }
                }
            }
            return null;
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
                    if (MathUtilities.DistanceBetweenWinPoints(wPoint, p) <= maxDistance)
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
                    var result = MathUtilities.DistanceFromWinPointToLine(wPoint, endPoint1, endPoint2);
                    if (!(result is null) && result.Item1 <= maxDistance)
                    {
                        return result.Item2;
                    }
                }
            }
            return null; 
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
