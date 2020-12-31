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
    public abstract class BoxLayout : Group
    {
        protected internal BoxLayout(IntPtr cPointer)
            : base(cPointer)
        {
        }

        public BoxLayout(BoxLayout copy)
            : base(copy)
        {
        }

        public new BoxLayoutRenderer Renderer
        {
            get { return new BoxLayoutRenderer(tguiWidget_getRenderer(CPointer)); }
            set { SetRenderer(value.Data); }
        }

        public new BoxLayoutRenderer SharedRenderer
        {
            get { return new BoxLayoutRenderer(tguiWidget_getSharedRenderer(CPointer)); }
        }

        public void Insert(uint index, Widget widget, string widgetName = "")
        {
            tguiBoxLayout_insert(CPointer, index, widget.CPointer, Util.ConvertStringForC_UTF32(widgetName));
        }

        public bool Remove(uint index)
        {
            return tguiBoxLayout_removeAtIndex(CPointer, index);
        }

        public Widget Get(uint index)
        {
            return Util.GetWidgetFromC(tguiBoxLayout_getAtIndex(CPointer, index), ParentGui);
        }


        #region Imports

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private void tguiBoxLayout_insert(IntPtr cPointer, uint index, IntPtr widgetCPointer, IntPtr widgetName);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private bool tguiBoxLayout_removeAtIndex(IntPtr cPointer, uint index);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private IntPtr tguiBoxLayout_getAtIndex(IntPtr cPointer, uint index);

        #endregion
    }
}
