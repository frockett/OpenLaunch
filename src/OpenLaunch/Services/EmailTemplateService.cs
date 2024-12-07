using Microsoft.EntityFrameworkCore;
using OpenLaunch.Data;
using OpenLaunch.Models;
using Serilog;

namespace OpenLaunch.Services;

public class EmailTemplateService
{
    private readonly AppDbContext _context;

    public EmailTemplateService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<EmailTemplate?>> GetAllAsync()
    {
        return await _context.EmailTemplates.ToListAsync();
    }

    public async Task<EmailTemplate?> AddAsync(EmailTemplate emailTemplate)
    {
        try
        {
            var result = await _context.EmailTemplates.AddAsync(emailTemplate);
            await _context.SaveChangesAsync();
            return result.Entity;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error while adding email template");
            return null;
        }
    }

    public async Task<EmailTemplate?> UpdateAsync(EmailTemplate emailTemplate)
    {
        try
        {
            var templateToUpdate = await _context.EmailTemplates.FindAsync(emailTemplate.Id);

            if (templateToUpdate == null)
                return null;

            var result = _context.EmailTemplates.Update(emailTemplate);
            await _context.SaveChangesAsync();
            return result.Entity;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error while updating email template");
            return null;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var emailTemplate = await _context.EmailTemplates.FindAsync(id);
            
            if (emailTemplate == null)
                return false;
            
            _context.EmailTemplates.Remove(emailTemplate);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error while deleting email template");
            return false;
        }
    }
}