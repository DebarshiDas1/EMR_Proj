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
    /// The pricelistitemService responsible for managing pricelistitem related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting pricelistitem information.
    /// </remarks>
    public interface IPriceListItemService
    {
        /// <summary>Retrieves a specific pricelistitem by its primary key</summary>
        /// <param name="id">The primary key of the pricelistitem</param>
        /// <param name="fields">The fields is fetch data of selected fields</param>
        /// <returns>The pricelistitem data</returns>
        Task<dynamic> GetById(Guid id, string fields);

        /// <summary>Retrieves a list of pricelistitems based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of pricelistitems</returns>
        Task<List<PriceListItem>> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new pricelistitem</summary>
        /// <param name="model">The pricelistitem data to be added</param>
        /// <returns>The result of the operation</returns>
        Task<Guid> Create(PriceListItem model);

        /// <summary>Updates a specific pricelistitem by its primary key</summary>
        /// <param name="id">The primary key of the pricelistitem</param>
        /// <param name="updatedEntity">The pricelistitem data to be updated</param>
        /// <returns>The result of the operation</returns>
        Task<bool> Update(Guid id, PriceListItem updatedEntity);

        /// <summary>Updates a specific pricelistitem by its primary key</summary>
        /// <param name="id">The primary key of the pricelistitem</param>
        /// <param name="updatedEntity">The pricelistitem data to be updated</param>
        /// <returns>The result of the operation</returns>
        Task<bool> Patch(Guid id, JsonPatchDocument<PriceListItem> updatedEntity);

        /// <summary>Deletes a specific pricelistitem by its primary key</summary>
        /// <param name="id">The primary key of the pricelistitem</param>
        /// <returns>The result of the operation</returns>
        Task<bool> Delete(Guid id);
    }

    /// <summary>
    /// The pricelistitemService responsible for managing pricelistitem related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting pricelistitem information.
    /// </remarks>
    public class PriceListItemService : IPriceListItemService
    {
        private readonly EMRProjContext _dbContext;
        private readonly IFieldMapperService _mapper;

        /// <summary>
        /// Initializes a new instance of the PriceListItem class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        /// <param name="mapper">mapper value to set.</param>
        public PriceListItemService(EMRProjContext dbContext, IFieldMapperService mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        /// <summary>Retrieves a specific pricelistitem by its primary key</summary>
        /// <param name="id">The primary key of the pricelistitem</param>
        /// <param name="fields">The fields is fetch data of selected fields</param>
        /// <returns>The pricelistitem data</returns>
        public async Task<dynamic> GetById(Guid id, string fields)
        {
            var query = _dbContext.PriceListItem.AsQueryable();
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

            string[] navigationProperties = ["PriceListId_PriceList","ProductId_Product","ProductUomId_ProductUom"];
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

        /// <summary>Retrieves a list of pricelistitems based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of pricelistitems</returns>/// <exception cref="Exception"></exception>
        public async Task<List<PriceListItem>> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = await GetPriceListItem(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new pricelistitem</summary>
        /// <param name="model">The pricelistitem data to be added</param>
        /// <returns>The result of the operation</returns>
        public async Task<Guid> Create(PriceListItem model)
        {
            model.Id = await CreatePriceListItem(model);
            return model.Id;
        }

        /// <summary>Updates a specific pricelistitem by its primary key</summary>
        /// <param name="id">The primary key of the pricelistitem</param>
        /// <param name="updatedEntity">The pricelistitem data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> Update(Guid id, PriceListItem updatedEntity)
        {
            await UpdatePriceListItem(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific pricelistitem by its primary key</summary>
        /// <param name="id">The primary key of the pricelistitem</param>
        /// <param name="updatedEntity">The pricelistitem data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> Patch(Guid id, JsonPatchDocument<PriceListItem> updatedEntity)
        {
            await PatchPriceListItem(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific pricelistitem by its primary key</summary>
        /// <param name="id">The primary key of the pricelistitem</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> Delete(Guid id)
        {
            await DeletePriceListItem(id);
            return true;
        }
        #region
        private async Task<List<PriceListItem>> GetPriceListItem(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.PriceListItem.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<PriceListItem>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(PriceListItem), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<PriceListItem, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private async Task<Guid> CreatePriceListItem(PriceListItem model)
        {
            _dbContext.PriceListItem.Add(model);
            await _dbContext.SaveChangesAsync();
            return model.Id;
        }

        private async Task UpdatePriceListItem(Guid id, PriceListItem updatedEntity)
        {
            _dbContext.PriceListItem.Update(updatedEntity);
            await _dbContext.SaveChangesAsync();
        }

        private async Task<bool> DeletePriceListItem(Guid id)
        {
            var entityData = _dbContext.PriceListItem.FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.PriceListItem.Remove(entityData);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        private async Task PatchPriceListItem(Guid id, JsonPatchDocument<PriceListItem> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.PriceListItem.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.PriceListItem.Update(existingEntity);
            await _dbContext.SaveChangesAsync();
        }
        #endregion
    }
}