using Microsoft.EntityFrameworkCore;
using Role_Based_Invitation.Data;
using Role_Based_Invitation.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Role_Based_Invitation.Services
{
    public class PractExamService
    {
        private readonly AppDbContext _context;

        public PractExamService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<PractExam>> GetAllExamsAsync()
        {
            return await _context.PracticalExaminations.ToListAsync();
        }
        public async Task<List<PractExam>> GetExamsByOrganizationAsync(string organizationName)
        {
          
            var adminIds = await _context.Users
                .Where(u => u.Role == Role.Admin && u.OrganizationName == organizationName)
                .Select(u => u.Id)
                .ToListAsync();

            return await _context.PracticalExaminations
                .Where(e => adminIds.Contains(e.CreatedByAdminId))
                .ToListAsync();
        }
        public async Task<List<PractExam>> GetExamsByAdminIdAsync(int adminId)
        {
            return await _context.PracticalExaminations
                .Where(e => e.CreatedByAdminId == adminId)
                .ToListAsync();
        }

    }
}
