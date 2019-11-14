using Entities.Models;
using Repository.RepositoryFactoryCore;
using Repository.RepositoryModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public class WorkingHoursBusiness
    {
        public WorkingHoursRepository _workingHoursRepository;
        private UnitOfWork _unitOfWork;

        public WorkingHoursBusiness(DatabaseFactory df = null, UnitOfWork uow = null)
        {
            DatabaseFactory dfactory = df == null ? new DatabaseFactory() : df;
            _unitOfWork = uow == null ? new UnitOfWork(dfactory) : uow;
            _workingHoursRepository = new WorkingHoursRepository(dfactory);
        }

        public workingHours Insert(workingHours workinghour)
        {
            _workingHoursRepository.Insert(workinghour);
            return workinghour;
        }

        public workingHours Update(workingHours workinghour)
        {
            _workingHoursRepository.Update(workinghour);
            return workinghour;
        }

        public void Delete(long? id)
        {
            _workingHoursRepository.Delete(id);
        }

        public void Delete(workingHours workinghour)    
        {
            _workingHoursRepository.Delete(workinghour);
        }

        public IEnumerable<workingHours> GetAllWorkingHours()
        {
            return _workingHoursRepository.Query().Select();
        }


        public bool AddUpdateDeleteWorkingHours(workingHours workinghour, string action)
        {
            bool isSuccess = true;
            try
            {
                if(action == "I")
                {
                    Insert(workinghour);
                }
                else if (action == "U")
                {
                    Update(workinghour);
                }
                else if (action == "D")
                {
                    Delete(workinghour);
                }
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                isSuccess = false;
                throw ex;
            }
            return isSuccess;
        }
    }
}
