namespace riwi_music.Models;

public class Ticket
{
    private static int _idCounter = 1;

    public int Id { get; }
    public decimal Price { get; set; }
    public string Locality { get; set; }
    public DateTime Date { get; set; }

    public Ticket(decimal price, string locality, DateTime date)
    {
        Id = _idCounter++;
        Price = price;
        Locality = locality;
        Date = date;
    }
}