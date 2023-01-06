using BasketService.Api.Core.Application.Repository;
using BasketService.Api.Core.Application.Services;
using BasketService.Api.Core.Domain.Models;
using BasketService.Api.IntegrationEvents.Events;
using EventBus.Base.Abstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BasketService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository repository;
        private readonly IIdentityService identityService;
        private readonly IEventBus eventBus;
        private readonly ILogger<BasketController> logger;

        public BasketController(IBasketRepository repository,
                                IIdentityService identityService,
                                IEventBus eventBus,
                                ILogger<BasketController> logger)
        {
            this.repository = repository;
            this.identityService = identityService;
            this.eventBus = eventBus;
            this.logger = logger;
        }

        // GET: api/<BasketController>
        [HttpGet]
        public IActionResult GetBasketServiceStatus()
        {
            return Ok("Basket service is running.");
        }

        // GET api/<BasketController>/5
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CustomerBasket), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CustomerBasket>> GetBasketById(string id)
        {
            var basket = await repository.GetBasketAsync(id);

            return Ok(basket ?? new CustomerBasket(id));
        }

        // POST api/<BasketController>/update
        [HttpPost]
        [Route("update")]
        [ProducesResponseType(typeof(CustomerBasket), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CustomerBasket>> UpdateBasketAsync([FromBody] CustomerBasket basket)
        {
            return Ok(await repository.UpdateBasketAsync(basket));
        }

        // POST api/<BasketController>/additem
        [HttpPost]
        [Route("additem")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> AddItemToBasketAsync([FromBody] BasketItem item)
        {
            var userId = identityService.GetUserName().ToString();

            var basket = await repository.GetBasketAsync(userId);

            if (basket == null)
            {
                basket = new CustomerBasket(userId);
            }

            basket.BasketItems.Add(item);

            await repository.UpdateBasketAsync(basket);

            return Ok("Item added to basket.");
        }

        // POST api/<BasketController>/checkout
        [HttpPost]
        [Route("checkout")]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> CheckoutAsync([FromBody] BasketCheckout checkout)
        {
            var userId = checkout.Buyer;

            var basket = await repository.GetBasketAsync(userId);

            if (basket == null)
            {
                return BadRequest("Basket is empty. Please add products to basket first then process to checkout again!");
            }

            var username = identityService.GetUserName();

            var eventMessage = new OrderCreatedIntegrationEvent(userId, username, checkout.City, checkout.Street, checkout.Country, checkout.State, checkout.ZipCode, checkout.CardNumber, checkout.CardHolderName, checkout.CardExpiration, checkout.CardSecurityNumber, checkout.CardTypeId, checkout.Buyer, basket);
            try
            {
                eventBus.Publish(eventMessage);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while publishing Integration Event: {IntegrationEventId} from {BasketService.App}", eventMessage.Id);

                throw;
            }

            return Accepted();
        }

        // DELETE api/<BasketController>/5
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        public async Task DeleteBasketAsync(string id)
        {

        }
    }
}
