namespace riwi_music.Models;

public class Customer : Person
{
    public List<Ticket> Purchases { get; set; } = new();

    public Customer(string name, string lastname, string email, string password)
        : base(name, lastname, email, password)
    {
    }

    public void BuyTicket(Ticket ticket)
    {
        Purchases.Add(ticket);
    }
}