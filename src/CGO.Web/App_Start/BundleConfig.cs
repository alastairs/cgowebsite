﻿using System.Web.Optimization;

namespace CGO.Web
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-1.*"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-1.*"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        //"~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate.js"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/knockout").Include(
                        "~/Scripts/knockout-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                        "~/Scripts/bootstrap.min.js"));
            
            bundles.Add(new ScriptBundle("~/bundles/openid").Include(
                        "~/Scripts/openid-jquery.js",
                        "~/Scripts/openid-en.js"));

            bundles.Add(new ScriptBundle("~/bundles/markdownEditor").Include(
                        "~/Scripts/Markdown.Converter.js",
                        "~/Scripts/Markdown.Sanitizer.js",
                        "~/Scripts/Markdown.Editor.js"));

            bundles.Add(new StyleBundle("~/styles/site").Include("~/Content/bootstrap/bootstrap.css",
                                                                 "~/Content/bootstrap/bootstrap-theme.css",
                                                                 "~/Content/common.css",
                                                                 "~/Content/Site.css"));

            bundles.Add(new StyleBundle("~/styles/admin").Include("~/Content/bootstrap/bootstrap.css",
                                                                  "~/Content/bootstrap/bootstrap-theme.css",
                                                                  "~/Content/common.css",
                                                                  "~/Areas/Admin/Content/admin.css"));
            
            bundles.Add(new StyleBundle("~/bundles/jquery-ui").Include("~/Content/jquery-ui-1.8.16.custom.css",
                                                                       "~/Content/jquery.ui.1.8.16.ie.css"));

            bundles.Add(new StyleBundle("~/bundles/font-awesome").Include("~/Content/font-awesome.css",
                                                                          "~/Content/font-awesome-ie7.css"));

            bundles.Add(new StyleBundle("~/bundles/boilerplate").Include("~/Content/Boilerplate.css"));
        
            bundles.Add(new StyleBundle("~/bundles/openid-css").Include("~/Content/openid.css", "~/Content/openid-shadow.css"));

            bundles.Add(new StyleBundle("~/bundles/markdownEditor.css").Include("~/Content/wmd.css"));
            bundles.Add(new StyleBundle("~/bundles/admin.css").Include("~/Areas/Admin/Content/admin.css"));
        }
    }
}