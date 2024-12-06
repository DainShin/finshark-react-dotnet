using api.Data;
using api.Dtos.Stock;
using api.Helpers;
using api.Interfaces;
using api.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/stock")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IStockRepository _stockRepo;
        public StockController(ApplicationDBContext context, IStockRepository stockRepo)
        {
            _stockRepo = stockRepo;
            _context = context;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] QueryObject query)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // _context.Stocks : Returns objects
            // ToList(): In order to be able to create the sql to go out the database to get whatever you need
            var stocks = await _stockRepo.GetAllAsync(query);

            var stockDto = stocks.Select(s => s.ToStockDto()).ToList();   // 각 Stock 객체를 StockDto로 변환

            return Ok(stockDto); // 변환된 StockDto 리스트를 클라이언트에 반환
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var stock = await _stockRepo.GetByIdAsync(id);

            if (stock == null) return NotFound();
            
            return Ok(stock.ToStockDto());
        }

        // url을 통해서가 아니라 http body에 있는 데이터를 전달하기 때문에 FromBody를 써줘야함
        // CreatedAtAction(string actionName, object routeValues, object return value); Http 201 상태코드 & 리소스의 위치 정보 반환
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateStockRequestDto stockDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var stockModel = stockDto.ToStockFromCreateDTO(); // stockDto -> stock
            await _stockRepo.CreateAsync(stockModel); // save the stock in the database
            return CreatedAtAction(nameof(GetById), new { id = stockModel.Id }, stockModel.Id);
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateStockRequestDto updateDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var stockModel = await _stockRepo.UpdateAsync(id, updateDto);

            if (stockModel == null) return NotFound();
            
            return Ok(stockModel.ToStockDto());
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var stockModel = await _stockRepo.DeleteAsync(id);

            if (stockModel == null) return NotFound();

            return NoContent();
        }
    }
}