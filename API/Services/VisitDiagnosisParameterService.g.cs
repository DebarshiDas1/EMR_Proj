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
    /// The visitdiagnosisparameterService responsible for managing visitdiagnosisparameter related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting visitdiagnosisparameter information.
    /// </remarks>
    public interface IVisitDiagnosisParameterService
    {
        /// <summary>Retrieves a specific visitdiagnosisparameter by its primary key</summary>
        /// <param name="id">The primary key of the visitdiagnosisparameter</param>
        /// <param name="fields">The fields is fetch data of selected fields</param>
        /// <returns>The visitdiagnosisparameter data</returns>
        Task<dynamic> GetById(Guid id, string fields);

        /// <summary>Retrieves a list of visitdiagnosisparameters based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of visitdiagnosisparameters</returns>
        Task<List<VisitDiagnosisParameter>> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new visitdiagnosisparameter</summary>
        /// <param name="model">The visitdiagnosisparameter data to be added</param>
        /// <returns>The result of the operation</returns>
        Task<Guid> Create(VisitDiagnosisParameter model);

        /// <summary>Updates a specific visitdiagnosisparameter by its primary key</summary>
        /// <param name="id">The primary key of the visitdiagnosisparameter</param>
        /// <param name="updatedEntity">The visitdiagnosisparameter data to be updated</param>
        /// <returns>The result of the operation</returns>
        Task<bool> Update(Guid id, VisitDiagnosisParameter updatedEntity);

        /// <summary>Updates a specific visitdiagnosisparameter by its primary key</summary>
        /// <param name="id">The primary key of the visitdiagnosisparameter</param>
        /// <param name="updatedEntity">The visitdiagnosisparameter data to be updated</param>
        /// <returns>The result of the operation</returns>
        Task<bool> Patch(Guid id, JsonPatchDocument<VisitDiagnosisParameter> updatedEntity);

        /// <summary>Deletes a specific visitdiagnosisparameter by its primary key</summary>
        /// <param name="id">The primary key of the visitdiagnosisparameter</param>
        /// <returns>The result of the operation</returns>
        Task<bool> Delete(Guid id);
    }

    /// <summary>
    /// The visitdiagnosisparameterService responsible for managing visitdiagnosisparameter related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting visitdiagnosisparameter information.
    /// </remarks>
    public class VisitDiagnosisParameterService : IVisitDiagnosisParameterService
    {
        private readonly EMRProjContext _dbContext;
        private readonly IFieldMapperService _mapper;

        /// <summary>
        /// Initializes a new instance of the VisitDiagnosisParameter class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        /// <param name="mapper">mapper value to set.</param>
        public VisitDiagnosisParameterService(EMRProjContext dbContext, IFieldMapperService mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        /// <summary>Retrieves a specific visitdiagnosisparameter by its primary key</summary>
        /// <param name="id">The primary key of the visitdiagnosisparameter</param>
        /// <param name="fields">The fields is fetch data of selected fields</param>
        /// <returns>The visitdiagnosisparameter data</returns>
        public async Task<dynamic> GetById(Guid id, string fields)
        {
            var query = _dbContext.VisitDiagnosisParameter.AsQueryable();
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

            string[] navigationProperties = ["VisitDiagnosisId_VisitDiagnosis","ClinicalParameterId_ClinicalParameter","ClinicalParameterValueId_ClinicalParameterValue","UomId_Uom"];
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

        /// <summary>Retrieves a list of visitdiagnosisparameters based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of visitdiagnosisparameters</returns>/// <exception cref="Exception"></exception>
        public async Task<List<VisitDiagnosisParameter>> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = await GetVisitDiagnosisParameter(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new visitdiagnosisparameter</summary>
        /// <param name="model">The visitdiagnosisparameter data to be added</param>
        /// <returns>The result of the operation</returns>
        public async Task<Guid> Create(VisitDiagnosisParameter model)
        {
            model.Id = await CreateVisitDiagnosisParameter(model);
            return model.Id;
        }

        /// <summary>Updates a specific visitdiagnosisparameter by its primary key</summary>
        /// <param name="id">The primary key of the visitdiagnosisparameter</param>
        /// <param name="updatedEntity">The visitdiagnosisparameter data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> Update(Guid id, VisitDiagnosisParameter updatedEntity)
        {
            await UpdateVisitDiagnosisParameter(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific visitdiagnosisparameter by its primary key</summary>
        /// <param name="id">The primary key of the visitdiagnosisparameter</param>
        /// <param name="updatedEntity">The visitdiagnosisparameter data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> Patch(Guid id, JsonPatchDocument<VisitDiagnosisParameter> updatedEntity)
        {
            await PatchVisitDiagnosisParameter(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific visitdiagnosisparameter by its primary key</summary>
        /// <param name="id">The primary key of the visitdiagnosisparameter</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> Delete(Guid id)
        {
            await DeleteVisitDiagnosisParameter(id);
            return true;
        }
        #region
        private async Task<List<VisitDiagnosisParameter>> GetVisitDiagnosisParameter(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.VisitDiagnosisParameter.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<VisitDiagnosisParameter>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(VisitDiagnosisParameter), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<VisitDiagnosisParameter, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private async Task<Guid> CreateVisitDiagnosisParameter(VisitDiagnosisParameter model)
        {
            _dbContext.VisitDiagnosisParameter.Add(model);
            await _dbContext.SaveChangesAsync();
            return model.Id;
        }

        private async Task UpdateVisitDiagnosisParameter(Guid id, VisitDiagnosisParameter updatedEntity)
        {
            _dbContext.VisitDiagnosisParameter.Update(updatedEntity);
            await _dbContext.SaveChangesAsync();
        }

        private async Task<bool> DeleteVisitDiagnosisParameter(Guid id)
        {
            var entityData = _dbContext.VisitDiagnosisParameter.FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.VisitDiagnosisParameter.Remove(entityData);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        private async Task PatchVisitDiagnosisParameter(Guid id, JsonPatchDocument<VisitDiagnosisParameter> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.VisitDiagnosisParameter.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.VisitDiagnosisParameter.Update(existingEntity);
            await _dbContext.SaveChangesAsync();
        }
        #endregion
    }
}