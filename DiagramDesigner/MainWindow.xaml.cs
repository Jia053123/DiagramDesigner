using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Windows.Interop;
using System.Runtime.InteropServices;

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
            this.UpdateDiagramRendering();

            this.ProgramRequirementsTable.DataContext = this.MainViewModel.ProgramsTable;
            this.MainViewModel.ProgramsTable.ColumnChanged += this.OnProgramRequirementsTableChanged;
            this.MainViewModel.ProgramsTable.RowChanged += this.OnProgramRequirementsTableChanged;
            this.MainViewModel.ProgramsTable.TableNewRow += this.OnProgramRequirementsTableChanged;
            this.MainViewModel.ProgramsTable.RowDeleted += this.OnProgramRequirementsTableChanged;
            this.MainViewModel.ProgramsTable.TableCleared += this.OnProgramRequirementsTableChanged;
            this.UpdateProgramsRequirementsPieChart();
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine("OnPropertyChanged");
            switch (args.PropertyName)
            {
                case "PointsToRender":
                    this.UpdateDiagramRendering();
                    break;
                case "IsInDrawingState":
                    this.PrimaryDiagramCanvas.Cursor = this.MainViewModel.IsInDrawingState ? Cursors.Cross : Cursors.Arrow;
                    break;
            }
        }

        private void OnProgramRequirementsTableChanged(object sender, EventArgs e)
        {
            this.UpdateProgramsRequirementsPieChart();
        }

        public void UpdateDiagramRendering()
        {
            PrimaryDiagramCanvas.RenderVisual(this.MainViewModel.PointsToRender);
        }

        public void UpdateProgramsRequirementsPieChart()
        {
            ProgramRequirementsChart.RenderPieChart(this.MainViewModel.ProgramsTable, "Name", "TotalArea");
        }
    }
}
