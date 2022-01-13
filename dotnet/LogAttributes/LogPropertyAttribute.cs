namespace LogAttributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class LogPropertyAttribute : Attribute
{
    public string? Key { get; }

    public LogPropertyAttribute()
    {
    }

    public LogPropertyAttribute(string key) => Key = key;
}
