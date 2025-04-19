using Entities_DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_Contracts
{
    public interface IPersonsAdderService
    {
        /// <summary>
        /// Add a new Person
        /// </summary>
        /// <param name="personRequest"></param>
        /// <returns></returns>
        public Task<PersonResponse> AddPerson(PersonRequest personRequest);
    }
}
