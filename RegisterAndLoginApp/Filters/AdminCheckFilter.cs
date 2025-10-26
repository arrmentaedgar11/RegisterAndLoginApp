using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RegisterAndLoginApp.Models;
using ServiceStack.Text;

namespace RegisterAndLoginApp.Filters
{
    public class AdminCheckFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            
            var userJson = context.HttpContext.Session.GetString("User");
            if (string.IsNullOrEmpty(userJson))
            {
                context.Result = new RedirectResult("/User/Index");
                return;
            }

            try
            {
                var user = JsonSerializer.DeserializeFromString<UserModel>(userJson);

             
                if (user == null || string.IsNullOrWhiteSpace(user.Groups) || !user.Groups.Contains("Admin"))
                {
                    context.Result = new RedirectResult("/User/Index");
                    return;
                }
            }
            catch
            {
               
                context.Result = new RedirectResult("/User/Index");
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
