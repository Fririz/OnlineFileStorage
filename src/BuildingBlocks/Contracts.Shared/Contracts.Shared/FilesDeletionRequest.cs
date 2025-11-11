using System.ComponentModel.DataAnnotations;

namespace Contracts.Shared;

public class FilesDeletionRequest
{
    public required IEnumerable<Guid> IdsToDelete { get; init; }
}