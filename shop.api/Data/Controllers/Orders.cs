using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace shop.api.Data.Controllers
{
    [ApiController]
    public class OrdersController : ControllerBase
    {
        // GET api/orders — мои заказы (Buyer)
        [HttpGet("api/orders")]
        public IActionResult GetMyOrders()
        {
            throw new NotImplementedException();
        }

        // GET api/orders/{id} — детали заказа
        [HttpGet("api/orders/{id}")]
        public IActionResult GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        // POST api/orders — создать заказ (Buyer)
        [HttpPost("api/orders")]
        public IActionResult Create()
        {
            throw new NotImplementedException();
        }

        // PUT api/orders/{id}/status — обновить статус (Seller/Admin)
        [HttpPut("api/orders/{id}/status")]
        public IActionResult UpdateStatus(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
