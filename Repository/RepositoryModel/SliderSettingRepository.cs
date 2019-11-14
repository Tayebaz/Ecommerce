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
    public class SliderSettingRepository : RepositoryFactory<SliderSetting>
    {
        public SliderSettingRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory) { }
    }
}
