using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace CGO.Web.Helpers
{
    public static class InputTypes
    {
        public static IHtmlString DateFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            return DateFor(htmlHelper, expression, null);
        }

        public static IHtmlString DateFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            return DateFor(htmlHelper, expression, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static IHtmlString DateFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            return InputForHelper(htmlHelper, "date", ExpressionHelper.GetExpressionText(expression), htmlAttributes);
        }

        public static IHtmlString TimeFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            return TimeFor(htmlHelper, expression, null);
        }

        public static IHtmlString TimeFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            return TimeFor(htmlHelper, expression, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static IHtmlString TimeFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            return InputForHelper(htmlHelper, "time", ExpressionHelper.GetExpressionText(expression), htmlAttributes);
        }

        private static IHtmlString InputForHelper(HtmlHelper htmlHelper, string type, string name, IDictionary<string, object> htmlAttributes)
        {
            var htmlFieldName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
            if (string.IsNullOrWhiteSpace(htmlFieldName))
            {
                throw new ArgumentException("Please specify a member name in the lambda expression", "name");
            }

            var tagBuilder = new TagBuilder("input");
            tagBuilder.GenerateId(htmlFieldName);
            tagBuilder.MergeAttributes(htmlAttributes, true);
            tagBuilder.MergeAttribute("type", type, true);
            tagBuilder.MergeAttribute("name", htmlFieldName, true);
            tagBuilder.MergeAttribute("value", GetValueAttributeFromModelState(htmlHelper, htmlFieldName));

            ModelState modelState;
            if (htmlHelper.ViewData.ModelState.TryGetValue(htmlFieldName, out modelState) && modelState.Errors.Count > 0)
            {
                tagBuilder.AddCssClass(HtmlHelper.ValidationInputCssClassName);
            }

            tagBuilder.MergeAttributes(htmlHelper.GetUnobtrusiveValidationAttributes(name));

            return new MvcHtmlString(tagBuilder.ToString(TagRenderMode.SelfClosing));
        }

        private static string GetValueAttributeFromModelState(HtmlHelper htmlHelper, string htmlFieldName)
        {
            ModelState modelState;
            if (htmlHelper.ViewData.ModelState.TryGetValue(htmlFieldName, out modelState))
            {
                return modelState.Value.ConvertTo(typeof(string)) as string;
            }

            return null;
        }
    }
}