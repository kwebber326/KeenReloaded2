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

            if (Listeners.TryGetValue(eventName, out List<EventHandler<ControlEventArgs.ControlEventArgs<T>>> callbacks))
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

        public static void Subscribe(string eventName, EventHandler<ControlEventArgs.ControlEventArgs<T>> callback, bool singleInstancePerType = false)
        {
            if (Listeners == null)
                Listeners = new Dictionary<string, List<EventHandler<ControlEventArgs.ControlEventArgs<T>>>>();

            if (Listeners.TryGetValue(eventName, out List<EventHandler<ControlEventArgs.ControlEventArgs<T>>> callbacks))
            {
                //by default, only reject the exact same delegate (same target instance + method) so that
                //multiple instances of the same control type (e.g. PointMarkerControl) can each subscribe.
                //callers that want at most one subscriber per type (e.g. SoundPlayer, to avoid double sounds)
                //pass singleInstancePerType: true.
                bool alreadySubscribed = singleInstancePerType
                    ? callbacks.Any(c => c.Method.DeclaringType == callback.Method.DeclaringType)
                    : callbacks.Any(c => c.Method == callback.Method && ReferenceEquals(c.Target, callback.Target));

                if (!alreadySubscribed)
                    callbacks.Add(callback);
            }
            else
            {
                Listeners.Add(eventName, new List<EventHandler<ControlEventArgs.ControlEventArgs<T>>>() { callback });
            }
        }

        public static void UnSubscribe(string eventName, EventHandler<ControlEventArgs.ControlEventArgs<T>> callback)
        {
            if (Listeners == null || callback?.Method == null)
                return;

            if (Listeners.TryGetValue(eventName, out List<EventHandler<ControlEventArgs.ControlEventArgs<T>>> callbackList))
            {
                callbackList.RemoveAll(c => c.Method.DeclaringType == callback.Method.DeclaringType);
            }
        }
    }
}
