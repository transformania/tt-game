using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc.Routing;

namespace TT.Web.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class InheritedRouteAttribute : Attribute, IDirectRouteFactory
    {
        public string Name { get; set; }
        public int Order { get; set; }
        public string Template { get; private set; }

        public InheritedRouteAttribute(string template)
        {
            Template = template;
        }

        public RouteEntry CreateRoute(DirectRouteFactoryContext context)
        {
            // context.Actions will always contain at least one action - and all of the 
            // actions will always belong to the same controller.
            var controllerDescriptor = context.Actions.First().ControllerDescriptor;
            var template = Template.Replace("{controller}",
                controllerDescriptor.ControllerName);
            IDirectRouteBuilder builder = context.CreateBuilder(template);
            builder.Name = controllerDescriptor.ControllerName + " : Base";
            builder.Order = Order;
            return builder.Build();
        }
    }
}