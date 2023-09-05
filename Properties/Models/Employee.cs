namespace HoneyRaesAPI.Models;
//* IMPORTANT GOLD NUGGET - "Notice that we did not have to use a using directive to have the ServiceTicket class in the Employee.cs file. This is because both classes are in the HoneyRaesAPI.Models namespace, so there is no need to "import" one into other!"

public class Employee {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Specialty { get; set; }
    public List<ServiceTicket> ServiceTickets { get; set; } // so effectively we are adding a list as a property of the Employee class. A list of service tickets is a property of each employee instance.
}