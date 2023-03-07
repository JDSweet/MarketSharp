public struct Order
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
