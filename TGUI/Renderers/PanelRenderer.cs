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
    public class PanelRenderer : GroupRenderer
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public PanelRenderer()
            : base(tguiPanelRenderer_create())
        {
        }

        /// <summary>
        /// Constructor that creates the object from its C pointer
        /// </summary>
        /// <param name="cPointer">Pointer to object in C code</param>
        protected internal PanelRenderer(IntPtr cPointer)
            : base(cPointer)
        {
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="copy">Object to copy</param>
        public PanelRenderer(PanelRenderer copy)
            : base(tguiPanelRenderer_copy(copy.CPointer))
        {
        }

        /// <summary>
        /// Gets or sets the size of the borders
        /// </summary>
        public Outline Borders
        {
            get { return new Outline(tguiPanelRenderer_getBorders(CPointer)); }
            set { tguiPanelRenderer_setBorders(CPointer, value.CPointer); }
        }

        /// <summary>
        /// Gets or sets the background color
        /// </summary>
        public Color BackgroundColor
        {
            get { return tguiPanelRenderer_getBackgroundColor(CPointer); }
            set { tguiPanelRenderer_setBackgroundColor(CPointer, value); }
        }

        /// <summary>
        /// Gets or sets the color of the borders
        /// </summary>
        public Color BorderColor
        {
            get { return tguiPanelRenderer_getBorderColor(CPointer); }
            set { tguiPanelRenderer_setBorderColor(CPointer, value); }
        }

        /// <summary>
        /// Gets or sets the background texture
        /// </summary>
        public Texture TextureBackground
        {
            set { tguiPanelRenderer_setTextureBackground(CPointer, value.CPointer); }
        }

        #region Imports

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private IntPtr tguiPanelRenderer_create();

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private IntPtr tguiPanelRenderer_copy(IntPtr cPointer);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private void tguiPanelRenderer_setBorders(IntPtr cPointer, IntPtr borders);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private IntPtr tguiPanelRenderer_getBorders(IntPtr cPointer);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private void tguiPanelRenderer_setBackgroundColor(IntPtr cPointer, Color color);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private Color tguiPanelRenderer_getBackgroundColor(IntPtr cPointer);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private void tguiPanelRenderer_setBorderColor(IntPtr cPointer, Color color);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private Color tguiPanelRenderer_getBorderColor(IntPtr cPointer);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private void tguiPanelRenderer_setTextureBackground(IntPtr cPointer, IntPtr texture);

        #endregion
    }
}
