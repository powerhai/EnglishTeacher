using Prism.Modularity;
using Prism.Regions;
using System;
using Fool.Domain;
using Prism.Events;
namespace Fool.Main
{
    public class MainModule : IModule
    {
        readonly IRegionManager mRegionManager;
        private readonly IEventAggregator mEventAggregator;
        public MainModule(IRegionManager regionManager,IEventAggregator eventAggregator)
        {
            mRegionManager = regionManager;
            mEventAggregator = eventAggregator;
        }

        public void Initialize()
        {
            
            mEventAggregator.GetEvent<LoginEvent>().Subscribe(a =>
            {
                mRegionManager.RegisterViewWithRegion(RegionNames.MAIN, typeof(Views.WorkPanel));
                
                mRegionManager.RequestNavigate(RegionNames.MAIN,  typeof(Views.WorkPanel).FullName );

            });
        }
    }
}