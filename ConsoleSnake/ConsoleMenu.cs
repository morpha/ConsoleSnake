using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CC = ConsoleCompanion;

namespace ConsoleSnake
{
    class ConsoleMenu
    {
        private enum InputAction
        {
            Invalid,
            Up,
            Down,
            Left,
            Right,
            Enter,
            Escape
        }
        internal struct MenuItem
        {
            /// <summary>
            /// Action to perform when the menu entry is selected.
            /// </summary>
            /// <returns>true = the menu will resume after executing the action, false = the menu will return flow to where <see cref="Show"/> was called from.</returns>
            public delegate bool MenuAction();
            public string Label { get; set; } = "";
            public string Description { get; set; } = "";
            public MenuAction? Action { get; set; }

            public MenuItem(string label, string desc = "", MenuAction? action = null)
            {
                Label = label;
                Description = desc;
                Action = action;
            }
        }

        private List<MenuItem> _menuItems = new();
        private (Int32 Selected, Int32 Previous) _selection = (-1, -1);
        private Int32 _longestLabel = 0;
        private (Int32 left, Int32 top, Int32 right, Int32 bottom) _bounds = (0, 0, 0, 0);
        public ConsoleMenu() { }
        public void Show()
        {

            CalculateBounds();
            Render();

            bool running = true;
            InputAction action;
            while (running)
            {
                if(Program.WindowIntegrity())
                {
                    Console.Clear();
                    CalculateBounds();
                    Render();
                }
                action = HandleInput();
                switch(action)
                {
                    case InputAction.Up:
                        _selection.Previous = _selection.Selected;
                        if (_selection.Selected > 0)
                            _selection.Selected -= 1;
                        else
                            _selection.Selected = 0;
                        break;
                    case InputAction.Down:
                        _selection.Previous = _selection.Selected;
                        if (_selection.Selected < _menuItems.Count - 1)
                            _selection.Selected += 1;
                        else
                            _selection.Selected = _menuItems.Count - 1;
                        break;
                    case InputAction.Enter:
                        running = _menuItems[_selection.Selected].Action();
                        Render();
                        break;
                    case InputAction.Escape:
                        break;
                }

                if (_selection.Selected != _selection.Previous)
                    UpdateSelection();
            }
            Hide();
        }

        private void UpdateSelection()
        {
            Console.SetCursorPosition(_bounds.left + 2, _bounds.top + 2 + _selection.Selected);
            Console.BackgroundColor = ConsoleColor.Green;
            Console.WriteLine(_menuItems[_selection.Selected].Label);
            Console.SetCursorPosition(_bounds.left + 2, _bounds.top + 2 + _selection.Previous);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine(_menuItems[_selection.Previous].Label);
        }

        private void CalculateBounds()
        {
            _bounds.left = CC.CenterX - _longestLabel / 2 - 2;
            _bounds.top = CC.CenterY - _menuItems.Count / 2 - 2;
            _bounds.right = _bounds.left + _longestLabel + 4 - (_longestLabel % 2);
            _bounds.bottom = _bounds.top + _menuItems.Count + 3;
        }
        private void Hide()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            for(Int32 top = _bounds.top; top <= _bounds.bottom; ++top)
            {
                Console.CursorTop = top;
                for(Int32 left = _bounds.left; left <= _bounds.right; ++left)
                {
                    Console.CursorLeft = left;
                    Console.Write(' ');
                }
            }
        }

        private void Render()
        {
            Console.CursorLeft = _bounds.left;            
            Console.CursorTop = _bounds.top;

            Console.BackgroundColor = ConsoleColor.Black;

            // Draw frame
            Console.ForegroundColor = ConsoleColor.Yellow;
            for (Int32 top = _bounds.top; top <= _bounds.bottom; ++top)
            {
                Console.CursorTop = top;
                for (Int32 left = _bounds.left; left <= _bounds.right; ++left)
                {
                    Console.CursorLeft = left;

                    if(top == _bounds.top && left == _bounds.left)
                        Console.Write('╔');
                    else if (top == _bounds.top && left == _bounds.right)
                        Console.Write('╗');
                    else if (top == _bounds.bottom && left == _bounds.left)
                        Console.Write('╚');
                    else if (top == _bounds.bottom && left == _bounds.right)
                        Console.Write('╝');
                    else if (left == _bounds.left || left == _bounds.right)
                        Console.Write('║');
                    else if (top == _bounds.top || top == _bounds.bottom)
                        Console.Write('═');
                    else
                        Console.Write(' ');
                }
            }

            // Draw items
            Console.CursorTop = _bounds.top + 2;
            for (Int32 i = 0; i < _menuItems.Count; ++i)
            {
                Console.CursorLeft = _bounds.left + 2;
                
                if (i == _selection.Selected)
                    Console.BackgroundColor = ConsoleColor.Green;
                else
                    Console.BackgroundColor = ConsoleColor.Black;
                
                Console.WriteLine(_menuItems[i].Label);
            }
        }

        /// <summary>
        /// Wait for input and determine which action the user requested (or return Invalid)
        /// </summary>
        /// <returns>Menu.InputAction assigned to the key the user pressed, or InputAction.Invalid if the key did not match any InputAction.</returns>
        private static InputAction HandleInput()
        {
            return Console.ReadKey(true).Key switch
            {
                ConsoleKey.UpArrow or ConsoleKey.W      => InputAction.Up,
                ConsoleKey.DownArrow or ConsoleKey.S    => InputAction.Down,
                ConsoleKey.Enter or ConsoleKey.Spacebar => InputAction.Enter,
                ConsoleKey.Escape                       => InputAction.Escape,
                _                                       => InputAction.Invalid
            };
        }

        /// <summary>
        /// Add an item to the menu, must be performed before the menu is Show()n.
        /// </summary>
        /// <param name="label">The label (visible text, name) of the menu item.</param>
        /// <param name="description">A description shown when the menu item is in focus.</param>
        /// <param name="action">The action to perform when the menu item is selected, return true to return to the menu after performing the action, return false to return flow to where the Show()-method was called from.</param>
        /// <param name="selected">Whether this menu item should be selected when Show() is called. The last item added with this set will take precedence.</param>
        public void AddItem(string label, string description, MenuItem.MenuAction action, bool selected = false)
        {
            MenuItem menuItem = new()
            {
                Label = label,
                Description = description,
                Action = action
            };

            if (_selection.Selected == -1 || selected)
                _selection.Selected = _selection.Previous = _menuItems.Count;
            if(label.Length > _longestLabel)
                _longestLabel = label.Length;

            _menuItems.Add(menuItem);
        }

        public bool RemoveItem(string label) => _menuItems.Remove(_menuItems.Find(x => x.Label == label));
        public void Clear() => _menuItems.Clear();
    }
}
