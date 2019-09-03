
# Extended Colors

![Version 1.0.0](https://img.shields.io/badge/Version-1.0.0-brightgreen.svg) ![Licence MIT](https://img.shields.io/badge/Licence-MIT-blue.svg) [![Build status](https://ci.appveyor.com/api/projects/status/m1dcmuhncgaxkpk9?svg=true)](https://ci.appveyor.com/project/FredEkstrand/enigmabinarycipher-6qww6)



![Project image](https://github.com/FredEkstrand/ImageFiles/raw/master/ColorChips/ProjectImage.PNG)

# Overview
The ExtendedColors project represents an ARGB (alpha, red, green, blue) color of an additional 3,000+ defined named colors not found in the .Net Framework color structure.

#### Features
The ExtendedColors structure have the following features:
* Can convert implicitly and explicitly to/from System.Drawing Color.
* You can create a custom color by using one of the FromArgb methods.
* Equality and Inequality tests on the colors.
* Null or empty color.
* Creates a Color structure from the specified name of a predefined color.

# Getting started
The souce code is written in C# and targeted for the .Net Framework 4.0 and later. Download the entire project and compile.

# Usage
Once you have compiled the project reference the dll in your Visual Studio project.
Then in your code file add the following to the collection of using statement.

```csharp
using Ekstrand.Drawing;
```
##### Some basic examples
Set form background color to MareaBaja.
```csharp
Form form = new Form();
form.BackColor = ExtendedColors.MareaBaja;
```
Set .Net color structure from extended colors structure.
```csharp
ExtendedColors exColors = ExtendedColors.BarnDoor;
Color aColor = exColors;
```
Set ExtendedColors structure from known ExtendedColors name.
```csharp
ExtendedColors exColor = ExtendedColors.FromKnownColor(KnownExtendedColors.PlymouthBlue);
```

# Code Documentation
MSDN-style code documentation can be found [here](http://fredekstrand.github.io/ColorChart).

# History
 1.0.0 Initial release into the wild.

# Contributing

If you'd like to contribute, please fork the repository and use a feature
branch. Pull requests are always welcome.

# Contact
Fred Ekstrand
email: fredekstrandgithub@gmail.com

# Licensing

The code in this project is licensed under MIT license.
