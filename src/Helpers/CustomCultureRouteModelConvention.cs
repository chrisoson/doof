using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace doof.Helpers;

public class CustomCultureRouteModelConvention : IPageRouteModelConvention
{
    public void Apply(PageRouteModel model)
    {
        List<SelectorModel> selectorModels = new List<SelectorModel>();
        foreach (var selector in model.Selectors.ToList())
        {
            var template = selector.AttributeRouteModel.Template;
            selectorModels.Add(new SelectorModel(){
                AttributeRouteModel = new AttributeRouteModel
                {
                    Template = "/{culture}" + "/" + template
                }
            });
        }
        foreach(var m in selectorModels){
            model.Selectors.Add(m);
        }
    }
}