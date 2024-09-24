using EMRProj.Models;
using EMRProj.Data;
using EMRProj.Filter;
using EMRProj.Entities;
using EMRProj.Logger;
using Microsoft.AspNetCore.JsonPatch;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using System.Dynamic;
using Task = System.Threading.Tasks.Task;

namespace EMRProj.Services
{
    /// <summary>
    /// The medicationService responsible for managing medication related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting medication information.
    /// </remarks>
    public interface IMedicationService
    {
        /// <summary>Retrieves a specific medication by its primary key</summary>
        /// <param name="id">The primary key of the medication</param>
        /// <param name="fields">The fields is fetch data of selected fields</param>
        /// <returns>The medication data</returns>
        Task<dynamic> GetById(Guid id, string fields);

        /// <summary>Retrieves a list of medications based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of medications</returns>
        Task<List<Medication>> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new medication</summary>
        /// <param name="model">The medication data to be added</param>
        /// <returns>The result of the operation</returns>
        Task<Guid> Create(Medication model);

        /// <summary>Updates a specific medication by its primary key</summary>
        /// <param name="id">The primary key of the medication</param>
        /// <param name="updatedEntity">The medication data to be updated</param>
        /// <returns>The result of the operation</returns>
        Task<bool> Update(Guid id, Medication updatedEntity);

        /// <summary>Updates a specific medication by its primary key</summary>
        /// <param name="id">The primary key of the medication</param>
        /// <param name="updatedEntity">The medication data to be updated</param>
        /// <returns>The result of the operation</returns>
        Task<bool> Patch(Guid id, JsonPatchDocument<Medication> updatedEntity);

        /// <summary>Deletes a specific medication by its primary key</summary>
        /// <param name="id">The primary key of the medication</param>
        /// <returns>The result of the operation</returns>
        Task<bool> Delete(Guid id);
    }

    /// <summary>
    /// The medicationService responsible for managing medication related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting medication information.
    /// </remarks>
    public class MedicationService : IMedicationService
    {
        private readonly EMRProjContext _dbContext;
        private readonly IFieldMapperService _mapper;

        /// <summary>
        /// Initializes a new instance of the Medication class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        /// <param name="mapper">mapper value to set.</param>
        public MedicationService(EMRProjContext dbContext, IFieldMapperService mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        /// <summary>Retrieves a specific medication by its primary key</summary>
        /// <param name="id">The primary key of the medication</param>
        /// <param name="fields">The fields is fetch data of selected fields</param>
        /// <returns>The medication data</returns>
        public async Task<dynamic> GetById(Guid id, string fields)
        {
            var query = _dbContext.Medication.AsQueryable();
            List<string> allfields = new List<string>();
            if (!string.IsNullOrEmpty(fields))
            {
                allfields.AddRange(fields.Split(","));
                fields = $"Id,{fields}";
            }
            else
            {
                fields = "Id";
            }

            string[] navigationProperties = ["RouteId_RouteInfo","MedicationDosages_MedicationDosage","MedicationCompositions_MedicationComposition","DoctorFavouriteMedication_DoctorFavouriteMedication","Product_Product","DrugListItems_DrugListItems"];
            foreach (var navigationProperty in navigationProperties)
            {
                if (allfields.Any(field => field.StartsWith(navigationProperty + ".", StringComparison.OrdinalIgnoreCase)))
                {
                    query = query.Include(navigationProperty);
                }
            }

            query = query.Where(entity => entity.Id == id);
            return _mapper.MapToFields(await query.FirstOrDefaultAsync(),fields);
        }

        /// <summary>Retrieves a list of medications based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of medications</returns>/// <exception cref="Exception"></exception>
        public async Task<List<Medication>> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = await GetMedication(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new medication</summary>
        /// <param name="model">The medication data to be added</param>
        /// <returns>The result of the operation</returns>
        public async Task<Guid> Create(Medication model)
        {
            model.Id = await CreateMedication(model);
            return model.Id;
        }

        /// <summary>Updates a specific medication by its primary key</summary>
        /// <param name="id">The primary key of the medication</param>
        /// <param name="updatedEntity">The medication data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> Update(Guid id, Medication updatedEntity)
        {
            await UpdateMedication(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific medication by its primary key</summary>
        /// <param name="id">The primary key of the medication</param>
        /// <param name="updatedEntity">The medication data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> Patch(Guid id, JsonPatchDocument<Medication> updatedEntity)
        {
            await PatchMedication(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific medication by its primary key</summary>
        /// <param name="id">The primary key of the medication</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> Delete(Guid id)
        {
            await DeleteMedication(id);
            return true;
        }
        #region
        private async Task<List<Medication>> GetMedication(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Medication.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Medication>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Medication), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Medication, object>>(Expression.Convert(property, typeof(object)), parameter);
                if (sortOrder.Equals("asc", StringComparison.OrdinalIgnoreCase))
                {
                    result = result.OrderBy(lambda);
                }
                else if (sortOrder.Equals("desc", StringComparison.OrdinalIgnoreCase))
                {
                    result = result.OrderByDescending(lambda);
                }
                else
                {
                    throw new ApplicationException("Invalid sort order. Use 'asc' or 'desc'");
                }
            }

            var paginatedResult = await result.Skip(skip).Take(pageSize).ToListAsync();
            return paginatedResult;
        }

        private async Task<Guid> CreateMedication(Medication model)
        {
            _dbContext.Medication.Add(model);
            await _dbContext.SaveChangesAsync();
            return model.Id;
        }

        private async Task UpdateMedication(Guid id, Medication updatedEntity)
        {
            _dbContext.Medication.Update(updatedEntity);
            await _dbContext.SaveChangesAsync();
        }

        private async Task<bool> DeleteMedication(Guid id)
        {
            var entityData = _dbContext.Medication.FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Medication.Remove(entityData);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        private async Task PatchMedication(Guid id, JsonPatchDocument<Medication> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Medication.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Medication.Update(existingEntity);
            await _dbContext.SaveChangesAsync();
        }
        #endregion
    }
}