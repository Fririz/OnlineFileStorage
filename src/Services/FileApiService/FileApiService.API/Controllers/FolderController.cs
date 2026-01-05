using FileApiService.Application.Contracts;
using FileApiService.Application.Dto;
using FileApiService.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace FileApiService.API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class FolderController : ControllerBase
{
    private readonly IFolderWorker _folderWorker;
    public FolderController(IFolderWorker fileWorker)
    {
        _folderWorker = fileWorker;
    }
    [HttpPost]
    [Route("createfolder")]
    public async Task<ActionResult> CreateFolder(ItemCreateDto itemCreate)
    {
        Guid ownerId = Guid.Parse(Request.Headers["Id"].ToString());
        await _folderWorker.CreateFolder(itemCreate, ownerId);
        return Ok();
    }
    [HttpGet]
    [Route("getallchildren/{id}/")]
    public async Task<ActionResult<List<Item>>> GetAllChildren(Guid id)
    {
        var children = await _folderWorker.GetChildrenAsync(id);
        return Ok(children);
    }

    [HttpDelete]
    [Route("deletefolder/{id}/")]
    public async Task<ActionResult> DeleteFolderWithChildren(Guid id)
    {
        try
        {
            await _folderWorker.DeleteFolderWithAllChildren(id, Guid.Parse(Request.Headers["Id"].ToString()));
            return Ok();
        }

        catch (InvalidOperationException)
        {
            return BadRequest("Incorrect type of item");
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (DirectoryNotFoundException)
        {
            return NotFound();
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
    
}