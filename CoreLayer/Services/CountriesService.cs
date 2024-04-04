using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceContract;
using ServiceContract.DTO;
using ServiceContract.Enums;
using Entities;

using RepositoryContracts;
namespace BaseProjectServices
{
    public class CountriesService :ICountriesService
    {
        //private field
        private readonly ICountriesRepository _db;

        //constructor
        public CountriesService(ICountriesRepository DbCountryRepo)
        {
            _db = DbCountryRepo;
        }

        public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
        {

            //Validation: countryAddRequest parameter can't be null
            if (countryAddRequest == null)
            {
                throw new ArgumentNullException(nameof(countryAddRequest));
            }

            //Validation: CountryName can't be null
            if (countryAddRequest.CountryName == null)
            {
                throw new ArgumentException(nameof(countryAddRequest.CountryName));
            }

            //Validation: CountryName can't be duplicate
            //if (await _db.Countries.CountAsync(temp => temp.CountryName == countryAddRequest.CountryName) > 0) // 
            if (await _db.GetCountryByCountryName(countryAddRequest.CountryName) != null)
                {
                throw new ArgumentException("Given country name already exists");
            }

            //Convert object from CountryAddRequest to Country type
            Country country = countryAddRequest.ToCountry();

            //generate CountryID
            country.CountryID = Guid.NewGuid();

            //Add country object into _countries

            //await _db.Countries.AddAsync(country); // 
            //await _db.SaveChangesAsync(); // 
            await _db.AddCountry(country); //  Da co save change o trong ham addcountry tu repository
           
            

            return country.ToCountryResponse();
        }

        public async Task<List<CountryResponse>> GetAllCountries()
        {

            //return await _db.Countries.Select(country => country.ToCountryResponse()).ToListAsync();
            return (await _db.GetAllCountries()).Select(country => country.ToCountryResponse()).ToList();
        }

        public async Task<CountryResponse?> GetCountryByCountryID(Guid? countryID)
        {
            if (countryID == null)
                return null;

            //Country? country_response_from_list = await _db.Countries.FirstOrDefaultAsync(temp => temp.CountryID == countryID);
            Country? country_response_from_list = await _db.GetCountryByCountryID(countryID.Value); //TH cho no' null thi khi muon truyen tham so vao thi phai su dung value 
            if (country_response_from_list == null)
                return null;

            return country_response_from_list.ToCountryResponse();
        }
    }
}
