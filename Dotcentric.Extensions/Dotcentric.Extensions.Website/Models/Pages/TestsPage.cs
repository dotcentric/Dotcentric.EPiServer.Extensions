using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;

namespace Dotcentric.Extensions.Website.Models.Pages
{
    [ContentType(DisplayName = "Tests Page",
        GUID = "125d790f-9342-44cd-be7a-f6aa10d7592d",
        Description = "Page used to test extensions methods")]
    public class TestsPage : SitePageData
    {
        public TestsPage()
        {
        }
    }
}