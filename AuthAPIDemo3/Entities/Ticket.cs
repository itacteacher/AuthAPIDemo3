namespace AuthAPIDemo3.Entities;

public class Ticket
{
    public Guid Id { get; set; }
    public string EventTitle { get; set; }
    public DateTime EventDate { get; set; }
    public double Price { get; set; }
}
