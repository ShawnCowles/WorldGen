# WorldGen #

WorldGen aims to be a multipurpose world generator for use in other projects. It generates worlds based on a grid of descrete cells. Each of which can have height, temperature, rainfall, biome or other information.

The included generators handle several types of physical world generation, including height, rainfall (through several different models), temperature, river simulation, and mapping height/temperature/rainfall to biomes. They heavily leverage interfaces and generics, letting them operate on user specific models, as long as they implement the needed interfaces.

The Universal Name Markup Parser is a library for generating names / text for use in procedural generation. It operates on an input pattern, parsing tags to substitute fragments into the final output.

--------------------------------------------

Code licensed under the MIT License. See LICENSE.txt for details.