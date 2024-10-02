using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MusicWebAppBackend.Infrastructure.Commands;


namespace MusicWebAppBackend.Infrastructure.Handlers
{
    public class SendEmailHandler : IRequestHandler<SendEmailCommand, string>
    {
        private readonly ICompositeViewEngine _viewEngine;
        private readonly IServiceProvider _serviceProvider;
        private readonly ITempDataProvider _tempDataProvider;
        public SendEmailHandler(
             IServiceProvider serviceProvider,
            ITempDataProvider tempDataProvider,
        ICompositeViewEngine viewEngine)
        {
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
        }
        public async Task<string> Handle(SendEmailCommand request, CancellationToken cancellationToken)
        {
            string message = "";
            using (var scope = _serviceProvider.CreateScope())
            {
                var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());
                viewData.Model = request.Model;

                var actionContext = new ActionContext(
                    new DefaultHttpContext { RequestServices = scope.ServiceProvider },
                new RouteData(),
                    new ActionDescriptor()
                );

                using (var writers = new StringWriter())
                {
                    try
                    {
                        var viewResult = _viewEngine.FindView(actionContext, request.ViewName, isMainPage: false);
                        var viewContext = new ViewContext(actionContext, viewResult.View, viewData, new TempDataDictionary(actionContext.HttpContext, _tempDataProvider), writers, new HtmlHelperOptions());

                        await viewResult.View.RenderAsync(viewContext);
                        return await Task.FromResult(writers.ToString());
                    }
                    catch (Exception ex)
                    {
                        message = $"{ex.Message}";
                    }
                }
            }
            return await Task.FromResult(message);
        }
    }
}
