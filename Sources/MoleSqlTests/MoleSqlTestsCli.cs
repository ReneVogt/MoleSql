using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using MoleSql;

namespace MoleSqlTests
{
    [ExcludeFromCodeCoverage]
    class Configuration
    {
        public string Category { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        /// <inheritdoc />
        public override string ToString() => $"{Category ?? "<null>"} {Name ?? "<null>"} {Value ?? "<null>"} ";
    }

    [ExcludeFromCodeCoverage]
    public class PrinTaurusContext : MoleSqlDataContext
    {
        const string CONNECTIONSTRING = "Data Source=sql12dev01;Initial Catalog=RVo_PrinTaurus;Integrated Security=true";
        public PrinTaurusContext() : base(CONNECTIONSTRING)
        { }

        internal MoleQuery<Configuration> Configuration => GetTable<Configuration>();

    }
    //const string CONNECTIONSTRING = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Flow;Integrated Security=True;";
    [ExcludeFromCodeCoverage]
    static class MoleSqlTestsCli
    {
        static void Main()
        {
            try
            {
                using var context = new PrinTaurusContext { Log = Console.Out };

                List<string> categories = new List<string> { "License", "LDAP" };
                //var query = context.Configuration.Select(c => new { c.Category, c.Name })
                //                   .Where(c => c.Category == categories[0])
                //                   .Select(c => new { c.Name, Karl = 5 });
                var query = context.Configuration.Select(c => new {Cat = c.Category});
                //var query = context.ExecuteQuery<Configuration>($"SELECT * FROM Configuration WHERE Category IN {categories}");
                //var query = context.ExecuteQuery($"SELECT * FROM Configuration WHERE Category in {categories}");
                Console.WriteLine(string.Join(Environment.NewLine,
                                              query.AsEnumerable()));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            Console.WriteLine();
            Console.WriteLine("Done.");
            Console.ReadLine();
        }
    }
}
