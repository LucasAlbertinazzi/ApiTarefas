using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseInMemoryDatabase("TarefasDb"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

#region EndPoints

//Retorna todas as tarefas
app.MapGet("/tarefas", async (AppDbContext db) => await db.Tarefas.ToListAsync());


//Retorna as tarefas de acordo com o id
app.MapGet("/tarefas/{id}", async (int id, AppDbContext db) =>
    await db.Tarefas.FindAsync(id) is Tarefa tarefa ? Results.Ok(tarefa) : Results.NotFound());


//Retorna as tarefas que estão concluidas
app.MapGet("/tarefas/concluida", async (AppDbContext db) =>
                                 await db.Tarefas.Where(t => t.Concluida).ToListAsync());


//Adiciona as tarefas
app.MapPost("/tarefas", async (Tarefa tarefa, AppDbContext db) =>
{
    db.Tarefas.Add(tarefa);
    await db.SaveChangesAsync();
    return Results.Created($"/tarefas/{tarefa.Id}", tarefa);
});


//Edita as tarefas de acorco com o id
app.MapPut("/tarefas/{id}", async (int id, Tarefa inputTarefa, AppDbContext db) =>
{
    var tarefa = await db.Tarefas.FindAsync(id);

    if (tarefa is null) return Results.NotFound();

    tarefa.Nome = inputTarefa.Nome;
    tarefa.Concluida = inputTarefa.Concluida;

    await db.SaveChangesAsync();
    return Results.NoContent();
});


//Deleta as tarefas pelo id
app.MapDelete("/tarefas/{id}", async (int id, AppDbContext db) =>
{
    if (await db.Tarefas.FindAsync(id) is Tarefa tarefa)
    {
        db.Tarefas.Remove(tarefa);
        await db.SaveChangesAsync();
        return Results.Ok(tarefa);
    }
    return Results.NotFound();
});

#endregion


app.Run();

#region Class
class Tarefa
{
    public int Id { get; set; }
    public string? Nome { get; set; }
    public bool Concluida { get; set; }
}

class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    { }
    public DbSet<Tarefa> Tarefas => Set<Tarefa>();
}
#endregion



