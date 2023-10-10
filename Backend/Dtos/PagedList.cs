using System.Collections;

namespace Dtos;

public class PagedList<T> : IEnumerable<T>
{
	public ICollection<T> Items { get; set; }
	public ResultPaginationDto Pagination { get; set; }
	
	public PagedList(ICollection<T> items, int totalCount, int pageNumber, int pageSize)
	{
		Items = items;
		Pagination = new()
		{
			TotalCount = totalCount,
			PageNumber = pageNumber,
			PageSize = pageSize,
			Count = items.Count,
			TotalPages = totalCount > 0 ? (totalCount - 1) / pageSize : 0
		};
	}
	
	public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => Items.GetEnumerator();
}

public class PaginationDto 
{
	public int PageSize { get; set; } = 10;
	public int PageNumber { get; set; }
}

public class ResultPaginationDto: PaginationDto
{
	public int TotalPages { get; set; }
	public int TotalCount { get; set; }
	public int Count { get; set; }
}