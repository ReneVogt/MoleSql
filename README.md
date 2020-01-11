# MoleSQL
My Own Linq (er...?) SQL provider.

René Vogt, Dresden 2020/01/11

---

I'm finally trying to develop my own IQueryProvider/Mini-ORM to understand how all this really works. With this project I hope to get a deeper understanding of how query providers and ORMs work under the hood. And I have a little bit of hope that I'll be able to create a provider
that supports my daily work somewhat better and more customized than Linq2Sql or EF do.

I started by fighting my way through [Matt Warren's tutorial][1] and just implemented part X, _"GroupBy and Aggregates"_.  
At this point, Matt started converting his sample code into his [iqtoolkit][2] project. And for me it's time to take a step back and look again:  
I certainly did not understand every part of the code very well. So I should take another look. And maybe I should start to write excessive source code comments to see which parts I'm able
to explain and where I should try to figure it out for myself before moving on.  
And I surely have to start writing unit tests before starting to extend the code on my own risk.

If that is finished, I plan to implement more operators (distinct, first, last, take, skip, any, contains etc.), add support for custom mapping (declaring table and column names in attributes)
and later hopefully start to think about crud mechanics.

Oh, and before you call me [Bobby Tables][3]: parameterization is already implemented. 

Since this code is **still in a creation phase and not tested at all**, I cannot recommend to use it or parts of it yet. However, constructive comments are already appreciated.

[1]: https://blogs.msdn.microsoft.com/mattwar/2008/11/18/linq-building-an-iqueryable-provider-series/
[2]: https://github.com/mattwar/iqtoolkit
[3]: https://bobby-tables.com/