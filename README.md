![Project type](https://github.com/FredEkstrand/ImageFiles/raw/master/CodeIcon.png ) 

![Version 1.0.0](https://img.shields.io/badge/Version-1.0.0-brightgreen.svg) ![Licence MIT](https://img.shields.io/badge/Licence-MIT-blue.svg) [![codecov](https://codecov.io/gh/FredEkstrand/ColorChart/branch/master/graph/badge.svg)](https://codecov.io/gh/FredEkstrand/ColorChart)

# Overview
The ExtendedColors project represents an ARGB (alpha, red, green, blue) color of an additional 3,000+ defined named colors not found in the .Net Framework color structure.

#### Features
The ExtendedColors structure have the following features:
* Can convert implicitly and explicitly to/from System.Drawing Color.
* You can create a custom color by using one of the FromArgb methods.
* Equality and Inequality tests on the colors.
* Null or empty color.
* Creates a Color structure from the specified name of a predefined color.

## Download
The souce code and provided DLL is written in C# and targeted for the .Net Framework 4.0 and later.
You can download the DLL [here](#).

## Getting started
Once downloaded add a reference to the dll in your Visual Studio project.
Then in your code file add the following to the collection of using statement.
```csharp
using Ekstrand.Drawing;
```
## Code Example
```csharp
Form form = new Form();
form.BackColor = ExtendedColors.MareaBaja;
```
### Documentation
Class documentation can be found [here](#). 

## History
 1.0.0 Initial release into the wild.

## Contributing

If you'd like to contribute, please fork the repository and use a feature
branch. Pull requests are always welcome.

## Contact
Fred Ekstrand 
email: fredekstrandgithub@gmail.com
## Licensing

The code in this project is licensed under MIT license.
