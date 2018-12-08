using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Fool.Contracts;
using Prism.Events;
namespace Fool.Login.ViewModels
{
    public class LoginViewModel : BindableBase
    {
        private readonly ILoginService mLoginService;
        public LoginViewModel(ILoginService loginService)
        {
            mLoginService = loginService; 
        }
        public ICommand LoginCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    mLoginService.Login();
                });
            }
        }
    }
}
