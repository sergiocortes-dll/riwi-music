namespace riwi_music.Models;

public class Concert
{
    private static int _idCounter = 1;

    public int Id { get; }
    public DateTime Date { get; set; }
    public string Location { get; set; }
    public int Capacity { get; set; }
    public List<string> Artists { get; set; } = new();
    public Dictionary<string, decimal> Localities { get; set; } = new();

    public Concert(DateTime date, string location, int capacity)
    {
        Id = _idCounter++;
        Date = date;
        Location = location;
        Capacity = capacity;

        // Precios y localidades por defecto
        Localities["VIP"] = 200m;
        Localities["Platea"] = 150m;
        Localities["General"] = 100m;
    }
}