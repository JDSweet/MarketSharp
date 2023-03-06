using System;
using System.Collections;
using System.Collections.Generic;

public class Market
{
    public static Random random = new Random();
    
    
    
    public static void Main(string[] args)
    {
        Market myMarket = new Market();
        
        List<string> tags = new List<string>();
        List<float> mins = new List<float>();
        List<float> maxes = new List<float>();
        List<float> basePrices = new List<float>();
        
        List<GoodData> goods = new List<GoodData>();
        string[] tagsList = {
            "wood", "steel", "food", "water", "recreation"
        };
        
        int iter = tagsList.Length;
        
        for(int i = 0; i < iter; i++)
        {
            tags.Add(tagsList[i]);
            mins.Add((float)(RandomDouble(0.01, 1.0)));
            maxes.Add((float)(RandomDouble(mins[i], 10)));
            basePrices.Add((float)(RandomDouble(0.01, 10)));
            
            GoodData data = new GoodData();
            data.tag = tags[i];
            data.min = mins[i];
            data.max = maxes[i];
            data.basePrice = basePrices[i];
            data.price = basePrices[i];
            goods.Add(data);
            myMarket.RegisterGood(tags[i], data);
            //Console.WriteLine(basePrices[i]);
        }
        for(int i = 0; i < iter; i++)
        {
            Console.WriteLine("Tick " + i);
            
            float newInflationRate = (float)RandomDouble(-1.0, 1.0);
            myMarket.inflation += newInflationRate;
            Console.WriteLine("Inflation Rate: " + myMarket.inflation);
            
            UpdateSellOrders(myMarket, goods);
            UpdateBuyOrders(myMarket, goods);
            myMarket.Update();
        }
    }
    
    static int minSellAmount = 1;
    static int minBuyAmount = 1;
    static int maxSellAmount = 1000;
    static int maxBuyAmount = 1000;
    
    public static void UpdateSellOrders(Market market, List<GoodData> goods)
    {
        foreach(GoodData good in goods)
        {
            float sellAmnt = (float)RandomDouble(minSellAmount, maxSellAmount);
            Console.WriteLine("Selling " + sellAmnt + " of " + good.tag);
            market.SubmitSellOrder(sellAmnt, good.tag);
        }
    }
    
    public static void UpdateBuyOrders(Market market, List<GoodData> goods)
    {
        foreach(GoodData good in goods)
        {
            float buyAmnt = (float)RandomDouble(minBuyAmount, maxBuyAmount);
            Console.WriteLine("Buying " + buyAmnt + " of " + good.tag);
            market.SubmitBuyOrder(buyAmnt, good.tag);
        }
    }
    
    public static double RandomDouble(double min, double max)
    {
        return random.NextDouble() * (max - min) + min;
    }
    
    //GoodData[] goods;
    
    Dictionary<string, GoodData> goods;
    Dictionary<int, MarketAgent> agents;
    List<Order> orders;
    
    bool refreshPrices = true;
    bool refreshOrders = true;
    
    public float inflation
    {
        get; set;
    }
    
    
    //float[] prices;
    //float[] quantities;
    //float[] buy_orders;
    //float[] sell_orders;
    
    public Market()
    {
        orders = new List<Order>();
        goods = new Dictionary<string, GoodData>();
        agents = new Dictionary<int, MarketAgent>();
    }
    
    //This returns the number of the given good that is actually successfully purchased, or -1.0f if no goods are purchased.
    public float SubmitBuyOrder(float amount, string goodKey)
    {
        if(goods.ContainsKey(goodKey))
        {
            float buys = goods[goodKey].buys;
            buys += amount;
            goods[goodKey].buys = amount;
            
            float newSells = goods[goodKey].sells - amount;
            
            if(newSells >= 0)
            {
                goods[goodKey].sells = newSells;
                return goods[goodKey].sells;
            }
            else //If newSells is a negative number, then sells - newSells = the number of elements available to actually buy.
            {
                goods[goodKey].sells = goods[goodKey].sells - (goods[goodKey].sells - newSells);
                return goods[goodKey].sells;
            }
            //return -1.0f;
        }
        Console.WriteLine("Good " + goodKey + " is out of bounds and does not exist. Buy Order cancelled.");
        return -1.0f;
    }
    
    public void Refresh()
    {
        refreshPrices = true;
        refreshOrders = true;
    }
    
    public float SubmitSellOrder(float amount, string goodKey)
    {
        if(goods.ContainsKey(goodKey))
        {
            float buys = goods[goodKey].buys;
            buys += amount;
            goods[goodKey].buys = amount;
            Refresh();
            
            float newBuys = goods[goodKey].buys - amount;
            
            if(newBuys >= 0)
            {
                goods[goodKey].buys = newBuys;
                return goods[goodKey].buys;
            }
            else //If newSells is a negative number, then sells - newSells = the number of elements available to actually buy.
            {
                goods[goodKey].buys = goods[goodKey].buys - (goods[goodKey].buys - newBuys);
                return goods[goodKey].buys;
            }
            //return -1.0f;
        }
        
        Console.WriteLine("Good " + goodKey + " is out of bounds and does not exist. Sell Order cancelled.");
        return -1.0f;
    }
    
    public void RegisterGood(string goodKey, float basePrice, float max, float min)
    {
        GoodData data = new GoodData();
        data.max = max;
        data.min = min;
        data.tag = goodKey;
        data.basePrice = basePrice;
        data.price = basePrice;
        RegisterGood(goodKey, data);
    }
    
    public void RegisterGood(string goodKey, GoodData data)
    {
        if(goods.ContainsKey(goodKey))
        {
            Console.WriteLine("Good " + goodKey + " already exists.");
            return;
        }
        goods.Add(goodKey, data);
    }
    
    public void Update()
    {
        if(refreshPrices) //Update prices in the market based on evaluated order data.
            UpdatePrices();
        if(refreshOrders) //Add and remove orders as necessary. This also re-caches the total number of ordered items for each good type.
            UpdateOrders();
    }
    
    public void UpdatePrices()
    {
        foreach(KeyValuePair<string, GoodData> goodData in goods)
        {
            //variance is the real inflation of the market ("inflation" is just an inflation modifier that can be manipulated by currency injection and other methods to get different market behavior).
            float variance = (goodData.Value.buys/goodData.Value.sells) * (float)Math.Clamp((goodData.Value.buys/goodData.Value.sells) * (1 + inflation), (double)goodData.Value.min, (double)goodData.Value.max);
            //variance = Math.Abs(variance);
            variance = 1 + variance;
            
            goodData.Value.price = goodData.Value.basePrice * variance;
            Console.WriteLine("New price calculated for good " + goodData.Value.tag + " is $" + goodData.Value.price + "\n");
            Console.WriteLine("Variance is " + variance);
        }
        refreshPrices = false;
    }
    
    //This adds all orders to the market's cached supply/demand curve.
    public void UpdateOrders()
    {
        foreach(Order order in orders)
        {
            if(goods.ContainsKey(order.goodKey))
            {
                if(order.orderType == OrderType.BUY)
                    goods[order.goodKey].buys += order.amount;
                else
                    goods[order.goodKey].sells += order.amount;
            }
            else
            {
                Console.WriteLine("No good of type " + order.goodKey + " exists, erasing relevant order...");
            }
        }
        refreshOrders = false;
        orders.Clear();
    }
}

struct Order
{
    public OrderType orderType;
    public int marketID;
    public int agentID;
    public string goodKey;
    public float amount;
    
    public Order(int marketID, int agentID, OrderType orderType, float amount, string goodKey)
    {
        this.orderType = orderType;
        this.marketID = marketID;
        this.agentID = agentID;
        this.amount = amount;
        this.goodKey = goodKey;
    }
}

enum OrderType
{
    BUY = 1,
    SELL = 2
}

interface MarketAgent
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

public class GoodData
{
    public string tag;
    public float basePrice;
    public float max, min;
    public float price;
    public float buys;
    public float sells;
}