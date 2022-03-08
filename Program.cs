using BaltaDataAccess.Models;
using Dapper;
using Microsoft.Data.SqlClient;

const string connectionString = "Server=localhost\\SQLEXPRESS;Database=baltaCourse;Trusted_Connection=True";



using (var connection = new SqlConnection(connectionString))
{
    //CreateCategory(connection);
    // UpdateCategory(connection);
    // DeleteCategory(connection);
     ListCategories(connection);

}

static void ListCategories(SqlConnection connection)
{
    var categories = connection.Query<Category>("SELECT [Id], [Title] FROM [Category]");
    foreach (var category in categories)
    {
        Console.WriteLine($"{category.Id} --- {category.Title}");
    }
}

static void CreateCategory(SqlConnection connection)
{
    var category = new Category();
    category.Id = Guid.NewGuid();
    category.Title = "BackEnd 2022";
    category.Url = "backend";
    category.Description = "Categoria Back End 2022";
    category.Summary = "Curso de backEnd Completo de 0 ao pro";
    category.Featured = true;

    var sqlInsert = @"INSERT INTO
            [Category]
    Values(
    @Id,
    @Title,
    @Url,
    @Description,
    @Order,
    @Summary,
    @Featured)";

    var rows = connection.Execute(sqlInsert, new
    {
        category.Id,
        category.Title,
        category.Url,
        category.Description,
        category.Order,
        category.Summary,
        category.Featured
    });

    Console.WriteLine($"{rows} Linhas afetadas");

}
static void DeleteCategory(SqlConnection connection)
{
    var deleteQuery = "DELETE [Category] WHERE [Id] = @Id";
    var rows = connection.Execute(deleteQuery, new
    {
        Id = new Guid("0c62b70e-1d92-484b-b568-a0d883beeb2a")
    });

    Console.WriteLine($"{rows} registros excluídos");
}

static void UpdateCategory(SqlConnection connection)
{
    var updateQuery = "UPDATE [Category] SET [Title]=@title WHERE [Id]=@id";
    var rows = connection.Execute(updateQuery, new
    {
        id = new Guid("af3407aa-11ae-4621-a2ef-2028b85507c4"),
        title = "Front 2022"
    });

    Console.WriteLine($"{rows} registros atualizados");
}