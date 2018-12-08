using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fool.Contracts;
using Fool.Domain;
using Prism.Events;
namespace Fool.Services
{
    public class LoginService : ILoginService 
    {
        private readonly IEventAggregator mEventAggregator;
        public LoginService(IEventAggregator eventAggregator)
        {
            mEventAggregator = eventAggregator;
        }
        public void Login()
        { 
            mEventAggregator.GetEvent<LoginEvent>().Publish(true);
        }
        
    }
}
