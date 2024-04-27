using BaseProjectServices.Helpers;
using Entities;
using ServiceContract;
using ServiceContract.DTO;
using ServiceContract.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepositoryContracts;
namespace BaseProjectServices
{
    public class PersonsService : IPersonsService
    {
        //private field
        private readonly IPersonsRepository  _db;
        private readonly ICountriesService _countriesService;

        //constructor
        public PersonsService(IPersonsRepository personsRepository, ICountriesService countriesService)
        {
            _db = personsRepository;
            _countriesService = countriesService;
        }


        private PersonResponse ConvertPersonToPersonResponse(Person person)
        {
            PersonResponse personResponse = person.ToPersonResponse();
            //personResponse.Country = _countriesService.GetCountryByCountryID(person.CountryID)?.CountryName;
            //Khong can phai dung method nua ma ta co the goi thang luon khoa ngoai de lien ket voi country table tuong ung voi  no luon
            personResponse.Country = person.Country?.CountryName;
            return personResponse;
        }

        public async Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest)
        {
            //check if PersonAddRequest is not null
            if (personAddRequest == null)
            {
                throw new ArgumentNullException(nameof(personAddRequest));
            }

            //Model validation
            ValidationHelper.ModelValidation(personAddRequest);

            //convert personAddRequest into Person type
            Person person = personAddRequest.ToPerson();

            //generate PersonID
            person.PersonID = Guid.NewGuid();

            //add person object to persons list
            
            //_db.Persons.Add(person);
            //await _db.SaveChangesAsync();
            await _db.AddPerson(person); //From repository

            // add person with procedure
            //_db.sp_InsertPerson(person);
            //convert the Person object into PersonResponse type
            return person.ToPersonResponse();
        }


        public async Task<List<PersonResponse>> GetAllPersons()
        {
            //Test relation, thong thuong thi Icollection hay Model Property lien ket se khong hien khi debug ma ta phai include no vao thi no moi hien 

            //var persons = _db.Persons.ToList(); // country no se null, khong hien thong tin ve table chinh ma trong khi do no la khoa ngoai cua country

            //Muon hien thi ta include vao
            //var persons = await _db.Persons.Include(nameof(Person.Country)).ToListAsync(); // property country tai Person duoc set up khoa ngoai la Country ID, co nghia la no se lay Country dua tren countryId khoa ngoai lien ket voi Country => tu match dung y chang voi thong tin khoa ngoai khoa chinh luon
            // thanng nay de test, khong co ap dung gi o trong pj ca
            //
            //[
            //return _db.Persons.ToList() // tuong tu //Select *from Person
            //          .Select(temp => ConvertPersonToPersonResponse(temp)).ToList(); // thằng này không async là do nó không action trực tiếp với database
            return (await _db.GetAllPersons()).Select(country => country.ToPersonResponse()).ToList();                 
            //return _db.Persons.Select(temp => ConvertPersonToPersonResponse(temp)).ToList(); error, cannot use your own method to linq dbcontext
                                                                                     //]

            //use procedure
            //return _db.sp_GetAllPersons().Select(temp => ConvertPersonToPersonResponse(temp)).ToList();
        }


        public async Task<PersonResponse?> GetPersonByPersonID(Guid? personID)
        {
            if (personID == null)
                return null;

            //Person? person = await _db.Persons.Include("Country")
            //        .FirstOrDefaultAsync(temp => temp.PersonID == personID);
            // Repository
            Person? person = await _db.GetPersonByPersonID(personID.Value);
            // lam vay de lam gi? o ben duoi ne
            string? testString = person.Country?.CountryName; // Co the truy cap vao country tuong ung cua  person o debug ne ne. // thanng nay de test, khong co ap dung gi o trong pj ca
                                                              //Noi nom na incluce de cho de test cac gia tri o trong debug thoi

            if (person == null)
                return null;

            return ConvertPersonToPersonResponse(person);
        }

        public async Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString)
        {
            List<PersonResponse> allPersons = await GetAllPersons();
            List<PersonResponse> matchingPersons = allPersons;
            

            if (string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchString))
                return matchingPersons;


            switch (searchBy)
            {
                case nameof(PersonResponse.PersonName):
                    matchingPersons = allPersons.Where(temp =>
                    (!string.IsNullOrEmpty(temp.PersonName) ?
                    temp.PersonName.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;

                case nameof(PersonResponse.Email):
                    matchingPersons = allPersons.Where(temp =>
                    (!string.IsNullOrEmpty(temp.Email) ?
                    temp.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;


                case nameof(PersonResponse.DateOfBirth):
                    matchingPersons = allPersons.Where(temp =>
                    (temp.DateOfBirth != null) ?
                    temp.DateOfBirth.Value.ToString("dd MMMM yyyy").Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                    break;

                case nameof(PersonResponse.Gender):
                    matchingPersons = allPersons.Where(temp =>
                    (!string.IsNullOrEmpty(temp.Gender) ?
                    temp.Gender.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;

                case nameof(PersonResponse.CountryID):
                    matchingPersons = allPersons.Where(temp =>
                    (!string.IsNullOrEmpty(temp.Country) ?
                    temp.Country.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;

                case nameof(PersonResponse.Address):
                    matchingPersons = allPersons.Where(temp =>
                    (!string.IsNullOrEmpty(temp.Address) ?
                    temp.Address.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
                    break;

                default: matchingPersons = allPersons; break;
            }
            return matchingPersons;
        }


        public async Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder)
        {
            if (string.IsNullOrEmpty(sortBy))
                return allPersons;

            List<PersonResponse> sortedPersons = (sortBy, sortOrder) switch
            {
                (nameof(PersonResponse.PersonName), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.PersonName), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Email), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Email), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.DateOfBirth).ToList(),

                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.DateOfBirth).ToList(),

                (nameof(PersonResponse.Age), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Age).ToList(),

                (nameof(PersonResponse.Age), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Age).ToList(),

                (nameof(PersonResponse.Gender), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Gender, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Gender), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Gender, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Country), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Country, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Country), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Country, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Address), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Address, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Address), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Address, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.ReceiveNewsLetters).ToList(),

                (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.ReceiveNewsLetters).ToList(),

                _ => allPersons
            };

            return sortedPersons;
        }


        public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
        {
            if (personUpdateRequest == null)
                throw new ArgumentNullException(nameof(Person));

            //validation
            ValidationHelper.ModelValidation(personUpdateRequest);

            //get matching person object to update
            /*Person? matchingPerson = await _db.Persons.FirstOrDefaultAsync(temp => temp.PersonID == personUpdateRequest.PersonID);*/ // matchingPerson luc nay duoc dbset tro toi roi. Cho nen neu thay doi gi xong ma dung SaveChanges() thi tai do no se tu thay doi

            //Repository
            Person? matchingPerson = await _db.GetPersonByPersonID(personUpdateRequest.PersonID);
            if (matchingPerson == null)
            {
                throw new ArgumentException("Given person id doesn't exist");
            }

            //update all details
            matchingPerson.PersonName = personUpdateRequest.PersonName;
            matchingPerson.Email = personUpdateRequest.Email;
            matchingPerson.DateOfBirth = personUpdateRequest.DateOfBirth;
            matchingPerson.Gender = personUpdateRequest.Gender.ToString();
            matchingPerson.CountryID = personUpdateRequest.CountryID;
            matchingPerson.Address = personUpdateRequest.Address;
            matchingPerson.ReceiveNewsLetters = personUpdateRequest.ReceiveNewsLetters;

            //await _db.SaveChangesAsync(); //update 
            //Repository thay thế cho cái update trên
            await _db.UpdatePerson(matchingPerson);
            return ConvertPersonToPersonResponse(matchingPerson);
        }

        public async Task<bool> DeletePerson(Guid? personID)
        {
            if (personID == null)
            {
                throw new ArgumentNullException(nameof(personID));
            }

            //Person? person = await _db.Persons.FirstOrDefaultAsync(temp => temp.PersonID == personID);
            Person? person = await _db.GetPersonByPersonID(personID.Value); 
            if (person == null)
                return false;

            //_db.Persons.Remove(_db.Persons.First(temp => temp.PersonID == personID));
            //await _db.SaveChangesAsync();
            await _db.DeletePersonByPersonID(personID.Value);
            return true;
        }
    }
}
