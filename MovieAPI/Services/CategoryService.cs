using Microsoft.EntityFrameworkCore;
using MovieAPI.Context;
using MovieAPI.Domain.Categories;

namespace MovieAPI.Services;

public class CategoryService
{
    private readonly ApplicationDbContext _context;

    public CategoryService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<CategoryDTO>> GetAllCategoriesAsync()
    {
        return await _context.Categories
            .Select(c => new CategoryDTO (c.CategoryId, c.Name ))
            .ToListAsync();
    }

    public async Task<CategoryDTO?> GetCategoryByIdAsync(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null) return null;

        return new CategoryDTO ( category.CategoryId, category.Name );
    }

    public async Task<CategoryDTO> CreateCategoryAsync(CreateCategoryDTO categoryDTO)
    {
        var category = new Category { Name = categoryDTO.Name };
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return new CategoryDTO ( category.CategoryId, category.Name );
    }

    public async Task<CategoryDTO?> UpdateCategoryAsync(int id, CategoryDTO categoryDTO)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null) return null;

        category.Name = categoryDTO.Name;
        await _context.SaveChangesAsync();

        return new CategoryDTO ( category.CategoryId, category.Name );
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null) return false;

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();

        return true;
    }
}
