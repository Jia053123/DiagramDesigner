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

        int selectedItem;
        IntPtr hwndListBox;
        Win32Host listControl;
        Application app;
        Window myWindow;
        int itemCount;

        public MainWindow()
        {
            InitializeComponent();
            this.mainViewModel = new MainViewModel();
            this.DataContext = mainViewModel;
        }

        private void On_UIReady(object sender, EventArgs e)
        {
            app = System.Windows.Application.Current;
            myWindow = app.MainWindow;
            myWindow.SizeToContent = SizeToContent.WidthAndHeight;
            // create a new hosted control
            listControl = new Win32Host(PrimaryWindow.ActualHeight, PrimaryWindow.ActualWidth);
            PrimaryWindow.Child = listControl;
            // attaches a handler to the MessageHook event of the ControlHost to receive messages from the control.
            listControl.MessageHook += new HwndSourceHook(ControlMsgFilter);
            hwndListBox = listControl.HwndListBox;
            for (int i = 0; i < 15; i++) //populate listbox
            {
                string itemText = "Item" + i.ToString();
                SendMessage(hwndListBox, LB_ADDSTRING, IntPtr.Zero, itemText);
            }
            itemCount = SendMessage(hwndListBox, LB_GETCOUNT, IntPtr.Zero, IntPtr.Zero);
            numItems.Text = "" + itemCount.ToString();
        }

        private IntPtr ControlMsgFilter(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            int textLength;

            handled = false;
            if (msg == WM_COMMAND)
            {
                switch ((uint)wParam.ToInt32() >> 16 & 0xFFFF) //extract the HIWORD
                {
                    case LBN_SELCHANGE: //Get the item text and display it
                        selectedItem = SendMessage(listControl.HwndListBox, LB_GETCURSEL, IntPtr.Zero, IntPtr.Zero);
                        textLength = SendMessage(listControl.HwndListBox, LB_GETTEXTLEN, IntPtr.Zero, IntPtr.Zero);
                        StringBuilder itemText = new StringBuilder();
                        SendMessage(hwndListBox, LB_GETTEXT, selectedItem, itemText);
                        selectedText.Text = itemText.ToString();
                        handled = true;
                        break;
                }
            }
            return IntPtr.Zero;
        }
        internal const int
          LBN_SELCHANGE = 0x00000001,
          WM_COMMAND = 0x00000111,
          LB_GETCURSEL = 0x00000188,
          LB_GETTEXTLEN = 0x0000018A,
          LB_ADDSTRING = 0x00000180,
          LB_GETTEXT = 0x00000189,
          LB_DELETESTRING = 0x00000182,
          LB_GETCOUNT = 0x0000018B;

        [DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Unicode)]
        internal static extern int SendMessage(IntPtr hwnd,
                                               int msg,
                                               IntPtr wParam,
                                               IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Unicode)]
        internal static extern int SendMessage(IntPtr hwnd,
                                               int msg,
                                               int wParam,
                                               [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lParam);

        [DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Unicode)]
        internal static extern IntPtr SendMessage(IntPtr hwnd,
                                                  int msg,
                                                  IntPtr wParam,
                                                  String lParam);

        // To append items, send the list box an LB_ADDSTRING message. 
        // To delete items, send LB_GETCURSEL to get the index of the current selection and then LB_DELETESTRING to delete the item. 
        // The sample also sends LB_GETCOUNT, and uses the returned value to update the display that shows the number of items. 
        // Both these instances of SendMessage use one of the PInvoke declarations discussed in the previous section.
        private void AppendText(object sender, EventArgs args)
        {
            if (!string.IsNullOrEmpty(txtAppend.Text))
            {
                SendMessage(hwndListBox, LB_ADDSTRING, IntPtr.Zero, txtAppend.Text);
            }
            itemCount = SendMessage(hwndListBox, LB_GETCOUNT, IntPtr.Zero, IntPtr.Zero);
            numItems.Text = "" + itemCount.ToString();
        }
        private void DeleteText(object sender, EventArgs args)
        {
            selectedItem = SendMessage(listControl.hwndListBox, LB_GETCURSEL, IntPtr.Zero, IntPtr.Zero);
            if (selectedItem != -1) //check for selected item
            {
                SendMessage(hwndListBox, LB_DELETESTRING, (IntPtr)selectedItem, IntPtr.Zero);
            }
            itemCount = SendMessage(hwndListBox, LB_GETCOUNT, IntPtr.Zero, IntPtr.Zero);
            numItems.Text = "" + itemCount.ToString();
        }
    }
}
