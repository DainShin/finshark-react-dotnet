using api.Dtos.Stock;
using api.Models;

/*
    Mapper
    - 컨트롤러나 서비스에서 직접 변환 로직을 작성하면 동일한 로직이 여러곳에 중복됨
    - 매핑 로직에 추가적인 변환이나 데이터 가공이 필요할 경우, 기존 로직을 확장하거나 새로운 메서드를 추가할 수 있음
*/
namespace api.Mappers
{
    // 인스턴스 생성하지 않고 사용하기 위해 static 키워드 사용
    public static class StockMappers
    {
        // Stock 데이터베이스에서 가져온 필드들을 StockDto 객체에 대응되는 필드에 복사함
        // 불필요한 정보는 제외, 클라이언트가 필요한 정보만 변환하여 반환
        // this Stock stockModel: Stock 클래스의 객체를 확장 대상으로 지정. Stock 클래스의 인스턴스에 대해 ToStockDto 메서드를 호출할 수 있게 만듬
        // 인스턴스 메서드처럼 호출: stock.ToStockDto();
        // 정적 메서드처럼 호출: StockMappers.ToStockDto(stock);
        public static StockDto ToStockDto(this Stock stockModel)
        {
            return new StockDto
            {
                Id = stockModel.Id,
                Symbol = stockModel.Symbol,
                CompanyName = stockModel.CompanyName,
                Purchase = stockModel.Purchase,
                LastDiv = stockModel.LastDiv,
                Industry = stockModel.Industry,
                MarketCap = stockModel.MarketCap,
                Comments = stockModel.Comments.Select(c=>c.ToCommentDto()).ToList(),
            };
        }

        public static Stock ToStockFromCreateDTO(this CreateStockRequestDto stockDto)
        {
            return new Stock
            {
                Symbol = stockDto.Symbol,
                CompanyName = stockDto.CompanyName,
                Purchase = stockDto.Purchase,
                LastDiv = stockDto.LastDiv,
                Industry = stockDto.Industry,
                MarketCap = stockDto.MarketCap,
            };
        }
    }
}