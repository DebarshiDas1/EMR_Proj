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
    /// Controller responsible for managing bloodgroup related operations.
    /// </summary>
    /// <remarks>
    /// This Controller provides endpoints for adding, retrieving, updating, and deleting bloodgroup information.
    /// </remarks>
    [Route("api/bloodgroup")]
    [Authorize]
    public class BloodGroupController : BaseApiController
    {
        private readonly IBloodGroupService _bloodGroupService;

        /// <summary>
        /// Initializes a new instance of the BloodGroupController class with the specified context.
        /// </summary>
        /// <param name="ibloodgroupservice">The ibloodgroupservice to be used by the controller.</param>
        public BloodGroupController(IBloodGroupService ibloodgroupservice)
        {
            _bloodGroupService = ibloodgroupservice;
        }

        /// <summary>Adds a new bloodgroup</summary>
        /// <param name="model">The bloodgroup data to be added</param>
        /// <returns>The result of the operation</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [UserAuthorize("BloodGroup", Entitlements.Create)]
        public async Task<IActionResult> Post([FromBody] BloodGroup model)
        {
            model.TenantId = TenantId;
            var id = await _bloodGroupService.Create(model);
            return Ok(new { id });
        }

        /// <summary>Retrieves a list of bloodgroups based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of bloodgroups</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        [UserAuthorize("BloodGroup", Entitlements.Read)]
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

            var result = await _bloodGroupService.Get(filterCriteria, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return Ok(result);
        }

        /// <summary>Retrieves a specific bloodgroup by its primary key</summary>
        /// <param name="id">The primary key of the bloodgroup</param>
        /// <param name="fields">The fields is fetch data of selected fields</param>
        /// <returns>The bloodgroup data</returns>
        [HttpGet]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [UserAuthorize("BloodGroup", Entitlements.Read)]
        public async Task<IActionResult> GetById([FromRoute] Guid id, string fields = null)
        {
            var result = await _bloodGroupService.GetById( id, fields);
            return Ok(result);
        }

        /// <summary>Deletes a specific bloodgroup by its primary key</summary>
        /// <param name="id">The primary key of the bloodgroup</param>
        /// <returns>The result of the operation</returns>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [Route("{id:Guid}")]
        [UserAuthorize("BloodGroup", Entitlements.Delete)]
        public async Task<IActionResult> DeleteById([FromRoute] Guid id)
        {
            var status = await _bloodGroupService.Delete(id);
            return Ok(new { status });
        }

        /// <summary>Updates a specific bloodgroup by its primary key</summary>
        /// <param name="id">The primary key of the bloodgroup</param>
        /// <param name="updatedEntity">The bloodgroup data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPut]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        [UserAuthorize("BloodGroup", Entitlements.Update)]
        public async Task<IActionResult> UpdateById(Guid id, [FromBody] BloodGroup updatedEntity)
        {
            if (id != updatedEntity.Id)
            {
                return BadRequest("Mismatched Id");
            }

            updatedEntity.TenantId = TenantId;
            var status = await _bloodGroupService.Update(id, updatedEntity);
            return Ok(new { status });
        }

        /// <summary>Updates a specific bloodgroup by its primary key</summary>
        /// <param name="id">The primary key of the bloodgroup</param>
        /// <param name="updatedEntity">The bloodgroup data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPatch]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        [UserAuthorize("BloodGroup", Entitlements.Update)]
        public async Task<IActionResult> UpdateById(Guid id, [FromBody] JsonPatchDocument<BloodGroup> updatedEntity)
        {
            if (updatedEntity == null)
                return BadRequest("Patch document is missing.");
            var status = await _bloodGroupService.Patch(id, updatedEntity);
            return Ok(new { status });
        }
    }
}