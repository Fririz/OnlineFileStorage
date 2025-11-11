namespace Contracts.Shared;

public class FileDeletionRequested
{
    public required Guid FileId { get; init; }
}