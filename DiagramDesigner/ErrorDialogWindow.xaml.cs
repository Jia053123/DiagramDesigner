using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DiagramDesigner
{
	/// <summary>
	/// Interaction logic for ErrorPopupWindow.xaml
	/// </summary>
	public partial class ErrorPopupWindow : Window
	{
		public ErrorPopupWindow()
		{
			InitializeComponent();
		}

		public ErrorPopupWindow(String title, String errorMessage)
		{
			InitializeComponent();
			this.Title.Text = title;
			this.ErrorMessage.Text = errorMessage;
		}
	}
}
