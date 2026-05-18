namespace PetHaven.Domain;

public interface IPricingStrategy
{
    string Name { get; }
    decimal CalculatePrice(decimal basePrice, int days, PetType petType);
}

// Стратегія 1: Стандартний тариф
public class RegularPricingStrategy : IPricingStrategy
{
    public string Name => "Стандартний";
    public decimal CalculatePrice(decimal basePrice, int days, PetType petType) => basePrice * days;
}

public class AnimalSpecificPricingStrategy : IPricingStrategy
{
    public string Name => "Диференційований за типом тварини";
    public decimal CalculatePrice(decimal basePrice, int days, PetType petType)
    {
        decimal multiplier = petType switch
        {
            PetType.Cat => 0.9m,      // 10% знижка для котів
            PetType.Dog => 1.1m,      // 11% націнка для собак (потребують вигулу)
            _ => 1.0m
        };
        return basePrice * days * multiplier;
    }
}