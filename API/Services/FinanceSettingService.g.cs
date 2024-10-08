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
    /// The financesettingService responsible for managing financesetting related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting financesetting information.
    /// </remarks>
    public interface IFinanceSettingService
    {
        /// <summary>Retrieves a specific financesetting by its primary key</summary>
        /// <param name="id">The primary key of the financesetting</param>
        /// <param name="fields">The fields is fetch data of selected fields</param>
        /// <returns>The financesetting data</returns>
        Task<dynamic> GetById(Guid id, string fields);

        /// <summary>Retrieves a list of financesettings based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of financesettings</returns>
        Task<List<FinanceSetting>> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new financesetting</summary>
        /// <param name="model">The financesetting data to be added</param>
        /// <returns>The result of the operation</returns>
        Task<Guid> Create(FinanceSetting model);

        /// <summary>Updates a specific financesetting by its primary key</summary>
        /// <param name="id">The primary key of the financesetting</param>
        /// <param name="updatedEntity">The financesetting data to be updated</param>
        /// <returns>The result of the operation</returns>
        Task<bool> Update(Guid id, FinanceSetting updatedEntity);

        /// <summary>Updates a specific financesetting by its primary key</summary>
        /// <param name="id">The primary key of the financesetting</param>
        /// <param name="updatedEntity">The financesetting data to be updated</param>
        /// <returns>The result of the operation</returns>
        Task<bool> Patch(Guid id, JsonPatchDocument<FinanceSetting> updatedEntity);

        /// <summary>Deletes a specific financesetting by its primary key</summary>
        /// <param name="id">The primary key of the financesetting</param>
        /// <returns>The result of the operation</returns>
        Task<bool> Delete(Guid id);
    }

    /// <summary>
    /// The financesettingService responsible for managing financesetting related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting financesetting information.
    /// </remarks>
    public class FinanceSettingService : IFinanceSettingService
    {
        private readonly EMRProjContext _dbContext;
        private readonly IFieldMapperService _mapper;

        /// <summary>
        /// Initializes a new instance of the FinanceSetting class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        /// <param name="mapper">mapper value to set.</param>
        public FinanceSettingService(EMRProjContext dbContext, IFieldMapperService mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        /// <summary>Retrieves a specific financesetting by its primary key</summary>
        /// <param name="id">The primary key of the financesetting</param>
        /// <param name="fields">The fields is fetch data of selected fields</param>
        /// <returns>The financesetting data</returns>
        public async Task<dynamic> GetById(Guid id, string fields)
        {
            var query = _dbContext.FinanceSetting.AsQueryable();
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

            string[] navigationProperties = ["ProductId_Product"];
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

        /// <summary>Retrieves a list of financesettings based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of financesettings</returns>/// <exception cref="Exception"></exception>
        public async Task<List<FinanceSetting>> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = await GetFinanceSetting(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new financesetting</summary>
        /// <param name="model">The financesetting data to be added</param>
        /// <returns>The result of the operation</returns>
        public async Task<Guid> Create(FinanceSetting model)
        {
            model.Id = await CreateFinanceSetting(model);
            return model.Id;
        }

        /// <summary>Updates a specific financesetting by its primary key</summary>
        /// <param name="id">The primary key of the financesetting</param>
        /// <param name="updatedEntity">The financesetting data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> Update(Guid id, FinanceSetting updatedEntity)
        {
            await UpdateFinanceSetting(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific financesetting by its primary key</summary>
        /// <param name="id">The primary key of the financesetting</param>
        /// <param name="updatedEntity">The financesetting data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> Patch(Guid id, JsonPatchDocument<FinanceSetting> updatedEntity)
        {
            await PatchFinanceSetting(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific financesetting by its primary key</summary>
        /// <param name="id">The primary key of the financesetting</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> Delete(Guid id)
        {
            await DeleteFinanceSetting(id);
            return true;
        }
        #region
        private async Task<List<FinanceSetting>> GetFinanceSetting(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.FinanceSetting.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<FinanceSetting>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(FinanceSetting), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<FinanceSetting, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private async Task<Guid> CreateFinanceSetting(FinanceSetting model)
        {
            _dbContext.FinanceSetting.Add(model);
            await _dbContext.SaveChangesAsync();
            return model.Id;
        }

        private async Task UpdateFinanceSetting(Guid id, FinanceSetting updatedEntity)
        {
            _dbContext.FinanceSetting.Update(updatedEntity);
            await _dbContext.SaveChangesAsync();
        }

        private async Task<bool> DeleteFinanceSetting(Guid id)
        {
            var entityData = _dbContext.FinanceSetting.FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.FinanceSetting.Remove(entityData);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        private async Task PatchFinanceSetting(Guid id, JsonPatchDocument<FinanceSetting> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.FinanceSetting.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.FinanceSetting.Update(existingEntity);
            await _dbContext.SaveChangesAsync();
        }
        #endregion
    }
}