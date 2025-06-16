using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace QuickDelivery.Database.Extensions
{
    public static class DataSeeding
    {
        public static async Task SeedManyToManyRelationshipsAsync(this ApplicationDbContext context, ILogger logger)
        {
            try
            {
                // Check if many-to-many relationships already exist
                var existingRelationships = await context.Database
                    .SqlQueryRaw<int>("SELECT COUNT(*) as Value FROM ProductCategories")
                    .FirstOrDefaultAsync();

                if (existingRelationships > 0)
                {
                    logger.LogInformation("Many-to-many relationships already seeded");
                    return;
                }

                logger.LogInformation("Seeding many-to-many relationships...");

                // Get products and categories
                var products = await context.Products.ToListAsync();
                var categories = await context.Categories.ToListAsync();

                if (products.Any() && categories.Any())
                {
                    // Margherita Pizza -> Pizza, Fast Food
                    var margherita = products.FirstOrDefault(p => p.Name == "Margherita Pizza");
                    if (margherita != null)
                    {
                        var pizzaCategory = categories.FirstOrDefault(c => c.Name == "Pizza");
                        var fastFoodCategory = categories.FirstOrDefault(c => c.Name == "Fast Food");

                        if (pizzaCategory != null && fastFoodCategory != null)
                        {
                            await context.Database.ExecuteSqlRawAsync(
                                "INSERT INTO ProductCategories (ProductsProductId, CategoriesCategoryId) VALUES ({0}, {1})",
                                margherita.ProductId, pizzaCategory.CategoryId);

                            await context.Database.ExecuteSqlRawAsync(
                                "INSERT INTO ProductCategories (ProductsProductId, CategoriesCategoryId) VALUES ({0}, {1})",
                                margherita.ProductId, fastFoodCategory.CategoryId);
                        }
                    }

                    // Cheeseburger -> Fast Food
                    var cheeseburger = products.FirstOrDefault(p => p.Name == "Cheeseburger");
                    if (cheeseburger != null)
                    {
                        var fastFoodCategory = categories.FirstOrDefault(c => c.Name == "Fast Food");
                        if (fastFoodCategory != null)
                        {
                            await context.Database.ExecuteSqlRawAsync(
                                "INSERT INTO ProductCategories (ProductsProductId, CategoriesCategoryId) VALUES ({0}, {1})",
                                cheeseburger.ProductId, fastFoodCategory.CategoryId);
                        }
                    }

                    // Chicken Pad Thai -> Asian Food
                    var padThai = products.FirstOrDefault(p => p.Name == "Chicken Pad Thai");
                    if (padThai != null)
                    {
                        var asianCategory = categories.FirstOrDefault(c => c.Name == "Asian Food");
                        if (asianCategory != null)
                        {
                            await context.Database.ExecuteSqlRawAsync(
                                "INSERT INTO ProductCategories (ProductsProductId, CategoriesCategoryId) VALUES ({0}, {1})",
                                padThai.ProductId, asianCategory.CategoryId);
                        }
                    }

                    // Chocolate Cake -> Desserts
                    var cake = products.FirstOrDefault(p => p.Name == "Chocolate Cake");
                    if (cake != null)
                    {
                        var dessertsCategory = categories.FirstOrDefault(c => c.Name == "Desserts");
                        if (dessertsCategory != null)
                        {
                            await context.Database.ExecuteSqlRawAsync(
                                "INSERT INTO ProductCategories (ProductsProductId, CategoriesCategoryId) VALUES ({0}, {1})",
                                cake.ProductId, dessertsCategory.CategoryId);
                        }
                    }

                    logger.LogInformation("Many-to-many relationships seeded successfully");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while seeding many-to-many relationships");
            }
        }
    }
}