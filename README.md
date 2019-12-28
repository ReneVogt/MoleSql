# MoleSQL
My Own Linq (er...?) SQL provider.

René Vogt, Dresden 2019/12/28

---

**THIS PROJECT IS STILL IN ITS CREATION PHASE**  

I'm finally trying to develop my own IQueryProvider/Mini-ORM to understand how all this really works. I started by reading the well known blog by Matt Warren and use his
[iqtoolkit](https://github.com/mattwar/iqtoolkit) code.  
Currently I'm trying to understand what happened in part V of that blog, and although I think I understood the basic concept, I don't seem to get my head around the details
enough to explain (to myself and others, in source code comments or [documentation](Documentation/index.md)) how the translation with all its column bindings and weird projections
etc. works in detail.

With this project I hope to get a deeper understanding of how query providers and ORMs work under the hood. And I have a little bit of hope that I'll be able to create a provider
that supports my daily work somewhat better and more customized than Linq2Sql or EF do.

Since this code is still in a creation phase and not tested at all, I cannot recommend to use it or parts of it yet. However, constructive comments are already appreciated.

_ACKNOWLEDGMENT_: In its current state, about 95% of this code are borrowed from [Matt Warren](https://github.com/mattwar/iqtoolkit), and another 1% from [Jon Skeet](https://github.com/jskeet/DemoCode/blob/master/Abusing%20CSharp/Code/StringInterpolation/ParameterizedSql.cs).