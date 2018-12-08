using Prism.Modularity;
using Prism.Regions;
using System;
using Fool.Domain;
using Fool.TextManagement.Models;
using Fool.TextManagement.Views;
namespace Fool.TextManagement
{
    public class TextManagementModule : IModule
    {
        readonly IRegionManager mRegionManager;

        public TextManagementModule(IRegionManager regionManager)
        {
            mRegionManager = regionManager;
        }

        public void Initialize()
        {
            ObjectRemap.Init();
            mRegionManager.RegisterViewWithRegion(RegionNames.MAIN_BUTTONS, typeof(MainButton));
            mRegionManager.RegisterViewWithRegion(RegionNames.CONTENT, typeof(TextManageView));
            mRegionManager.RegisterViewWithRegion(RegionNames.CONTENT, typeof(TextEditView));

        }
    }
}