using EntityFramework_Slider.Data;
using EntityFramework_Slider.Models;
using EntityFramework_Slider.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework_Slider.Services
{
    public class ExpertService : IExpertService
    {
        private readonly AppDbContext _context;
        public ExpertService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Experts>> GetAll() => await _context.Experts.ToListAsync();

        public async Task<ExpertsHeader> GetHeader() => await _context.ExpertsHeaders.OrderByDescending(m=>m.Id).FirstOrDefaultAsync();
        
    }
}
