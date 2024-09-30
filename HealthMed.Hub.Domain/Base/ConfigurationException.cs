using System.Runtime.Serialization;

namespace HealthMed.Hub.Domain.Base;

[Serializable]
public class ConfigurationException : Exception
{
    public ConfigurationException()
    {
    }

    public ConfigurationException(string? message) : base(message)
    {
    }

    public ConfigurationException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

#pragma warning disable SYSLIB0051 // Type or member is obsolete
    protected ConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context)
#pragma warning restore SYSLIB0051 // Type or member is obsolete
    {
    }
}
