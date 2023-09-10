# MiniXForms

Inside the [XForms spec](https://www.w3.org/community/xformsusers/wiki/XForms_2.0) is defined a recalculation engine similar to that of Excel, aka spreadsheet evaluation. However the spec is quite complex, therefore this little project.

Here we extract just the necessary bits to implement the recalculation engine:

* an extremely simple, uni-typed expression language is defined (see `Expr`)
* a type of "mutable variables" (see `InstanceNode`) is introduced
* some of those mutable variables are automatically calculated using a rule attached to them

The user of the library can modify the "variables" and run the recalculation step, which will recalculate just the necessary expressions, very much like a spreadsheet engine would.
