using EPiServer.Core;

namespace Dotcentric.EPiServer.Extensions.Website.Models.Pages
{
    public interface IHasRelatedContent
    {
        ContentArea RelatedContentArea { get; }
    }
}
