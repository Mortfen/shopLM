using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace shop.api.Data.Controllers
{
    [ApiController]
    public class ProductsController : ControllerBase
    {
        // GET api/products — каталог с фильтрами
        [HttpGet("api/products")]
        public IActionResult GetAll()
        {
            throw new NotImplementedException();
        }

        // GET api/products/{id} — карточка товара
        [HttpGet("api/products/{id}")]
        public IActionResult GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        // POST api/products — создать товар (Seller)
        [HttpPost("api/products")]
        public IActionResult Create()
        {
            throw new NotImplementedException();
        }

        // PUT api/products/{id} — редактировать товар (Seller)
        [HttpPut("api/products/{id}")]
        public IActionResult Update(Guid id)
        {
            throw new NotImplementedException();
        }

        // DELETE api/products/{id} — удалить товар (Seller/Admin)
        [HttpDelete("api/products/{id}")]
        public IActionResult Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        // POST api/products/{id}/images — загрузить фото
        [HttpPost("api/products/{id}/images")]
        public IActionResult UploadImage(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
