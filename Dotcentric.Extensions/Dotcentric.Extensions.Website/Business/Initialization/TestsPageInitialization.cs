using Dotcentric.Extensions.Website.Models.Pages;
using EPiServer;
using EPiServer.Core;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using System.Linq;

namespace Dotcentric.Extensions.Website.Business.Initialization
{
    /// <summary>
    /// We will create a tests page under start page for our tests
    /// </summary>
    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class TestsPageInitialization : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            var contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();

            var testsPageChildren = contentRepository.GetChildren<TestsPage>(ContentReference.StartPage);

            //there's already a tests page under start page, nothing to worry about
            if (testsPageChildren.Any())
                return;

            //let's create one 
            var testsPage = contentRepository.GetDefault<TestsPage>(ContentReference.StartPage);
            testsPage.Name = "Tests Page";

            contentRepository.Save(testsPage, EPiServer.DataAccess.SaveAction.Publish, EPiServer.Security.AccessLevel.NoAccess);
        }

        public void Uninitialize(InitializationEngine context)
        {
            //Add uninitialization logic
        }
    }
}