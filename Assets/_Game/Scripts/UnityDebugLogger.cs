using Microsoft.Extensions.Logging;
using UnityEngine;
using System;

public class UnityDebugLogger : Microsoft.Extensions.Logging.ILogger
{
    private readonly string _categoryName;

    public UnityDebugLogger(string categoryName)
    {
        _categoryName = categoryName;
    }

    public IDisposable BeginScope<TState>(TState state) => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId,
                            TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        if (!IsEnabled(logLevel)) return;

        string message = $"[{logLevel}] {_categoryName}: {formatter(state, exception)}";
        switch (logLevel)
        {
            case LogLevel.Critical:
            case LogLevel.Error:
                Debug.LogError(message);
                break;
            case LogLevel.Warning:
                Debug.LogWarning(message);
                break;
            default:
                Debug.Log(message);
                break;
        }
    }
}

public class UnityDebugLoggerProvider : ILoggerProvider
{
    public Microsoft.Extensions.Logging.ILogger CreateLogger(string categoryName)
    {
        return new UnityDebugLogger(categoryName);
    }

    public void Dispose()
    {
    }
}