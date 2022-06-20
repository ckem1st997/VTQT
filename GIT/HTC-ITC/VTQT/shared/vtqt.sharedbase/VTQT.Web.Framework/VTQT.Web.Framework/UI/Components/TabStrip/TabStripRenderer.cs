using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using VTQT.Core.Infrastructure;
using VTQT.Web.Framework.Extensions;
using VTQT.Web.Framework.Modelling;

namespace VTQT.Web.Framework.UI
{
    public class TabStripRenderer : ComponentRenderer<TabStrip>
    {
        public TabStripRenderer()
        {
        }

        [SuppressMessage("ReSharper", "Mvc.AreaNotResolved")]
        protected override void WriteHtmlCore(TextWriter writer)
        {
            var tab = base.Component;

            MoveSpecialTabToEnd(tab.Items);

            var hasContent = tab.Items.Any(x => x.Content != null || x.Ajax);
            var isTabbable = tab.Position != TabsPosition.Top;
            var urlHelper = EngineContext.Current.Resolve<IUrlHelper>();

            if (tab.Items.Count == 0)
                return;

            tab.HtmlAttributes.AppendCssClass("tabbable");

            if (isTabbable)
            {
                tab.HtmlAttributes.AppendCssClass("tabs-{0}".FormatInvariant(tab.Position.ToString().ToLower()));
            }

            if (tab.XBaseTabSelection)
            {
                tab.HtmlAttributes.AppendCssClass("tabs-autoselect");
                tab.HtmlAttributes.Add("data-tabselector-href", urlHelper.Action("SetSelectedTab", "AdminMvcCommon"));
            }

            if (tab.OnAjaxBegin.HasValue())
            {
                tab.HtmlAttributes.Add("data-ajax-onbegin", tab.OnAjaxBegin);
            }

            if (tab.OnAjaxSuccess.HasValue())
            {
                tab.HtmlAttributes.Add("data-ajax-onsuccess", tab.OnAjaxSuccess);
            }

            if (tab.OnAjaxFailure.HasValue())
            {
                tab.HtmlAttributes.Add("data-ajax-onfailure", tab.OnAjaxFailure);
            }

            if (tab.OnAjaxComplete.HasValue())
            {
                tab.HtmlAttributes.Add("data-ajax-oncomplete", tab.OnAjaxComplete);
            }

            var tabContainerTag = new TagBuilder("div");
            tabContainerTag.MergeAttributes(tab.HtmlAttributes);

            if (tab.Position == TabsPosition.Below && hasContent)
                RenderTabContent(tabContainerTag, tab);

            // Tabs
            var ulAttrs = new Dictionary<string, object>();
            ulAttrs.AppendCssClass("nav nav-{0}".FormatInvariant(tab.Style.ToString().ToLower()));
            if (tab.Stacked)
            {
                ulAttrs.AppendCssClass("nav-stacked");
            }

            string selector = null;
            var loadedTabs = new List<string>();

            var navTag = new TagBuilder("ul");
            navTag.MergeAttributes(ulAttrs);

            // enable smart tab selection
            if (tab.XBaseTabSelection)
            {
                selector = TrySelectRememberedTab();
            }

            var navIndex = 1;
            foreach (var item in tab.Items)
            {
                var (liTag, loadedTabName) = this.RenderItemLink(navTag, item, navIndex);
                if (loadedTabName.HasValue())
                {
                    loadedTabs.Add(loadedTabName);
                }
                navIndex++;
            }

            tabContainerTag.InnerHtml.AppendHtml(navTag);

            if (tab.Position != TabsPosition.Below && hasContent)
                RenderTabContent(tabContainerTag, tab);

            writer.Write(tabContainerTag.RenderHtmlContent());

            if (loadedTabs.Count > 0)
            {
                foreach (var tabName in loadedTabs)
                {
                    writer.WriteLine("<input type='hidden' class='loaded-tab-name' name='LoadedTabs' value='{0}' />", tabName);
                }
            }

            if (selector != null)
            {
                writer.WriteLine(
                    @"<script>
	                    $(function() {{
		                    _.delay(function() {{
			                    $('{0}').trigger('show');
		                    }}, 100);
	                    }})
                    </script>"
                        .FormatInvariant(selector)
                );
            }
        }

        private void MoveSpecialTabToEnd(List<Tab> tabs)
        {
            var idx = tabs.FindIndex(x => x.Name == "tab-special-plugin-widgets");
            if (idx > -1 && idx < (tabs.Count - 1))
            {
                var tab = tabs[idx];
                tabs.RemoveAt(idx);
                tabs.Add(tab);
            }
        }

        // returns a query selector
        private string TrySelectRememberedTab()
        {
            var tab = this.Component;

            if (tab.Id.IsEmpty())
                return null;

            var model = ViewContext.ViewData.Model as BaseEntityModel;
            if (model != null && string.IsNullOrWhiteSpace(model.Id))
            {
                // it's a "create" operation: don't select
                return null;
            }

            //var rememberedTab = (SelectedTabInfo)ViewContext.TempData["SelectedTab." + tab.Id];
            var rememberedTab = ViewContext.TempData.Get<SelectedTabInfo>($"SelectedTab.{tab.Id}");
            if (rememberedTab != null && rememberedTab.Path.Equals(ViewContext.HttpContext.Request.GetDisplayUrl(), StringComparison.OrdinalIgnoreCase))
            {
                // get tab to select
                var tabToSelect = GetTabById(rememberedTab.TabId);

                if (tabToSelect != null)
                {
                    // unselect former selected tab(s)
                    tab.Items.Each(x => x.Selected = false);

                    // select the new tab
                    tabToSelect.Selected = true;

                    // persist again for the next request
                    //ViewContext.TempData["SelectedTab." + tab.Id] = rememberedTab;
                    ViewContext.TempData.Put($"SelectedTab.{tab.Id}", rememberedTab);

                    if (tabToSelect.Ajax && tabToSelect.Content == null)
                    {
                        return ".nav a[data-ajax-url][href=#{0}]".FormatInvariant(rememberedTab.TabId);
                    }
                }
            }

            return null;
        }

        private Tab GetTabById(string tabId)
        {
            int i = 1;
            foreach (var item in Component.Items)
            {
                var id = BuildItemId(item, i);
                if (id == tabId)
                {
                    if (!item.Visible || !item.Enabled)
                        break;

                    return item;
                }
                i++;
            }

            return null;
        }

        protected virtual TagBuilder RenderTabContent(TagBuilder tabContainerTag, TabStrip tab)
        {
            var tabContentTag = new TagBuilder("div");
            tabContentTag.AddCssClass("tab-content");
            var tabIndex = 1;
            foreach (var item in tab.Items)
            {
                RenderItemContent(tabContentTag, item, tabIndex);

                tabIndex++;
            }

            tabContainerTag.InnerHtml.AppendHtml(tabContentTag);

            return tabContentTag;
        }

        protected virtual (TagBuilder liTag, string loadedTabName) RenderItemLink(TagBuilder navTag, Tab item, int index)
        {
            string temp = "";
            string loadedTabName = null;

            // <li [class="active [hide]"]><a href="#{id}" data-toggle="tab">{text}</a></li>
            if (item.Selected)
            {
                item.HtmlAttributes.AppendCssClass("active");
            }
            else
            {
                if (!item.Visible)
                {
                    item.HtmlAttributes.AppendCssClass("hide");
                }
            }

            if (item.Pull == TabPull.Right)
            {
                item.HtmlAttributes.AppendCssClass("pull-right");
            }

            var liTag = new TagBuilder("li");
            liTag.MergeAttributes(item.HtmlAttributes);
            var aTag = new TagBuilder("a");

            var itemId = "#" + BuildItemId(item, index);
            if (item.Content != null)
            {
                aTag.MergeAttribute("href", itemId);
                aTag.MergeAttribute("data-toggle", "tab");
                aTag.MergeAttribute("data-loaded", "true");
                loadedTabName = GetTabName(item) ?? itemId;
            }
            else
            {
                // no content, create real link instead
                var url = item.GenerateUrl(base.ViewContext.RouteData.Values).NullEmpty();

                if (url == null)
                {
                    aTag.MergeAttribute("href", "#");
                }
                else
                {
                    if (item.Ajax)
                    {
                        aTag.MergeAttribute("href", itemId);
                        aTag.MergeAttribute("data-ajax-url", url);
                        aTag.MergeAttribute("data-toggle", "tab");
                    }
                    else
                    {
                        aTag.MergeAttribute("href", url);
                    }
                }
            }

            if (item.BadgeText.HasValue())
            {
                item.LinkHtmlAttributes.AppendCssClass("clearfix");
            }

            aTag.MergeAttributes(item.LinkHtmlAttributes);
            // Tab Icon
            if (item.Icon.HasValue())
            {
                var iTag = new TagBuilder("i");
                iTag.MergeAttribute("class", item.Icon);

                aTag.InnerHtml.AppendHtml(iTag);
            }
            else if (item.ImageUrl.HasValue())
            {
                var urlHelper = EngineContext.Current.Resolve<IUrlHelper>();
                var iTag = new TagBuilder("img");
                iTag.MergeAttribute("src", urlHelper.Content(item.ImageUrl));
                iTag.MergeAttribute("alt", "Icon");

                aTag.InnerHtml.AppendHtml(iTag);
            }

            // Badge
            if (item.BadgeText.HasValue())
            {
                // caption
                var capTag = new TagBuilder("span");
                capTag.MergeAttribute("class", "tab-caption");
                capTag.InnerHtml.Append(item.Text);

                aTag.InnerHtml.AppendHtml(capTag);

                // label
                temp = "label";
                if (item.BadgeStyle != BadgeStyle.Default)
                {
                    temp += " label-" + item.BadgeStyle.ToString().ToLower();
                }
                if (base.Component.Position == TabsPosition.Left)
                {
                    temp += " pull-right"; // looks nicer 
                }
                var labelTag = new TagBuilder("span");
                labelTag.MergeAttribute("class", temp);
                labelTag.InnerHtml.Append(item.BadgeText);

                aTag.InnerHtml.AppendHtml(labelTag);
            }
            else
            {
                aTag.InnerHtml.Append(item.Text);
            }

            liTag.InnerHtml.AppendHtml(aTag);

            navTag.InnerHtml.AppendHtml(liTag);

            return (liTag, loadedTabName);
        }

        private string GetTabName(Tab tab)
        {
            object value;
            if (tab.LinkHtmlAttributes.TryGetValue("data-tab-name", out value))
            {
                return value.ToString();
            }
            return null;
        }

        protected virtual void RenderItemContent(TagBuilder tabContentTag, Tab item, int index)
        {
            // <div class="tab-pane fade in [active]" id="{id}">{content}</div>
            item.ContentHtmlAttributes.AppendCssClass("tab-pane");
            if (base.Component.Fade)
            {
                item.ContentHtmlAttributes.AppendCssClass("fade");
            }
            if (item.Selected)
            {
                if (base.Component.Fade)
                {
                    item.ContentHtmlAttributes.AppendCssClass("in");
                }
                item.ContentHtmlAttributes.AppendCssClass("active");
            }
            var tabTag = new TagBuilder("div");
            tabTag.MergeAttributes(item.ContentHtmlAttributes);
            tabTag.MergeAttribute("id", BuildItemId(item, index));
            if (item.Content != null)
                tabTag.InnerHtml.AppendHtml(item.Content.RenderHtmlContent());

            tabContentTag.InnerHtml.AppendHtml(tabTag);
        }

        private string BuildItemId(Tab item, int index)
        {
            if (item.Name.HasValue())
            {
                return item.Name;
            }
            return "{0}-{1}".FormatInvariant(this.Component.Id, index);
        }


    }

}
