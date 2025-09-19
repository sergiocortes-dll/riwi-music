
namespace riwi_music.Models;

class Concert
{
    private int _idConcert;
    public DateTime Date;
    public string Location;
    public int Capacity;
    public List<string> artists;

    public Concert(DateTime date, string location, int capacity)
    {
        this.Date = date;
        this.Location = location;
        this.Capacity = capacity;
    }
}