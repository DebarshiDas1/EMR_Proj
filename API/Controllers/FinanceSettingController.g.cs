using Microsoft.AspNetCore.Mvc;
using EMRProj.Models;
using EMRProj.Services;
using EMRProj.Entities;
using EMRProj.Filter;
using EMRProj.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Task = System.Threading.Tasks.Task;
using EMRProj.Authorization;

namespace EMRProj.Controllers
{
    /// <summary>
    /// Controller responsible for managing financesetting related operations.
    /// </summary>
    /// <remarks>
    /// This Controller provides endpoints for adding, retrieving, updating, and deleting financesetting information.
    /// </remarks>
    [Route("api/financesetting")]
    [Authorize]
    public class FinanceSettingController : BaseApiController
    {
        private readonly IFinanceSettingService _financeSettingService;

        /// <summary>
        /// Initializes a new instance of the FinanceSettingController class with the specified context.
        /// </summary>
        /// <param name="ifinancesettingservice">The ifinancesettingservice to be used by the controller.</param>
        public FinanceSettingController(IFinanceSettingService ifinancesettingservice)
        {
            _financeSettingService = ifinancesettingservice;
        }

        /// <summary>Adds a new financesetting</summary>
        /// <param name="model">The financesetting data to be added</param>
        /// <returns>The result of the operation</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [UserAuthorize("FinanceSetting", Entitlements.Create)]
        public async Task<IActionResult> Post([FromBody] FinanceSetting model)
        {
            model.TenantId = TenantId;
            model.CreatedBy = UserId;
            model.CreatedOn = DateTime.UtcNow;
            var id = await _financeSettingService.Create(model);
            return Ok(new { id });
        }

        /// <summary>Retrieves a list of financesettings based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of financesettings</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        [UserAuthorize("FinanceSetting", Entitlements.Read)]
        public async Task<IActionResult> Get([FromQuery] string filters, string searchTerm, int pageNumber = 1, int pageSize = 10, string sortField = null, string sortOrder = "asc")
        {
            List<FilterCriteria> filterCriteria = null;
            if (pageSize < 1)
            {
                return BadRequest("Page size invalid.");
            }

            if (pageNumber < 1)
            {
                return BadRequest("Page mumber invalid.");
            }

            if (!string.IsNullOrEmpty(filters))
            {
                filterCriteria = JsonHelper.Deserialize<List<FilterCriteria>>(filters);
            }

            var result = await _financeSettingService.Get(filterCriteria, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return Ok(result);
        }

        /// <summary>Retrieves a specific financesetting by its primary key</summary>
        /// <param name="id">The primary key of the financesetting</param>
        /// <param name="fields">The fields is fetch data of selected fields</param>
        /// <returns>The financesetting data</returns>
        [HttpGet]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [UserAuthorize("FinanceSetting", Entitlements.Read)]
        public async Task<IActionResult> GetById([FromRoute] Guid id, string fields = null)
        {
            var result = await _financeSettingService.GetById( id, fields);
            return Ok(result);
        }

        /// <summary>Deletes a specific financesetting by its primary key</summary>
        /// <param name="id">The primary key of the financesetting</param>
        /// <returns>The result of the operation</returns>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [Route("{id:Guid}")]
        [UserAuthorize("FinanceSetting", Entitlements.Delete)]
        public async Task<IActionResult> DeleteById([FromRoute] Guid id)
        {
            var status = await _financeSettingService.Delete(id);
            return Ok(new { status });
        }

        /// <summary>Updates a specific financesetting by its primary key</summary>
        /// <param name="id">The primary key of the financesetting</param>
        /// <param name="updatedEntity">The financesetting data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPut]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        [UserAuthorize("FinanceSetting", Entitlements.Update)]
        public async Task<IActionResult> UpdateById(Guid id, [FromBody] FinanceSetting updatedEntity)
        {
            if (id != updatedEntity.Id)
            {
                return BadRequest("Mismatched Id");
            }

            updatedEntity.TenantId = TenantId;
            updatedEntity.UpdatedBy = UserId;
            updatedEntity.UpdatedOn = DateTime.UtcNow;
            var status = await _financeSettingService.Update(id, updatedEntity);
            return Ok(new { status });
        }

        /// <summary>Updates a specific financesetting by its primary key</summary>
        /// <param name="id">The primary key of the financesetting</param>
        /// <param name="updatedEntity">The financesetting data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPatch]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        [UserAuthorize("FinanceSetting", Entitlements.Update)]
        public async Task<IActionResult> UpdateById(Guid id, [FromBody] JsonPatchDocument<FinanceSetting> updatedEntity)
        {
            if (updatedEntity == null)
                return BadRequest("Patch document is missing.");
            var status = await _financeSettingService.Patch(id, updatedEntity);
            return Ok(new { status });
        }
    }
}