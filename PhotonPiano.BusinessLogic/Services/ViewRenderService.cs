using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using PhotonPiano.BusinessLogic.Interfaces;

namespace PhotonPiano.BusinessLogic.Services;

public class ViewRenderService : IViewRenderService
{
    private readonly IRazorViewEngine _razorViewEngine;
    private readonly ITempDataProvider _tempDataProvider;
    private readonly IServiceProvider _serviceProvider;

    public ViewRenderService(
        IRazorViewEngine razorViewEngine,
        ITempDataProvider tempDataProvider,
        IServiceProvider serviceProvider)
    {
        _razorViewEngine = razorViewEngine;
        _tempDataProvider = tempDataProvider;
        _serviceProvider = serviceProvider;
    }

    public async Task<string> RenderToStringAsync(string viewName, object model)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var httpContext = new DefaultHttpContext
            {
                RequestServices = scope.ServiceProvider
            };

            var actionContext = new ActionContext(
                httpContext,
                new RouteData(),
                new ActionDescriptor());

            using (var sw = new StringWriter())
            {
                var viewPath = $"~/Views/{viewName}.cshtml";
                var viewEngineResult = _razorViewEngine.FindView(actionContext, viewName, false);

                if (!viewEngineResult.Success)
                {
                    viewEngineResult = _razorViewEngine.GetView(null, viewPath, false);
                    if (!viewEngineResult.Success)
                    {
                        throw new FileNotFoundException($"Could not find view '{viewName}'");
                    }
                }

                var viewData = new ViewDataDictionary(
                    new EmptyModelMetadataProvider(),
                    new ModelStateDictionary())
                {
                    Model = model
                };

                var tempData = new TempDataDictionary(
                    httpContext,
                    _tempDataProvider);

                var viewContext = new ViewContext(
                    actionContext,
                    viewEngineResult.View,
                    viewData,
                    tempData,
                    sw,
                    new HtmlHelperOptions());

                await viewEngineResult.View.RenderAsync(viewContext);
                return sw.ToString();
            }
        }
    }
}