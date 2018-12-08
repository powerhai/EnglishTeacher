using Prism.Modularity;
using Prism.Regions;
using System;
using Fool.AudioManagement.Views;
using Fool.Domain;
namespace Fool.AudioManagement
{
    public class AudioManagementModule : IModule
    {
        readonly IRegionManager mRegionManager;

        public AudioManagementModule(IRegionManager regionManager)
        {
            mRegionManager = regionManager;
        }

        public void Initialize()
        {
            mRegionManager.RegisterViewWithRegion(RegionNames.MAIN_BUTTONS, typeof(MainButtonsView));
            mRegionManager.RegisterViewWithRegion(RegionNames.CONTENT, typeof(SentenceAudioManageView));
            mRegionManager.RegisterViewWithRegion(RegionNames.CONTENT, typeof(SentenceAudioEditView));
        }
    }
}