using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Entities_Core;

namespace Repository_Contracts
{
    public interface IPersonsRepository
    {
        /// <summary>
        /// Add a Person
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        public Task<Person> AddPerson(Person person);

        /// <summary>
        /// Get all Persons
        /// </summary>
        /// <returns></returns>
        public Task<List<Person>?> GetAllPersons();

        /// <summary>
        /// Get Person By Id
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        public Task<Person?> GetPersonById(Guid personId);

        /// <summary>
        /// Update Person
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="Person"></param>
        /// <returns></returns>
        public Task<Person> UpdatePerson(Guid personId, Person Person);

        /// <summary>
        /// Delete Person
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        public Task<bool> DeletePerson(Guid personId);

        /// <summary>
        /// Get Filtered Persons
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public Task<List<Person>?> GetFilteredPersons(Expression<Func<Person, bool>> predicate);

        /// <summary>
        /// Get Person By Email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public Task<Person?> GetPersonByEmail(string email);

        /// <summary>
        /// Get Person By Mobile Number
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public Task<Person?> GetPersonByMobile(string mobile);


    }
}
