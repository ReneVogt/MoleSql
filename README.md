# MoleSQL
My Own Linq (er...?) SQL provider.

René Vogt, Dresden 2020/01/01

---

I'm finally trying to develop my own IQueryProvider/Mini-ORM to understand how all this really works. I started by fighting my way through the well known blog by Matt Warren and use his
[iqtoolkit](https://github.com/mattwar/iqtoolkit) code, and just implemented part IX, _"Removing redundant subqueries"_ (but did not yet fully understand it).    
Beside that I was able to implement query parameterization (to avoid SQL injection) and added array support to Jon Skeet's `SqlFormattableString` class.

With this project I hope to get a deeper understanding of how query providers and ORMs work under the hood. And I have a little bit of hope that I'll be able to create a provider
that supports my daily work somewhat better and more customized than Linq2Sql or EF do.

Since this code is **still in a creation phase and not tested at all**, I cannot recommend to use it or parts of it yet. However, constructive comments are already appreciated.

_ACKNOWLEDGMENT_: In its current state, about 95% of this code are borrowed from [Matt Warren](https://github.com/mattwar/iqtoolkit), and another 1% from [Jon Skeet](https://github.com/jskeet/DemoCode/blob/master/Abusing%20CSharp/Code/StringInterpolation/ParameterizedSql.cs).