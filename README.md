# MarketSharp

The goal of this project is to create a C# general purpose simulation of market mechanics that is lightweight, extensible, flexible, and simple/unbloated enough to be useful in multiple different contexts (though this system was primarily orginally developed to serve as the backbone of an economy system in a strategy game).

The goal isn't necessarily absolute accuracy (such a system might be unnecessarily complex/bloated/inextensible - though input/suggestions from economists is always welcome, the existence of different unverifiable, contradictory, and competing theories of value and other theories on how economies work makes complete accuracy to reality impossible), but rather something that can be converted/modified to serve in many different contexts (though, as mentioned earlier, I'm mostly interested in creating a market economy in a video game that is generally compelling and interesting).

Note: This project is currently in a mostly usable state. Prices fluctuate in a somewhat rational way based on supply/demand curves and an inflation modifier (that you can modify to manually simulate the effects of natural disasters and various government and private entities on the cost of goods and services) on a market-by-market basis.

Tests are included in the Market class, to demonstrate how the class should be used and the set-up steps required in order for it to work as intended.

The project is licensed under the MIT license.
