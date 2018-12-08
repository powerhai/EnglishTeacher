using Prism.Modularity;
using Prism.Regions;
using System;

namespace Fool.UserManagement
{
    public class UserManagementModule : IModule
    {
        IRegionManager _regionManager;

        public UserManagementModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }
    }
}