using System;
using System.Collections;
using System.Collections.Generic;

public interface MarketAgent
{
    //Returns this agent's demand for the specified good.
    float CalculateDemand(string goodKey);
    
    public int MarketID //Gets the ID of the market.
    {
        get;
    }
    
    public int ID //Gets the ID of this agent in the market.
    {
        get;
    }
}
