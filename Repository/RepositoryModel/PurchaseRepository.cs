using Entities.Models;
using Repository.RepositoryFactoryBase;
using Repository.RepositoryFactoryCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Repository.RepositoryModel
{
    public class PurchaseRepository : RepositoryFactory<Purchase>
    {
        public PurchaseRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory) { }
    }
}
