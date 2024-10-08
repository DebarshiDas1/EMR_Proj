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
    /// Controller responsible for managing purchaseorderfile related operations.
    /// </summary>
    /// <remarks>
    /// This Controller provides endpoints for adding, retrieving, updating, and deleting purchaseorderfile information.
    /// </remarks>
    [Route("api/purchaseorderfile")]
    [Authorize]
    public class PurchaseOrderFileController : BaseApiController
    {
        private readonly IPurchaseOrderFileService _purchaseOrderFileService;

        /// <summary>
        /// Initializes a new instance of the PurchaseOrderFileController class with the specified context.
        /// </summary>
        /// <param name="ipurchaseorderfileservice">The ipurchaseorderfileservice to be used by the controller.</param>
        public PurchaseOrderFileController(IPurchaseOrderFileService ipurchaseorderfileservice)
        {
            _purchaseOrderFileService = ipurchaseorderfileservice;
        }

        /// <summary>Adds a new purchaseorderfile</summary>
        /// <param name="model">The purchaseorderfile data to be added</param>
        /// <returns>The result of the operation</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [UserAuthorize("PurchaseOrderFile", Entitlements.Create)]
        public async Task<IActionResult> Post([FromBody] PurchaseOrderFile model)
        {
            model.TenantId = TenantId;
            model.CreatedBy = UserId;
            model.CreatedOn = DateTime.UtcNow;
            var id = await _purchaseOrderFileService.Create(model);
            return Ok(new { id });
        }

        /// <summary>Retrieves a list of purchaseorderfiles based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of purchaseorderfiles</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        [UserAuthorize("PurchaseOrderFile", Entitlements.Read)]
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

            var result = await _purchaseOrderFileService.Get(filterCriteria, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return Ok(result);
        }

        /// <summary>Retrieves a specific purchaseorderfile by its primary key</summary>
        /// <param name="id">The primary key of the purchaseorderfile</param>
        /// <param name="fields">The fields is fetch data of selected fields</param>
        /// <returns>The purchaseorderfile data</returns>
        [HttpGet]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [UserAuthorize("PurchaseOrderFile", Entitlements.Read)]
        public async Task<IActionResult> GetById([FromRoute] Guid id, string fields = null)
        {
            var result = await _purchaseOrderFileService.GetById( id, fields);
            return Ok(result);
        }

        /// <summary>Deletes a specific purchaseorderfile by its primary key</summary>
        /// <param name="id">The primary key of the purchaseorderfile</param>
        /// <returns>The result of the operation</returns>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [Route("{id:Guid}")]
        [UserAuthorize("PurchaseOrderFile", Entitlements.Delete)]
        public async Task<IActionResult> DeleteById([FromRoute] Guid id)
        {
            var status = await _purchaseOrderFileService.Delete(id);
            return Ok(new { status });
        }

        /// <summary>Updates a specific purchaseorderfile by its primary key</summary>
        /// <param name="id">The primary key of the purchaseorderfile</param>
        /// <param name="updatedEntity">The purchaseorderfile data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPut]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        [UserAuthorize("PurchaseOrderFile", Entitlements.Update)]
        public async Task<IActionResult> UpdateById(Guid id, [FromBody] PurchaseOrderFile updatedEntity)
        {
            if (id != updatedEntity.Id)
            {
                return BadRequest("Mismatched Id");
            }

            updatedEntity.TenantId = TenantId;
            var status = await _purchaseOrderFileService.Update(id, updatedEntity);
            return Ok(new { status });
        }

        /// <summary>Updates a specific purchaseorderfile by its primary key</summary>
        /// <param name="id">The primary key of the purchaseorderfile</param>
        /// <param name="updatedEntity">The purchaseorderfile data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPatch]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        [UserAuthorize("PurchaseOrderFile", Entitlements.Update)]
        public async Task<IActionResult> UpdateById(Guid id, [FromBody] JsonPatchDocument<PurchaseOrderFile> updatedEntity)
        {
            if (updatedEntity == null)
                return BadRequest("Patch document is missing.");
            var status = await _purchaseOrderFileService.Patch(id, updatedEntity);
            return Ok(new { status });
        }
    }
}