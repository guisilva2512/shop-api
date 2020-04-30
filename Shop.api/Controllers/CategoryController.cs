using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.api.Data;
using Shop.api.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shop.api.Controllers
{
    //[Route("api/[controller]")]
    [Route("api/categoria")]
    [ApiController]
    public class CategoryController : ControllerBase
    {        
        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        [ResponseCache(VaryByHeader = "User-Agent", Location = ResponseCacheLocation.Any, Duration = 30)]
        // [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<ActionResult<List<Category>>> Get([FromServices]DataContext context)
        {
            var categories = await context.Categories.AsNoTracking().ToListAsync();
            return categories;
        }

        [HttpGet]
        [Route("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<Category>> GetById(int id, [FromServices]DataContext context)
        {
            var category = await context.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            return category;
        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles = "manager,employee")]
        public async Task<ActionResult<Category>> Post([FromBody]Category model, [FromServices]DataContext context)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                context.Categories.Add(model);
                await context.SaveChangesAsync();
                return model;
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "manager,employee")]
        public async Task<ActionResult<Category>> Put(int id, [FromBody]Category model, [FromServices]DataContext context)
        {
            // Verifica se o Id informado é o mesmo do modelo
            if (model.Id != id)
                return NotFound(new { message= "Categoria não encontrada" });

            // Verifica se os dados são válidos
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                context.Entry<Category>(model).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return model;
            }
            catch(DbUpdateConcurrencyException ex)
            {
                return BadRequest(new { message = $"Este registro já foi atualizado, detalhes: {ex.Message}" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "manager,employee")]
        public async Task<ActionResult<Category>> Delete(int id, [FromServices]DataContext context)
        {
            try
            {
                Category category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
                if (category == null)
                    return NotFound(new { message = "Categoria não encontrada"});

                context.Categories.Remove(category);
                await context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Erro ao excluir categoria, detalhes: " + ex.Message });
            }
        }
    }
}