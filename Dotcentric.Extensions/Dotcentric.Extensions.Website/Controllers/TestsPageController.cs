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
            var startPage = _contentLoader.GetAncestorOrSelf<StartPage>(currentPage);

            //test the 'or self' part of the function - must return the currentPage
            var selfPage = _contentLoader.GetAncestorOrSelf<TestsPage>(currentPage);

            //testing the 'get ancestor' function
            var ancestorSitePage = _contentLoader.GetAncestor<StartPage>(currentPage);

            //find the first descendent with the name = 'Find a reseller'
            var descendentTest = _contentLoader.GetDescendent<StandardPage>(startPage, x => x.Name == "Find a reseller");

            //find all the ancestors of the 'Find a reseller' page
            var ancestors = _contentLoader.GetAncestors(descendentTest);

            //find all the standard page ancestors of the 'Find a reseller' page
            var ancestorsPredicateType = _contentLoader.GetAncestors<StandardPage>(descendentTest);

            //find all the ancestors with more than 2 children
            Func<IContent, bool> predicateNumChildren = x => _contentLoader.GetChildren<IContentData>(x.ContentLink).Count() > 2;
            var ancestorsPredicateNumChildren = _contentLoader.GetAncestors(descendentTest, predicateNumChildren);

            //find the first product descendant of start page
            var descendantCurrentPage = _contentLoader.GetDescendent<ProductPage>(startPage);

            //find the first descendant of start page that is a standard page and starts with 'White'
            var descendentMultiPredicate = _contentLoader.GetDescendent<StandardPage>(startPage, x => x.Name.StartsWith("White"));

            //find all the product descendents of start page 
            var descendentsProductPages = _contentLoader.GetDescendents<ProductPage>(startPage);

            //find all the siblings of the current page
            var testSiblingsCurrentPage = _contentLoader.Siblings(currentPage);

            //find all the product siblings of the current page
            var testTypedSiblingsCurrentPage = _contentLoader.Siblings<ProductPage>(currentPage);

            //find the next sibling by name of the page named 'alloy plan'
            var alloyPlanPage = _contentLoader.FirstChild<IContent>(ContentReference.StartPage);
            var nextSibling = _contentLoader.FollowingSibling<IContent, string>(alloyPlanPage, x => x.Name);

            //find the previous sibling by 'sort order' property
            Func<IContent, int> sortingPredicate = x => (int)x.Property["PagePeerOrder"].Value;
            var previousSibling = _contentLoader.PreviousSibling<ProductPage, int>(currentPage, sortingPredicate);

            var testValues = new TestsViewModel
            {
                TestAncestorStartPage = startPage,
                TestAncestorOrSelf = selfPage,
                TestAncestor = ancestorSitePage,
                TestDescendentWithPredicate = descendentTest,
                TestAncestors = ancestors,
                TestAncestorsPredicateType = ancestorsPredicateType,
                TestAncestorsPredicateNumChildren = ancestorsPredicateNumChildren,
                TestStartDescendentProductPage = descendantCurrentPage,
                TestDescendentMultiPredicate = descendentMultiPredicate,
                TestDescendentsPredicateType = descendentsProductPages,
                TestSiblingsCurrentPage = testSiblingsCurrentPage,
                TestTypedSiblingsCurrentPage = testTypedSiblingsCurrentPage,
                TestNextSiblingByName = nextSibling,
                TestPreviousSiblingBySortOrder = previousSibling,
            };

            ViewBag.TestsObjects = testValues;

            var model = PageViewModel.Create(currentPage);
            return View(model);
        }
    }
}