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
    /// Controller responsible for managing medicationdosage related operations.
    /// </summary>
    /// <remarks>
    /// This Controller provides endpoints for adding, retrieving, updating, and deleting medicationdosage information.
    /// </remarks>
    [Route("api/medicationdosage")]
    [Authorize]
    public class MedicationDosageController : BaseApiController
    {
        private readonly IMedicationDosageService _medicationDosageService;

        /// <summary>
        /// Initializes a new instance of the MedicationDosageController class with the specified context.
        /// </summary>
        /// <param name="imedicationdosageservice">The imedicationdosageservice to be used by the controller.</param>
        public MedicationDosageController(IMedicationDosageService imedicationdosageservice)
        {
            _medicationDosageService = imedicationdosageservice;
        }

        /// <summary>Adds a new medicationdosage</summary>
        /// <param name="model">The medicationdosage data to be added</param>
        /// <returns>The result of the operation</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [UserAuthorize("MedicationDosage", Entitlements.Create)]
        public async Task<IActionResult> Post([FromBody] MedicationDosage model)
        {
            model.TenantId = TenantId;
            var id = await _medicationDosageService.Create(model);
            return Ok(new { id });
        }

        /// <summary>Retrieves a list of medicationdosages based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of medicationdosages</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        [UserAuthorize("MedicationDosage", Entitlements.Read)]
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

            var result = await _medicationDosageService.Get(filterCriteria, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return Ok(result);
        }

        /// <summary>Retrieves a specific medicationdosage by its primary key</summary>
        /// <param name="id">The primary key of the medicationdosage</param>
        /// <param name="fields">The fields is fetch data of selected fields</param>
        /// <returns>The medicationdosage data</returns>
        [HttpGet]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [UserAuthorize("MedicationDosage", Entitlements.Read)]
        public async Task<IActionResult> GetById([FromRoute] Guid id, string fields = null)
        {
            var result = await _medicationDosageService.GetById( id, fields);
            return Ok(result);
        }

        /// <summary>Deletes a specific medicationdosage by its primary key</summary>
        /// <param name="id">The primary key of the medicationdosage</param>
        /// <returns>The result of the operation</returns>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [Route("{id:Guid}")]
        [UserAuthorize("MedicationDosage", Entitlements.Delete)]
        public async Task<IActionResult> DeleteById([FromRoute] Guid id)
        {
            var status = await _medicationDosageService.Delete(id);
            return Ok(new { status });
        }

        /// <summary>Updates a specific medicationdosage by its primary key</summary>
        /// <param name="id">The primary key of the medicationdosage</param>
        /// <param name="updatedEntity">The medicationdosage data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPut]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        [UserAuthorize("MedicationDosage", Entitlements.Update)]
        public async Task<IActionResult> UpdateById(Guid id, [FromBody] MedicationDosage updatedEntity)
        {
            if (id != updatedEntity.Id)
            {
                return BadRequest("Mismatched Id");
            }

            updatedEntity.TenantId = TenantId;
            var status = await _medicationDosageService.Update(id, updatedEntity);
            return Ok(new { status });
        }

        /// <summary>Updates a specific medicationdosage by its primary key</summary>
        /// <param name="id">The primary key of the medicationdosage</param>
        /// <param name="updatedEntity">The medicationdosage data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPatch]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        [UserAuthorize("MedicationDosage", Entitlements.Update)]
        public async Task<IActionResult> UpdateById(Guid id, [FromBody] JsonPatchDocument<MedicationDosage> updatedEntity)
        {
            if (updatedEntity == null)
                return BadRequest("Patch document is missing.");
            var status = await _medicationDosageService.Patch(id, updatedEntity);
            return Ok(new { status });
        }
    }
}