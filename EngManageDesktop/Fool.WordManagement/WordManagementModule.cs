using Prism.Modularity;
using Prism.Regions;
using System;

namespace Fool.WordManagement
{
    public class WordManagementModule : IModule
    {
        IRegionManager _regionManager;

        public WordManagementModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }
    }
}