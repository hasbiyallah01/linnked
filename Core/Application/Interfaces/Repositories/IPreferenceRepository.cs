
using Linnked.Core.Domain.Entities;

namespace Linnked.Core.Application.Interfaces.Repositories
{
    public interface IPreferenceRepository
    {
        Task<Preference?> GetByScrambledGuidAsync(string scrambledGuid);
        Task AddAsync(Preference preference);
        Task UpdateAsync(Preference preference);
        Task<Preference?> GetByUserIdAsync(string userId);
    }
}
