using Domain.ValueObjects;

public class InvoiceItem
{
    private InvoiceItem() { } // EF

    public string Name { get; private set; }
    public Money Price { get; private set; }
    public int Quantity { get; private set; }

    public InvoiceItem(
    string name,
    Money price,
    int quantity)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.");

        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.");

        Name = name;
        Price = price;
        Quantity = quantity;
    }

    public Money Total => new Money(Price.Amount * Quantity);
}