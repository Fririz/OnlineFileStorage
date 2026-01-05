using FileStorageService.Application.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace FileStorageService.API.Controllers;
//Its temporary solution, need to be changed to grpc in future
//TODO make grpc
[ApiController]
[Route("api/[controller]")]
public class LinkController: ControllerBase
{
    private readonly ILinkGenerator _linkGenerator;
    public LinkController(ILinkGenerator linkGenerator)
    {
        _linkGenerator = linkGenerator;
    }
    /// <summary>
    /// Generate download link
    /// </summary>
    /// <param name="id"></param>
    /// <param name="filename"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("GetDownloadLink/{id}/{filename:minlength(1)}/")]
    public ActionResult<string> GetDownloadLink(Guid id, string filename)
    {
        var link = _linkGenerator.GenerateDownloadLink(id, filename);
        return link;
    }
    /// <summary>
    /// Generate upload link
    /// </summary>
    /// <param name="fileId">file id</param>
    /// <returns></returns>
    [HttpGet]
    [Route("GetUploadLink/{id}")]
    public ActionResult<string> GetUploadLink(Guid fileId)
    {
        var link = _linkGenerator.GenerateUploadLink(fileId);
        return link;
    }
}