using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fool.Contracts;
namespace Fool.Services
{
    public class ConfigService : IConfigService
    {
        public string GetServerPath()
        {
            return "http://localhost:5000";
        }
    }
}
