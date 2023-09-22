using Npgsql; //This is a library that works with C# language and SQL databases. It allows me to use keywords to query the PostegrSQL database...words like SELECT and FROM...
//"Npgsql is a .NET Data Provider for PostgreSQL, which allows you to connect to and interact with PostgreSQL databases from C# code." "In summary, Npgsql is used in your C# code to connect to the PostgreSQL database, execute SQL queries, and retrieve data. Npgsql provides the necessary classes and methods to interact with the database seamlessly from your C# application."
using System.Text.Json.Serialization;
using HoneyRaesAPI.Models;
using Microsoft.AspNetCore.Http.Json;
var connectionString = "Host=localhost;Port=5432;Username=postgres;Password=dukebd11-11;Database=HoneyRaes";

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


//^ NEW ENDPOINT - get all employees
app.MapGet("/employees", () =>
{
    // create an empty list of employees to add to. 
    List<Employee> employees = new List<Employee>();
    //make a connection to the PostgreSQL database using the connection string
    using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
    //open the connection
    connection.Open();
    // create a sql command to send to the database
    using NpgsqlCommand command = connection.CreateCommand();
    command.CommandText = "SELECT * FROM Employee";
    //send the command. 
    using NpgsqlDataReader reader = command.ExecuteReader();
    //read the results of the command row by row
    while (reader.Read()) // reader.Read() returns a boolean, to say whether there is a row or not, it also advances down to that row if it's there. 
    {
        //This code adds a new C# employee object with the data in the current row of the data reader 
        employees.Add(new Employee
        {
            Id = reader.GetInt32(reader.GetOrdinal("Id")), //find what position the Id column is in, then get the integer stored at that position
            Name = reader.GetString(reader.GetOrdinal("Name")),
            Specialty = reader.GetString(reader.GetOrdinal("Specialty"))
        });
    }
    //once all the rows have been read, send the list of employees back to the client as JSON
    return employees;
});


// //^ OLD ENDPOINT - add endpoint to GET employees
// app.MapGet("/employees", () =>
// {
//     return employees;
// });



//^ NEW ENDPOINT - get employee by Id
//* Explanation for ths new endpoint: "This code block largely follows the same patterns as the previous endpoint. We need to create a connection, open it, and create a command."
app.MapGet("/employees/{id}", (int id) =>
{
    Employee employee = null;
    using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
    connection.Open();
    using NpgsqlCommand command = connection.CreateCommand();
    command.CommandText = @"
        SELECT 
            e.Id,
            e.Name, 
            e.Specialty, 
            st.Id AS serviceTicketId, 
            st.CustomerId,
            st.Description,
            st.Emergency,
            st.DateCompleted 
        FROM Employee e
        LEFT JOIN ServiceTicket st ON st.EmployeeId = e.Id
        WHERE e.Id = @id";
    // use command parameters to add the specific Id we are looking for to the query
    command.Parameters.AddWithValue("@id", id);
    using NpgsqlDataReader reader = command.ExecuteReader();
    // We are only expecting one row back, so we don't need a loop!
    while (reader.Read())
    {
        if (employee == null)
        {
            employee = new Employee
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                Specialty = reader.GetString(reader.GetOrdinal("Specialty")),
                ServiceTickets = new List<ServiceTicket>() //empty List to add service tickets to
            };
        }
        // reader.IsDBNull checks if a column in a particular position is null
        if (!reader.IsDBNull(reader.GetOrdinal("serviceTicketId")))
        {
            employee.ServiceTickets.Add(new ServiceTicket
            {
                Id = reader.GetInt32(reader.GetOrdinal("serviceTicketId")),
                CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                //we don't need to get this from the database, we already know it
                EmployeeId = id,
                Description = reader.GetString(reader.GetOrdinal("Description")),
                Emergency = reader.GetBoolean(reader.GetOrdinal("Emergency")),
                // Npgsql can't automatically convert NULL in the database to C# null, so we have to check whether it's null before trying to get it
                DateCompleted = reader.IsDBNull(reader.GetOrdinal("DateCompleted")) ? null : reader.GetDateTime(reader.GetOrdinal("DateCompleted"))
            });
        }
    }
     // Return 404 if the employee is never set (meaning, that reader.Read() immediately returned false because the id did not match an employee)
    // otherwise 200 with the employee data
    return employee == null ? Results.NotFound() : Results.Ok(employee);
});


// //^ OLD ENDPOINT - add endpoint to GET employees by id
// //* ROUTE PARAMETER
// //From curriculum: "This endpoint introduces some complexity. In the route the {id} part of the string is called a route parameter."
// app.MapGet("/employees/{id}", (int id) =>
// {
//     Employee employee = employees.FirstOrDefault(e => e.Id == id);
//     if (employee == null)
//     {
//         return Results.NotFound();
//     }
//     //^ METHOD / FIND the tickets assigned to this employee with WHERE method (recall this is a Linq method that finds/returns more than just one match)
//     employee.ServiceTickets = serviceTickets.Where(st => st.EmployeeId == id).ToList(); // this code gets us the related service tickets for the current employee and adds those service tickets to the employee's service tickets array/list of service tickets
//     return Results.Ok(employee);
// });



//^NEW ENDPOINT - Create a new employee
app.MapPost("/employees", (Employee employee) =>
{
    using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
    connection.Open();
    using NpgsqlCommand command = connection.CreateCommand();
    command.CommandText = @"
        INSERT INTO Employee (Name, Specialty)
        VALUES (@name, @specialty)
        RETURNING Id
    ";
    //? why do we add a return above? "We add RETURNING Id to the end of the query so that we get the new Id for the employee back after it has been created."
    command.Parameters.AddWithValue("@name", employee.Name);
    command.Parameters.AddWithValue("@specialty", employee.Specialty);

    //the database will return the new Id for the employee, add it to the C# object
    employee.Id = (int)command.ExecuteScalar();

    return employee;
});



//^NEW ENDPOINT - Update an employee
app.MapPut("/employees/{id}", (int id, Employee employee) =>
{
    if (id != employee.Id)
    {
        return Results.BadRequest();
    }
    using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
    connection.Open();
    using NpgsqlCommand command = connection.CreateCommand();
    command.CommandText = @"
        UPDATE Employee 
        SET Name = @name,
            Specialty = @specialty
        WHERE Id = @id
    ";
    // above: "We need to provide the id to the database to make sure we only update one employee. Without the WHERE, it would update every row with this data"
    command.Parameters.AddWithValue("@name", employee.Name);
    command.Parameters.AddWithValue("@specialty", employee.Specialty);
    command.Parameters.AddWithValue("@id", id);

    command.ExecuteNonQuery(); // this "is used for data changes when you do not need or expect any data back from the database after the query. In this case, as long as the query runs correctly, we do not need any information from the database."
    return Results.NoContent(); // "For the same reason, we return a 204 response No Content back to the client, because it is also not going to learn anything new from the response."
});




//^NEW ENDPOINT - Delete an employee
app.MapDelete("/employees/{id}", (int id) =>
{
    using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
    connection.Open();
    using NpgsqlCommand command = connection.CreateCommand();
    command.CommandText = @"
        DELETE FROM Employee WHERE Id=@id
    ";
    command.Parameters.AddWithValue("@id", id);
    command.ExecuteNonQuery(); //"We again use ExecuteNonQuery because we don't need any information back from the database so long as the delete operation was successful."
    return Results.NoContent(); // "The handler returns 204 No Content because we want to send a success message that is not going to have a body."
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


//^ ENDPOINT - add a custom endpoint that will complete a ticket AND log the date it was completed
app.MapPost("/servicetickets/{id}/complete", (int id) => //we just created a new endpoint by adding /complete after the service ticket id
{
    //get the service ticket from the database that needs to be completed
    ServiceTicket ticketToComplete = serviceTickets.FirstOrDefault( st => st.Id == id); //ServiceTicket is the class/type of object
    ticketToComplete.DateCompleted = DateTime.Today; // this updates the DateCompleted property of the serviceTicket we grabbed
});






app.Run();



//*LESSONS:
//1. These PUT, DELETE, POST, etc are called "handlers".
//2. Each handler takes two parameters:
    // a. an endpoint (and route parameter sometimes)
    // b. an anonymous function to tell the MapPut, MapDelete, etc what to do
// So the purpose of a handler is to handle the task of telling the API what data it needs and where to find it. 