namespace OppgaveUkeEnModul3.Core;

public class StoreSword
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "";
    public int Damage { get; set; }
    public string Description { get; set; } = "";
}