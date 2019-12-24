using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using MoleSql;

namespace MoleSqlTests
{
    class Configuration
    {
        public string Category { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        /// <inheritdoc />
        public override string ToString() => $"{Category ?? "<null>"} {Name ?? "<null>"} {Value ?? "<null>"} ";
    }

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

                string[] categories = { "License", "LDAP" };
                var query = context.Configuration.Select(c => new { Cat = c.Category, Misc = new { c.Name, c.Value } })
                                   .Where(c => c.Cat == categories[1]).Select(x => x.Misc.Value);
                //var query = context.ExecuteQuery<Configuration>("SELECT * FROM Configuration WHERE Category = {0}", categories[0]);
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
