using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uPlayAgain.Data.EF.Models;

namespace uPlayAgain.GameImporter.Service
{
    public interface IConnectionWebApi
    {
        Task<List<Game>> GetAllGame();
    }
}
