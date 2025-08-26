using Linnked.Core.Domain.Entities;

namespace Linnked.Core.Application.Interfaces.Services
{
    public interface IPreferenceService
    {
        Task<bool> SavePreference(string scrambledGuid, PreferenceRequest request);
        Task<Preference?> GetPreference(string scrambledGuid);
        Task<bool> UpdatePreference(string scrambledGuid, PreferenceRequest request);
        Task<bool> CanUserSetPreference(string userId);
        Task<bool> SetPreference(string userId, PreferenceRequest request);

    }
}
