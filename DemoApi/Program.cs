using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "HR System API",
        Description = "A simple example ASP.NET Web API",
    });
}

);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI();
}
app.UseSwagger();

app.UseHttpsRedirection();

app.MapGet("/api/employees", () =>
{
    return GetEmployees();
})
.WithName("GetAllEmployees")
.Produces<Employee[]>()
.WithOpenApi();

app.MapGet("/api/employees/{id}", (string id) =>
{
    var employees = GetEmployees();
    var employee = employees.FirstOrDefault(e => e.Id == id);

    if (employee == null)
    {
        return Results.NotFound();
    }
    else
    {
        return Results.Ok(employee);
    }
})
.WithName("GetEmployeeById")
.Produces<Employee>()
.Produces(StatusCodes.Status404NotFound)
.WithOpenApi();

app.MapPost("/api/employees", (Employee employee) =>
{
    var employees = GetEmployees().ToList();

    if (employees.Any(e => e.FirstName == employee.FirstName && e.LastName == employee.LastName))
    {
        return Results.Conflict("Employee already exists");
    }

    employee.Id = Guid.NewGuid().ToString();

    employees.Add(employee);

    SaveEmployees([.. employees]);

    return Results.Created($"/api/employees/{employee.Id}", employee);
})
.WithName("AddEmployee")
.Accepts<Employee>("application/json")
.Produces<Employee>()
.Produces(StatusCodes.Status201Created)
.WithOpenApi();

app.MapPut("/api/employees/{id}", (string id, Employee employee) =>
{
    var employees = GetEmployees().ToList();
    var existingEmployee = employees.FirstOrDefault(e => e.Id == id);

    if (existingEmployee == null)
    {
        return Results.NotFound();
    }

    existingEmployee.FirstName = employee.FirstName;
    existingEmployee.LastName = employee.LastName;
    existingEmployee.Dept = employee.Dept;
    existingEmployee.Salary = employee.Salary;

    SaveEmployees([.. employees]);

    return Results.Ok(existingEmployee);
})
.WithName("UpdateEmployee")
.Accepts<Employee>("application/json")
.Produces<Employee>()
.Produces(StatusCodes.Status404NotFound)
.WithOpenApi();

app.MapDelete("/api/employees/{id}", (string id) =>
{
    var employees = GetEmployees();
    var employee = employees.FirstOrDefault(e => e.Id == id);

    if (employee == null)
    {
        return Results.NotFound();
    }
    else
    {
        var newEmployees = employees.Where(e => e.Id != id).ToArray();
        
        SaveEmployees(newEmployees);

        return Results.NoContent();
    }
})
.WithName("DeleteEmployee")
.WithOpenApi();

app.Run();

Employee[] GetEmployees()
{
    var text = File.ReadAllText("employees.json");
    return JsonSerializer.Deserialize<Employee[]>(text) ?? [];
}

void SaveEmployees(Employee[] employees)
{
    var text = JsonSerializer.Serialize(employees);
    File.WriteAllText("employees.json", text);
}

class Employee
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = "";
    [JsonPropertyName("firstName")]
    public string FirstName { get; set; } = "";
    [JsonPropertyName("lastName")]
    public string LastName { get; set; } = "";
    [JsonPropertyName("dept")]
    public string Dept { get; set; } = "";
    [JsonPropertyName("salary")]
    public decimal Salary { get; set; }
}