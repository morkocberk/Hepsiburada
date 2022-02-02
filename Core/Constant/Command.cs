using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Constant
{
    public static class Command
    {
        public static readonly  string CreateProduct = "create_product PRODUCTCODE PRICE STOCK";
        public static readonly string GetProductInfo = "get_product_info PRODUCTCODE";
        public static readonly string CreateOrder = "create_order PRODUCTCODE QUANTITY";
        public static readonly string CreateCampaign = "create_campaign NAME PRODUCTCODE DURATION PMLIMIT TARGETSALESCOUNT";
        public static readonly string GetCampaignInfo = "get_campaign_info NAME";
        public static readonly string IncreaseTime = "increase_time HOUR";
    }

    public enum COMMAND_TYPE
    {
        CREATEPRODUCT,
        GETPRODUCTINFO,
        CREATEORDER,
        CREATECAMPAIGN,
        GETCAMPAINGINFO,
        INCREASETIME,
        UNDEFINED
    }
}
