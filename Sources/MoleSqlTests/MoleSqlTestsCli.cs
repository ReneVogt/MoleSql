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
using System.Threading.Tasks;
using MoleSqlTests.AsyncTests;

// ReSharper disable AccessToDisposedClosure

namespace MoleSqlTests
{
    [ExcludeFromCodeCoverage]
    static class MoleSqlTestsCli
    {
        static async Task Main()
        {
            try
            {
                await MoleSqlTestContext.RunTest<ExecuteAsyncTests>(nameof(ExecuteAsyncTests.ToListAsync_BasicTest));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            Console.Write("Done");
            Console.ReadLine();
        }
    }
}
