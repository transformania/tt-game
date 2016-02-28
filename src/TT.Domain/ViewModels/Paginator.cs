using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TT.Domain.ViewModels
{
    /// <summary>
    /// This class assists with pagination.
    /// </summary>
    public class Paginator
    {
        /// <summary>
        /// The count of paginatable items
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// The number of items to list in a page
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// The highest page number to be rendered when paginating
        /// </summary>
        public int MaxPageNumber { get; set; }

        /// <summary>
        /// The current page number that the viewer is looking at
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Default constructor accepting total count of items and the size of a page.
        /// </summary>
        /// <param name="totalCount"></param>
        /// <param name="pageSize"></param>
        public Paginator(int totalCount, int pageSize)
        {
            this.CurrentPage = 0;
            this.TotalCount = totalCount;
            this.PageSize = pageSize;
            this.MaxPageNumber = (int)Math.Ceiling((double)totalCount / (double)PageSize);
        }

        /// <summary>
        /// Returns the number of items to skip when querying
        /// </summary>
        /// <returns></returns>
        public int GetSkipCount()
        {
            return PageSize * (CurrentPage - 1);
        }

        /// <summary>
        /// Returns the name of the class to use for a link on the view based on whether the number matches the current page
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        public string GetStylingClass(int pageNumber)
        {
            if (pageNumber == CurrentPage-1)
            {
                return "";
            } else
            {
                return "timeago";
            }
        }

  
    }
}
