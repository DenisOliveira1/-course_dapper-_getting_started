using Dapper;
using DapperDemo.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DapperDemo.Repository
{
    public class CompanyRepositoryDapper : ICompanyRepositoryDapper
    {
        private readonly IDbConnection _database;

        public CompanyRepositoryDapper(IConfiguration config)
        {
            _database = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        }

        public Company Add(Company company)
        {
            var sql = @"INSERT INTO Companies (Name, Address, City, State, PostalCode)
                        VALUES(@Name, @Address, @City, @State, @PostalCode);
                        SELECT CAST(SCOPE_IDENTITY() as int);";
            // Já que o nome da propriedade e é o mesmo na anotação não precisa escrever eles
            // Se poderia passar somente o objeto também, desde que os nomes sejam iguais
            var id = _database.Query<int>(sql, new {
                company.Name,
                company.Address,
                company.City,
                company.State,
                company.PostalCode,
            }).Single();

            company.CompanyId = id;

            return company;
        }

        public Company Get(int id)
        {
            var sql = "SELECT * FROM Companies WHERE CompanyId = @Id";
            return _database.Query<Company>(sql, new { @Id = id}).Single();
        }

        public List<Company> GetAll()
        {
            var sql = "SELECT * FROM Companies";
            return _database.Query<Company>(sql).ToList();
        }

        public void Remove(int id)
        {
            var sql = "DELETE FROM Companies WHERE CompanyId = @Id";
            _database.Execute(sql, new { id});
        }

        public Company Update(Company company)
        {
            var sql = @"UPDATE Companies SET Name = @Name, Address = @Address,
                        City = @City, State = @State, PostalCode = @PostalCode
                        WHERE CompanyId = @CompanyId;";
            // Já que o nome da propriedade e é o mesmo na anotação não precisa escrever eles
            // Se poderia passar somente o objeto também, desde que os nomes sejam iguais
            // Execute é usado quando não se espera um retorno da query
            _database.Execute(sql, company);
            return company;
        }
    }
}
