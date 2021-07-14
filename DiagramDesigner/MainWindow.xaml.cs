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
        private MainViewModel mainViewModel;

        public MainWindow()
        {
            InitializeComponent();
            this.mainViewModel = new MainViewModel();
            this.mainViewModel.PropertyChanged += OnPointsChanged;
            this.DataContext = mainViewModel;

            this.UpdateRendering();
        }

        private void OnPointsChanged(object sender, PropertyChangedEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine("OnPointsChanged");
            switch (args.PropertyName)
            {
                case "PointsToRender":
                    this.UpdateRendering();
                    break;
            }
        }

        public void UpdateRendering()
        {
            primaryDiagramCanvas.RenderVisual(this.mainViewModel.PointsToRender);
        }
    }
}
