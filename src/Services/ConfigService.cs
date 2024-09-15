using AlphabetUpdateServer.Entities;
using Microsoft.EntityFrameworkCore;

namespace AlphabetUpdateServer.Services;

public class ConfigService
{
    public const string MaintenanceModeId = "MaintenanceMode";
    
    private readonly ApplicationDbContext _context;

    public ConfigService(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<bool> GetMaintenanceMode()
    {
        return GetBool(MaintenanceModeId);
    }

    public Task SetMaintenanceMode(bool value)
    {
        return SetBool(MaintenanceModeId, value);
    }
    
    public async Task<string> GetString(string id)
    {
        return await _context.Configs
            .Where(config => config.Id == id)
            .Select(config => config.Value)
            .FirstOrDefaultAsync()
            ?? string.Empty;
    }

    public async Task<bool> GetBool(string id)
    {
        try
        {
            var value = await GetString(id);
            return bool.Parse(value);
        }
        catch (FormatException)
        {
            return false;
        }
    }

    public async Task SetString(string id, string value)
    {
        await _context.Configs
            .Upsert(new ConfigEntity
            {
                Id = id,
                Value = value
            })
            .RunAsync();
    }

    public async Task SetBool(string id, bool value)
    {
        await SetString(id, value.ToString());
    }
}