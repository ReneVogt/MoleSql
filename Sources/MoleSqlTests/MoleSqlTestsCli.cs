/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 * This file is used as command line testing interface for MoleSql.
 *
 */
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using MoleSql;
// ReSharper disable AccessToDisposedClosure

namespace MoleSqlTests
{
    [ExcludeFromCodeCoverage]
    class USR_Subjects
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }

    [ExcludeFromCodeCoverage]
    class USR_Rights
    {
        public long Id { get; set; }
        public long SubjectId { get; set; }
        public int Type { get; set; }
    }

    [ExcludeFromCodeCoverage]
    class FlowContext : MoleSqlDataContext
    {
        const string CONNECTIONSTRING = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Flow;Integrated Security=True;MultipleActiveResultSets=True;";
        public FlowContext() : base(CONNECTIONSTRING) { }

        public MoleQuery<USR_Subjects> Subjects => GetTable<USR_Subjects>();
        public MoleQuery<USR_Rights> Rights => GetTable<USR_Rights>();
    }

    [ExcludeFromCodeCoverage]
    static class MoleSqlTestsCli
    {
        static void Main()
        {
            try
            {
                using var context = new FlowContext {Log = Console.Out};
                //var query = from subject in context.Subjects
                //            where subject.Name == "admin"
                //            from right in context.Rights
                //            where right.SubjectId == subject.Id
                //            select new {subject.Name, right.Type};

                var query = context.Subjects.Where(subject => subject.Name == "admin")
                                   .SelectMany(subject => context.Rights.Where(right => right.SubjectId == subject.Id),
                                               (subject, right) => new {subject.Name, right.Type});

                Console.WriteLine(string.Join(Environment.NewLine, query.AsEnumerable()));
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
