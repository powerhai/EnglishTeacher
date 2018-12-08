using Prism.Modularity;
using Prism.Regions;
using System;
using System.ComponentModel;
using Fool.Contracts;
using Microsoft.Practices.Unity;
namespace Fool.Services
{
    public class ServicesModule : IModule
    {
        private readonly IUnityContainer mContainer;
        IRegionManager mRegionManager;

        public ServicesModule(IUnityContainer container,  IRegionManager regionManager)
        {
            mContainer = container;
            mRegionManager = regionManager;
        }

        public void Initialize()
        {
            mContainer.RegisterInstance(typeof(IConfigService), mContainer.Resolve<ConfigService>());
            mContainer.RegisterInstance(typeof(ILoginService), mContainer.Resolve<LoginService>());
            mContainer.RegisterInstance(typeof(IAudioService), mContainer.Resolve<AudioService>());
            mContainer.RegisterInstance(typeof(IPublisherService), mContainer.Resolve<PublisherService>());
            mContainer.RegisterInstance(typeof(ISentenceTranslateService), mContainer.Resolve<BaiduSentenceTranslateService>());
            mContainer.RegisterInstance(typeof(IText2AudioService), mContainer.Resolve<BaiduText2AudioService>());
            mContainer.RegisterInstance(typeof(BaiduTokenService), mContainer.Resolve<BaiduTokenService>());
            mContainer.RegisterInstance(typeof(ITextService), mContainer.Resolve<TextService>());
            mContainer.RegisterInstance(typeof(ISentenceService), mContainer.Resolve<SentenceService>());
        }
    }
}