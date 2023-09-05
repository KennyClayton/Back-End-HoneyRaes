namespace HoneyRaesAPI.Models;

public class ServiceTicket {
    public int Id { get; set; }
    public int? CustomerId { get; set; }
    public int? EmployeeId { get; set; } // We can use the question mark after "int" to tell C# to allow an integer-type variable to equal "null" instead of zero
    public string Description { get; set; }
    public bool Emergency { get; set; }
    public DateTime DateCompleted { get; set; }
    public Employee Employee { get; set; }
    public Customer Customer { get; set; } //this brings in the Customer class with its properties on each customer instance
}