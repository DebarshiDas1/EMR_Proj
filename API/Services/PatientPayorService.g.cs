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
    /// The patientpayorService responsible for managing patientpayor related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting patientpayor information.
    /// </remarks>
    public interface IPatientPayorService
    {
        /// <summary>Retrieves a specific patientpayor by its primary key</summary>
        /// <param name="id">The primary key of the patientpayor</param>
        /// <param name="fields">The fields is fetch data of selected fields</param>
        /// <returns>The patientpayor data</returns>
        Task<dynamic> GetById(Guid id, string fields);

        /// <summary>Retrieves a list of patientpayors based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of patientpayors</returns>
        Task<List<PatientPayor>> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new patientpayor</summary>
        /// <param name="model">The patientpayor data to be added</param>
        /// <returns>The result of the operation</returns>
        Task<Guid> Create(PatientPayor model);

        /// <summary>Updates a specific patientpayor by its primary key</summary>
        /// <param name="id">The primary key of the patientpayor</param>
        /// <param name="updatedEntity">The patientpayor data to be updated</param>
        /// <returns>The result of the operation</returns>
        Task<bool> Update(Guid id, PatientPayor updatedEntity);

        /// <summary>Updates a specific patientpayor by its primary key</summary>
        /// <param name="id">The primary key of the patientpayor</param>
        /// <param name="updatedEntity">The patientpayor data to be updated</param>
        /// <returns>The result of the operation</returns>
        Task<bool> Patch(Guid id, JsonPatchDocument<PatientPayor> updatedEntity);

        /// <summary>Deletes a specific patientpayor by its primary key</summary>
        /// <param name="id">The primary key of the patientpayor</param>
        /// <returns>The result of the operation</returns>
        Task<bool> Delete(Guid id);
    }

    /// <summary>
    /// The patientpayorService responsible for managing patientpayor related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting patientpayor information.
    /// </remarks>
    public class PatientPayorService : IPatientPayorService
    {
        private readonly EMRProjContext _dbContext;
        private readonly IFieldMapperService _mapper;

        /// <summary>
        /// Initializes a new instance of the PatientPayor class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        /// <param name="mapper">mapper value to set.</param>
        public PatientPayorService(EMRProjContext dbContext, IFieldMapperService mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        /// <summary>Retrieves a specific patientpayor by its primary key</summary>
        /// <param name="id">The primary key of the patientpayor</param>
        /// <param name="fields">The fields is fetch data of selected fields</param>
        /// <returns>The patientpayor data</returns>
        public async Task<dynamic> GetById(Guid id, string fields)
        {
            var query = _dbContext.PatientPayor.AsQueryable();
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

            string[] navigationProperties = ["PatientId_Patient","ContactMemberId_ContactMember"];
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

        /// <summary>Retrieves a list of patientpayors based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of patientpayors</returns>/// <exception cref="Exception"></exception>
        public async Task<List<PatientPayor>> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = await GetPatientPayor(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new patientpayor</summary>
        /// <param name="model">The patientpayor data to be added</param>
        /// <returns>The result of the operation</returns>
        public async Task<Guid> Create(PatientPayor model)
        {
            model.Id = await CreatePatientPayor(model);
            return model.Id;
        }

        /// <summary>Updates a specific patientpayor by its primary key</summary>
        /// <param name="id">The primary key of the patientpayor</param>
        /// <param name="updatedEntity">The patientpayor data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> Update(Guid id, PatientPayor updatedEntity)
        {
            await UpdatePatientPayor(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific patientpayor by its primary key</summary>
        /// <param name="id">The primary key of the patientpayor</param>
        /// <param name="updatedEntity">The patientpayor data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> Patch(Guid id, JsonPatchDocument<PatientPayor> updatedEntity)
        {
            await PatchPatientPayor(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific patientpayor by its primary key</summary>
        /// <param name="id">The primary key of the patientpayor</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> Delete(Guid id)
        {
            await DeletePatientPayor(id);
            return true;
        }
        #region
        private async Task<List<PatientPayor>> GetPatientPayor(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.PatientPayor.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<PatientPayor>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(PatientPayor), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<PatientPayor, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private async Task<Guid> CreatePatientPayor(PatientPayor model)
        {
            _dbContext.PatientPayor.Add(model);
            await _dbContext.SaveChangesAsync();
            return model.Id;
        }

        private async Task UpdatePatientPayor(Guid id, PatientPayor updatedEntity)
        {
            _dbContext.PatientPayor.Update(updatedEntity);
            await _dbContext.SaveChangesAsync();
        }

        private async Task<bool> DeletePatientPayor(Guid id)
        {
            var entityData = _dbContext.PatientPayor.FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.PatientPayor.Remove(entityData);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        private async Task PatchPatientPayor(Guid id, JsonPatchDocument<PatientPayor> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.PatientPayor.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.PatientPayor.Update(existingEntity);
            await _dbContext.SaveChangesAsync();
        }
        #endregion
    }
}