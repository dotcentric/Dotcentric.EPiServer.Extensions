using Dotcentric.Extensions.Website.Models.Pages;
using Dotcentric.Extensions.Website.Models.ViewModels;
using EPiServer;
using EPiServer.Core;
using EPiServer.Web.Mvc;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Dotcentric.Extensions.Website.Controllers
{
    public class TestsPageController : PageController<TestsPage>
    {
        private readonly IContentLoader _contentLoader;

        public TestsPageController(IContentLoader contentLoader)
        {
            _contentLoader = contentLoader;
        }

        public ActionResult Index(TestsPage currentPage)
        {
            //retrieving the start page from the current page (current page must not be a sibling of Start page)
            var ancestorOrSelfStartPage = _contentLoader.GetAncestorOrSelf<StartPage>(currentPage);

            //test the 'or self' part of the function - must return the currentPage
            var selfPage = _contentLoader.GetAncestorOrSelf<TestsPage>(currentPage);

            //testing the 'get ancestor' function
            var ancestorSitePage = _contentLoader.GetAncestor<StartPage>(currentPage);

            //find the first descendent with the name = 'Find a reseller'
            var descendentTest = _contentLoader.GetDescendent<StandardPage>(ancestorOrSelfStartPage, x => x.Name == "Find a reseller");

            //find all the ancestors of the 'Find a reseller' page
            var ancestors = _contentLoader.GetAncestors(descendentTest);

            //find all the standard page ancestors of the 'Find a reseller' page
            var ancestorsPredicateType = _contentLoader.GetAncestors<StandardPage>(descendentTest);

            //find all the ancestors with more than 2 children
            Func<IContent, bool> predicateNumChildren = x => _contentLoader.GetChildren<IContentData>(x.ContentLink).Count() > 2;
            var ancestorsPredicateNumChildren = _contentLoader.GetAncestors(descendentTest, predicateNumChildren);

            var testValues = new TestsViewModel
            {
                TestAncestorStartPage = ancestorOrSelfStartPage,
                TestAncestorOrSelf = selfPage,
                TestAncestor = ancestorSitePage,
                TestDescendentWithPredicate = descendentTest,
                TestAncestors = ancestors,
                TestAncestorsPredicateType = ancestorsPredicateType,
                TestAncestorsPredicateNumChildren = ancestorsPredicateNumChildren
            };

            ViewBag.TestsObjects = testValues;

            var model = PageViewModel.Create(currentPage);
            return View(model);
        }
    }
}