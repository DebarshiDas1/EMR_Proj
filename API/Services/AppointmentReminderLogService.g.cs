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
    /// The appointmentreminderlogService responsible for managing appointmentreminderlog related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting appointmentreminderlog information.
    /// </remarks>
    public interface IAppointmentReminderLogService
    {
        /// <summary>Retrieves a specific appointmentreminderlog by its primary key</summary>
        /// <param name="id">The primary key of the appointmentreminderlog</param>
        /// <param name="fields">The fields is fetch data of selected fields</param>
        /// <returns>The appointmentreminderlog data</returns>
        Task<dynamic> GetById(Guid id, string fields);

        /// <summary>Retrieves a list of appointmentreminderlogs based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of appointmentreminderlogs</returns>
        Task<List<AppointmentReminderLog>> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new appointmentreminderlog</summary>
        /// <param name="model">The appointmentreminderlog data to be added</param>
        /// <returns>The result of the operation</returns>
        Task<Guid> Create(AppointmentReminderLog model);

        /// <summary>Updates a specific appointmentreminderlog by its primary key</summary>
        /// <param name="id">The primary key of the appointmentreminderlog</param>
        /// <param name="updatedEntity">The appointmentreminderlog data to be updated</param>
        /// <returns>The result of the operation</returns>
        Task<bool> Update(Guid id, AppointmentReminderLog updatedEntity);

        /// <summary>Updates a specific appointmentreminderlog by its primary key</summary>
        /// <param name="id">The primary key of the appointmentreminderlog</param>
        /// <param name="updatedEntity">The appointmentreminderlog data to be updated</param>
        /// <returns>The result of the operation</returns>
        Task<bool> Patch(Guid id, JsonPatchDocument<AppointmentReminderLog> updatedEntity);

        /// <summary>Deletes a specific appointmentreminderlog by its primary key</summary>
        /// <param name="id">The primary key of the appointmentreminderlog</param>
        /// <returns>The result of the operation</returns>
        Task<bool> Delete(Guid id);
    }

    /// <summary>
    /// The appointmentreminderlogService responsible for managing appointmentreminderlog related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting appointmentreminderlog information.
    /// </remarks>
    public class AppointmentReminderLogService : IAppointmentReminderLogService
    {
        private readonly EMRProjContext _dbContext;
        private readonly IFieldMapperService _mapper;

        /// <summary>
        /// Initializes a new instance of the AppointmentReminderLog class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        /// <param name="mapper">mapper value to set.</param>
        public AppointmentReminderLogService(EMRProjContext dbContext, IFieldMapperService mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        /// <summary>Retrieves a specific appointmentreminderlog by its primary key</summary>
        /// <param name="id">The primary key of the appointmentreminderlog</param>
        /// <param name="fields">The fields is fetch data of selected fields</param>
        /// <returns>The appointmentreminderlog data</returns>
        public async Task<dynamic> GetById(Guid id, string fields)
        {
            var query = _dbContext.AppointmentReminderLog.AsQueryable();
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

            string[] navigationProperties = ["AppointmentId_Appointment"];
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

        /// <summary>Retrieves a list of appointmentreminderlogs based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of appointmentreminderlogs</returns>/// <exception cref="Exception"></exception>
        public async Task<List<AppointmentReminderLog>> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = await GetAppointmentReminderLog(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new appointmentreminderlog</summary>
        /// <param name="model">The appointmentreminderlog data to be added</param>
        /// <returns>The result of the operation</returns>
        public async Task<Guid> Create(AppointmentReminderLog model)
        {
            model.Id = await CreateAppointmentReminderLog(model);
            return model.Id;
        }

        /// <summary>Updates a specific appointmentreminderlog by its primary key</summary>
        /// <param name="id">The primary key of the appointmentreminderlog</param>
        /// <param name="updatedEntity">The appointmentreminderlog data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> Update(Guid id, AppointmentReminderLog updatedEntity)
        {
            await UpdateAppointmentReminderLog(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific appointmentreminderlog by its primary key</summary>
        /// <param name="id">The primary key of the appointmentreminderlog</param>
        /// <param name="updatedEntity">The appointmentreminderlog data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> Patch(Guid id, JsonPatchDocument<AppointmentReminderLog> updatedEntity)
        {
            await PatchAppointmentReminderLog(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific appointmentreminderlog by its primary key</summary>
        /// <param name="id">The primary key of the appointmentreminderlog</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> Delete(Guid id)
        {
            await DeleteAppointmentReminderLog(id);
            return true;
        }
        #region
        private async Task<List<AppointmentReminderLog>> GetAppointmentReminderLog(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.AppointmentReminderLog.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<AppointmentReminderLog>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(AppointmentReminderLog), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<AppointmentReminderLog, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private async Task<Guid> CreateAppointmentReminderLog(AppointmentReminderLog model)
        {
            _dbContext.AppointmentReminderLog.Add(model);
            await _dbContext.SaveChangesAsync();
            return model.Id;
        }

        private async Task UpdateAppointmentReminderLog(Guid id, AppointmentReminderLog updatedEntity)
        {
            _dbContext.AppointmentReminderLog.Update(updatedEntity);
            await _dbContext.SaveChangesAsync();
        }

        private async Task<bool> DeleteAppointmentReminderLog(Guid id)
        {
            var entityData = _dbContext.AppointmentReminderLog.FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.AppointmentReminderLog.Remove(entityData);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        private async Task PatchAppointmentReminderLog(Guid id, JsonPatchDocument<AppointmentReminderLog> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.AppointmentReminderLog.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.AppointmentReminderLog.Update(existingEntity);
            await _dbContext.SaveChangesAsync();
        }
        #endregion
    }
}