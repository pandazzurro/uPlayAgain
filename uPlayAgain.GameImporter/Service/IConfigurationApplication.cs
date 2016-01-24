using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uPlayAgain.Data.Utils;
using uPlayAgain.GameImporter.Model;

namespace uPlayAgain.GameImporter.Service
{
    public interface IConfigurationApplication
    {
        Configuration GetConfig();
    }
}
