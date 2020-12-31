/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// TGUI - Texus' Graphical User Interface
// Copyright (C) 2012-2020 Bruno Van de Velde (vdv_b@TGUI.eu)
//
// This software is provided 'as-is', without any express or implied warranty.
// In no event will the authors be held liable for any damages arising from the use of this software.
//
// Permission is granted to anyone to use this software for any purpose,
// including commercial applications, and to alter it and redistribute it freely,
// subject to the following restrictions:
//
// 1. The origin of this software must not be misrepresented;
//    you must not claim that you wrote the original software.
//    If you use this software in a product, an acknowledgment
//    in the product documentation would be appreciated but is not required.
//
// 2. Altered source versions must be plainly marked as such,
//    and must not be misrepresented as being the original software.
//
// 3. This notice may not be removed or altered from any source distribution.
//
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Security;
using System.Runtime.InteropServices;

namespace TGUI
{
    public class Grid : Container
    {
        public Grid()
            : base(tguiGrid_create())
        {
        }

        protected internal Grid(IntPtr cPointer)
            : base(cPointer)
        {
        }

        public Grid(Grid copy)
            : base(copy)
        {
        }

        public bool AutoSize
        {
            get { return tguiGrid_getAutoSize(CPointer); }
            set { tguiGrid_setAutoSize(CPointer, value); }
        }

        public void AddWidget(Widget widget, uint row, uint col)
        {
            tguiGrid_addWidget(CPointer, widget.CPointer, row, col, (new Outline()).CPointer, Alignment.Center);
        }

        public void AddWidget(Widget widget, uint row, uint col, Outline padding, Alignment alignment = Alignment.Center)
        {
            tguiGrid_addWidget(CPointer, widget.CPointer, row, col, padding.CPointer, alignment);
        }

        public Widget GetWidget(uint row, uint col)
        {
            return Util.GetWidgetFromC(tguiGrid_getWidget(CPointer, row, col), ParentGui);
        }

        public void SetWidgetPadding(Widget widget, Outline padding)
        {
            tguiGrid_setWidgetPadding(CPointer, widget.CPointer, padding.CPointer);
        }

        public void SetWidgetPadding(uint row, uint col, Outline padding)
        {
            tguiGrid_setWidgetPaddingByCell(CPointer, row, col, padding.CPointer);
        }

        public Outline GetWidgetPadding(Widget widget)
        {
            return new Outline(tguiGrid_getWidgetPadding(CPointer, widget.CPointer));
        }

        public Outline GetWidgetPadding(uint row, uint col)
        {
            return new Outline(tguiGrid_getWidgetPaddingByCell(CPointer, row, col));
        }

        public void SetWidgetAlignment(Widget widget, Alignment alignment)
        {
            tguiGrid_setWidgetAlignment(CPointer, widget.CPointer, alignment);
        }

        public void SetWidgetAlignment(uint row, uint col, Alignment alignment)
        {
            tguiGrid_setWidgetAlignmentByCell(CPointer, row, col, alignment);
        }

        public Alignment GetWidgetAlignment(Widget widget)
        {
            return tguiGrid_getWidgetAlignment(CPointer, widget.CPointer);
        }

        public Alignment GetWidgetAlignment(uint row, uint col)
        {
            return tguiGrid_getWidgetAlignmentByCell(CPointer, row, col);
        }


        #region Imports

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private IntPtr tguiGrid_create();

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private void tguiGrid_setAutoSize(IntPtr cPointer, bool autoSize);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private bool tguiGrid_getAutoSize(IntPtr cPointer);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private void tguiGrid_addWidget(IntPtr cPointer, IntPtr widgetCPointer, uint row, uint col, IntPtr paddingCPointer, Alignment alignment);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private IntPtr tguiGrid_getWidget(IntPtr cPointer, uint row, uint col);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private void tguiGrid_setWidgetPadding(IntPtr cPointer, IntPtr widgetCPointer, IntPtr paddingCPointer);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private void tguiGrid_setWidgetPaddingByCell(IntPtr cPointer, uint row, uint col, IntPtr paddingCPointer);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private IntPtr tguiGrid_getWidgetPadding(IntPtr cPointer, IntPtr widgetCPointer);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private IntPtr tguiGrid_getWidgetPaddingByCell(IntPtr cPointer, uint row, uint col);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private void tguiGrid_setWidgetAlignment(IntPtr cPointer, IntPtr widgetCPointer, Alignment alignment);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private void tguiGrid_setWidgetAlignmentByCell(IntPtr cPointer, uint row, uint col, Alignment alignment);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private Alignment tguiGrid_getWidgetAlignment(IntPtr cPointer, IntPtr widgetCPointer);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private Alignment tguiGrid_getWidgetAlignmentByCell(IntPtr cPointer, uint row, uint col);

        #endregion
    }
}
