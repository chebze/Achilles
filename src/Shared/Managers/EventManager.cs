using Achilles.Shared.Abstractions;

namespace Achilles.Shared.Managers;

public class EventManager : IEventManager
{
    private readonly Dictionary<string, List<object>> _listeners = new();

    private void AddHandler(string eventName, object handler)
    {
        if (!_listeners.ContainsKey(eventName))
            _listeners.Add(eventName, new List<object>());

        _listeners[eventName].Add(handler);
    }

    public void Add<T1>(string eventName, Func<T1, Task> handler) => AddHandler(eventName, handler);
    public void Add<T1, T2>(string eventName, Func<T1, T2, Task> handler) => AddHandler(eventName, handler);
    public void Add<T1, T2, T3>(string eventName, Func<T1, T2, T3, Task> handler) => AddHandler(eventName, handler);
    public void Add<T1, T2, T3, T4>(string eventName, Func<T1, T2, T3, T4, Task> handler) => AddHandler(eventName, handler);
    public void Add<T1, T2, T3, T4, T5>(string eventName, Func<T1, T2, T3, T4, T5, Task> handler) => AddHandler(eventName, handler);
    public void Add<T1, T2, T3, T4, T5, T6>(string eventName, Func<T1, T2, T3, T4, T5, T6, Task> handler) => AddHandler(eventName, handler);
    public void Add<T1, T2, T3, T4, T5, T6, T7>(string eventName, Func<T1, T2, T3, T4, T5, T6, T7, Task> handler) => AddHandler(eventName, handler);
    public void Add<T1, T2, T3, T4, T5, T6, T7, T8>(string eventName, Func<T1, T2, T3, T4, T5, T6, T7, T8, Task> handler) => AddHandler(eventName, handler);
    public void Add<T1, T2, T3, T4, T5, T6, T7, T8, T9>(string eventName, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, Task> handler) => AddHandler(eventName, handler);
    public void Add<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(string eventName, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, Task> handler) => AddHandler(eventName, handler);

    private void RemoveHandler(string eventName, object handler)
    {
        if (_listeners.TryGetValue(eventName, out var handlers))
            if(handlers.Contains(handler))
                handlers.Remove(handler);
    }

    public void Remove<T1>(string eventName, Func<T1, Task> handler) => RemoveHandler(eventName, handler);
    public void Remove<T1, T2>(string eventName, Func<T1, T2, Task> handler) => RemoveHandler(eventName, handler);
    public void Remove<T1, T2, T3>(string eventName, Func<T1, T2, T3, Task> handler) => RemoveHandler(eventName, handler);
    public void Remove<T1, T2, T3, T4>(string eventName, Func<T1, T2, T3, T4, Task> handler) => RemoveHandler(eventName, handler);
    public void Remove<T1, T2, T3, T4, T5>(string eventName, Func<T1, T2, T3, T4, T5, Task> handler) => RemoveHandler(eventName, handler);
    public void Remove<T1, T2, T3, T4, T5, T6>(string eventName, Func<T1, T2, T3, T4, T5, T6, Task> handler) => RemoveHandler(eventName, handler);
    public void Remove<T1, T2, T3, T4, T5, T6, T7>(string eventName, Func<T1, T2, T3, T4, T5, T6, T7, Task> handler) => RemoveHandler(eventName, handler);
    public void Remove<T1, T2, T3, T4, T5, T6, T7, T8>(string eventName, Func<T1, T2, T3, T4, T5, T6, T7, T8, Task> handler) => RemoveHandler(eventName, handler);
    public void Remove<T1, T2, T3, T4, T5, T6, T7, T8, T9>(string eventName, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, Task> handler) => RemoveHandler(eventName, handler);
    public void Remove<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(string eventName, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, Task> handler) => RemoveHandler(eventName, handler);

    public async Task<bool> Trigger<T1>(string eventName, T1 arg1)
    {
        if (!_listeners.TryGetValue(eventName, out var handlers))
            return false;

        foreach (var handler in handlers)
            if (handler is Func<T1, Task> func)
                await func(arg1);

        return true;
    }

    public async Task<bool> Trigger<T1, T2>(string eventName, T1 arg1, T2 arg2)
    {
        if (!_listeners.TryGetValue(eventName, out var handlers))
            return false;

        foreach (var handler in handlers)
            if (handler is Func<T1, T2, Task> func)
                await func(arg1, arg2);

        return true;
    }

    public async Task<bool> Trigger<T1, T2, T3>(string eventName, T1 arg1, T2 arg2, T3 arg3)
    {
        if (!_listeners.TryGetValue(eventName, out var handlers))
            return false;

        foreach (var handler in handlers)
            if (handler is Func<T1, T2, T3, Task> func)
                await func(arg1, arg2, arg3);

        return true;
    }

    public async Task<bool> Trigger<T1, T2, T3, T4>(string eventName, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
        if (!_listeners.TryGetValue(eventName, out var handlers))
            return false;

        foreach (var handler in handlers)
            if (handler is Func<T1, T2, T3, T4, Task> func)
                await func(arg1, arg2, arg3, arg4);

        return true;
    }

    public async Task<bool> Trigger<T1, T2, T3, T4, T5>(string eventName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
    {
        if (!_listeners.TryGetValue(eventName, out var handlers))
            return false;

        foreach (var handler in handlers)
            if (handler is Func<T1, T2, T3, T4, T5, Task> func)
                await func(arg1, arg2, arg3, arg4, arg5);

        return true;
    }

    public async Task<bool> Trigger<T1, T2, T3, T4, T5, T6>(string eventName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
    {
        if (!_listeners.TryGetValue(eventName, out var handlers))
            return false;

        foreach (var handler in handlers)
            if (handler is Func<T1, T2, T3, T4, T5, T6, Task> func)
                await func(arg1, arg2, arg3, arg4, arg5, arg6);

        return true;
    }

    public async Task<bool> Trigger<T1, T2, T3, T4, T5, T6, T7>(string eventName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
    {
        if (!_listeners.TryGetValue(eventName, out var handlers))
            return false;

        foreach (var handler in handlers)
            if (handler is Func<T1, T2, T3, T4, T5, T6, T7, Task> func)
                await func(arg1, arg2, arg3, arg4, arg5, arg6, arg7);

        return true;
    }

    public async Task<bool> Trigger<T1, T2, T3, T4, T5, T6, T7, T8>(string eventName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
    {
        if (!_listeners.TryGetValue(eventName, out var handlers))
            return false;

        foreach (var handler in handlers)
            if (handler is Func<T1, T2, T3, T4, T5, T6, T7, T8, Task> func)
                await func(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);

        return true;
    }

    public async Task<bool> Trigger<T1, T2, T3, T4, T5, T6, T7, T8, T9>(string eventName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
    {
        if (!_listeners.TryGetValue(eventName, out var handlers))
            return false;

        foreach (var handler in handlers)
            if (handler is Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, Task> func)
                await func(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);

        return true;
    }

    public async Task<bool> Trigger<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(string eventName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
    {
        if (!_listeners.TryGetValue(eventName, out var handlers))
            return false;

        foreach (var handler in handlers)
            if (handler is Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, Task> func)
                await func(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);

        return true;
    }
}