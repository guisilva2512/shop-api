using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.api.Data;
using Shop.api.Models;

namespace Shop.api.Controllers
{
    [Route("api/produto")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Product>>> Get([FromServices]DataContext context)
        {
            var products = await context.Products.Include(x => x.Category).AsNoTracking().ToListAsync();
            return products;
        }

        [HttpGet]
        [Route("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<Product>> GetById(int id, [FromServices]DataContext context)
        {
            var product = await context.Products.Include(x => x.Category).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            return product;
        }

        [HttpGet] // produto/categoria/1
        [Route("categoria/{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Product>>> GetByCategory(int id, [FromServices]DataContext context)
        {
            var products = await context.Products.Include(x => x.Category).AsNoTracking().Where(x => x.CategoryId == id).ToListAsync();
            return products;
        }

        [HttpPost]
        [Route("")]
        [Authorize]
        public async Task<ActionResult<Product>> Post([FromBody]Product model, [FromServices]DataContext context)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    context.Products.Add(model);
                    await context.SaveChangesAsync();
                    return model;
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Erro ao salvar o produto, detalhes: " + ex.Message });
            }
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize]
        public async Task<ActionResult<Product>> Put(int id, [FromBody]Product model, [FromServices]DataContext context)
        {
            // Verifica se o Id informado é o mesmo do modelo
            if (model.Id != id)
                return NotFound(new { message = "Categoria não encontrada" });

            // Verifica se os dados são válidos
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                context.Entry<Product>(model).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return model;
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Erro ao salvar o produto, detalhes: " + ex.Message });
            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult> Delete(int id, [FromServices]DataContext context)
        {
            try
            {
                Product product = await context.Products.FirstOrDefaultAsync(x => x.Id == id);
                if (product == null)
                    return NotFound(new { message = "Produto não encontrado" });

                context.Products.Remove(product);
                await context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Erro ao excluir o produto, detalhes: " + ex.Message });
            }
        }

    }
}