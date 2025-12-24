using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizBio.Infrastructure.Data.Seeding
{
    using MySqlConnector;

    public static class MenuSeeder
    {
        public static async Task SeedAsync(string connectionString)
        {
            await using var conn = new MySqlConnection(connectionString);
            await conn.OpenAsync();

            await using var tx = await conn.BeginTransactionAsync();

            try
            {
                if (await AlreadySeeded(conn, tx))
                    return;

                var categoryIds = new Dictionary<string, long>();

                // Insert Categories
                foreach (var cat in MenuData.Categories)
                {
                    var id = await InsertCategory(conn, tx, cat);
                    categoryIds[cat.Name] = id;
                }

                // Insert Items
                foreach (var item in MenuData.Items)
                {
                    try
                    {

                    await InsertItem(conn, tx, item, categoryIds[item.CategoryName]);
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine($"Error inserting item '{item.Name}': {ex.Message}");
                    }
                }

                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        private static async Task<bool> AlreadySeeded(MySqlConnection c, MySqlTransaction t)
        {
            var cmd = new MySqlCommand(
                "SELECT 1 FROM Categories WHERE CatalogId = 100 LIMIT 1",
                c, t);

            return await cmd.ExecuteScalarAsync() != null;
        }

        private static async Task<long> InsertCategory(
            MySqlConnection c, MySqlTransaction t, CategorySeed cat)
        {
            var cmd = new MySqlCommand("""
            INSERT INTO Categories
            (CatalogId, Name, Description, SortOrder,
             IsActive, IsValid, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
            VALUES
            (1, @name, @desc, @sort,
             1, 1, NOW(), NOW(), 'System', 'System');
            SELECT LAST_INSERT_ID();
        """, c, t);

            cmd.Parameters.AddWithValue("@name", cat.Name);
            cmd.Parameters.AddWithValue("@desc", cat.Description);
            cmd.Parameters.AddWithValue("@sort", cat.SortOrder);

            return Convert.ToInt64(await cmd.ExecuteScalarAsync());
        }

        private static async Task InsertItem(
            MySqlConnection c, MySqlTransaction t, ItemSeed item, long categoryId)
        {
            var cmd = new MySqlCommand("""
            INSERT INTO CatalogItems
            (CatalogId, CategoryId, ItemType,
             Name, Description, Price, Tags,
             AvailableInEventMode, EventModeOnly, SortOrder,
             IsActive, IsValid, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
            VALUES
            (1, @catId, 0,
             @name, @desc, @price, @tags,
             1, 0, @sort,
             1, 1, NOW(), NOW(), 'System', 'System');
        """, c, t);

            cmd.Parameters.AddWithValue("@catId", categoryId);
            cmd.Parameters.AddWithValue("@name", item.Name);
            cmd.Parameters.AddWithValue("@desc", item.Description);
            cmd.Parameters.AddWithValue("@price", item.Price);
            cmd.Parameters.AddWithValue("@tags", item.Tags);
            cmd.Parameters.AddWithValue("@sort", item.SortOrder);

            await cmd.ExecuteNonQueryAsync();
        }
    }

}
