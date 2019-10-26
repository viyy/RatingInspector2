using System;

namespace ConsoleUI.Pages
{
    public abstract class ScreenModel
    {
        public int PageGuid { get; protected set; }
        public string MenuButton { get; set; } 
        public string Title { get; set; }
        public int Id { get; set; }
        
        public bool Active { get; set; }

        public abstract bool Render();

        public virtual void Input(ConsoleKeyInfo key)
        {
        }

        public virtual void Enter()
        {
        }

        public virtual void AddParam(string name, string data)
        {
        }

        public virtual void AddParam(string name, int data)
        {
        }

        public virtual void AddParam(string name, double data)
        {
        }

        public virtual void AddParam(string name, object data)
        {
        }
    }
}