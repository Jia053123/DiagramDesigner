using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DiagramDesignerEngine;
using Point = DiagramDesignerEngine.Point;

namespace DiagramDesigner
{
    partial class DiagramRenderingCanvas: Canvas
    {
        private System.Windows.Point LastClickedLocation = new System.Windows.Point(0, 0);

        private DrawingVisual sourceVisual = null;

        public DiagramRenderingCanvas()
        {
            this.Background = Brushes.White;
            this.sourceVisual = new DrawingVisual();
            AddVisualChild(sourceVisual);
            AddLogicalChild(sourceVisual);
        }

        public void RenderVisual(List<Point> pointsToRender)
        {
            using (DrawingContext dc = sourceVisual.RenderOpen())
            {
                //TODO: draw!
                for (int i = 0; i < pointsToRender.Count-1; i++)
                {
                    var startPoint = new System.Windows.Point((int)pointsToRender[i].coordinateX, (int)pointsToRender[i].coordinateY);
                    var endPoint = new System.Windows.Point((int)pointsToRender[i + 1].coordinateX, (int)pointsToRender[i + 1].coordinateY);
                    dc.DrawLine(new Pen(Brushes.Black, 1), startPoint, endPoint);
                }
            }
        }

        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index != 0)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            return sourceVisual;
        }        
        
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            System.Windows.Point location = e.GetPosition(this);
            this.LastClickedLocation = location;

            if (this.Command != null)
            {
                this.Command.Execute(this.LastClickedLocation);
            }
        }
    }



    partial class DiagramRenderingCanvas : ICommandSource
    {
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            "Command",
            typeof(ICommand),
            typeof(DiagramRenderingCanvas),
            new PropertyMetadata((ICommand)null, new PropertyChangedCallback(CommandChanged)));
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public object CommandParameter { get { return this.LastClickedLocation; } }
        public IInputElement CommandTarget { get { return null; } }

        // Command dependency property change callback.
        private static void CommandChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            DiagramRenderingCanvas drc = (DiagramRenderingCanvas)d;
            drc.HookUpCommand((ICommand)e.OldValue, (ICommand)e.NewValue);
        }

        // Add a new command to the Command Property.
        private void HookUpCommand(ICommand oldCommand, ICommand newCommand)
        {
            // If oldCommand is not null, then we need to remove the handlers.
            if (oldCommand != null)
            {
                RemoveCommand(oldCommand, newCommand);
            }
            AddCommand(oldCommand, newCommand);
        }

        // Remove an old command from the Command Property.
        private void RemoveCommand(ICommand oldCommand, ICommand newCommand)
        {
            EventHandler handler = CanExecuteChanged;
            oldCommand.CanExecuteChanged -= handler;
        }

        // Add the command.
        private void AddCommand(ICommand oldCommand, ICommand newCommand)
        {
            EventHandler canExecuteChangedHandler = new EventHandler(CanExecuteChanged);

            if (newCommand != null)
            {
                newCommand.CanExecuteChanged += canExecuteChangedHandler;
            }
        }

        private void CanExecuteChanged(object sender, EventArgs e)
        { 
            if (this.Command != null)
            {
                RoutedCommand command = this.Command as RoutedCommand;

                // If a RoutedCommand.
                if (command != null)
                {
                    if (command.CanExecute(CommandParameter, CommandTarget))
                    {
                        //this.IsEnabled = true;
                    }
                    else
                    {
                        //this.IsEnabled = false;
                    }
                }
                // If a not RoutedCommand.
                else
                {
                    if (Command.CanExecute(CommandParameter))
                    {
                        //this.IsEnabled = true;
                    }
                    else
                    {
                        //this.IsEnabled = false;
                    }
                }
            }
        }
    }
}

