using BaltaDataAccess.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

const string connectionString = "Server=localhost\\SQLEXPRESS;Database=baltaDb;Trusted_Connection=True";



using (var connection = new SqlConnection(connectionString))
{

    //ListCategories(connection);
    //CreateCategory(connection);
    //CreateManyCategory(connection);
    //UpdateCategory(connection);
    //DeleteCategory(connection);
    //ExecuteProcedure(connection);
    //ExecuteReadProcedure(connection);
    //ExetuteScalar(connection);
    //ReadView(connection);
    //OneToOne(connection);
    OneToMany(connection);

}

static void ListCategories(SqlConnection connection)
{
    var categories = connection.Query<Category>("SELECT [Id], [Title] FROM [Category] ORDER BY [Title]");
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

static void DeleteCategory(SqlConnection connection)
{
    var deleteQuery = "DELETE [Category] WHERE [Id] = @Id";
    var rows = connection.Execute(deleteQuery, new
    {
        Id = new Guid("0c62b70e-1d92-484b-b568-a0d883beeb2a")
    });

    Console.WriteLine($"{rows} registros excluídos");
}


static void CreateManyCategory(SqlConnection connection)
{
    var category = new Category();
    category.Id = Guid.NewGuid();
    category.Title = "BackEnd 2022";
    category.Url = "backend";
    category.Description = "Categoria Back End 2022";
    category.Summary = "Curso de backEnd Completo de 0 ao pro";
    category.Featured = true;

    var category2 = new Category();
    category2.Id = Guid.NewGuid();
    category2.Title = "Categoria Nova";
    category2.Url = "Categoria-Nova";
    category2.Description = "Categoria nova";
    category2.Summary = "Categoria";
    category2.Featured = true;

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

    var rows = connection.Execute(sqlInsert, new[]
    {
        new
        {
            category.Id,
            category.Title,
            category.Url,
            category.Description,
            category.Order,
            category.Summary,
            category.Featured
        },

        new
        {
            category2.Id,
            category2.Title,
            category2.Url,
            category2.Description,
            category2.Order,
            category2.Summary,
            category2.Featured
        }
    });

    Console.WriteLine($"{rows} novos registros");
}


static void ExecuteProcedure(SqlConnection connection)
{
    var procedure = "[spDeleteStudent]";
    var pars = new { StudentId = "026ff2cc-cb1f-4ee7-9a99-e84c2d071066" };
    var rows = connection.Execute(
        procedure,
        pars,
        commandType: CommandType.StoredProcedure);

    Console.WriteLine($"{rows} linhas afetadas");
}

static void ExecuteReadProcedure(SqlConnection connection)
{
    var procedure = "[spGetCourseByCategory]";
    var pars = new { CategoryId = "06d73e6b-315f-4cfc-b462-f643e1a50e97" };
    var couses = connection.Query(
        procedure,
        pars,
        commandType: CommandType.StoredProcedure);

    foreach (var item in couses)
    {
        Console.WriteLine(item.Title);
    }
}

static void ExetuteScalar(SqlConnection connection)
{
    var category = new Category();
    category.Id = Guid.NewGuid();
    category.Title = "BackEnd 2040";
    category.Url = "backend";
    category.Description = "Categoria Back End 2022";
    category.Summary = "Curso de backEnd Completo de 0 ao pro";
    category.Featured = true;

    var sqlInsert = @"INSERT INTO
            [Category]
    OUTPUT inserted.[Title]
    Values(
    @Id,
    @Title,
    @Url,
    @Description,
    @Order,
    @Summary,
    @Featured)";

    var id = connection.ExecuteScalar<string>(sqlInsert, new
    {
        category.Id,
        category.Title,
        category.Url,
        category.Description,
        category.Order,
        category.Summary,
        category.Featured
    });

    Console.WriteLine($"A categoria inserida foi: {id}");
}

static void ReadView(SqlConnection connection)
{
    var sql = "SELECT * FROM [vwCourses]";

    var courses = connection.Query(sql);
    foreach (var item in courses)
    {
        Console.WriteLine($"{item.Id} - {item.Title}");
    }
}


static void OneToOne(SqlConnection connection)
{
    var sql = @"SELECT 
                    * 
                FROM 
                    [CareerItem] 
                INNER JOIN 
                    [Course] ON [CareerItem].[CourseId] = [Course].[Id]";

    var items = connection.Query<CareerItem, Course, CareerItem>(sql,
        (careerItem, course) =>
        {
            careerItem.Course = course;
            return careerItem;
        }, splitOn: "Id");

    foreach (var item in items)
    {
        Console.WriteLine($"{item.Title} - Curso: {item.Course.Title}");
    }
}

static void OneToMany(SqlConnection connection)
{
    var sql = @"
                SELECT 
                    [Career].[Id],
                    [Career].[Title],
                    [CareerItem].[CareerId],
                    [CareerItem].[Title]
                FROM 
                    [Career] 
                INNER JOIN 
                    [CareerItem] ON [CareerItem].[CareerId] = [Career].[Id]
                ORDER BY
                    [Career].[Title]";

    var careers = new List<Career>();
    var items = connection.Query<Career, CareerItem, Career>(
        sql,
        (career, item) =>
        {
            var car = careers.Where(x => x.Id == career.Id).FirstOrDefault();
            if (car == null)
            {
                car = career;
                car.Items.Add(item);
                careers.Add(car);
            }
            else
            {
                car.Items.Add(item);
            }

            return career;
        }, splitOn: "CareerId");

    foreach (var career in careers)
    {
        Console.WriteLine($"{career.Title}");
        foreach (var item in career.Items)
        {
            Console.WriteLine($" - {item.Title}");
        }
    }

}

static void QueryMutiple(SqlConnection connection)
{
    var query = "SELECT * FROM [Category]; SELECT * FROM [Course]";

    using (var multi = connection.QueryMultiple(query))
    {
        var categories = multi.Read<Category>();
        var courses = multi.Read<Course>();

        foreach (var item in categories)
        {
            Console.WriteLine(item.Title);
        }

        foreach (var item in courses)
        {
            Console.WriteLine(item.Title);
        }
    }
}

static void SelectIn(SqlConnection connection)
{
    var query = @"select * from Career where [Id] IN @Id";

    var items = connection.Query<Career>(query, new
    {
        Id = new[]
        {
           "4327ac7e-963b-4893-9f31-9a3b28a4e72b",
           "e6730d1c-6870-4df3-ae68-438624e04c72"
        }
    });

    foreach (var item in items)
    {
        Console.WriteLine(item.Title);
    }

}

static void Like(SqlConnection connection, string term)
{
    var query = @"SELECT * FROM [Course] WHERE [Title] LIKE @exp";

    var items = connection.Query<Course>(query, new
    {
        exp = $"%{term}%"
    });

    foreach (var item in items)
    {
        Console.WriteLine(item.Title);
    }
}

static void Transaction(SqlConnection connection)
{
    var category = new Category();
    category.Id = Guid.NewGuid();
    category.Title = "Minha categoria que não";
    category.Url = "amazon";
    category.Description = "Categoria destinada a serviços do AWS";
    category.Order = 8;
    category.Summary = "AWS Cloud";
    category.Featured = false;

    var insertSql = @"INSERT INTO 
                    [Category] 
                VALUES(
                    @Id, 
                    @Title, 
                    @Url, 
                    @Summary, 
                    @Order, 
                    @Description, 
                    @Featured)";

    connection.Open();
    using (var transaction = connection.BeginTransaction())
    {
        var rows = connection.Execute(insertSql, new
        {
            category.Id,
            category.Title,
            category.Url,
            category.Summary,
            category.Order,
            category.Description,
            category.Featured
        }, transaction);

        transaction.Commit();
        // transaction.Rollback();

        Console.WriteLine($"{rows} linhas inseridas");
    }
}
