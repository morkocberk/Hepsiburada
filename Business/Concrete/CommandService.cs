using Business.Abstract;
using Core.Constant;
using Core.Utilities.FileReader.Abstract;
using Data.Abstract;
using Data.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Business.Concrete
{
    public class CommandService : ICommandService
    {
        private readonly IFileStreamReader _fileStreamReader;
        private readonly IOrderDataDal _orderDataDal;
        private readonly IProductDataDal _productDataDal;
        private readonly ICampaignDataDal _campaignDataDal;
        private int Time;

        public CommandService(IFileStreamReader fileStreamReader, IOrderDataDal orderDataDal, IProductDataDal productDataDal, ICampaignDataDal campaignDataDal)
        {
            _fileStreamReader = fileStreamReader;
            _orderDataDal = orderDataDal;
            _productDataDal = productDataDal;
            _campaignDataDal = campaignDataDal;
            Time = 0;
        }

        public void DefineCommand()
        {
            Console.Write("Please indicate the file name located in the running exe path to start the reading procedure: ");
            var fileName = Console.ReadLine();
            var filePath = new StringBuilder(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)).Append("\\").Append(fileName).ToString();
            var commands = _fileStreamReader.ReadAllLines(filePath);
            COMMAND_TYPE commandType;
            foreach (var command in commands)
            {
                commandType = ValidateCommand(command);
                if (commandType != COMMAND_TYPE.UNDEFINED)
                    ExecuteCommand(command, commandType);
                else
                {
                    Console.WriteLine("Undefined command detected!");
                    break;
                }
            }
        }
        private void ExecuteCommand(string command, COMMAND_TYPE type)
        {
            var splittedCommand = command.Split(' ');
            if (type == COMMAND_TYPE.CREATEPRODUCT)
                CreateProduct(splittedCommand);
            else if (type == COMMAND_TYPE.CREATECAMPAIGN)
                CreateCampaign(splittedCommand);
            else if (type == COMMAND_TYPE.CREATEORDER) //campigndeki bilgileri güncelle!
                CreateOrder(splittedCommand);
            else if (type == COMMAND_TYPE.GETCAMPAINGINFO)
            {
                var campaignName = splittedCommand[1];
                //var campaign = Campaigns.SingleOrDefault(x => string.Equals(x.Name, campaignName));
                //if (campaign != null)
                //    Console.WriteLine($"Campaign {campaign.Name} info; Status {campaign.Status}, Target Sales {campaign.TargetSalesCount}, Total Sales {campaign.TotalSales}, " +
                //        $"Turnover {campaign.Turnover}, Average Item Price {campaign.AverageItemPrice}");
                //else
                //    Console.WriteLine("Campaign not found!");
            }
        }
        private void CreateOrder(string[] splittedCommand)
        {
            var order = new Order
            {
                ProductCode = splittedCommand[1],
                Quantity = int.Parse(splittedCommand[2])
            };
            if (_productDataDal.Any(x => x.ProductCode == order.ProductCode))
            {
                _orderDataDal.Insert(order);
                Console.WriteLine($"Order created; product {order.ProductCode}, quantity {order.Quantity}");
                var campaign = _campaignDataDal.Get(x => x.ProductCode == order.ProductCode && x.Status == STATUS.ACTIVE);
                if (campaign != null)
                {
                    var product = _productDataDal.Get(x => x.ProductCode == order.ProductCode);
                    var productPrice = CalculateDiscountedPrice(product.Price, campaign.PriceManipulationLimit);
                    campaign.Turnover += productPrice;
                    campaign.AverageItemPrice = ((campaign.AverageItemPrice * campaign.TotalSales) + productPrice) / (campaign.TotalSales + 1);
                    campaign.TotalSales += 1;
                }
            }
            else
                Console.WriteLine("No product found for the ProductCode given.");
        }
        private void CreateCampaign(string[] splittedCommand)
        {
            var campaign = new Campaign
            {
                Name = splittedCommand[1],
                ProductCode = splittedCommand[2],
                Duration = int.Parse(splittedCommand[3]),
                PriceManipulationLimit = double.Parse(splittedCommand[4]),
                TargetSalesCount = int.Parse(splittedCommand[5])
            };
            if (_productDataDal.Any(x => x.ProductCode == campaign.ProductCode))
            {
                _campaignDataDal.Insert(campaign);
                Console.WriteLine($"Campaign created; name {campaign.Name}, product {campaign.ProductCode}, " +
                    $"duration {campaign.Duration}, limit {campaign.PriceManipulationLimit}, target sales count {campaign.TargetSalesCount}");
            }
            else
                Console.WriteLine("No product found for the ProductCode given.");
        }
        private void CreateProduct(string[] splittedCommand)
        {
            var product = new Product
            {
                ProductCode = splittedCommand[1],
                Price = double.Parse(splittedCommand[2]),
                Stock = int.Parse(splittedCommand[3])
            };
            _productDataDal.Insert(product);
            Console.WriteLine($"Product created; code {product.ProductCode}, price {product.Price}, stock {product.Stock}");
        }
        private COMMAND_TYPE ValidateCommand(string command)
        {
            var splittedCommand = command.Split(' ');
            if (string.Equals(splittedCommand[0], Command.CreateProduct))
            {
                if (splittedCommand.Length == Command.CreateProduct.Split(' ').Length && double.TryParse(splittedCommand[2], out _)
                    && int.TryParse(splittedCommand[3], out _))
                    return COMMAND_TYPE.CREATEPRODUCT;
            }
            else if (string.Equals(splittedCommand[0], Command.CreateCampaign))
            {
                if (splittedCommand.Length == Command.CreateCampaign.Split(' ').Length && int.TryParse(splittedCommand[3], out _)
                    && double.TryParse(splittedCommand[4], out _) && int.TryParse(splittedCommand[5], out _))
                    return COMMAND_TYPE.CREATECAMPAIGN;
            }
            else if (string.Equals(splittedCommand[0], Command.CreateOrder))
            {
                if (splittedCommand.Length == Command.CreateOrder.Split(' ').Length && int.TryParse(splittedCommand[2], out _))
                    return COMMAND_TYPE.CREATEORDER;
            }
            else if (string.Equals(splittedCommand[0], Command.GetCampaignInfo))
            {
                if (splittedCommand.Length == Command.GetCampaignInfo.Split(' ').Length)
                    return COMMAND_TYPE.GETCAMPAINGINFO;
            }
            else if (string.Equals(splittedCommand[0], Command.GetProductInfo))
            {
                if (splittedCommand.Length == Command.GetProductInfo.Split(' ').Length)
                    return COMMAND_TYPE.GETPRODUCTINFO;
            }
            else
            {
                if (splittedCommand.Length == Command.IncreaseTime.Split(' ').Length && int.TryParse(splittedCommand[1], out _))
                    return COMMAND_TYPE.INCREASETIME;
            }
            return COMMAND_TYPE.UNDEFINED;
        }

        private double CalculateDiscountedPrice(double itemPrice, double manipulationLimit)
        {
            if (itemPrice - (Time * 5) >= (itemPrice * (100 - manipulationLimit) / 100))
                return (itemPrice - (Time * 5));
            else
                return (itemPrice * (100 - manipulationLimit) / 100)
        }
    }
}
