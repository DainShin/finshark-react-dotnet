using api.Data;
using api.Dtos.Stock;
using api.Helpers;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class StockRepository : IStockRepository
    {
        private readonly ApplicationDBContext _context;

        public StockRepository(ApplicationDBContext context)
        {
            _context = context;
        }


        public async Task<List<Stock>> GetAllAsync(QueryObject query)
        {
            // AsQueryable: 데이터를 쿼리 가능한 형태로 가져옴
            var stocks = _context.Stocks.Include(c => c.Comments).ThenInclude(a => a.AppUser).AsQueryable();

            // 비어있거나 공백으로만 이뤄져있는지 확인
            if(!string.IsNullOrWhiteSpace(query.CompanyName))
            {
                stocks = stocks.Where(s => s.CompanyName.Contains(query.CompanyName));
            }

            if(!string.IsNullOrWhiteSpace(query.Symbol))
            {
                stocks = stocks.Where(s => s.Symbol.Contains(query.Symbol));    
            }

            if(!string.IsNullOrWhiteSpace(query.SortBy))
            {
                // SortBy값이 Symbol 인지 대소문자 구분없이 확인
                if(query.SortBy.Equals("Symbol", StringComparison.OrdinalIgnoreCase))
                {
                    // true-> Symbol 기준으로 내림차순 정렬. false-> Symbol 기준으로 오름차순 정렬
                    stocks = query.IsDescending ? stocks.OrderByDescending(s => s.Symbol) : stocks.OrderBy(s=>s.Symbol);
                }
            }

            // 현재페이지 이전에 표시된 데이터의 총 페이지 수 계산 * 한페이지에 표시할 데이터의 갯수
            var skipNumber = (query.PageNumber - 1) * query.PageSize;

            // 앞의 x개 데이터 건너뜀
            return await stocks.Skip(skipNumber).Take(query.PageSize).ToListAsync();
        }

        public async Task<Stock?> GetByIdAsync(int id)
        {
            // Find(id): Include와 함께 사용 x
            return await _context.Stocks.Include(c => c.Comments).FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<Stock> CreateAsync(Stock stockModel)
        {
            await _context.Stocks.AddAsync(stockModel);
            await _context.SaveChangesAsync();
            return stockModel;
        }

        public async Task<Stock?> UpdateAsync(int id, UpdateStockRequestDto stockDto)
        {
            var existingStock = await _context.Stocks.FirstOrDefaultAsync(x => x.Id == id);

            if (existingStock == null)
            {
                return null;
            }

            existingStock.Symbol = stockDto.Symbol;
            existingStock.CompanyName = stockDto.CompanyName;
            existingStock.Purchase = stockDto.Purchase;
            existingStock.LastDiv = stockDto.LastDiv;
            existingStock.Industry = stockDto.Industry;
            existingStock.MarketCap = stockDto.MarketCap;

            await _context.SaveChangesAsync();

            return existingStock;
        }

        public async Task<Stock?> DeleteAsync(int id)
        {
            var stockModel = _context.Stocks.FirstOrDefault(x => x.Id == id);
            if (stockModel == null)
            {
                return null;
            }
            _context.Stocks.Remove(stockModel);
            await _context.SaveChangesAsync();
            return stockModel;
        }

        public Task<bool> StockExist(int id)
        {
            return _context.Stocks.AnyAsync(s => s.Id == id);
        }

        public async Task<Stock?> GetBySymbolAsync(string symbol)
        {
            return await _context.Stocks.FirstOrDefaultAsync(s => s.Symbol == symbol); 
        }
    }
}