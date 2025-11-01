using FileStorageService.Application.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace FileStorageService.API.Controllers;
//Its temporary solution, need to be changed to grpc in futury
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
    [HttpGet]
    [Route("GetDownloadLink/{id}/{filename:minlength(1)}/")]
    public ActionResult<string> GetDownloadLink(Guid id, string filename)
    {
        var link = _linkGenerator.GenerateDownloadLink(id, filename);
        return link;
    }
    [HttpGet]
    [Route("GetUploadLink/{id}")]
    public ActionResult<string> GetUploadLink(Guid id)
    {
        var link = _linkGenerator.GenerateUploadLink(id);
        return link;
    }
}