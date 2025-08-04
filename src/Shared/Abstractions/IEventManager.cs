namespace Achilles.Shared.Abstractions;

public interface IEventManager
{
    void Add<T1>(string eventName, Func<T1, Task> handler);
    void Add<T1, T2>(string eventName, Func<T1, T2, Task> handler);
    void Add<T1, T2, T3>(string eventName, Func<T1, T2, T3, Task> handler);
    void Add<T1, T2, T3, T4>(string eventName, Func<T1, T2, T3, T4, Task> handler);
    void Add<T1, T2, T3, T4, T5>(string eventName, Func<T1, T2, T3, T4, T5, Task> handler);
    void Add<T1, T2, T3, T4, T5, T6>(string eventName, Func<T1, T2, T3, T4, T5, T6, Task> handler);
    void Add<T1, T2, T3, T4, T5, T6, T7>(string eventName, Func<T1, T2, T3, T4, T5, T6, T7, Task> handler);
    void Add<T1, T2, T3, T4, T5, T6, T7, T8>(string eventName, Func<T1, T2, T3, T4, T5, T6, T7, T8, Task> handler);
    void Add<T1, T2, T3, T4, T5, T6, T7, T8, T9>(string eventName, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, Task> handler);
    void Add<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(string eventName, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, Task> handler);

    void Remove<T1>(string eventName, Func<T1, Task> handler);
    void Remove<T1, T2>(string eventName, Func<T1, T2, Task> handler);
    void Remove<T1, T2, T3>(string eventName, Func<T1, T2, T3, Task> handler);
    void Remove<T1, T2, T3, T4>(string eventName, Func<T1, T2, T3, T4, Task> handler);
    void Remove<T1, T2, T3, T4, T5>(string eventName, Func<T1, T2, T3, T4, T5, Task> handler);
    void Remove<T1, T2, T3, T4, T5, T6>(string eventName, Func<T1, T2, T3, T4, T5, T6, Task> handler);
    void Remove<T1, T2, T3, T4, T5, T6, T7>(string eventName, Func<T1, T2, T3, T4, T5, T6, T7, Task> handler);
    void Remove<T1, T2, T3, T4, T5, T6, T7, T8>(string eventName, Func<T1, T2, T3, T4, T5, T6, T7, T8, Task> handler);
    void Remove<T1, T2, T3, T4, T5, T6, T7, T8, T9>(string eventName, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, Task> handler);
    void Remove<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(string eventName, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, Task> handler);

    Task<bool> Trigger<T1>(string eventName, T1 arg1);
    Task<bool> Trigger<T1, T2>(string eventName, T1 arg1, T2 arg2);
    Task<bool> Trigger<T1, T2, T3>(string eventName, T1 arg1, T2 arg2, T3 arg3);
    Task<bool> Trigger<T1, T2, T3, T4>(string eventName, T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    Task<bool> Trigger<T1, T2, T3, T4, T5>(string eventName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    Task<bool> Trigger<T1, T2, T3, T4, T5, T6>(string eventName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
    Task<bool> Trigger<T1, T2, T3, T4, T5, T6, T7>(string eventName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);
    Task<bool> Trigger<T1, T2, T3, T4, T5, T6, T7, T8>(string eventName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);
    Task<bool> Trigger<T1, T2, T3, T4, T5, T6, T7, T8, T9>(string eventName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9);
    Task<bool> Trigger<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(string eventName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10);
}