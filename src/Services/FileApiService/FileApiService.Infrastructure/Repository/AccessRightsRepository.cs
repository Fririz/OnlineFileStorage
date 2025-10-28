using FileApiService.Application.Contracts;
using System.Data.Common;
using FileApiService.Domain.Common;
using FileApiService.Domain.Entities;
using FileApiService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using FileApiService.Application.Contracts;

namespace FileApiService.Infrastructure.Repository;

public class AccessRightsRepository : RepositoryBase<AccessRights>, IAccessRightsRepository
{
    public AccessRightsRepository(Context context) : base(context)
    {
        
    }
}