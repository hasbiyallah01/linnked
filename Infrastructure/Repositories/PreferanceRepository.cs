using Microsoft.EntityFrameworkCore;
using Linnked.Core.Application.Interfaces.Repositories;


namespace Linnked.Infrastructure.Repositories
{
    public class PreferenceRepository : IPreferenceRepository
    {
        private readonly AppDbContext _context;

        public PreferenceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Preference?> GetByScrambledGuidAsync(string scrambledGuid)
        {
            return await _context.Preferences.FirstOrDefaultAsync(p => p.ScrambledGuid == scrambledGuid);
        }

        public async Task AddAsync(Preference preference)
        {
            await _context.Preferences.AddAsync(preference);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Preference preference)
        {
            _context.Preferences.Update(preference);
            await _context.SaveChangesAsync();
        }
        public async Task<Preference?> GetByUserIdAsync(string userId)
        {
            return await _context.Preferences.FirstOrDefaultAsync(p => p.ScrambledGuid == userId); // Assuming userId is stored in ScrambledGuid
        }
    }
}