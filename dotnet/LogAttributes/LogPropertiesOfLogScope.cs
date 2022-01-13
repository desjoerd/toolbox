using System.Reflection;

namespace LogAttributes;

public static class LogPropertiesOfLogScope
{
    public static LogPropertiesOfLogScope<T> Create<T>(T subject)
        where T : notnull
        => new(subject);
}

public class LogPropertiesOfLogScope<T> : ValuesLogScope
    where T : notnull
{
    private class LogProperty
    {
        private readonly PropertyInfo _propertyInfo;

        public LogProperty(PropertyInfo propertyInfo, LogPropertyAttribute logRequestProperty)
        {
            _propertyInfo = propertyInfo;
            Key = logRequestProperty.Key is null ? _propertyInfo.Name : logRequestProperty.Key;
        }

        public string Key { get; }

        public object? GetValue(object request) => _propertyInfo.GetMethod?.Invoke(request, null);
    }

    private static readonly List<LogProperty> LogProperties = typeof(T)
        .GetProperties()
        .Select(property => (property, logRequestPropertyAttribute: property.GetCustomAttribute<LogPropertyAttribute>()))
        .Where(x => x.logRequestPropertyAttribute is not null)
        .Select(x => new LogProperty(x.property, x.logRequestPropertyAttribute!))
        .ToList();

    public LogPropertiesOfLogScope(T subject)
        : base(LogProperties
            .Select(prop => new KeyValuePair<string, object?>(prop.Key, prop.GetValue(subject))))
    {
    }
}
