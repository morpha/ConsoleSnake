using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleSnake
{
    internal class Menu
    {
        internal class MenuItem
        {
            //public delegate bool MenuAction(object? customData = null);
            public delegate bool MenuAction();
            public string Label { get; set; }
            public string Description { get; set; }
            public MenuAction Action { get; set; }
        }

        private Dictionary<string, MenuItem> _menuItems = new Dictionary<string, MenuItem>();
        private string _selectedEntry = "";
        public Menu() { }
        public void Show()
        {

            Console.CursorTop = 10;
            foreach (var item in _menuItems)
            {
                Console.CursorLeft = 30;
                if (_selectedEntry == item.Key) 
                {
                    Console.BackgroundColor = ConsoleColor.Green;
                }
                else
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                }
                Console.WriteLine(item.Key);
            }
            while (true)
            {
                Thread.Sleep(1000);
                break;
            }
        }

        public void AddItem(string label, string description, MenuItem.MenuAction action, bool selected = false)
        {
            MenuItem menuItem = new MenuItem();

            menuItem.Label = label;
            menuItem.Description = description;
            menuItem.Action = action;

            if (_selectedEntry == null || selected)
                _selectedEntry = label;

            _menuItems.Add(label, menuItem);
        }

        public bool RemoveItem(string label) => _menuItems.Remove(label);
        public void Clear() => _menuItems.Clear();
    }
}
