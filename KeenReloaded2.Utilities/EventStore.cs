using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeenReloaded2.Utilities
{
    public static class EventStore<T>
    {
        private static Dictionary<string, List<EventHandler<ControlEventArgs.ControlEventArgs<T>>>> Listeners;
        public static void Publish(string eventName, T data)
        {
            if (Listeners == null)
                Listeners = new Dictionary<string, List<EventHandler<ControlEventArgs.ControlEventArgs<T>>>>();

            if (Listeners.TryGetValue(eventName,  out List<EventHandler<ControlEventArgs.ControlEventArgs<T>>> callbacks))
            {
                foreach (var callback in callbacks)
                {
                    ControlEventArgs.ControlEventArgs<T> e = new ControlEventArgs.ControlEventArgs<T>()
                    {
                        Data = data,
                        EventName = eventName
                    };
                    callback?.Invoke(eventName, e);
                }
            }
        }

        public static void Subscribe(string eventName, EventHandler<ControlEventArgs.ControlEventArgs<T>> callback)
        {
            if (Listeners == null)
                Listeners = new Dictionary<string, List<EventHandler<ControlEventArgs.ControlEventArgs<T>>>>();

            if (Listeners.TryGetValue(eventName, out List<EventHandler<ControlEventArgs.ControlEventArgs<T>>> callbacks))
            {
                callbacks.Add(callback);
            }
            else
            {
                Listeners.Add(eventName, new List<EventHandler<ControlEventArgs.ControlEventArgs<T>>>() { callback });
            }
        }
    }
}
