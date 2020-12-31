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
using System.Collections.Generic;

namespace TGUI
{
    public class Tabs : Widget
    {
        public Tabs()
            : base(tguiTabs_create())
        {
        }

        protected internal Tabs(IntPtr cPointer)
            : base(cPointer)
        {
        }

        public Tabs(Tabs copy)
            : base(copy)
        {
        }

        public new TabsRenderer Renderer
        {
            get { return new TabsRenderer(tguiWidget_getRenderer(CPointer)); }
            set { SetRenderer(value.Data); }
        }

        public new TabsRenderer SharedRenderer
        {
            get { return new TabsRenderer(tguiWidget_getSharedRenderer(CPointer)); }
        }

        public bool AutoSize
        {
            get { return tguiTabs_getAutoSize(CPointer); }
            set { tguiTabs_setAutoSize(CPointer, value); }
        }

        public uint Add(string text, bool selectTab = true)
        {
            return tguiTabs_add(CPointer, Util.ConvertStringForC_UTF32(text), selectTab);
        }

        public void Insert(uint index, string text, bool selectTab = true)
        {
            tguiTabs_insert(CPointer, index, Util.ConvertStringForC_UTF32(text), selectTab);
        }

        public string GetText(uint index)
        {
            return Util.GetStringFromC_UTF32(tguiTabs_getText(CPointer, index));
        }

        public bool ChangeText(uint index, string text)
        {
            return tguiTabs_changeText(CPointer, index, Util.ConvertStringForC_UTF32(text));
        }

        public bool Select(string text)
        {
            return tguiTabs_selectByText(CPointer, Util.ConvertStringForC_UTF32(text));
        }

        public bool Select(uint index)
        {
            return tguiTabs_selectByIndex(CPointer, index);
        }

        public void Deselect()
        {
            tguiTabs_deselect(CPointer);
        }

        public bool Remove(string text)
        {
            return tguiTabs_removeByText(CPointer, Util.ConvertStringForC_UTF32(text));
        }

        public bool Remove(uint index)
        {
            return tguiTabs_removeByIndex(CPointer, index);
        }

        public void RemoveAll()
        {
            tguiTabs_removeAll(CPointer);
        }

        public string GetSelected()
        {
            return Util.GetStringFromC_UTF32(tguiTabs_getSelected(CPointer));
        }

        public int GetSelectedIndex()
        {
            return tguiTabs_getSelectedIndex(CPointer);
        }

        public uint GetTabsCount()
        {
            return tguiTabs_getTabsCount(CPointer);
        }

        public void SetTabVisible(uint index, bool visible)
        {
            tguiTabs_setTabVisible(CPointer, index, visible);
        }

        public bool GetTabVisible(uint index)
        {
            return tguiTabs_getTabVisible(CPointer, index);
        }

        public void SetTabEnabled(uint index, bool enabled)
        {
            tguiTabs_setTabEnabled(CPointer, index, enabled);
        }

        public bool GetTabEnabled(uint index)
        {
            return tguiTabs_getTabEnabled(CPointer, index);
        }

        public float MaximumTabWidth
        {
            get { return tguiTabs_getMaximumTabWidth(CPointer); }
            set { tguiTabs_setMaximumTabWidth(CPointer, value); }
        }

        public float MinimumTabWidth
        {
            get { return tguiTabs_getMinimumTabWidth(CPointer); }
            set { tguiTabs_setMinimumTabWidth(CPointer, value); }
        }

        protected override void InitSignals()
        {
            base.InitSignals();

            TabSelectedCallback = new CallbackActionString((tab) => SendSignal(myTabSelectedEventKey, new SignalArgsString(Util.GetStringFromC_UTF32(tab))));
            AddInternalSignal(tguiWidget_connectString(CPointer, Util.ConvertStringForC_ASCII("TabSelected"), TabSelectedCallback));
        }

        /// <summary>Event handler for the TabSelected signal</summary>
        public event EventHandler<SignalArgsString> TabSelected
        {
            add { myEventHandlerList.AddHandler(myTabSelectedEventKey, value); }
            remove { myEventHandlerList.RemoveHandler(myTabSelectedEventKey, value); }
        }

        private CallbackActionString TabSelectedCallback;
        static readonly object myTabSelectedEventKey = new object();

        #region Imports

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private IntPtr tguiTabs_create();

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private void tguiTabs_setAutoSize(IntPtr cPointer, bool autoSize);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private bool tguiTabs_getAutoSize(IntPtr cPointer);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private uint tguiTabs_add(IntPtr cPointer, IntPtr text, bool selectTab);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private void tguiTabs_insert(IntPtr cPointer, uint index, IntPtr text, bool selectTab);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private IntPtr tguiTabs_getText(IntPtr cPointer, uint index);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private bool tguiTabs_changeText(IntPtr cPointer, uint index, IntPtr text);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private bool tguiTabs_selectByText(IntPtr cPointer, IntPtr text);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private bool tguiTabs_selectByIndex(IntPtr cPointer, uint index);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private void tguiTabs_deselect(IntPtr cPointer);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private bool tguiTabs_removeByText(IntPtr cPointer, IntPtr text);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private bool tguiTabs_removeByIndex(IntPtr cPointer, uint index);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private void tguiTabs_removeAll(IntPtr cPointer);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private IntPtr tguiTabs_getSelected(IntPtr cPointer);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private int tguiTabs_getSelectedIndex(IntPtr cPointer);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private uint tguiTabs_getTabsCount(IntPtr cPointer);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private void tguiTabs_setTabVisible(IntPtr cPointer, uint index, bool visible);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private bool tguiTabs_getTabVisible(IntPtr cPointer, uint index);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private void tguiTabs_setTabEnabled(IntPtr cPointer, uint index, bool enabled);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private bool tguiTabs_getTabEnabled(IntPtr cPointer, uint index);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private void tguiTabs_setMaximumTabWidth(IntPtr cPointer, float maximumTabWidth);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private float tguiTabs_getMaximumTabWidth(IntPtr cPointer);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private void tguiTabs_setMinimumTabWidth(IntPtr cPointer, float minimumTabWidth);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private float tguiTabs_getMinimumTabWidth(IntPtr cPointer);

        #endregion
    }
}
