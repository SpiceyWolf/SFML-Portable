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
    /// <summary>
    /// Text box widget
    /// </summary>
    public class TextBox : Widget
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public TextBox()
            : base(tguiTextBox_create())
        {
        }

        /// <summary>
        /// Constructor that creates the object from its C pointer
        /// </summary>
        /// <param name="cPointer">Pointer to object in C code</param>
        protected internal TextBox(IntPtr cPointer)
            : base(cPointer)
        {
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="copy">Object to copy</param>
        public TextBox(TextBox copy)
            : base(copy)
        {
        }

        /// <summary>
        /// Gets or sets the renderer, which gives access to properties that determine how the widget is displayed
        /// </summary>
        /// <remarks>
        /// After retrieving the renderer, the widget has its own copy of the renderer and it will no longer be shared.
        /// </remarks>
        public new TextBoxRenderer Renderer
        {
            get { return new TextBoxRenderer(tguiWidget_getRenderer(CPointer)); }
        }

        /// <summary>
        /// Gets the renderer, which gives access to properties that determine how the widget is displayed
        /// </summary>
        public new TextBoxRenderer SharedRenderer
        {
            get { return new TextBoxRenderer(tguiWidget_getSharedRenderer(CPointer)); }
        }

        /// <summary>
        /// Gets or sets the text of the text box
        /// </summary>
        public string Text
        {
            get { return Util.GetStringFromC_UTF32(tguiTextBox_getText(CPointer)); }
            set { tguiTextBox_setText(CPointer, Util.ConvertStringForC_UTF32(value)); }
        }

        /// <summary>
        /// Gets or sets the default text of the text box. This is the text drawn when the text box is empty.
        /// </summary>
        public string DefaultText
        {
            get { return Util.GetStringFromC_UTF32(tguiTextBox_getDefaultText(CPointer)); }
            set { tguiTextBox_setDefaultText(CPointer, Util.ConvertStringForC_UTF32(value)); }
        }

        /// <summary>
        /// Appends some text to the text that was already in the text box
        /// </summary>
        /// <param name="text">Text to add</param>
        public void AddText(string text)
        {
            tguiTextBox_addText(CPointer, Util.ConvertStringForC_UTF32(text));
        }

        /// <summary>
        /// Sets which part of the text is selected
        /// </summary>
        /// <param name="selectionStartIndex">Amount of characters before the start of the selection</param>
        /// <param name="selectionEndIndex">Amount of characters before the end of the selection</param>
        public void SetSelectedText(uint selectionStartIndex, uint selectionEndIndex)
        {
            tguiTextBox_setSelectedText(CPointer, selectionStartIndex, selectionEndIndex);
        }

        /// <summary>
        /// Gets the currently selected text
        /// </summary>
        public string SelectedText
        {
            get { return Util.GetStringFromC_UTF32(tguiTextBox_getSelectedText(CPointer)); }
        }

        /// <summary>
        /// Gets the amount of characters before the start of the selected text
        /// </summary>
        /// <remarks>
        /// The start of the selection may be behind the end of the selection when the user selected the text from
        /// right to left or from bottom to top.
        /// </remarks>
        public uint SelectionStart
        {
            get { return tguiTextBox_getSelectionStart(CPointer); }
        }

        /// <summary>
        /// Gets the amount of characters before the end of the selected text
        /// </summary>
        /// <remarks>
        /// The start of the selection may be behind the end of the selection when the user selected the text from
        /// right to left or from bottom to top.
        /// </remarks>
        public uint SelectionEnd
        {
            get { return tguiTextBox_getSelectionEnd(CPointer); }
        }

        /// <summary>
        /// Gets or sets the character limit (0 to disable, default)
        /// </summary>
        public uint MaximumCharacters
        {
            get { return tguiTextBox_getMaximumCharacters(CPointer); }
            set { tguiTextBox_setMaximumCharacters(CPointer, value); }
        }

        /// <summary>
        /// Gets or sets after which character the blinking cursor is located
        /// </summary>
        public uint CaretPosition {
            get { return tguiTextBox_getCaretPosition (CPointer); }
            set { tguiTextBox_setCaretPosition (CPointer, value); }
        }

        /// <summary>
        /// Gets or sets whether the text box is read-only
        /// </summary>
        /// <remarks>
        /// When the text box is read-only, you can no longer delete characters and type text.
        /// Selecting text, copying text and even calling the setText function will still work.
        /// </remarks>
        public bool ReadOnly
        {
            get { return tguiTextBox_isReadOnly(CPointer); }
            set { tguiTextBox_setReadOnly(CPointer, value); }
        }

        /// <summary>
        /// Gets or sets whether the vertical scrollbar should be displayed when the text does not fit in the text box
        /// </summary>
        [Obsolete("Use VerticalScrollbarPolicy instead")]
        public bool VerticalScrollbarPresent
        {
            get { return tguiTextBox_isVerticalScrollbarPresent(CPointer); }
            set { tguiTextBox_setVerticalScrollbarPresent(CPointer, value); }
        }

        /// <summary>
        /// Gets or sets when the vertical scrollbar should be displayed
        /// </summary>
        public Scrollbar.Policy VerticalScrollbarPolicy
        {
            get { return tguiTextBox_getVerticalScrollbarPolicy(CPointer); }
            set { tguiTextBox_setVerticalScrollbarPolicy(CPointer, value); }
        }

        /// <summary>
        /// Gets or sets when the horizontal scrollbar should be displayed
        /// </summary>
        public Scrollbar.Policy HorizontalScrollbarPolicy
        {
            get { return tguiTextBox_getHorizontalScrollbarPolicy(CPointer); }
            set { tguiTextBox_setHorizontalScrollbarPolicy(CPointer, value); }
        }

        /// <summary>
        /// Gets or sets the thumb position of the vertical scrollbar
        /// </summary>
        public uint VerticalScrollbarValue
        {
            get { return tguiTextBox_getVerticalScrollbarValue(CPointer); }
            set { tguiTextBox_setVerticalScrollbarValue(CPointer, value); }
        }

        /// <summary>
        /// Gets or sets the thumb position of the horizontal scrollbar
        /// </summary>
        public uint HorizontalScrollbarValue
        {
            get { return tguiTextBox_getHorizontalScrollbarValue(CPointer); }
            set { tguiTextBox_setHorizontalScrollbarValue(CPointer, value); }
        }

        /// <summary>
        /// Gets the amount of lines that the text occupies in the TextBox
        /// </summary>
        /// <remarks>
        /// Note that this is the amount of lines after word-wrap is applied.
        /// </remarks>
        public uint LinesCount {
            get { return tguiTextBox_getLinesCount (CPointer); }
        }

        /// <summary>
        /// Initializes the signals
        /// </summary>
        protected override void InitSignals()
        {
            base.InitSignals();

            TextChangedCallback = new CallbackActionString((text) => SendSignal(myTextChangedEventKey, new SignalArgsString(Util.GetStringFromC_UTF32(text))));
            AddInternalSignal(tguiWidget_connectString(CPointer, Util.ConvertStringForC_ASCII("TextChanged"), TextChangedCallback));

            SelectionChangedCallback = new CallbackAction(() => SendSignal(mySelectionChangedEventKey));
            AddInternalSignal(tguiWidget_connect(CPointer, Util.ConvertStringForC_ASCII("SelectionChanged"), SelectionChangedCallback));
        }

        /// <summary>Event handler for the TextChanged signal</summary>
        public event EventHandler<SignalArgsString> TextChanged
        {
            add { myEventHandlerList.AddHandler(myTextChangedEventKey, value); }
            remove { myEventHandlerList.RemoveHandler(myTextChangedEventKey, value); }
        }

        /// <summary>Event handler for the SelectionChanged signal</summary>
        public event EventHandler SelectionChanged
        {
            add { myEventHandlerList.AddHandler(mySelectionChangedEventKey, value); }
            remove { myEventHandlerList.RemoveHandler(mySelectionChangedEventKey, value); }
        }

        private CallbackActionString TextChangedCallback;
        private CallbackAction SelectionChangedCallback;

        static readonly object myTextChangedEventKey = new object();
        static readonly object mySelectionChangedEventKey = new object();


        #region Imports

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private IntPtr tguiTextBox_create();

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private void tguiTextBox_setText(IntPtr cPointer, IntPtr value);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private void tguiTextBox_addText(IntPtr cPointer, IntPtr value);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private IntPtr tguiTextBox_getText(IntPtr cPointer);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private void tguiTextBox_setDefaultText(IntPtr cPointer, IntPtr defaultText);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private IntPtr tguiTextBox_getDefaultText(IntPtr cPointer);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private void tguiTextBox_setSelectedText(IntPtr cPointer, uint selectionStartIndex, uint selectionEndIndex);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private IntPtr tguiTextBox_getSelectedText(IntPtr cPointer);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private uint tguiTextBox_getSelectionStart(IntPtr cPointer);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private uint tguiTextBox_getSelectionEnd(IntPtr cPointer);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private void tguiTextBox_setMaximumCharacters(IntPtr cPointer, uint maximumCharacters);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private uint tguiTextBox_getMaximumCharacters(IntPtr cPointer);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private void tguiTextBox_setCaretPosition(IntPtr cPointer, uint charactersBeforeCaret);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private uint tguiTextBox_getCaretPosition(IntPtr cPointer);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private void tguiTextBox_setReadOnly(IntPtr cPointer, bool readOnly);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private bool tguiTextBox_isReadOnly(IntPtr cPointer);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private void tguiTextBox_setVerticalScrollbarPresent(IntPtr cPointer, bool present);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private bool tguiTextBox_isVerticalScrollbarPresent(IntPtr cPointer);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private void tguiTextBox_setVerticalScrollbarPolicy(IntPtr cPointer, Scrollbar.Policy policy);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private Scrollbar.Policy tguiTextBox_getVerticalScrollbarPolicy(IntPtr cPointer);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private void tguiTextBox_setHorizontalScrollbarPolicy(IntPtr cPointer, Scrollbar.Policy policy);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private Scrollbar.Policy tguiTextBox_getHorizontalScrollbarPolicy(IntPtr cPointer);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private void tguiTextBox_setVerticalScrollbarValue(IntPtr cPointer, uint newValue);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private uint tguiTextBox_getVerticalScrollbarValue(IntPtr cPointer);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private void tguiTextBox_setHorizontalScrollbarValue(IntPtr cPointer, uint newValue);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private uint tguiTextBox_getHorizontalScrollbarValue(IntPtr cPointer);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private uint tguiTextBox_getLinesCount (IntPtr cPointer);

        #endregion
    }
}
