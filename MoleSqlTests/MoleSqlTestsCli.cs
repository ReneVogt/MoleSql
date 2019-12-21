using System;
using System.Data.SqlClient;
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
    }
    [ExcludeFromCodeCoverage]
    static class MoleSqlTestsCli
    {
        const string CONNECTIONSTRING = "Data Source=sql12dev01;Initial Catalog=RVo_PrinTaurus;Integrated Security=true";
        //const string CONNECTIONSTRING = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Flow;Integrated Security=True;";

        static void Main()
        {
            try
            {
                using var connection = new SqlConnection(CONNECTIONSTRING);
                using var context = new MoleSqlDataContext(connection) {Log = Console.Out};
                Console.WriteLine(string.Join(Environment.NewLine,
                                              context.GetTable<Configuration>().AsEnumerable().Select(c => $"{c.Category} {c.Name} {c.Value}")));
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
