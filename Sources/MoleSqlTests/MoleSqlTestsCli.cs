/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 * This file is used as command line testing interface for MoleSql.
 *
 */
using System;
using System.Configuration;
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
        public FlowContext() : base(ConfigurationManager.ConnectionStrings["TestDatabase"].ConnectionString)
        {
        }

        public MoleQuery<USR_Subjects> Subjects => GetTable<USR_Subjects>();
        public MoleQuery<USR_Rights> Rights => GetTable<USR_Rights>();
    }

    [ExcludeFromCodeCoverage]
    static class MoleSqlTestsCli
    {
        static void Main()
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", AppDomain.CurrentDomain.BaseDirectory);
            using var context = new FlowContext() { Log = Console.Out };
            try
            {
                //var query = from subject in context.Subjects
                //            where subject.Name == "admin"
                //            from right in context.Rights
                //            where right.SubjectId == subject.Id
                //            select new {subject.Name, right.Type};

                //var query = context.Subjects.Where(subject => subject.Name == "admin")
                //                   .SelectMany(subject => context.Rights.Where(right => right.SubjectId == subject.Id),
                //                               (subject, right) => new { subject.Name, right.Type });

                //var query = from subject in context.Subjects
                //            join right in context.Rights
                //                on subject.Id equals right.SubjectId
                //            let name = subject.Name
                //            orderby subject.Id
                //            where subject.Name != "Paul"
                //            where right.Type < 100000
                //            select new { subject.Name, subject.Id }
                //            into x
                //            where x.Name != "Max"
                //            select x;

                //var query = from right in context.Rights
                //            group right by right.Type
                //            into g
                //            select new
                //            {
                //                Type = g.Key,
                //                Total = g.Sum(r => r.Id)//,
                //                //Min = g.Min(r => r.Id),
                //                //Max = g.Max(r => r.Id),
                //                //Avg = g.Average(r => r.Id)
                //            };

                var query = context
                            .Subjects.Join(context.Rights, subject => subject.Id, right => right.SubjectId,
                                            (subject, right) => new { subject.Name, right.Type })
                            .GroupBy(x => x.Name)
                            .Select(g => new { Name = g.Key, Total = g.Sum(a => a.Type) });

                Console.WriteLine(string.Join(Environment.NewLine, query.AsEnumerable()));

                //Console.WriteLine(context.Rights.Max(right => right.Id));
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
