# InternalEngineerCalculator

**Simple AST-tree math console calculator that supports functions and variables**

## Functional

Base math operators support : +, -, \*, /, ^, % |x|, x!

Base math variables : π, e, 𝜏

Base math functions : sin, cos, tg, ctg, log, exp, pow, etc.

Create variables : my_var = 1488 - 52 / 2

Create custom functions : f(x, y) = x^2 + |sin(y)| - 3!


## How to build ?

1. Download .NET SDK from [official source](https://dotnet.microsoft.com/en-us/download).
2. Clone project from GitHub to local machine :
``` bash
git clone https://github.com/gritsruslan/InternalEngineerCalculator.git
```
3. Build the project in Release mode :
```bash
dotnet build --configuration Release
```
4. Run program :
```bash
dotnet run
```
