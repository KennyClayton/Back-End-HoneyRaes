using System.Text.Json.Serialization;
using HoneyRaesAPI.Models;
using Microsoft.AspNetCore.Http.Json;

var builder = WebApplication.CreateBuilder(args);
//
// Set the JSON serializer options
builder.Services.Configure<JsonOptions>(options => //
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

List<Customer> customers = new List<Customer> {
    new Customer
    {
        Id = 1,
        Name = "Luke",
        Address = "100 Main St."
    },
    new Customer
    {
        Id = 2,
        Name = "Carroll",
        Address = "1300 West Broad St."
    },
    new Customer
    {
        Id = 3,
        Name = "Bob",
        Address = "97 Ravencrest Dr."
    }
};

List<Employee> employees = new List<Employee> {
    new Employee
    {
        Id = 1,
        Name = "Gertrude",
        Specialty = "Plumbing"
    },
    new Employee
    {
        Id = 2,
        Name = "Tanner",
        Specialty = "Electrical"
    }
};

List<ServiceTicket> serviceTickets = new List<ServiceTicket> {
    new ServiceTicket {
        Id = 1,
        CustomerId = 1,
        EmployeeId = 1,
        Description = "My kitchen sink drain line is busted.",
        Emergency = true,
        DateCompleted = new DateTime (2023, 8, 20)
    },
    new ServiceTicket {
        Id = 2,
        CustomerId = 1,
        EmployeeId = 1,
        Description = "The mower got the hose; the hose pulled the spigot off the wall.",
        Emergency = false,
        DateCompleted = new DateTime (2023, 8, 24)
    },
    new ServiceTicket {
        Id = 3,
        CustomerId = 2,
        EmployeeId = 2,
        Description = "The meter mast came down during the storm.",
        Emergency = true,
        DateCompleted = new DateTime ()
    },
    new ServiceTicket {
        Id = 4,
        CustomerId = 2,
        EmployeeId = null,
        Description = "Lights were flickering after the storm",
        Emergency = true,
        DateCompleted = new DateTime ()
    },
    new ServiceTicket {
        Id = 5,
        CustomerId = 3,
        EmployeeId = null,
        Description = "Toilet won't fill back up after flush.",
        Emergency = false,
        DateCompleted = new DateTime ()
    },
};


//* Below is called the "handler" as it handles the task of telling the API what data it needs and where to find it
//* The first argument is the /weatherforecast endpoint
//* The second argument being passed to app.MapGet is the anonymous function for creating a forecast
//KEEPING below code for learning / reference later
// app.MapGet("/weatherforecast", () =>
// {
//     var forecast = Enumerable.Range(1, 5).Select(index =>
//         new WeatherForecast
//         (
//             DateTime.Now.AddDays(index),
//             Random.Shared.Next(-20, 55),
//             summaries[Random.Shared.Next(summaries.Length)]
//         ))
//         .ToArray();
//     return forecast;
// })
// .WithName("GetWeatherForecast");


//^ ENDPOINT - add endpoint to GET service tickets
// Here is a second endpoint. Go to the url ending in hello and return the word hello. 
app.MapGet("/servicetickets", () =>
{
    return serviceTickets;
});


//^ ENDPOINT - add endpoint to GET service tickets by id
//* ROUTE PARAMETER
//From curriculum: "This endpoint introduces some complexity. In the route the {id} part of the string is called a route parameter."
app.MapGet("/servicetickets/{id}", (int id) =>
{
    ServiceTicket serviceTicket = serviceTickets.FirstOrDefault(st => st.Id == id);
    if (serviceTicket == null)
    {
        return Results.NotFound();
    }
    //the below service ticket for employee 3 (for example) is now the first one that matches 
    serviceTicket.Employee = employees.FirstOrDefault(e => e.Id == serviceTicket.EmployeeId);
    serviceTicket.Customer = customers.FirstOrDefault(c => c.Id == serviceTicket.CustomerId);
    return Results.Ok(serviceTicket);

    // below was the first version of our return for this before incorporating HTTP status codes on our own 
    // return serviceTickets.FirstOrDefault(st => st.Id == id); //FirstOrDefault method is used to find the first service ticket with the Id we tell it to look for (in Postman)
});


//^ ENDPOINT - add endpoint to GET employees
app.MapGet("/employees", () =>
{
    return employees;
});


//^ ENDPOINT - add endpoint to GET employees by id
//* ROUTE PARAMETER
//From curriculum: "This endpoint introduces some complexity. In the route the {id} part of the string is called a route parameter."
app.MapGet("/employees/{id}", (int id) =>
{
    Employee employee = employees.FirstOrDefault(e => e.Id == id);
    if (employee == null)
    {
        return Results.NotFound();
    }
    //^ METHOD / FIND the tickets assigned to this employee with WHERE method (recall this is a Linq method that finds/returns more than just one match)
    employee.ServiceTickets = serviceTickets.Where(st => st.EmployeeId == id).ToList(); // this code gets us the related service tickets for the current employee and adds those service tickets to the employee's service tickets array/list of service tickets
    return Results.Ok(employee);
});



//^ ENDPOINT - add endpoint to GET customers
app.MapGet("/customers", () =>
{
    return customers;
});


//^ ENDPOINT - add endpoint to get customers by id
//* ROUTE PARAMETER
//From curriculum: "This endpoint introduces some complexity. In the route the {id} part of the string is called a route parameter."
app.MapGet("/customers/{id}", (int id) => // this says, look up this url/path/location and interpolate the id - as a route parameter - and pass id as a parameter so we can use it in {}
{
    //^ METHOD this customers list with FOD (FirstOrDefault) and store the matching customer object in "customer" variable
    Customer customer = customers.FirstOrDefault(e => e.Id == id); // look through customers list for the first customer id matching the one being passed into this function
    if (customer == null) //...and if the customer (which is whatever integer the user input) is not found in our database/list of customers' Id...
    {
        return Results.NotFound(); // then do this
    }
    
    //^ SEARCH Use .Where method and then .ToList method to assemble a list of serviceTickets 
    //How? by looking into each one and WHEREver the parameter "id" matches a service ticket's CustomerId...send that to the list capture the customer object with a matching id to the one above and grab the service tickets associated with that customer
    customer.ServiceTicket = serviceTickets.Where(st => st.CustomerId == id).ToList();

    //? But to what list is the .ToList method sending the matching service ticket properties? I asked ChatGPT which explained it well: 
    //* ".ToList() is used to convert the filtered sequence of ServiceTicket objects into a list. Finally, the resulting list of service tickets is assigned to the customer.ServiceTicket property."

    return Results.Ok(customer); // otherwise, return the OK response and send on the customer Id that we stored in "customer" variable
}
);

//& POSTS BELOW
//^ ENDPOINT - add endpoint to POST new service tickets to the database
//* Recall that this below function or method is called a "handler" that takes two parameters.The purpose of a handler is to handle the task of telling the API what data it needs and where to find it.
//* The first parameter is the endpoint (where to go), the second parameter is the anonymous function
app.MapPost("/servicetickets", (ServiceTicket serviceTicket) =>
{
    // creates a new id (When we get to it later, our SQL database will do this for us like JSON Server did!)
    //^ CREATE a new id for the new service ticket
    serviceTicket.Id = serviceTickets.Count > 0 ?serviceTickets.Max(st => st.Id) + 1 : 1; //if the number of service tickets is more than zero then look at the current highest id number and add one...otherwise, just use the number 1...
    //^ ADD it to the object that was sent from the client
    serviceTickets.Add(serviceTicket);
    //^ ADD the new object to the database
    return serviceTicket;
});

//& DELETE BELOW
//^ ENDPOINT - add an endpoint for DELETING a ticket
//* Route parameter is the id part of the below endpoint/partial url
app.MapDelete("/servicetickets/{id}", (int id) => 
{
    //Grab a specific ticket by id
    ServiceTicket deleteMe = serviceTickets.FirstOrDefault(st => st.Id == id);
    if (deleteMe == null)
    {
        return Results.NotFound(); // Return a not found response if the ticket doesn't exist
    }
    //delete method on that ticket
    serviceTickets.Remove(deleteMe);
    //call the list of tickets again to have the list updated?
    return Results.Ok();

});


//& PUT (UPDATE) BELOW
//^ ENDPOINT - add an endpoint for UPDATING a ticket...include a route parameter
app.MapPut("/servicetickets/{id}", (int id, ServiceTicket serviceTicket) => 
{
    // new variable to hold the matching service ticket of the id number we ask for
    ServiceTicket ticketToUpdate = serviceTickets.FirstOrDefault(st => st.Id == id);
    
    // new variable to hold the index number of that service ticket stored in ticketToUpdate
    int ticketIndex = serviceTickets.IndexOf(ticketToUpdate); 
    //* this IndexOf method finds the position of a specific element within a COLLECTION (array) of service tickets. That's why we have to reference the serviceTickets collection ... so that we can feed the IndexOf method off of that serviceTickets collection. 
    
    // if the integer we provided as the id (in the request route) is null....return a Not Found result
    if (ticketToUpdate == null)
    {
        return Results.NotFound();
    }
    // if the integer we provided as the id does NOT match the id in the request body...bad request
    //? how could that happen? idk
    if (id != serviceTicket.Id)
    {
        return Results.BadRequest();
    }
    // so we grabbed the ticket by id...then grabbed the id itself off that ticket...now we need to update the service ticket somehow. We insert the integer (held in ticketIndex) to tell serviceTickets which index to go to....then make it equal an individual serviceTicket. Essentially, whatever new data we give it in Postman (employee id for example) is brought in with the serviceTicket as the new serviceTicket object....so it overwrites the old one.
    serviceTickets[ticketIndex] = serviceTicket;
    return Results.Ok();

}
);

app.Run();



//*LESSONS:
//1. These PUT, DELETE, POST, etc are called "handlers".
//2. Each handler takes two parameters:
    // a. an endpoint (and route parameter sometimes)
    // b. an anonymous function to tell the MapPut, MapDelete, etc what to do
// So the purpose of a handler is to handle the task of telling the API what data it needs and where to find it. 