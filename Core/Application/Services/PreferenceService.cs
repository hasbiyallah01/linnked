using Linnked.Core.Application.Interfaces.Repositories;
using Linnked.Core.Application.Interfaces.Services;
namespace Linnked.Core.Application.Services
{
    public class PreferenceService : IPreferenceService
    {
        private readonly IPreferenceRepository _preferenceRepository;

        public PreferenceService(IPreferenceRepository preferenceRepository)
        {
            _preferenceRepository = preferenceRepository;
        }

        public async Task<bool> SavePreference(string scrambledGuid, PreferenceRequest request)
        {
            var existingPreference = await _preferenceRepository.GetByScrambledGuidAsync(scrambledGuid);

            if (existingPreference != null)
            {
                existingPreference.Color = request.Color;
                existingPreference.Background = request.Background;
                existingPreference.Font = request.Font;
                existingPreference.FormatType = request.FormatType;
                await _preferenceRepository.UpdateAsync(existingPreference);
            }
            else
            {
                var newPreference = new Preference
                {
                    ScrambledGuid = scrambledGuid,
                    FormatType = request.FormatType,
                    Color = request.Color,
                    Background = request.Background,
                    Font = request.Font,
                    DateCreated = DateTime.UtcNow,
                };
                await _preferenceRepository.AddAsync(newPreference);
            }

            return true;
        }

        public async Task<Preference?> GetPreference(string scrambledGuid)
        {
            return await _preferenceRepository.GetByScrambledGuidAsync(scrambledGuid);
        }

        public async Task<bool> UpdatePreference(string scrambledGuid, PreferenceRequest request)
        {
            var existingPreference = await _preferenceRepository.GetByScrambledGuidAsync(scrambledGuid);
            if (existingPreference == null) return false;

            existingPreference.Color = request.Color;
            existingPreference.Background = request.Background;
            existingPreference.Font = request.Font;
            existingPreference.FormatType = request.FormatType;

            await _preferenceRepository.UpdateAsync(existingPreference);
            return true;
        }

        public async Task<bool> CanUserSetPreference(string userId)
        {
            var preference = await _preferenceRepository.GetByUserIdAsync(userId);
            if (preference == null) return false;

            return true;
        }

        public async Task<bool> SetPreference(string userId, PreferenceRequest request)
        {
            var preference = await _preferenceRepository.GetByUserIdAsync(userId);
            if (preference == null) return false;

            if (preference.TrialCount >= 2)
                return false;

            preference.FormatType = request.FormatType;
            preference.Color = request.Color;
            preference.Background = request.Background;
            preference.Font = request.Font;


            await _preferenceRepository.UpdateAsync(preference);
            return true;
        }
    }
}

