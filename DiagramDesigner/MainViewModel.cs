using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using System.Runtime.CompilerServices;
using System.Linq;
using WinPoint = System.Windows.Point;
using MyPoint = BasicGeometries.Point;
using System.Data;
using DiagramDesignerModel;
using System.Diagnostics;
using BasicGeometries;
using ShapeGrammarEngine;
using System.Windows.Controls;
using System.Windows;

namespace DiagramDesigner
{
	/// <summary>
	/// Manage events between the UI and the engine
	/// </summary>
	class MainViewModel : INotifyPropertyChanged
    {
        public double DisplayUnitOverRealUnit { get; set; } = 5;

		private DiagramDesignerModel.DiagramDesignerModel Model = new DiagramDesignerModel.DiagramDesignerModel();
        public DataTable ProgramRequirementsDataTable => this.Model.ProgramRequirements;
        public DataTable GrammarRulesDataTable => this.Model.CurrentRulesInfo;
        public DataTable LayersDataTable { get; } = new LayersDataTable();
        public ProgramsSummaryTable CurrentProgramsDataTable { get;} = new ProgramsSummaryTable(); // for the pie chart

        /// <summary>
        /// Walls to be displayed by the view. They are guaranteed to match the WallEntities property in Model, with each sub list corresponding to each WallEntity in Model
        /// When trying to add or remove elements, do that by calling Model's methods and WallsToRender will be updated automatically through events
        /// </summary>
        public List<List<WinPoint>> WallsToRender { get; private set; } = new List<List<WinPoint>>(); // TODO: refactor with ReadOnlyCollection
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

        private Guid? _currentlySelectedRule = null;
        public Guid? CurrentlySelectedRule
		{
            private set
			{
                SetProperty(ref _currentlySelectedRule, value);
			}
            get { return this._currentlySelectedRule; }
		}

        private DraftingConstrainsApplier draftingConstrainsApplier;
        private ModelScreenGeometriesConverter modelScreenGeometriesConverter;

        public bool IsDrawingOrthogonally => this.draftingConstrainsApplier.DoesDrawOrthogonally;

        private bool _doesAcceptChangeInOrthogonalityOption = true;
        public bool DoesAcceptChangeInOrthogonalityOption
		{
            private set { SetProperty(ref _doesAcceptChangeInOrthogonalityOption, value); }
            get { return this._doesAcceptChangeInOrthogonalityOption; }
        }

        public ICommand AddNewLayerCommand { set; get; }
        public ICommand StartDrawingCommand { set; get; }
        public ICommand EndDrawingCommand { set; get; }
        public ICommand CreateNewRuleCommand { set; get; }
        public ICommand DonePickingContextForRuleCreationCommand { set; get; }
        public ICommand DoneCreatingRuleCommand { set; get; }
        public ICommand RepeatSelectedRuleCommand { set; get; }
        public ICommand DonePickingContextForRuleRepetitionCommand { set; get; }
        public ICommand DoneRepeatingRuleCommand { set; get; }
        public ICommand ApplySelectedRuleCommand { set; get; }
        public ICommand DonePickingContextAndApplyRuleCommand { set; get; }
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

            this.CreateNewRuleCommand = new DelegateCommand(ExecuteCreateNewRule);
            this.DonePickingContextForRuleCreationCommand = new DelegateCommand(ExecuteDonePickingContextForRuleCreation);
            this.DoneCreatingRuleCommand = new DelegateCommand(ExecuteDoneAddingRule);

            this.RepeatSelectedRuleCommand = new DelegateCommand(ExecuteRepeatSelectedRule);
            this.DonePickingContextForRuleRepetitionCommand = new DelegateCommand(ExecuteDonePickingContextForRuleRepetition);
            this.DoneRepeatingRuleCommand = new DelegateCommand(ExecuteDoneRepeatingSelectedRule);

            this.ApplySelectedRuleCommand = new DelegateCommand(ExecuteApplySelectedRule);
            this.DonePickingContextAndApplyRuleCommand = new DelegateCommand(ExecuteDonePickingContextAndApplySelectedRule);

            this.ClearGeometryCommand = new DelegateCommand(ExecuteClearGeometry);
            this.ResolveProgramsCommand = new DelegateCommand(ExecuteResolvePrograms);
            this.AddNewProgramRequirementCommand = new DelegateCommand(ExecuteAddNewRowToRequirementsTable);
            this.ToggleOrthogonalDrawingCommand = new DelegateCommand(ExecuteToggleOrthogonalDrawing);

            this.Model.ModelChanged += this.HandelGraphicsModified;
            this.Model.ModelChanged += this.HandelProgramsModified;

            this.draftingConstrainsApplier = new DraftingConstrainsApplier();
            this.draftingConstrainsApplier.DoesDrawOrthogonally = false;

            this.modelScreenGeometriesConverter = new ModelScreenGeometriesConverter(this.DisplayUnitOverRealUnit);

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
                foreach (MyPoint p in we.Geometry.PathsDefinedByPoints)
				{
                    this.WallsToRender.Last().Add(MathUtilities.ConvertRealScaledPointToWindowsPointOnScreen(p, this.DisplayUnitOverRealUnit));
				}
			}

            // Programs
            this.ProgramsToRender = new List<ProgramToRender>();
            foreach (EnclosedProgram ep in this.Model.Programs)
			{
                var perimeter = new List<WinPoint>();
                foreach (MyPoint p in ep.Perimeter)
				{
                    perimeter.Add(MathUtilities.ConvertRealScaledPointToWindowsPointOnScreen(p, DisplayUnitOverRealUnit));
				}

                var innerPerimeters = new List<List<WinPoint>>();
                foreach (List<MyPoint> innerPerimeter in ep.InnerPerimeters)
				{
                    innerPerimeters.Add(new List<WinPoint>());
                    foreach (MyPoint p in innerPerimeter)
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
            this.WallsToHighlightAsContext.Clear();
            this.WallsToHighlightAsAdditions.Clear();
            this.draftingConstrainsApplier.ClearLastAddedPoint();

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

        private void ExecuteCreateNewRule(object obj)
        {
            this.Model.CreateNewWallEntity();
            this.State = MainViewModelState.RuleCreationContextPickingState;
        }

        private void ExecuteDonePickingContextForRuleCreation(object obj)
		{
            this.State = MainViewModelState.RuleCreationEditingState;
		}

        private void ExecuteDoneAddingRule(object obj)
        {
            var geo = this.modelScreenGeometriesConverter.GenerateGeometriesFromContextAndAdditions(this.WallsToRender, this.WallsToHighlightAsContext, this.WallsToHighlightAsAdditions);
			try
			{
				this.Model.CreateNewRuleFromExample(geo.Item1, geo.Item2);
			}
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Rule Creation Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

			this.State = MainViewModelState.ViewingState;
            this.CleanUpTempDataForDrawing();
        }

        private void ExecuteRepeatSelectedRule(object obj)
		{
            this.Model.CreateNewWallEntity(); // TODO: Move to ExecuteDonePickingContext
            this.State = MainViewModelState.RuleRepetitionContextPickingState;
		}

		private void ExecuteDonePickingContextForRuleRepetition(object obj)
		{
            this.State = MainViewModelState.RuleRepetitionEditingState;
		}

        private void ExecuteDoneRepeatingSelectedRule(object obj)
		{
            if (this.CurrentlySelectedRule == null)
			{
                throw new NoRuleSelectedException();
			}

            var geos = this.modelScreenGeometriesConverter.GenerateGeometriesFromContextAndAdditions(this.WallsToRender, this.WallsToHighlightAsContext, this.WallsToHighlightAsAdditions);
            try
			{
                this.Model.LearnFromExampleForRule(geos.Item1, geos.Item2, (Guid)this.CurrentlySelectedRule);
            }
            catch (Exception e)
			{
                MessageBox.Show(e.Message, "Repeat Rule Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
            
            this.State = MainViewModelState.ViewingState;
            this.CleanUpTempDataForDrawing();
		}

        private void ExecuteApplySelectedRule(object obj)
		{
            this.State = MainViewModelState.RuleApplicationContextPickingState;
		}

        private void ExecuteDonePickingContextAndApplySelectedRule(object obj)
		{
			if (this.CurrentlySelectedRule == null)
			{
				throw new NoRuleSelectedException();
			}

			// Step1: generate new right hand geometry
			var contextGeo = this.modelScreenGeometriesConverter.GenerateLeftHandGeometryFromContext(this.WallsToRender, this.WallsToHighlightAsContext);
			PolylinesGeometry newGeo;
			try
			{
				newGeo = this.Model.ApplyRuleGivenLeftHandGeometry(contextGeo, (Guid)this.CurrentlySelectedRule);
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message, "Apply Rule Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
			}

            // Step2: erase selected context segmenets. It is important to sort the segments so that deleting one does not change the subsequent indexes
            var wallsToHighlightAsContextInDescendingOrder = new List<Tuple<int, int, int>>(this.WallsToHighlightAsContext);
            wallsToHighlightAsContextInDescendingOrder.Sort(delegate (Tuple<int, int, int> w1, Tuple<int, int, int> w2)
			{
				if (w1.Item1 != w2.Item1) { return -1 * (w1.Item1 - w2.Item1); }
                else { return -1 * (w1.Item2 - w2.Item2); }
            });
			foreach (Tuple<int, int, int> segment in wallsToHighlightAsContextInDescendingOrder)
			{
				this.EraseWallSegment(segment);
			}

            // Step3: draw the right hand geometry
            var polylinesInRealUnit = newGeo.PolylinesCopy;
            var polylinesInScreenUnit = this.modelScreenGeometriesConverter.ConvertPolylinesInPointsToGeometriesOnScreen(polylinesInRealUnit);
            foreach (List<WinPoint> polyline in polylinesInScreenUnit)
			{
				this.Model.CreateNewWallEntity();
                foreach (WinPoint p in polyline)
				{
                    this.AddNewPoint(p);
				}
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
                this.draftingConstrainsApplier.DoesDrawOrthogonally = isOrthogonal;
            }
		}

        public void HandleMouseMovedEvent(object sender, EventArgs e)
        {
            var mea = (MouseEventArgs)e;
            switch (this.State)
            {
                case MainViewModelState.NormalEditingState:
                case MainViewModelState.RuleCreationEditingState:
                case MainViewModelState.RuleRepetitionEditingState:
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
                case MainViewModelState.RuleCreationContextPickingState:
                    this.MouseLeftClickedInRuleCreationContextPickingState(mea);
                    break;
                case MainViewModelState.RuleRepetitionContextPickingState:
                    this.MouseLeftClickedInRuleRepetitionContextPickingState(mea);
                    break;
                case MainViewModelState.RuleRepetitionEditingState:
                    this.MouseLeftClickedInRuleRepetitionEditingState(mea);
                    break;
                case MainViewModelState.RuleApplicationContextPickingState:
                    this.MouseLeftClickedInRuleApplicationContextPickingState(mea);
                    break;
                default:
					break;
            }
        }

        public void HandleSelectedCellsChangedEvent(object obj, SelectedCellsChangedEventArgs e)
        {
            IList<DataGridCellInfo> selectedcells = e.AddedCells;
            if (selectedcells.Count > 0)
			{
                // a new row is selected
                DataRowView? drv = null;
                foreach (DataGridCellInfo info in selectedcells) 
                {
                    drv = info.Item as DataRowView;
                    if (drv != null)
					{
                        break;
					}
                }
                if (drv != null)
				{
                    DataRow cr = drv.Row;
                    this.CurrentlySelectedRule = (Guid)cr["ID"];
                    Debug.WriteLine("select rule id#: " + this.CurrentlySelectedRule.ToString());
                }
            }
            else
			{
                // no row is being selected
                this.CurrentlySelectedRule = null;
                Debug.WriteLine("deselect rule");
			}
        }
        private void MouseLeftClickedInNormalEditingState(MouseEventArgs mea)
        {
			_ = this.AddNewPointFromMouseLeftClick(mea);
		}

		private void MouseLeftClickedInRuleCreationEditingState(MouseEventArgs mea)
		{
			var success = this.AddNewPointFromMouseLeftClick(mea);
			if (success)
			{
				this.HighlightLastAddition();
			}
		}

		private void MouseLeftClickedInRuleRepetitionEditingState(MouseEventArgs mea)
		{
			var success = this.AddNewPointFromMouseLeftClick(mea);
			if (success)
			{
				this.HighlightLastAddition();
			}
		}

        /// <summary>
        /// Handle adding a new point from a left mouse click
        /// </summary>
        /// <param name="mea"> the mouse event of a left click </param>
        /// <returns> Whether the addition succeeded </returns>
        private bool AddNewPointFromMouseLeftClick(MouseEventArgs mea)
		{
			var newPoint = new WinPoint(mea.LocationX, mea.LocationY);
			newPoint = this.draftingConstrainsApplier.ApplyAllRestrictions(newPoint, this.WallsToRender);
			var success = this.AddNewPoint(newPoint);
            if (success)
			{
                if (this.NewEdgePreviewData is null)
                {
                    this.NewEdgePreviewData = new DirectedLine(newPoint, newPoint);
                }
                else
                {
                    this.NewEdgePreviewData.StartPoint = newPoint;
                }
            }
            return success;
        }

        /// <summary>
        /// Add a new point to the last WallEntity
        /// </summary>
        /// <param name="newPoint"> the new point to add </param>
        /// <returns> whether the operation is successful </returns>
		private bool AddNewPoint(WinPoint newPoint)
		{
            if (this.WallsToRender != null) // TODO: remove this check and return value
            {
                this.Model.AddPointToWallEntityAtIndex(MathUtilities.ConvertWindowsPointOnScreenToRealScalePoint(newPoint, this.DisplayUnitOverRealUnit), this.Model.WallEntities.Count - 1);
                this.draftingConstrainsApplier.UpdateLastAddedPoint(newPoint);
                return true;
            }
            else
			{
                return false;
			}
        }

        /// <summary>
        /// Erase a specific wall segment from all walls on screen and from the Model. 
        /// If that leaves the containing WallEntity in Model with only a single point, that WallEntity object will also be removed. 
        /// </summary>
        /// <param name="segmentToRemove"> The wall segments to remove;
        /// each Tuple specifies the index of the geometry within allGeometries, and the two ascending consecutive indexes indicating the line segment within the geometry </param>
        private void EraseWallSegment(Tuple<int, int, int> segmentToRemove)
		{
            this.Model.RemoveSegmentFromWallEntityAtIndex(segmentToRemove.Item2, segmentToRemove.Item3, segmentToRemove.Item1);
		}

        private void HighlightLastAddition()
		{
            var wallIndex = this.WallsToRender.Count - 1;
            var pointIndex1 = this.WallsToRender.Last().Count - 2;
            var pointIndex2 = this.WallsToRender.Last().Count - 1;
            if (pointIndex1 >= 0 && pointIndex2 >= 0)
            {
                this.WallsToHighlightAsAdditions.Add(new Tuple<int, int, int>(wallIndex, pointIndex1, pointIndex2));
            }
        }

        private void MouseLeftClickedInRuleCreationContextPickingState(MouseEventArgs mea)
		{
            this.MouseLeftClickedInContextPickingState(mea);
		}

		private void MouseLeftClickedInRuleRepetitionContextPickingState(MouseEventArgs mea)
		{
            this.MouseLeftClickedInContextPickingState(mea);
        }

        private void MouseLeftClickedInRuleApplicationContextPickingState(MouseEventArgs mea)
        {
            this.MouseLeftClickedInContextPickingState(mea);
        }

        private void MouseLeftClickedInContextPickingState(MouseEventArgs mea)
		{
            var result = this.draftingConstrainsApplier.FindLineClicked(new WinPoint(mea.LocationX, mea.LocationY), this.WallsToRender);
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

    class NoRuleSelectedException : Exception
    {
        public NoRuleSelectedException() { }
        public NoRuleSelectedException(string message) : base(message) { }
    }
}
