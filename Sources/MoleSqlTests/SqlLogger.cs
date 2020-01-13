/*
 * (C)2020 by René Vogt
 *
 * Published under MIT license as described in the LICENSE.md file.
 *
 */
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MoleSqlTests
{
    [ExcludeFromCodeCoverage]
    sealed class SqlLogger : TextWriter
    {
        readonly TestContext testContext;
        readonly StringWriter buffer;
        
        public override Encoding Encoding => Encoding.Default;

        public SqlLogger(TestContext testContext, StringBuilder buffer)
        {
            this.testContext = testContext;
            this.buffer = buffer != null ? new StringWriter(buffer) : null;
        }
        public override void WriteLine(string value)
        {
            if (Environment.UserInteractive)
                Console.WriteLine(value);
            testContext?.WriteLine(value);
            buffer?.WriteLine(value);
            base.WriteLine(value);
        }
    }
}
