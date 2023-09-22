using System.Net.Sockets;


namespace HoneyRaesAPI.Models;

public class Customer {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public List<ServiceTicket> ServiceTicket { get; set; } // this was previously coded without the List type. When we were told to list all service tickets for each customer when we GET the customer by id, i had to update this to a List<> of service tickets by putting List<> around ServiceTicket
    //COMPOSITION - second pillar of OOP
}