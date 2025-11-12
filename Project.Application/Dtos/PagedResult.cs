using System;
using System.Collections.Generic;
using System.Linq;

namespace Project.Application.Dtos
{
  public class PagedResult<T>
    {
        public List<T> Items { get; set; }
        public int TotalCount { get; set; }
public int PageNumber { get; set; }
 public int PageSize { get; set; }
        
        // Propiedades calculadas
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasNextPage => PageNumber < TotalPages;
  public bool HasPreviousPage => PageNumber > 1;
        public int FirstItemIndex => ((PageNumber - 1) * PageSize) + 1;
 public int LastItemIndex => Math.Min(PageNumber * PageSize, TotalCount);
  public bool IsEmpty => Items == null || !Items.Any();
        
      public PagedResult()
        {
       Items = new List<T>();
  }

        public PagedResult(List<T> items, int totalCount, int pageNumber, int pageSize)
        {
       Items = items ?? new List<T>();
     TotalCount = totalCount;
            PageNumber = pageNumber;
 PageSize = pageSize;
        }

        // Método estático para crear instancias más fácilmente
        public static PagedResult<T> Create(List<T> items, int totalCount, int pageNumber, int pageSize)
        {
            return new PagedResult<T>(items, totalCount, pageNumber, pageSize);
    }

   // Método para crear un PagedResult vacío
      public static PagedResult<T> Empty(int pageNumber, int pageSize)
        {
            return new PagedResult<T>(new List<T>(), 0, pageNumber, pageSize);
        }
    }
}
