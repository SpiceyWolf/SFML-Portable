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
using System.Text;
using System.Security;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SFML.System;
using SFML.Window;
using SFML.Graphics;

namespace TGUI
{
    /// <summary>
    /// Gui class that acts as the root container
    /// </summary>
    public class Gui : SFML.ObjectBase
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <remarks>
        /// You will still need to set the Target property before using the Gui
        /// </remarks>
        public Gui()
            : base(tguiGui_create())
        {
        }

        /// <summary>
        /// Default constructor that sets the window on which the gui should be drawn
        /// </summary>
        /// <param name="window">Window to draw the gui on</param>
        public Gui(RenderWindow window)
            : this()
        {
            Target = window;
        }

        /// <summary>
        /// Destroy the object
        /// </summary>
        /// <param name="disposing">Is the GC disposing the object, or is it an explicit call?</param>
        protected override void Destroy(bool disposing)
        {
            foreach (var widget in myWidgets)
                widget.Parent = null;

            tguiGui_destroy(CPointer);
        }

        /// <summary>
        /// Gets or sets the window on which the gui should be drawn
        /// </summary>
        public RenderWindow Target
        {
            get { return myRenderTarget; }
            set
            {
                // RenderWindow instead of RenderTarget because we need the events
                myRenderTarget = value;
                tguiGui_setTargetRenderWindow(CPointer, value.CPointer);

                value.MouseMoved += new EventHandler<MouseMoveEventArgs>(OnMouseMoved);
                value.MouseButtonPressed += new EventHandler<MouseButtonEventArgs>(OnMousePressed);
                value.MouseButtonReleased += new EventHandler<MouseButtonEventArgs>(OnMouseReleased);
                value.KeyPressed += new EventHandler<KeyEventArgs>(OnKeyPressed);
                value.KeyReleased += new EventHandler<KeyEventArgs>(OnKeyReleased);
                value.TextEntered += new EventHandler<TextEventArgs>(OnTextEntered);
                value.MouseWheelScrolled += new EventHandler<MouseWheelScrollEventArgs>(OnMouseWheelScrolled);
                value.LostFocus += OnLostFocus;
                value.GainedFocus += OnGainedFocus;
            }
        }

        /// <summary>
        /// Sets the view that is used to render the gui
        /// </summary>
        public View View
        {
            set { tguiGui_setView(CPointer, value.CPointer); }
        }

        /// <summary>
        /// Sets the font that should be used by all widgets added to this gui
        /// </summary>
        public Font Font
        {
            set { tguiGui_setFont(CPointer, value.CPointer); }
        }

        /// <summary>
        /// Adds a widget to the gui
        /// </summary>
        /// <param name="widget">The widget you would like to add</param>
        /// <param name="widgetName">You can give the widget a unique name to retrieve it from the gui later</param>
        /// <remarks>
        /// The widget name should not contain whitespace
        /// </remarks>
        public void Add(Widget widget, string widgetName = "")
        {
            tguiGui_add(CPointer, widget.CPointer, Util.ConvertStringForC_UTF32(widgetName));

            widget.ParentGui = this;
            myWidgets.Add(widget);
        }

        /// <summary>
        /// Returns a widget that was added earlier
        /// </summary>
        /// <param name="widgetName">The name that was given to the widget when it was added to the gui</param>
        /// <returns>
        /// The earlier added widget
        /// </returns>
        /// <remarks>
        /// The gui will first search for widgets that are direct children of it, but when none of the child widgets match
        /// the given name, a recursive search will be performed.
        /// The function returns null when an unknown widget name was passed.
        /// </remarks>
        public Widget Get(string widgetName)
        {
            foreach (var widget in myWidgets)
            {
                if (widget.Name == widgetName)
                    return widget;
            }

            // If none of the child widgets matches then perform a recursive search
            foreach (var widget in myWidgets)
            {
                Container container = widget as Container;
                if (container != null)
                {
                    Widget foundWidget = container.Get(widgetName);
                    if (foundWidget != null)
                        return foundWidget;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets a list of all the widgets in this gui or completely replace all widgets with a new list
        /// </summary>
        /// <remarks>
        /// Setting this property is equivalent to calling RemoveAllWidgets() and then call Add(widget) for every widget in the list.
        /// By setting the widgets via this property you lose the ability to give a name to the widget for later retrieval.
        /// </remarks>
        public IReadOnlyList<Widget> Widgets
        {
            get { return myWidgets; }
            set
            {
                RemoveAllWidgets();
                foreach(Widget widget in value)
                    Add(widget);
            }
        }

        /// <summary>
        /// Returns a list of all the widgets in this gui
        /// </summary>
        /// <returns>
        /// List of widgets that have been added to the gui
        /// </returns>
        [Obsolete("Use Widgets property instead")]
        public List<Widget> GetWidgets()
        {
            unsafe
            {
                IntPtr* widgetsPtr = tguiGui_getWidgets(CPointer, out uint count);
                List<Widget> Widgets = new List<Widget>();
                for (uint i = 0; i < count; ++i)
                    Widgets.Add(Util.GetWidgetFromC(widgetsPtr[i], this));

                return Widgets;
            }
        }

        /// <summary>
        /// Gets the list of the names of all the widgets in this gui
        /// </summary>
        [Obsolete("Use Gui.Widgets property in combination with Widget.Name property instead")]
        public IReadOnlyList<string> WidgetNames
        {
            get
            {
                unsafe
                {
                    IntPtr* namesPtr = tguiGui_getWidgetNames(CPointer, out uint count);
                    string[] names = new string[count];
                    for (uint i = 0; i < count; ++i)
                        names[i] = Util.GetStringFromC_UTF32(namesPtr[i]);

                    return names;
                }
            }
        }

        /// <summary>
        /// Returns a list of the names of all the widgets in this gui
        /// </summary>
        /// <returns>
        /// List of widget names belonging to the widgets that were added to the gui
        /// </returns>
        [Obsolete("Use Gui.Widgets property in combination with Widget.Name property instead")]
        public List<string> GetWidgetNames()
        {
            unsafe
            {
                IntPtr* namesPtr = tguiGui_getWidgetNames(CPointer, out uint count);
                List<string> names = new List<string>();
                for (uint i = 0; i < count; ++i)
                    names.Add(Util.GetStringFromC_UTF32(namesPtr[i]));

                return names;
            }
        }

        /// <summary>
        /// Removes a single widget that was added to the gui
        /// </summary>
        /// <param name="widget">Widget to remove</param>
        /// <returns>
        /// True when widget is removed, false when widget was not found
        /// </returns>
        public bool Remove(Widget widget)
        {
            widget.Parent = null;
            myWidgets.Remove(widget);
            return tguiGui_remove(CPointer, widget.CPointer);
        }

        /// <summary>
        /// Removes all widgets that were added to the container
        /// </summary>
        public void RemoveAllWidgets()
        {
            foreach (var widget in myWidgets)
                widget.Parent = null;

            myWidgets.Clear();
            tguiGui_removeAllWidgets(CPointer);
        }

        /// <summary>
        /// Places a widget before all other widgets
        /// </summary>
        /// <remarks>
        public void MoveWidgetToFront(Widget widget)
        {
            var index = myWidgets.IndexOf(widget);
            if (index != -1)
            {
                myWidgets.RemoveAt(index);
                myWidgets.Add(widget);
            }

            tguiGui_moveWidgetToFront(CPointer, widget.CPointer);
        }

        /// <summary>
        /// Places a widget behind all other widgets
        /// </summary>
        public void MoveWidgetToBack(Widget widget)
        {
            var index = myWidgets.IndexOf(widget);
            if (index != -1)
            {
                myWidgets.RemoveAt(index);
                myWidgets.Insert(0, widget);
            }

            tguiGui_moveWidgetToBack(CPointer, widget.CPointer);
        }

        /// <summary>
        /// Returns the child widget that is focused inside this container
        /// </summary>
        /// <returns>Focused child widget or null if none of the widgets are currently focused</returns>
        /// <remarks>
        /// If the focused widget is a container then that container is returned. If you want to know which widget
        /// is focused inside that container (recursively) then you should use the GetFocusedLeaf() function.
        /// </remarks>
        public Widget GetFocusedChild()
        {
            int index = tguiGui_getFocusedChildIndex(CPointer);
            if (index >= 0)
                return myWidgets[index];
            else
                return null;
        }

        /// <summary>
        /// Returns the leaf child widget that is focused inside this container
        /// </summary>
        /// <returns>Focused leaf child widget or null if none of the widgets are currently focused</returns>
        /// <remarks>
        /// If the focused widget is a container then the GetFocusedLeaf() is recursively called on that container. If you want
        /// to limit the search to only direct children of this container then you should use the GetFocusedChild() function.
        /// </remarks>
        public Widget GetFocusedLeaf()
        {
            int index = tguiGui_getFocusedChildIndex(CPointer);
            if (index < 0)
                return null;

            Widget widget = myWidgets[index];
            Container container = widget as Container;
            if (container == null)
                return widget;

            Widget leafWidget = container.GetFocusedLeaf();
            if (leafWidget == null)
                return container;

            return leafWidget;
        }

        /// <summary>
        /// Returns the leaf child widget that is located at the given position
        /// </summary>
        /// <param name="x">The x position where the widget will be searched, relative to the gui view</param>
        /// <param name="y">The y position where the widget will be searched, relative to the gui view</param>
        /// <returns>Widget at the queried position, or null when there is no widget at that location</returns>
        /// <remarks>
        /// To use pixel coordinates instead of a position relative to the view, use the GetWidgetBelowMouseCursor function.
        /// </remarks>
        public Widget GetWidgetAtPosition(float x, float y)
        {
            unsafe
            {
                int* indicesPtr = tguiGui_getWidgetAtPositionIndices(CPointer, x, y, out uint count);
                if (count == 0)
                    return null;

                Widget widget = myWidgets[indicesPtr[0]];
                for (uint i = 1; i < count; ++i)
                    widget = (widget as Container).Widgets[indicesPtr[i]];

                return widget;
            }
        }

        /// <summary>
        /// Returns the leaf child widget below the mouse
        /// </summary>
        /// <param name="mouseX">X position of the mouse, in pixel coordinates, relative the the window</param>
        /// <param name="mouseY">Y position of the mouse, in pixel coordinates, relative the the window</param>
        /// <returns>Widget below the mouse, or null when the mouse isn't on top of any widgets</returns>
        /// <remarks>
        /// To coordinates relative to the view instead of absolute pixel coordinates, use the GetWidgetAtPosition function.
        /// </remarks>
        public Widget GetWidgetBelowMouseCursor(int mouseX, int mouseY)
        {
            unsafe
            {
                int* indicesPtr = tguiGui_getWidgetBelowMouseCursorIndices(CPointer, mouseX, mouseY, out uint count);
                if (count == 0)
                    return null;

                Widget widget = myWidgets[indicesPtr[0]];
                for (uint i = 1; i < count; ++i)
                    widget = (widget as Container).Widgets[indicesPtr[i]];

                return widget;
            }
        }

        /// <summary>
        /// Focuses the next widget in the gui
        /// </summary>
        /// <returns>
        /// Whether a new widget was focused
        /// </returns>
        public bool FocusNextWidget()
        {
            return tguiGui_focusNextWidget(CPointer);
        }

        /// <summary>
        /// Focuses the previous widget in the gui
        /// </summary>
        /// <returns>
        /// Whether a new widget was focused
        /// </returns>
        public bool FocusPreviousWidget()
        {
            return tguiGui_focusPreviousWidget(CPointer);
        }

        /// <summary>
        /// Gets or sets whether pressing tab will focus another widget
        /// </summary>
        public bool TabKeyUsageEnabled
        {
            get { return tguiGui_isTabKeyUsageEnabled(CPointer); }
            set { tguiGui_setTabKeyUsageEnabled(CPointer, value); }
        }

        /// <summary>
        /// Draws all the widgets that were added to the gui
        /// </summary>
        public void Draw()
        {
            tguiGui_draw(CPointer);
        }

        /// <summary>
        /// Gets or sets the opacity of all widgets that are added to the gui
        /// </summary>
        /// <remarks>
        /// 0 means completely transparent, while 1 (default) means fully opaque
        /// </remarks>
        public float Opacity
        {
            get { return tguiGui_getOpacity(CPointer); }
            set { tguiGui_setOpacity(CPointer, value); }
        }

        /// <summary>
        /// Gets or sets the character size of all existing and future child widgets
        /// </summary>
        /// <remarks>
        /// The text size specified here overrides the global text size property. By default, the gui does not
        /// pass any text size to the widgets and the widgets will use the global text size as default value.
        /// </remarks>
        public uint TextSize
        {
            get { return tguiGui_getTextSize(CPointer); }
            set { tguiGui_setTextSize(CPointer, value); }
        }

        /// <summary>
        /// Loads the child widgets from a text file
        /// </summary>
        /// <param name="filename">Filename of the widget file</param>
        /// <param name="replaceExisting">Remove existing widgets first if there are any</param>
        /// <exception cref="TGUIException">Thrown when file could not be opened or parsing failed</exception>
        public void LoadWidgetsFromFile(string filename, bool replaceExisting = true)
        {
            if (replaceExisting)
            {
                foreach (var widget in myWidgets)
                    widget.Parent = null;

                myWidgets.Clear();
            }

            if (!tguiGui_loadWidgetsFromFile(CPointer, Util.ConvertStringForC_ASCII(filename), replaceExisting))
                throw new TGUIException(Util.GetStringFromC_ASCII(tgui_getLastError()));

            AddNewWidgetsAfterLoadFromFile();
        }

        /// <summary>
        /// Saves the child widgets to a text file
        /// </summary>
        /// <param name="filename">Filename of the widget file</param>
        /// <exception cref="TGUIException">Thrown when file could not be opened for writing</exception>
        public void SaveWidgetsToFile(string filename)
        {
            if (!tguiGui_saveWidgetsToFile(CPointer, Util.ConvertStringForC_ASCII(filename)))
                throw new TGUIException(Util.GetStringFromC_ASCII(tgui_getLastError()));
        }

        /// <summary>
        /// Sets whether Draw() updates the clock (default), or whether you need to call UpdateTime() on the Gui
        /// </summary>
        public bool DrawingUpdatesTime
        {
            set { tguiGui_setDrawingUpdatesTime(CPointer, value); }
        }

        /// <summary>
        /// Updates the internal clock (for timers, animations and blinking edit cursors)
        /// </summary>
        /// <returns>True if the the contents of the screen changed, false if nothing changed</returns>
        /// <remarks>
        /// You do not need to call this function unless you set DrawingUpdatesTime to false (it is true by default).
        /// </remarks>
        public bool UpdateTime()
        {
            return tguiGui_updateTime(CPointer);
        }

        /// <summary>
        /// Gets or sets the filter function that determines whether the gui should handle the event
        /// </summary>
        /// <remarks>
        /// By default the event filter is null and the gui will handle all events.
        /// You can set a function here that takes an event as parameter and returns whether or not
        /// the gui should handle this event.
        /// </remarks>
        public Func<Event, bool> EventFilter
        {
            get { return myEventFilter; }
            set { myEventFilter = value; }
        }

        /// <summary>
        /// Passes the event to the widgets
        /// </summary>
        /// <param name="ev">The event that was polled from the window</param>
        /// <returns>
        /// Has the event been consumed?
        /// When this function returns false, then the event was ignored by all widgets.
        /// </returns>
        protected bool HandleEvent(Event ev)
        {
            if ((myEventFilter != null) && !myEventFilter(ev))
                return false;

            bool processed = tguiGui_handleEvent(CPointer, ev);
            EventProcessed?.Invoke(this, new SignalArgsEventProcessed(ev, processed));
            return processed;
        }

        /// <summary>
        /// Create the new C# widgets for all the the c++ widgets that got added
        /// </summary>
        protected internal void AddNewWidgetsAfterLoadFromFile()
        {
            unsafe
            {
                IntPtr* widgetsPtr = tguiGui_getWidgets(CPointer, out uint count);
                for (int i = myWidgets.Count; i < (int)count; ++i)
                {
                    Widget widget = Util.GetWidgetFromC(widgetsPtr[i], this);
                    myWidgets.Add(widget);

                    Container container = widget as Container;
                    if (container != null)
                        container.AddNewWidgetsAfterLoadFromFile();
                }
            }
        }

        private void OnMouseMoved(object sender, MouseMoveEventArgs e)
        {
            Event ev = new Event
            {
                Type = EventType.MouseMoved
            };
            ev.MouseMove.X = e.X;
            ev.MouseMove.Y = e.Y;
            HandleEvent(ev);
        }

        private void OnTouchMoved(object sender, TouchEventArgs e)
        {
            Event ev = new Event
            {
                Type = EventType.TouchMoved
            };
            ev.Touch.X = e.X;
            ev.Touch.Y = e.Y;
            ev.Touch.Finger = e.Finger;
            HandleEvent(ev);
        }

        private void OnMousePressed(object sender, MouseButtonEventArgs e)
        {
            Event ev = new Event
            {
                Type = EventType.MouseButtonPressed
            };
            ev.MouseButton.X = e.X;
            ev.MouseButton.Y = e.Y;
            ev.MouseButton.Button = e.Button;
            HandleEvent(ev);
        }

        private void OnTouchBegan(object sender, TouchEventArgs e)
        {
            Event ev = new Event
            {
                Type = EventType.TouchBegan
            };
            ev.Touch.X = e.X;
            ev.Touch.Y = e.Y;
            ev.Touch.Finger = e.Finger;
            HandleEvent(ev);
        }

        private void OnMouseReleased(object sender, MouseButtonEventArgs e)
        {
            Event ev = new Event
            {
                Type = EventType.MouseButtonReleased
            };
            ev.MouseButton.X = e.X;
            ev.MouseButton.Y = e.Y;
            ev.MouseButton.Button = e.Button;
            HandleEvent(ev);
        }

        private void OnTouchEnded(object sender, TouchEventArgs e)
        {
            Event ev = new Event
            {
                Type = EventType.TouchEnded
            };
            ev.Touch.X = e.X;
            ev.Touch.Y = e.Y;
            ev.Touch.Finger = e.Finger;
            HandleEvent(ev);
        }

        private void OnKeyPressed(object sender, KeyEventArgs e)
        {
            Event ev = new Event
            {
                Type = EventType.KeyPressed
            };
            ev.Key.Code = e.Code;
            ev.Key.Control = Convert.ToInt32(e.Control);
            ev.Key.Shift = Convert.ToInt32(e.Shift);
            ev.Key.Alt = Convert.ToInt32(e.Alt);
            ev.Key.System = Convert.ToInt32(e.System);
            HandleEvent(ev);
        }

        private void OnKeyReleased(object sender, KeyEventArgs e)
        {
            Event ev = new Event
            {
                Type = EventType.KeyReleased
            };
            ev.Key.Code = e.Code;
            ev.Key.Control = Convert.ToInt32(e.Control);
            ev.Key.Shift = Convert.ToInt32(e.Shift);
            ev.Key.Alt = Convert.ToInt32(e.Alt);
            ev.Key.System = Convert.ToInt32(e.System);
            HandleEvent(ev);
        }

        private void OnTextEntered(object sender, TextEventArgs e)
        {
            Event ev = new Event
            {
                Type = EventType.TextEntered
            };
            ev.Text.Unicode = (uint)Char.ConvertToUtf32(e.Unicode, 0);
            HandleEvent(ev);
        }

        private void OnMouseWheelScrolled(object sender, MouseWheelScrollEventArgs e)
        {
            Event ev = new Event
            {
                Type = EventType.MouseWheelScrolled
            };
            ev.MouseWheelScroll.Wheel = e.Wheel;
            ev.MouseWheelScroll.Delta = e.Delta;
            ev.MouseWheelScroll.X = e.X;
            ev.MouseWheelScroll.Y = e.Y;
            HandleEvent(ev);
        }

        private void OnLostFocus(object sender, EventArgs e)
        {
            Event ev = new Event
            {
                Type = EventType.LostFocus
            };
            HandleEvent(ev);
        }

        private void OnGainedFocus(object sender, EventArgs e)
        {
            Event ev = new Event
            {
                Type = EventType.GainedFocus
            };
            HandleEvent(ev);
        }

        /// <summary>Event handler that provides a callback for each event processed by the gui</summary>
        public event EventHandler<SignalArgsEventProcessed> EventProcessed = null;

        private RenderWindow myRenderTarget = null;
        private Func<Event, bool> myEventFilter = null;

        // Children need to be stored to keep their delegates alive
        private readonly List<Widget> myWidgets = new List<Widget>();


        #region Imports

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private IntPtr tgui_getLastError();

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private IntPtr tguiGui_create();

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private void tguiGui_destroy(IntPtr cPointer);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private void tguiGui_setTargetRenderWindow(IntPtr cPointer, IntPtr cPointerRenderWindow);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private void tguiGui_setView(IntPtr cPointer, IntPtr cPointerView);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private bool tguiGui_handleEvent(IntPtr cPointer, Event ev);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private void tguiGui_add(IntPtr cPointer, IntPtr cPointerWidget, IntPtr widgetName);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private IntPtr tguiGui_get(IntPtr cPointer, IntPtr widgetName);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        unsafe static extern private IntPtr* tguiGui_getWidgets(IntPtr cPointer, out uint count);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        unsafe static extern private IntPtr* tguiGui_getWidgetNames(IntPtr cPointer, out uint count);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private bool tguiGui_remove(IntPtr cPointer, IntPtr cPointerWidget);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private void tguiGui_removeAllWidgets(IntPtr cPointer);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private void tguiGui_moveWidgetToFront(IntPtr cPointer, IntPtr cPointerWidget);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private void tguiGui_moveWidgetToBack(IntPtr cPointer, IntPtr cPointerWidget);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private int tguiGui_getFocusedChildIndex(IntPtr cPointer);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        unsafe static extern private int* tguiGui_getWidgetAtPositionIndices(IntPtr cPointer, float x, float y, out uint count);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        unsafe static extern private int* tguiGui_getWidgetBelowMouseCursorIndices(IntPtr cPointer, int x, int y, out uint count);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private bool tguiGui_focusNextWidget(IntPtr cPointer);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private bool tguiGui_focusPreviousWidget(IntPtr cPointer);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private void tguiGui_setTabKeyUsageEnabled(IntPtr cPointer, bool enabled);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private bool tguiGui_isTabKeyUsageEnabled(IntPtr cPointer);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private void tguiGui_draw(IntPtr cPointer);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private void tguiGui_setFont(IntPtr cPointer, IntPtr font);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private void tguiGui_setOpacity(IntPtr cPointer, float opacity);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private float tguiGui_getOpacity(IntPtr cPointer);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private void tguiGui_setTextSize(IntPtr cPointer, uint textSize);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private uint tguiGui_getTextSize(IntPtr cPointer);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private bool tguiGui_loadWidgetsFromFile(IntPtr cPointer, IntPtr filename, bool replaceExisting);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private bool tguiGui_saveWidgetsToFile(IntPtr cPointer, IntPtr filename);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private void tguiGui_setDrawingUpdatesTime(IntPtr cPointer, bool drawUpdatesTime);

        [DllImport(Global.CTGUI, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        static extern private bool tguiGui_updateTime(IntPtr cPointer);

        #endregion
    }
}
