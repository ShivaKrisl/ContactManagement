using Entities_Core;
using Repository_Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Repository_Classes
{
    public class PersonsRepository : IPersonsRepository
    {

        private readonly ApplicationDbContext _db;

        public PersonsRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Add a Person to a data base
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        public async Task<Person> AddPerson(Person person)
        {
            _db.Persons.Add(person);
            await _db.SaveChangesAsync();
            return person;
        }

        /// <summary>
        /// Delete a Person from a data base
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        public async Task<bool> DeletePerson(Guid personId)
        {
            _db.Persons.RemoveRange(_db.Persons.Where(p => p.PersonId == personId));
            int deletedCount = await _db.SaveChangesAsync();
            return deletedCount > 0;
        }

        /// <summary>
        /// Get all Persons from data base
        /// </summary>
        /// <returns></returns>
        public async Task<List<Person>?> GetAllPersons()
        {
            return await _db.Persons.Include("Country").ToListAsync();
        }

        /// <summary>
        /// Get Filtered Persons from data base
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<List<Person>?> GetFilteredPersons(Expression<Func<Person, bool>> predicate)
        {
            return await _db.Persons.Include("Country").Where(predicate).ToListAsync();
        }

        /// <summary>
        /// Get Person By Id from data base
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        public async Task<Person?> GetPersonById(Guid personId)
        {
            Person? person = await _db.Persons.Include("Country").FirstOrDefaultAsync(p => p.PersonId == personId);
            if (person == null)
            {
                return null;
            }
            return person;
        }

        public async Task<Person> UpdatePerson(Guid personId, Person person)
        {
            Person? personToUpdate = await _db.Persons.FirstOrDefaultAsync(p => p.PersonId == personId);
            
            if(personToUpdate == null)
            {
                return person;
            }

            personToUpdate.FirstName = person.FirstName;
            personToUpdate.LastName = person.LastName;
            personToUpdate.Email = person.Email;
            personToUpdate.PhoneNumber = person.PhoneNumber;
            personToUpdate.CountryId = person.CountryId;
            personToUpdate.Country = person.Country;
            personToUpdate.Gender =  person.Gender;
            personToUpdate.DateOfBirth = person.DateOfBirth;

            await _db.SaveChangesAsync();
            return personToUpdate;
        }

        /// <summary>
        /// Get Person By Email Id from data base
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<Person?> GetPersonByEmail(string email)
        {
            Person? person = await _db.Persons.Include("Country").FirstOrDefaultAsync(p => p.Email == email);
            if (person == null)
            {
                return null;
            }
            return person;
        }

        /// <summary>
        /// Get Person By Mobile Number from data base
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public async Task<Person?> GetPersonByMobile(string mobile)
        {
            Person? person = await _db.Persons.Include("Country").FirstOrDefaultAsync(p => p.PhoneNumber == mobile);
            if (person == null)
            {
                return null;
            }
            return person;
        }
    }
}
