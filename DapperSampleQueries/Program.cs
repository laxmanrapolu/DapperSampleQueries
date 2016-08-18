using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace DapperSampleQueries
{
    class Program
    {
        static string connecttionString =
               @"Data Source=localhost;Initial Catalog=DappaerSample;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False";
        static void Main(string[] args)
        {
            SimpleSelect();
            SelectWithParam();
            SelectWithParamList();
            MultiQuery();
            MultipleResultsSet();
            Insert();
            InsertMultiple();
        }

        private static void InsertMultiple()
        {
            var form1 = new Form() { Id = 1, Name = "test1", OwnerId = 1 };
            var form2 = new Form() { Id = 2, Name = "test2", OwnerId = 2 };
            var form = new List<Form>() {form2, form1};
            using (var con = new SqlConnection(connecttionString))
            {
                con.Open();
                con.Execute("Insert into Forms (Id, Name, OwnerId) VALUES (@Id, @Name, @OwnerId)", form);
            }
        }

        private static void Insert()
        {
            var form = new Form() {Id =1, Name = "test", OwnerId = 1};
            using (var con = new SqlConnection(connecttionString))
            {
                con.Open();
                con.Execute("Insert into Forms (Id, Name, OwnerId) VALUES (@Id, @Name, @OwnerId)",form );
            }
        }

        private static void MultipleResultsSet()
        {
            var sql = @"Select * from Form
                        Select * from Owner";
            using (var con = new SqlConnection(connecttionString))
            {
                con.Open();
                using (var resultset = con.QueryMultiple(sql))
                {
                    var form = resultset.Read<Form>();
                    var owner = resultset.Read<Owner>();
                }
            }
        }

        private static void MultiQuery()
        {
            var sql = "Select * from Owner JOIN Form ON Form.OwnerId = Owner.Id";
            using (var con = new SqlConnection(connecttionString))
            {
                con.Open();
                con.Query<Owner, FormDto, FormDto>(sql, (owner, form) =>
                {
                    form.Owner = owner;
                    return form;
                } );
            }
        }

        private static void SelectWithParamList()
        {
            var ids = new[] {1, 2, 3};
            using (var con = new SqlConnection(connecttionString))
            {
                con.Open();
                con.Query<Form>("Select * from FORMS WHERE ID IN @Ids", new { Ids = ids });
            }
        }

        private static void SelectWithParam()
        {
            using (var con = new SqlConnection(connecttionString))
            {
                con.Open();
                con.Query<Form>("Select * from FORMS WHERE ID = @id", new {id = 1});
            }
        }

        private static void SimpleSelect()
        {
            using (var con = new SqlConnection(connecttionString))
            {
                con.Open();
                con.Query<Form>("Select * from FORMS"); //retuns Ienumerable
            }
        }
    }

    public class Form
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int OwnerId { get; set; }
    }

    public class Owner
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class FormDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Owner Owner { get; set; }
    }
}
