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
    /// Controller responsible for managing doctorfavouritemedication related operations.
    /// </summary>
    /// <remarks>
    /// This Controller provides endpoints for adding, retrieving, updating, and deleting doctorfavouritemedication information.
    /// </remarks>
    [Route("api/doctorfavouritemedication")]
    [Authorize]
    public class DoctorFavouriteMedicationController : BaseApiController
    {
        private readonly IDoctorFavouriteMedicationService _doctorFavouriteMedicationService;

        /// <summary>
        /// Initializes a new instance of the DoctorFavouriteMedicationController class with the specified context.
        /// </summary>
        /// <param name="idoctorfavouritemedicationservice">The idoctorfavouritemedicationservice to be used by the controller.</param>
        public DoctorFavouriteMedicationController(IDoctorFavouriteMedicationService idoctorfavouritemedicationservice)
        {
            _doctorFavouriteMedicationService = idoctorfavouritemedicationservice;
        }

        /// <summary>Adds a new doctorfavouritemedication</summary>
        /// <param name="model">The doctorfavouritemedication data to be added</param>
        /// <returns>The result of the operation</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [UserAuthorize("DoctorFavouriteMedication", Entitlements.Create)]
        public async Task<IActionResult> Post([FromBody] DoctorFavouriteMedication model)
        {
            model.TenantId = TenantId;
            var id = await _doctorFavouriteMedicationService.Create(model);
            return Ok(new { id });
        }

        /// <summary>Retrieves a list of doctorfavouritemedications based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of doctorfavouritemedications</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        [UserAuthorize("DoctorFavouriteMedication", Entitlements.Read)]
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

            var result = await _doctorFavouriteMedicationService.Get(filterCriteria, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return Ok(result);
        }

        /// <summary>Retrieves a specific doctorfavouritemedication by its primary key</summary>
        /// <param name="id">The primary key of the doctorfavouritemedication</param>
        /// <param name="fields">The fields is fetch data of selected fields</param>
        /// <returns>The doctorfavouritemedication data</returns>
        [HttpGet]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [UserAuthorize("DoctorFavouriteMedication", Entitlements.Read)]
        public async Task<IActionResult> GetById([FromRoute] Guid id, string fields = null)
        {
            var result = await _doctorFavouriteMedicationService.GetById( id, fields);
            return Ok(result);
        }

        /// <summary>Deletes a specific doctorfavouritemedication by its primary key</summary>
        /// <param name="id">The primary key of the doctorfavouritemedication</param>
        /// <returns>The result of the operation</returns>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [Route("{id:Guid}")]
        [UserAuthorize("DoctorFavouriteMedication", Entitlements.Delete)]
        public async Task<IActionResult> DeleteById([FromRoute] Guid id)
        {
            var status = await _doctorFavouriteMedicationService.Delete(id);
            return Ok(new { status });
        }

        /// <summary>Updates a specific doctorfavouritemedication by its primary key</summary>
        /// <param name="id">The primary key of the doctorfavouritemedication</param>
        /// <param name="updatedEntity">The doctorfavouritemedication data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPut]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        [UserAuthorize("DoctorFavouriteMedication", Entitlements.Update)]
        public async Task<IActionResult> UpdateById(Guid id, [FromBody] DoctorFavouriteMedication updatedEntity)
        {
            if (id != updatedEntity.Id)
            {
                return BadRequest("Mismatched Id");
            }

            updatedEntity.TenantId = TenantId;
            var status = await _doctorFavouriteMedicationService.Update(id, updatedEntity);
            return Ok(new { status });
        }

        /// <summary>Updates a specific doctorfavouritemedication by its primary key</summary>
        /// <param name="id">The primary key of the doctorfavouritemedication</param>
        /// <param name="updatedEntity">The doctorfavouritemedication data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPatch]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        [UserAuthorize("DoctorFavouriteMedication", Entitlements.Update)]
        public async Task<IActionResult> UpdateById(Guid id, [FromBody] JsonPatchDocument<DoctorFavouriteMedication> updatedEntity)
        {
            if (updatedEntity == null)
                return BadRequest("Patch document is missing.");
            var status = await _doctorFavouriteMedicationService.Patch(id, updatedEntity);
            return Ok(new { status });
        }
    }
}