﻿using System;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Controls;

namespace DiagramDesigner
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
    {
        private MainViewModel MainViewModel;

        public MainWindow()
        {
            InitializeComponent();
            this.MainViewModel = new MainViewModel();
            this.MainViewModel.PropertyChanged += OnPropertyChanged;

            this.DataContext = MainViewModel;
            this.PrimaryDiagramCanvas.MouseMovedEventHandler += this.MainViewModel.HandleMouseMovedEvent;
            this.PrimaryDiagramCanvas.MouseLeftClickedEventHandler += this.MainViewModel.HandleMouseLeftClickedEvent;

            this.ProgramRequirementsTable.IsReadOnly = false; // unlock table
            this.ProgramRequirementsTable.DataContext = this.MainViewModel.ProgramRequirementsDataTable;
            this.MainViewModel.ProgramRequirementsDataTable.ColumnChanged += this.OnProgramRequirementsTableChanged;
            this.MainViewModel.ProgramRequirementsDataTable.RowChanged += this.OnProgramRequirementsTableChanged;
            this.MainViewModel.ProgramRequirementsDataTable.TableNewRow += this.OnProgramRequirementsTableChanged;
            this.MainViewModel.ProgramRequirementsDataTable.RowDeleted += this.OnProgramRequirementsTableChanged;
            this.MainViewModel.ProgramRequirementsDataTable.TableCleared += this.OnProgramRequirementsTableChanged;

            this.CurrentRulesTable.DataContext = this.MainViewModel.CurrentRulesTable;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
		{
            this.UpdateDiagramRendering();
            this.UpdateProgramsRequirementsPieChart();
            this.UpdateCurrentProgramsCharts();
            this.OrthogonalityCheckBox.IsChecked = this.MainViewModel.IsDrawingOrthogonally; 
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            switch (args.PropertyName)
            {
                case "GraphicsToRender":
                    this.UpdateDiagramRendering();
                    break;
                case "ChartsToRender":
                    this.UpdateCurrentProgramsCharts();
                    break;
                case "IsInDrawingState":
                    this.AddNewRuleButton.IsEnabled = this.MainViewModel.IsInDrawingState ? false : true;
                    this.EndOfRuleButton.IsEnabled = this.MainViewModel.IsInDrawingState ? true : false;
                    this.DonePickingContextButton.IsEnabled = this.MainViewModel.IsInDrawingState ? true : false;

                    this.PrimaryDiagramCanvas.Cursor = this.MainViewModel.IsInDrawingState ? Cursors.Cross : Cursors.Arrow;
                    
                    break;
                case "IsOrthogonalityToggleEnabled":
                    this.OrthogonalityCheckBox.IsEnabled = this.MainViewModel.IsOrthogonalityToggleEnabled;
                    break;
            }
        }

        private void OnProgramRequirementsTableChanged(object sender, EventArgs e)
        {
            this.UpdateProgramsRequirementsPieChart();
        }

        public void UpdateDiagramRendering()
        {
            PrimaryDiagramCanvas.RenderVisual(this.MainViewModel.WallsToRender, this.MainViewModel.NewEdgePreview, this.MainViewModel.ProgramsToRender, this.MainViewModel.DisplayUnitOverRealUnit);
        }

        public void UpdateProgramsRequirementsPieChart()
        {
            ProgramRequirementsChart.RenderPieChart(this.MainViewModel.ProgramRequirementsDataTable, "Name", "TotalArea");
        }

        public void UpdateCurrentProgramsCharts()
		{
            CurrentProgramsChart.RenderPieChart(this.MainViewModel.CurrentProgramsDataTable, "Name", "TotalArea");
		}

        public void HandelCurrentRulesTableColumnGenerated(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            string headername = e.Column.Header.ToString();
            switch (headername)
			{
                case "LeftHandShape":
                    e.Cancel = true;
                    break;
                case "RightHandShape":
                    e.Cancel = true;
                    break;
				default:
					break;
			}
        }
    }
}
