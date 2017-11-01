using S_Umb_Turbine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Core.Models;
using Umbraco.Web.Mvc;

namespace S_Umb_Turbine.Controller
{
    public class SiteLayoutController: SurfaceController
    {
        private const string PARTIAL_VIEW_PATH = "~/Views/Partials/SiteLayout/";

        public ActionResult RenderHeader() {
            List<NavigationListItem> nav = GetNavigationModelFromDatabase();
            return PartialView(PARTIAL_VIEW_PATH + "_Header.cshtml", nav);
        }

        public ActionResult RenderFooter()
        {
            return PartialView(string.Format("{0}{1}", PARTIAL_VIEW_PATH, "_Footer.cshtml"));
        }

        private List<NavigationListItem> GetNavigationModelFromDatabase()
        {
            const int HOME_PAGE_POSITION_IN_PATH = 1;
            int homePageId = int.Parse(CurrentPage.Path.Split(',')[HOME_PAGE_POSITION_IN_PATH]);
            //var homePage = Umbraco.Content(homePageId);
            IPublishedContent homePage = CurrentPage;
            List<NavigationListItem> nav = new List<NavigationListItem>();
            if (homePage.DocumentTypeAlias.ToLower() == "page") {
                nav.Add(new NavigationListItem(new NavigationLink(homePage.Url, homePage.Name)));
            }
            
            nav.AddRange(GetChildNavigationList(homePage));
            return nav;
        }

        private List<NavigationListItem> GetChildNavigationList(IPublishedContent page)
        {
            List<NavigationListItem> listItems = new List<NavigationListItem>();
            var childPages = page.Children.Where(x => x.DocumentTypeAlias.ToLower() == "page");
            if (childPages != null && childPages.Any() && childPages.Count() > 0)
            {
                foreach (var childPage in childPages)
                {
                    NavigationListItem listItem = new NavigationListItem(new NavigationLink(childPage.Url, childPage.Name));
                    listItem.Items = GetChildNavigationList(childPage);
                    listItems.Add(listItem);
                }
            }
            return listItems;
        }
    }
}