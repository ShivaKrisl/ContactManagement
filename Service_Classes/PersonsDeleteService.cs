using Repository_Contracts;
using Service_Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_Classes
{
    public class PersonsDeleteService : IPersonsDeleterService
    {

        private readonly IPersonsRepository _personsRepository;

        public PersonsDeleteService(IPersonsRepository personsRepository)
        {
            _personsRepository = personsRepository;
        }

        /// <summary>
        /// Delete Person by Id
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<bool> DeletePerson(Guid personId)
        {
            if (personId == Guid.Empty)
            {
                throw new ArgumentException(nameof(personId), "Invalid Person Id");
            }

            return await _personsRepository.DeletePerson(personId);
        }
    }
}
