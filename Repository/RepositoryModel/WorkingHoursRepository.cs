using Entities.Models;
using Repository.RepositoryFactoryCore;
using Repository.RepositoryFactoryBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.RepositoryModel
{
    public class WorkingHoursRepository : RepositoryFactory<workingHours>
    {
        public WorkingHoursRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory) { }
    }
}
