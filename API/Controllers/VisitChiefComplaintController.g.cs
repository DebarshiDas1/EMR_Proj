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
    /// Controller responsible for managing visitchiefcomplaint related operations.
    /// </summary>
    /// <remarks>
    /// This Controller provides endpoints for adding, retrieving, updating, and deleting visitchiefcomplaint information.
    /// </remarks>
    [Route("api/visitchiefcomplaint")]
    [Authorize]
    public class VisitChiefComplaintController : BaseApiController
    {
        private readonly IVisitChiefComplaintService _visitChiefComplaintService;

        /// <summary>
        /// Initializes a new instance of the VisitChiefComplaintController class with the specified context.
        /// </summary>
        /// <param name="ivisitchiefcomplaintservice">The ivisitchiefcomplaintservice to be used by the controller.</param>
        public VisitChiefComplaintController(IVisitChiefComplaintService ivisitchiefcomplaintservice)
        {
            _visitChiefComplaintService = ivisitchiefcomplaintservice;
        }

        /// <summary>Adds a new visitchiefcomplaint</summary>
        /// <param name="model">The visitchiefcomplaint data to be added</param>
        /// <returns>The result of the operation</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [UserAuthorize("VisitChiefComplaint", Entitlements.Create)]
        public async Task<IActionResult> Post([FromBody] VisitChiefComplaint model)
        {
            model.TenantId = TenantId;
            var id = await _visitChiefComplaintService.Create(model);
            return Ok(new { id });
        }

        /// <summary>Retrieves a list of visitchiefcomplaints based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of visitchiefcomplaints</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        [UserAuthorize("VisitChiefComplaint", Entitlements.Read)]
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

            var result = await _visitChiefComplaintService.Get(filterCriteria, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return Ok(result);
        }

        /// <summary>Retrieves a specific visitchiefcomplaint by its primary key</summary>
        /// <param name="id">The primary key of the visitchiefcomplaint</param>
        /// <param name="fields">The fields is fetch data of selected fields</param>
        /// <returns>The visitchiefcomplaint data</returns>
        [HttpGet]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [UserAuthorize("VisitChiefComplaint", Entitlements.Read)]
        public async Task<IActionResult> GetById([FromRoute] Guid id, string fields = null)
        {
            var result = await _visitChiefComplaintService.GetById( id, fields);
            return Ok(result);
        }

        /// <summary>Deletes a specific visitchiefcomplaint by its primary key</summary>
        /// <param name="id">The primary key of the visitchiefcomplaint</param>
        /// <returns>The result of the operation</returns>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [Route("{id:Guid}")]
        [UserAuthorize("VisitChiefComplaint", Entitlements.Delete)]
        public async Task<IActionResult> DeleteById([FromRoute] Guid id)
        {
            var status = await _visitChiefComplaintService.Delete(id);
            return Ok(new { status });
        }

        /// <summary>Updates a specific visitchiefcomplaint by its primary key</summary>
        /// <param name="id">The primary key of the visitchiefcomplaint</param>
        /// <param name="updatedEntity">The visitchiefcomplaint data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPut]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        [UserAuthorize("VisitChiefComplaint", Entitlements.Update)]
        public async Task<IActionResult> UpdateById(Guid id, [FromBody] VisitChiefComplaint updatedEntity)
        {
            if (id != updatedEntity.Id)
            {
                return BadRequest("Mismatched Id");
            }

            updatedEntity.TenantId = TenantId;
            var status = await _visitChiefComplaintService.Update(id, updatedEntity);
            return Ok(new { status });
        }

        /// <summary>Updates a specific visitchiefcomplaint by its primary key</summary>
        /// <param name="id">The primary key of the visitchiefcomplaint</param>
        /// <param name="updatedEntity">The visitchiefcomplaint data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPatch]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        [UserAuthorize("VisitChiefComplaint", Entitlements.Update)]
        public async Task<IActionResult> UpdateById(Guid id, [FromBody] JsonPatchDocument<VisitChiefComplaint> updatedEntity)
        {
            if (updatedEntity == null)
                return BadRequest("Patch document is missing.");
            var status = await _visitChiefComplaintService.Patch(id, updatedEntity);
            return Ok(new { status });
        }
    }
}