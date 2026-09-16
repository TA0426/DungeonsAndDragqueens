namespace OppgaveUkeEnModul3.Core;

using System.ComponentModel.DataAnnotations;


public record CreateSwordDTO(
   [Required] string Name,
   [Range(0, int.MaxValue)] int Damage,
   [Required] string Description);