using System;

namespace Gamification.Domain.Exceptions;
public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}
public class NotEligibleException : DomainException
{
    public NotEligibleException() : base("Elegibilidade não satisfeita.") { }
}
public class AlreadyAwardedException : DomainException
{
    public AlreadyAwardedException() : base("Badge já concedida.") { }
}
public class ThemeNotConfiguredException : DomainException
{
    public ThemeNotConfiguredException() : base("Tema não configurado.") { }
}
public class ConfigurationException : DomainException
{
    public ConfigurationException(string message) : base(message) { }
}
