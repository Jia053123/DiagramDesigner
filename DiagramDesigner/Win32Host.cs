// // Copyright (c) Microsoft. All rights reserved.
// // Licensed under the MIT license. See LICENSE file in the project root for full license information.

#region Using directives

using System;
using System.Runtime.InteropServices;
using System.Windows.Interop;

#endregion

namespace DiagramDesigner
{
    public class Win32Host : HwndHost
    {
        // These constants are largely taken from Winuser.h, and allow you to use conventional names when calling Win32 functions.
        internal const int
            WsChild = 0x40000000,
            WsVisible = 0x10000000,
            LbsNotify = 0x00000001,
            HostId = 0x00000002,
            ListboxId = 0x00000001,
            WsVscroll = 0x00200000,
            WsBorder = 0x00800000;

        private readonly int _hostHeight;
        private readonly int _hostWidth;
        private IntPtr _hwndHost;

        // The constructor takes two parameters, height and width, which correspond to the height and width of the Border element that hosts the ListBox control. 
        // These values are used later to ensure that the size of the control matches the Border element.
        public Win32Host(double height, double width)
        {
            _hostHeight = (int)height;
            _hostWidth = (int)width;
        }

        // The HWND of the control is exposed through a read-only property, such that the host page can use it to send messages to the control.
        public IntPtr HwndListBox { get; private set; }

        // The HwndHost class allows you to process messages sent to the window that it is hosting. 
        // If you host a Win32 control directly, you receive the messages sent to the internal message loop of the control. 
        // You can display the control and send it messages, but you do not receive the notifications that the control sends to its parent window. 
        // This means, among other things, that you have no way of detecting when the user interacts with the control. 
        // Instead, create a host window and make the control a child of that window. 
        // This allows you to process the messages for the host window including the notifications sent to it by the control.
        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            HwndListBox = IntPtr.Zero;
            _hwndHost = IntPtr.Zero;

            _hwndHost = CreateWindowEx(0, "static", "",
                WsChild | WsVisible,
                0, 0,
                _hostHeight, _hostWidth,
                hwndParent.Handle,
                (IntPtr)HostId,
                IntPtr.Zero,
                0);

            HwndListBox = CreateWindowEx(0, "listbox", "",
                WsChild | WsVisible | LbsNotify
                | WsVscroll | WsBorder,
                0, 0,
                _hostHeight, _hostWidth,
                _hwndHost,
                (IntPtr)ListboxId,
                IntPtr.Zero,
                0);

            // returns a HandleRef object that contains the HWND of the host window.
            return new HandleRef(this, _hwndHost);
        }

        // In this example, the messages for the control are handled by the MessageHook handler, thus the implementation of WndProc and DestroyWindowCore is minimal.
        // set handled to false to indicate that the message was not handled and return 0.
        protected override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            handled = false;
            return IntPtr.Zero;
        }

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            DestroyWindow(hwnd.Handle);
        }

        //PInvoke declarations
        [DllImport("user32.dll", EntryPoint = "CreateWindowEx", CharSet = CharSet.Unicode)]
        internal static extern IntPtr CreateWindowEx(int dwExStyle,
            string lpszClassName,
            string lpszWindowName,
            int style,
            int x, int y,
            int width, int height,
            IntPtr hwndParent,
            IntPtr hMenu,
            IntPtr hInst,
            [MarshalAs(UnmanagedType.AsAny)] object pvParam);

        [DllImport("user32.dll", EntryPoint = "DestroyWindow", CharSet = CharSet.Unicode)]
        internal static extern bool DestroyWindow(IntPtr hwnd);
    }
}
