using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Entities.Models;

namespace Repository.RepositoryFactoryBase
{
    public interface IDatabaseFactory : IDisposable
    {
        EcommerceContext Get();
    }
}
