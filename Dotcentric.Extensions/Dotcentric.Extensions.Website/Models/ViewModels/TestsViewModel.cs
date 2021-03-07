using EPiServer.Core;
using System.Collections.Generic;

namespace Dotcentric.Extensions.Website.Models.ViewModels
{
    public class TestsViewModel
    {
        public IContent TestAncestorStartPage { get; set; }
        public IContent TestAncestorOrSelf { get; set; }
        public IContent TestAncestor { get; set; }
        public IContent TestStartDescendentProductPage { get; set; }
        public IContent TestDescendentMultiPredicate { get; set; }
        public IContent TestDescendentWithPredicate { get; set; }
        public IContent TestNextSiblingByName { get; set; }
        public IContent TestPreviousSiblingBySortOrder { get; set; }
        public IEnumerable<IContent> TestAncestors { get; set; }
        public IEnumerable<IContent> TestAncestorsPredicateType { get; set; }
        public IEnumerable<IContent> TestAncestorsPredicateNumChildren { get; set; }
        public IEnumerable<IContent> TestDescendentsPredicateType { get; set; }
        public IEnumerable<IContent> TestSiblingsCurrentPage { get; set; }
        public IEnumerable<IContent> TestTypedSiblingsCurrentPage { get; set; }
    }
}