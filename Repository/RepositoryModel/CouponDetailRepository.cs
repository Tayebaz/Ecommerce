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
    public class CouponDetailRepository : RepositoryFactory<CouponDetail>
    {
        public CouponDetailRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory) { }
    }
}
