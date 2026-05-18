namespace PetHaven.Domain;

public class Pet
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Name { get; }
    public PetType Type { get; }
    public int Age { get; }

    public Pet(string name, PetType type, int age)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Ім'я тварини не може бути порожнім.");
        if (age < 0 || age > 30)
            throw new ArgumentOutOfRangeException(nameof(age), "Некоректний вік тварини.");

        Name = name;
        Type = type;
        Age = age;
    }
}