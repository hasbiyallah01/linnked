
using Linnked.Models;
using Linnked.Models.RoleModel;

namespace Linnked.Core.Application.Interfaces.Services
{
    public interface IRoleService
    {
        Task<BaseResponse> CreateRole(RoleRequest request);
        Task<BaseResponse<RoleResponse>> GetRole(int id);
        Task<BaseResponse<ICollection<RoleResponse>>> GetAllRole();
        Task<BaseResponse> RemoveRole(int id);
        Task<BaseResponse> UpdateRole(int id, RoleRequest request);
    }

}
