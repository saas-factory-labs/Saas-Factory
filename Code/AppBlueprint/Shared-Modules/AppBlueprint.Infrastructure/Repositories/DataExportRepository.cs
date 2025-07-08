using AppBlueprint.Infrastructure.DatabaseContexts;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer.DataExport;
using AppBlueprint.Infrastructure.Repositories.Interfaces;
using AppBlueprint.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace AppBlueprint.Infrastructure.Repositories;

public class DataExportRepository : IDataExportRepository
{
    private readonly ApplicationDbContext _context;

    public DataExportRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<DataExportEntity>> GetAllAsync(CancellationToken cancellationToken)
    {
        List<DataExportEntity>? dataExports = await _context.DataExports.ToListAsync(cancellationToken);
        return dataExports;
    }
    public async Task<DataExportEntity> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        return await _context.DataExports.FindAsync(id, cancellationToken) ?? new DataExportEntity
        {
            Id = PrefixedUlid.Generate("dex"),
            DownloadUrl = new Uri("https://www.example.com"),
            FileName = "Example",
            FileSize = 100000
        };
    }

    public async Task AddAsync(DataExportEntity dataExport, CancellationToken cancellationToken)
    {
        await _context.DataExports.AddAsync(dataExport, cancellationToken);
    }

    public async Task UpdateAsync(DataExportEntity dataExport, CancellationToken cancellationToken)
    {
        _context.DataExports.Update(dataExport);
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken)
    {
        DataExportEntity? dataExport = await _context.DataExports.FindAsync(id, cancellationToken);
        if (dataExport is not null) _context.DataExports.Remove(dataExport);
    }
}
