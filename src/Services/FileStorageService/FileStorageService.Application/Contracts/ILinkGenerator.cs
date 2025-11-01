namespace FileStorageService.Application.Contracts;

public interface ILinkGenerator
{
    public string GenerateDownloadLink(Guid fileId, string filename);
    public string GenerateUploadLink(Guid fileId);
}