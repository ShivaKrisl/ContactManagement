using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_Contracts
{
    public interface IPersonsDeleterService
    {
        /// <summary>
        /// Delete a Person by Person Id
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        public Task<bool> DeletePerson(Guid personId);
    }
}
