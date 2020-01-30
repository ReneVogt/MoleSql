# MoleSQL
My Own Linq (er...?) SQL provider.

René Vogt, Dresden 2020/01/30

---

I'm finally trying to develop my own IQueryProvider/Mini-ORM to understand how all this really works. With this project I hope to get a deeper understanding of how query providers and ORMs work under the hood. And I have a little bit of hope that I'll be able to create a provider
that supports my daily work somewhat better and more customized than Linq2Sql or EF do.

I started by fighting my way through [Matt Warren's tutorial][1] and re-implemented it up to part X, _"GroupBy and Aggregates"_.  
Since then, I added custom mapping attributes, query parameterization and support for asynchronous queries.

The next steps will be to fix all those little problems in the current implementation (nested queries, nested nested queries, strange aggregations etc, see the failing unit-tests for details).  
Then I'll try to add more operators (like distinct, first, last, take, skip, any, contains etc.), date/time and string support, and later hopefully start to think about crud mechanics.

[1]: https://blogs.msdn.microsoft.com/mattwar/2008/11/18/linq-building-an-iqueryable-provider-series/
[2]: https://github.com/mattwar/iqtoolkit
