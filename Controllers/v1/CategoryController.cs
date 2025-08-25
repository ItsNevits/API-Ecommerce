using ApiEcommerce.Constants;
using ApiEcommerce.Models.Dtos;
using ApiEcommerce.Repository.IRepository;
using Asp.Versioning;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace ApiEcommerce.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize(Roles = RolesName.Admin)]
    //[EnableCors(PolicyNames.AllowSpecificOrigin)]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Obsolete("Use GetCategoriesOrderById in v2.0")]
        // [EnableCors(PolicyNames.AllowSpecificOrigin)]
        public IActionResult GetCategories()
        {
            var categories = _categoryRepository.GetCategories();
            var categoriesDto = categories.Adapt<IEnumerable<CategoryDto>>();
            return Ok(categoriesDto);
        }

        [AllowAnonymous]
        [HttpGet("{id:guid}", Name = "GetCategory")]
        //[ResponseCache(Duration = 10)]
        [ResponseCache(CacheProfileName = CacheProfiles.Default10)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetCategory(Guid id)
        {
            var categories = _categoryRepository.GetCategory(id);
            if (categories == null) return NotFound("Category not found");
            var categoriyDto = categories.Adapt<CategoryDto>();
            return Ok(categoriyDto);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateCategory([FromBody] CreateCategoryDto createCategoryDto)
        {
            if (createCategoryDto == null) return BadRequest(ModelState);

            if (_categoryRepository.CategoryExists(createCategoryDto.Name))
            {
                ModelState.AddModelError("CustomError", "Category already exists!");
                return BadRequest(ModelState);
            }

            Category category = createCategoryDto.Adapt<Category>();
            if (!_categoryRepository.CreateCategory(category))
            {
                ModelState.AddModelError("CustomError", $"Something went wrong when saving the record {category.Name}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetCategory", new { id = category.CategoryId }, category);
        }

        [HttpPatch("{id:guid}", Name = "UpdateCategory")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateCategory(Guid id, [FromBody] CreateCategoryDto updCategoryDto)
        {
            if (_categoryRepository.CategoryExists(id) == false) return NotFound("Category not found");
            if (updCategoryDto == null) return BadRequest(ModelState);

            if (_categoryRepository.CategoryExists(updCategoryDto.Name))
            {
                ModelState.AddModelError("CustomError", "Category already exists!");
                return BadRequest(ModelState);
            }

            Category category = updCategoryDto.Adapt<Category>();
            category.CategoryId = id;
            category.UpdatedOn = DateTime.UtcNow;

            if (!_categoryRepository.UpdateCategory(category))
            {
                ModelState.AddModelError("CustomError", $"Something went wrong when updating the record {category.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{id:guid}", Name = "DeleteCategory")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteCategory(Guid id)
        {
            if (_categoryRepository.CategoryExists(id) == false) return NotFound("Category not found");

            var category = _categoryRepository.GetCategory(id);
            if (category == null) return NotFound("Category not found");

            if (_categoryRepository.DeleteCategory(category) == false)
            {
                ModelState.AddModelError("CustomError", $"Something went wrong when deleting the record {category.Name}");
                return BadRequest(ModelState);
            }

            return NoContent();
        }
    }
}
