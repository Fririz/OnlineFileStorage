namespace Contracts.Shared;

public class FilesDeletionComplete
{
    public required IEnumerable<Guid> DeletedIds { get; init; }
}