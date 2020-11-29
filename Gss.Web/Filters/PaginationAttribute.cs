using System;
using System.Threading.Tasks;
using Gss.Core.Helpers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Gss.Web.Filters
{
  class PaginationAttribute: Attribute, IAsyncActionFilter
  {
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
      int pageSize = Settings.MinimumItemsPerPage;
      int pageNumber = 1;

      if (context.ActionArguments.ContainsKey("pageNumber")
        && Int32.TryParse(context.ActionArguments["pageNumber"].ToString(), out pageNumber))
      {
        pageNumber = pageNumber < 1 ? 1 : pageNumber;
      }

      if (context.ActionArguments.ContainsKey("pageSize")
        && Int32.TryParse(context.ActionArguments["pageSize"].ToString(), out pageSize))
      {
        pageSize = pageSize > Settings.MaximumItemsPerPage || pageSize < Settings.MinimumItemsPerPage
          ? Settings.MinimumItemsPerPage
          : pageSize;
      }

      context.ActionArguments["pageNumber"] = pageNumber;
      context.ActionArguments["pageSize"] = pageSize;

      context.ActionArguments["filterBy"] = context.ActionArguments.ContainsKey("filterBy")
        ? context.ActionArguments["filterBy"].ToString()
        : null;
      context.ActionArguments["filter"] = context.ActionArguments.ContainsKey("filter")
        ? context.ActionArguments["filter"].ToString()
        : String.Empty;

      context.ActionArguments["orderAsc"] = context.ActionArguments.ContainsKey("orderAsc")
        ? context.ActionArguments["orderAsc"].ToString() == "True"
        : false;

      context.ActionArguments["orderBy"] = context.ActionArguments.ContainsKey("orderBy")
        ? context.ActionArguments["orderBy"].ToString()
        : String.Empty;

      await next();
    }
  }
}
