using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.Azure.Mobile.Server;
using myevenService.DataObjects;
using myevenService.Models;
using System.Collections.Generic;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Azure.Mobile.Server.Config;

namespace myevenService.Controllers
{
    [Authorize]

    public class TodoItemController : TableController<TodoItem>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            myevenContext context = new myevenContext();
            DomainManager = new EntityDomainManager<TodoItem>(context, Request);
        }

        // GET tables/TodoItem
        public IQueryable<TodoItem> GetAllTodoItems()
        {
            return Query();
        }

        // GET tables/TodoItem/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<TodoItem> GetTodoItem(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/TodoItem/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<TodoItem> PatchTodoItem(string id, Delta<TodoItem> patch)
        {
            return UpdateAsync(id, patch);
        }

        // POST tables/TodoItem
        public async Task<IHttpActionResult> PostTodoItem(TodoItem item)
        {
            TodoItem current = await InsertAsync(item);
            HttpConfiguration config =this.Configuration;
            MobileAppSettingsDictionary settings = this.Configuration.GetMobileAppSettingsProvider().GetMobileAppSettings();
            string notifiname = settings.NotificationHubName;
            string notificon = settings.Connections[MobileAppSettingsKeys.NotificationHubConnectionString].ConnectionString;
            NotificationHubClient hub = NotificationHubClient.CreateClientFromConnectionString(notificon, notifiname);
            Dictionary<string, string> temp = new Dictionary<string, string>();
            temp["messageParag"] = item.Text + "was added in list";
            try
            { 
                var result = await hub.SendTemplateNotificationAsync(temp); 
                config.Services.GetTraceWriter().Info(result.State.ToString()); }
            catch (System.Exception ex) { 
                config.Services.GetTraceWriter().Error(ex.Message, null, "Push.SendAsync Error"); } 

                return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/TodoItem/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteTodoItem(string id)
        {
            return DeleteAsync(id);
        }
    }
}