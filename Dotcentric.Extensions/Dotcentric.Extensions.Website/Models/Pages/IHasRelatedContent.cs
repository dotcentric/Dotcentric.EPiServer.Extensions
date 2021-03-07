using EPiServer.Core;

namespace Dotcentric.Extensions.Website.Models.Pages
{
    public interface IHasRelatedContent
    {
        ContentArea RelatedContentArea { get; }
    }
}
