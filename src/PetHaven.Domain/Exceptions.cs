using System;

namespace PetHaven.Domain;

// Базовий доменний виняток
public class PetHavenDomainException : Exception
{
    public PetHavenDomainException(string message) : base(message) { }
}

// Помилка порушення бізнес-правил
public class BusinessRuleException : PetHavenDomainException
{
    public BusinessRuleException(string message) : base(message) { }
}

// Помилка інфраструктурного шару
public class InfrastructureException : Exception
{
    public InfrastructureException(string message, Exception innerException) 
        : base(message, innerException) { }
}