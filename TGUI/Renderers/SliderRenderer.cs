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
using SFML.Graphics;

namespace TGUI
{
    public class SliderRenderer : WidgetRenderer
    {
        public SliderRenderer()
            : base(tguiSliderRenderer_create())
        {
        }

        protected internal SliderRenderer(IntPtr cPointer)
            : base(cPointer)
        {
        }

        public SliderRenderer(SliderRenderer copy)
            : base(tguiSliderRenderer_copy(copy.CPointer))
        {
        }

        public Outline Borders
        {
            get { return new Outline(tguiSliderRenderer_getBorders(CPointer)); }
            set { tguiSliderRenderer_setBorders(CPointer, value.CPointer); }
        }

        public Color TrackColor
        {
            get { return tguiSliderRenderer_getTrackColor(CPointer); }
            set { tguiSliderRenderer_setTrackColor(CPointer, value); }
        }

        public Color TrackColorHover
        {
            get { return tguiSliderRenderer_getTrackColorHover(CPointer); }
            set { tguiSliderRenderer_setTrackColorHover(CPointer, value); }
        }

        public Color ThumbColor
        {
            get { return tguiSliderRenderer_getThumbColor(CPointer); }
            set { tguiSliderRenderer_setThumbColor(CPointer, value); }
        }

        public Color ThumbColorHover
        {
            get { return tguiSliderRenderer_getThumbColorHover(CPointer); }
            set { tguiSliderRenderer_setThumbColorHover(CPointer, value); }
        }

        public Color BorderColor
        {
            get { return tguiSliderRenderer_getBorderColor(CPointer); }
            set { tguiSliderRenderer_setBorderColor(CPointer, value); }
        }

        public Color BorderColorHover
        {
            get { return tguiSliderRenderer_getBorderColorHover(CPointer); }
            set { tguiSliderRenderer_setBorderColorHover(CPointer, value); }
        }

        public Texture TextureTrack
        {
            set { tguiSliderRenderer_setTextureTrack(CPointer, value.CPointer); }
        }

        public Texture TextureTrackHover
        {
            set { tguiSliderRenderer_setTextureTrackHover(CPointer, value.CPointer); }
        }

        public Texture TextureThumb
        {
            set { tguiSliderRenderer_setTextureThumb(CPointer, value.CPointer); }
        }

        public Texture TextureThumbHover
        {
            set { tguiSliderRenderer_setTextureThumbHover(CPointer, value.CPointer); }
        }

        #region Imports

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private IntPtr tguiSliderRenderer_create();

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private IntPtr tguiSliderRenderer_copy(IntPtr cPointer);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private void tguiSliderRenderer_setBorders(IntPtr cPointer, IntPtr borders);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private IntPtr tguiSliderRenderer_getBorders(IntPtr cPointer);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private void tguiSliderRenderer_setTrackColor(IntPtr cPointer, Color color);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private Color tguiSliderRenderer_getTrackColor(IntPtr cPointer);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private void tguiSliderRenderer_setTrackColorHover(IntPtr cPointer, Color color);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private Color tguiSliderRenderer_getTrackColorHover(IntPtr cPointer);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private void tguiSliderRenderer_setThumbColor(IntPtr cPointer, Color color);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private Color tguiSliderRenderer_getThumbColor(IntPtr cPointer);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private void tguiSliderRenderer_setThumbColorHover(IntPtr cPointer, Color color);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private Color tguiSliderRenderer_getThumbColorHover(IntPtr cPointer);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private void tguiSliderRenderer_setBorderColor(IntPtr cPointer, Color color);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private Color tguiSliderRenderer_getBorderColor(IntPtr cPointer);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private void tguiSliderRenderer_setBorderColorHover(IntPtr cPointer, Color color);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private Color tguiSliderRenderer_getBorderColorHover(IntPtr cPointer);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private void tguiSliderRenderer_setTextureTrack(IntPtr cPointer, IntPtr texture);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private void tguiSliderRenderer_setTextureTrackHover(IntPtr cPointer, IntPtr texture);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private void tguiSliderRenderer_setTextureThumb(IntPtr cPointer, IntPtr texture);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private void tguiSliderRenderer_setTextureThumbHover(IntPtr cPointer, IntPtr texture);

        #endregion
    }
}
