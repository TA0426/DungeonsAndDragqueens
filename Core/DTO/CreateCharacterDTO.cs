namespace OppgaveUkeEnModul3.Core;

using System.ComponentModel.DataAnnotations;

public record CreateCharacterDTO(
   [Required] string Name);