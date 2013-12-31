using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace doof.Helpers;

public class CustomCultureRouteModelConvention : IPageRouteModelConvention
{
    public void Apply(PageRouteModel model)
    {
        foreach (var selector in model.Selectors)
        {
            var template = selector.AttributeRouteModel.Template;
            selector.AttributeRouteModel.Template = "{culture=en-us}/" + template;
        }
    }
}