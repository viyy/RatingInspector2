using System;
using System.Collections.Generic;

namespace ConsoleUI.Utilities
{
    //Todo: maybe convert to simple static class?
    /// <summary>
    /// Менеджер событий. 
    /// </summary>
    public class EventManager
    {
        private static EventManager _eventManager;

        private Dictionary<string, Action<EventArgs>> _eventDictionary;

        private static EventManager Instance
        {
            get
            {
                if (_eventManager != null) return _eventManager;
                _eventManager = new EventManager();
                _eventManager.Init();
                return _eventManager;
            }
        }

        private void Init()
        {
            if (_eventDictionary == null) _eventDictionary = new Dictionary<string, Action<EventArgs>>();
        }

        /// <summary>
        ///     Регистриция слушателя события
        /// </summary>
        /// <param name="eventName">Тип события в игре</param>
        /// <param name="listener">Метод, вызывающийся на событии</param>
        public static void On(string eventName, Action<EventArgs> listener)
        {
            if (Instance._eventDictionary.TryGetValue(eventName, out var thisEvent))
            {
                Instance._eventDictionary[eventName] += listener;
            }
            else
            {
                Instance._eventDictionary.Add(eventName, listener);
            }
        }

        /// <summary>
        ///     Отписываемся от прослушки событий
        /// </summary>
        /// <param name="eventName">Тип события в игре</param>
        /// <param name="listener">Метод, который вызывался на событии</param>
        public static void Off(string eventName, Action<EventArgs> listener)
        {
            if (_eventManager == null) return;
            if (Instance._eventDictionary.ContainsKey(eventName))
                // ReSharper disable once DelegateSubtraction
                Instance._eventDictionary[eventName]-=listener;
        }

        /// <summary>
        ///     Вызываем событие
        /// </summary>
        /// <param name="eventName">Тип события</param>
        /// <param name="eventArgs">Параметры события</param>
        public static void Emit(string eventName, EventArgs eventArgs)
        {
            if (Instance._eventDictionary.TryGetValue(eventName, out var thisEvent)) 
                thisEvent.Invoke(eventArgs);
        }

        /// <summary>
        ///     Сброс всех слушателей
        /// </summary>
        public static void Reset()
        {
            _eventManager = new EventManager();
            _eventManager.Init();
        }
    }
}