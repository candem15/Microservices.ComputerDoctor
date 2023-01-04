using System.Net;
using CatalogService.Api.Core.Application.ViewModels;
using CatalogService.Api.Core.Domain;
using CatalogService.Api.Infrastructure;
using CatalogService.Api.Infrastructure.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CatalogService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly CatalogContext _context;
        private readonly CatalogSettings _settings;

        public CatalogController(CatalogContext context, IOptionsSnapshot<CatalogSettings> settings)
        {
            _context = context;
            _settings = settings.Value;
        }

        [HttpGet]
        [Route("items")]
        [ProducesResponseType(typeof(PaginatedItems_VM<CatalogItem>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(IEnumerable<CatalogItem>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetItemsAsync([FromQuery] int pageSize = 10, [FromQuery] int pageIndex = 0, string? ids = null)
        {
            if (!string.IsNullOrEmpty(ids))
            {
                var items = await GetItemsByIdsAsync(ids);

                if (!items.Any())
                {
                    return BadRequest("ids value invalid. Must include comma seperated list of numbers");
                }

                return Ok(items);
            }

            var totalItems = await _context.CatalogItems
                                        .LongCountAsync();

            var itemsOnPage = await _context.CatalogItems
                                        .OrderBy(x => x.Name)
                                        .Skip(pageSize * pageIndex)
                                        .Take(pageSize)
                                        .ToListAsync();

            itemsOnPage = ChangeUriPlaceholder(itemsOnPage);

            var model = new PaginatedItems_VM<CatalogItem>(pageIndex, pageSize, totalItems, itemsOnPage);

            return Ok(model);
        }

        private async Task<List<CatalogItem>> GetItemsByIdsAsync(string ids)
        {
            var numIds = ids.Split(',').Select(id => (Ok: int.TryParse(id, out int x), Value: x));

            if (!numIds.All(x => x.Ok))
            {
                return new List<CatalogItem>();
            }

            var idsToSelect = numIds
                                .Select(x => x.Value);

            var items = await _context.CatalogItems.Where(x => idsToSelect.Contains(x.Id)).ToListAsync();

            items = ChangeUriPlaceholder(items);

            return items;
        }

        [HttpGet]
        [Route("items/{id:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(CatalogItem), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CatalogItem>> GetItemByIdAsync(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var item = await _context.CatalogItems
                                .SingleOrDefaultAsync(x => x.Id == id);

            var baseUri = _settings.PicsBaseUrl;

            if (item != null)
            {
                item.PictureUri = baseUri + item.PictureFileName;
                return item;
            }

            return NotFound();
        }

        [HttpGet]
        [Route("items/withname/{name:minLength(1)}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(PaginatedItems_VM<CatalogItem>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<PaginatedItems_VM<CatalogItem>>> GetItemsWithNameAsync(string name, [FromQuery] int pageSize = 10, [FromQuery] int pageIndex = 0)
        {

            var totalItems = await _context.CatalogItems
                                        .Where(x => x.Name.StartsWith(name))
                                        .LongCountAsync();

            var itemsOnPage = await _context.CatalogItems
                                        .Where(x => x.Name.StartsWith(name))
                                        .Skip(pageSize * pageIndex)
                                        .Take(pageSize)
                                        .ToListAsync();

            if (itemsOnPage == null)
                return NotFound();

            itemsOnPage = ChangeUriPlaceholder(itemsOnPage);

            var model = new PaginatedItems_VM<CatalogItem>(pageIndex, pageSize, totalItems, itemsOnPage);

            return Ok(model);
        }

        [HttpGet]
        [Route("items/type/{catalogTypeId}/brand/{catalogBrandId:int?}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(PaginatedItems_VM<CatalogItem>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<PaginatedItems_VM<CatalogItem>>> GetItemsByTypeIdAndBrandIdAsync(int catalogTypeId, int? catalogBrandId, [FromQuery] int pageSize = 10, [FromQuery] int pageIndex = 0)
        {
            var root = _context.CatalogItems as IQueryable<CatalogItem>;

            root = root.Where(x => x.CatalogTypeId == catalogTypeId);

            if (catalogBrandId.HasValue)
            {
                root = root.Where(x => x.CatalogBrandId == catalogBrandId);
            }

            var totalItems = await root
                                    .LongCountAsync();

            var itemsOnPage = await root
                                     .Skip(pageSize * pageIndex)
                                     .Take(pageSize)
                                     .ToListAsync();

            itemsOnPage = ChangeUriPlaceholder(itemsOnPage);

            var model = new PaginatedItems_VM<CatalogItem>(pageIndex, pageSize, totalItems, itemsOnPage);

            return Ok(model);
        }

        [HttpGet]
        [Route("items/type/all/brand/{catalogTypeId:int?}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(PaginatedItems_VM<CatalogItem>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<PaginatedItems_VM<CatalogItem>>> GetItemsByBrandIdAsync(int? catalogBrandId, [FromQuery] int pageSize = 10, [FromQuery] int pageIndex = 0)
        {
            var root = _context.CatalogItems as IQueryable<CatalogItem>;

            if (catalogBrandId.HasValue)
            {
                root = root.Where(x => x.CatalogBrandId == catalogBrandId);
            }

            var totalItems = await root
                                    .LongCountAsync();

            var itemsOnPage = await root
                                     .Skip(pageSize * pageIndex)
                                     .Take(pageSize)
                                     .ToListAsync();

            itemsOnPage = ChangeUriPlaceholder(itemsOnPage);

            var model = new PaginatedItems_VM<CatalogItem>(pageIndex, pageSize, totalItems, itemsOnPage);

            return Ok(model);
        }

        [HttpGet]
        [Route("catalogtypes")]
        [ProducesResponseType(typeof(PaginatedItems_VM<CatalogType>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<PaginatedItems_VM<CatalogType>>> GetCatalogTypesAsync()
        {
            return Ok(_context.CatalogTypes.ToList());
        }

        [HttpGet]
        [Route("catalogbrands")]
        [ProducesResponseType(typeof(PaginatedItems_VM<CatalogBrand>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<PaginatedItems_VM<CatalogBrand>>> GetCatalogBrandsAsync()
        {
            return Ok(_context.CatalogBrands.ToList());
        }

        [HttpPut]
        [Route("items")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> UpdateProductAsync([FromBody] CatalogItem productToUpdate)
        {
            var itemToUpdate = await _context.CatalogItems.SingleOrDefaultAsync(x => x.Id == productToUpdate.Id);

            if (itemToUpdate == null)
                return NotFound(new { Message = $"Item with id: {productToUpdate.Id} not found." });

            var oldPrice = itemToUpdate.Price;
            var raiseProductPriceChangedEvent = oldPrice != productToUpdate.Price;

            itemToUpdate = productToUpdate;
            _context.CatalogItems.Update(itemToUpdate);

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetItemByIdAsync), new { id = productToUpdate.Id }, null);
        }

        [HttpPost]
        [Route("items")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<ActionResult> CreateProductAsync([FromBody] CatalogItem product)
        {

            _context.CatalogItems.Add(product);

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetItemByIdAsync), new { id = product.Id }, null);
        }

        [HttpDelete]
        [Route("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> DeleteProductAsync(int id)
        {
            var itemToDelete = await _context.CatalogItems.SingleOrDefaultAsync(x => x.Id == id);

            if (itemToDelete == null)
                return NotFound(new { Message = $"Item with id: {id} not found." });

            _context.CatalogItems.Remove(itemToDelete);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private List<CatalogItem> ChangeUriPlaceholder(List<CatalogItem> itemsOnPage)
        {
            var baseUri = _settings.PicsBaseUrl;

            foreach (var item in itemsOnPage)
            {
                if (item != null)
                {
                    item.PictureUri = baseUri + item.PictureFileName;
                }
            }

            return itemsOnPage;
        }

        // Ocelot Gateway endpoint testing

        [HttpGet("1")]
        public async Task<IActionResult> OcelotGatewayTest()
        {
            return Ok("Ocelot working");
        }
    }
}