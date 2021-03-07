using EPiServer.Core;
using System.Collections.Generic;

namespace Dotcentric.Extensions.Website.Models.ViewModels
{
    public class TestsViewModel
    {
        public IContent CurrentPage { get; set; }
        public IContent TestAncestorStartPage { get; set; }
        public IContent TestAncestorOrSelf { get; set; }
        public IContent TestAncestor { get; set; }

        public IContent TestDescendentWithPredicate { get; set; }
        public IEnumerable<IContent> TestAncestors { get; set; }
        public IEnumerable<IContent> TestAncestorsPredicateType { get; set; }
        public IEnumerable<IContent> TestAncestorsPredicateNumChildren { get; set; }
    }
}