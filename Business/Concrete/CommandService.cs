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
using Core.Extension;
using Core.Utilities.FileWatcher.Abstract;

namespace Business.Concrete
{
    public class CommandService : ICommandService
    {
        private readonly IFileStreamReader _fileStreamReader;
        private readonly IOrderDataDal _orderDataDal;
        private readonly IProductDataDal _productDataDal;
        private readonly ICampaignDataDal _campaignDataDal;
        private readonly IFileWatcher _fileWatcher;
        private int Time;

        public CommandService(IFileStreamReader fileStreamReader, IOrderDataDal orderDataDal, IProductDataDal productDataDal, ICampaignDataDal campaignDataDal, IFileWatcher fileWatcher)
        {
            _fileStreamReader = fileStreamReader;
            _orderDataDal = orderDataDal;
            _productDataDal = productDataDal;
            _campaignDataDal = campaignDataDal;
            _fileWatcher = fileWatcher;
            Time = 0;
        }

        public void CreateCommandWatcher()
        {
            Console.Write("Please add/copy the command file to the directory path of the application exe to start the reading procedure: ");
            Console.Write("\n\n");
            var fw = _fileWatcher.CreateWatcher("txt", Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
            fw.InvokeOnChange += ReadFile; 
        }

        private void DefineCommand(string filePath)
        {            
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

        private void ReadFile(object sender, FileSystemEventArgs e)
        {
            DefineCommand(e.FullPath);
            _fileWatcher.Dispose();
            Environment.Exit(0);
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
                GetCampaignInfo(splittedCommand);
            else if (type == COMMAND_TYPE.INCREASETIME)
                IncreaseTime(splittedCommand);
            else
                GetProductInfo(splittedCommand);
        }

        #region Commands
        private void GetProductInfo(string[] splittedCommand)
        {
            var productCode = splittedCommand[1];
            var product = _productDataDal.Get(x => x.ProductCode == productCode);
            if (product != null)
            {
                if (_campaignDataDal.Any(x => x.ProductCode == productCode && x.Status == STATUS.ACTIVE))
                {
                    var campaign = _campaignDataDal.Get(x => x.ProductCode == productCode && x.Status == STATUS.ACTIVE);
                    var productPrice = CalculateDiscountedPrice(product.Price, campaign.PriceManipulationLimit);
                    Console.WriteLine($"Product {product.ProductCode} info; price {productPrice}, stock {product.Stock}");
                }
                else
                    Console.WriteLine($"Product {product.ProductCode} info; price {product.Price}, stock {product.Stock}");
            }

        }
        private void IncreaseTime(string[] splittedCommand)
        {
            Time += int.Parse(splittedCommand[1]);
            Console.WriteLine($"Time is {Time.ConvertToTime()}");
            var activeToPassiveCampaigns = _campaignDataDal.GetList(x => x.Status == STATUS.ACTIVE && x.Duration < Time);
            activeToPassiveCampaigns.ForEach(campaign => campaign.Status = STATUS.PASSIVE);
            _campaignDataDal.BulkUpdate(x => x.Status == STATUS.ACTIVE && x.Duration < Time, activeToPassiveCampaigns);
        }
        private void GetCampaignInfo(string[] splittedCommand)
        {
            var campaignName = splittedCommand[1];
            var campaign = _campaignDataDal.Get(x => string.Equals(x.Name, campaignName));
            if (campaign != null)
                Console.WriteLine($"Campaign {campaign.Name} info; Status {campaign.Status}, Target Sales {campaign.TargetSalesCount}, Total Sales {campaign.TotalSales}, " +
                    $"Turnover {campaign.Turnover}, Average Item Price {campaign.AverageItemPrice}");
            else
                Console.WriteLine("Campaign not found!");
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
                var product = _productDataDal.Get(x => x.ProductCode == order.ProductCode);
                if (product.Stock < 1)
                {
                    Console.WriteLine("Stock is not enough to issue the order!");
                    return;
                }
                _orderDataDal.Insert(order);
                Console.WriteLine($"Order created; product {order.ProductCode}, quantity {order.Quantity}");
                var campaign = _campaignDataDal.Get(x => x.ProductCode == order.ProductCode && x.Status == STATUS.ACTIVE);
                if (campaign != null)
                {
                    var productPrice = CalculateDiscountedPrice(product.Price, campaign.PriceManipulationLimit);
                    campaign.Turnover += (productPrice * order.Quantity);
                    campaign.AverageItemPrice = ((campaign.AverageItemPrice * campaign.TotalSales) + (productPrice * order.Quantity)) / (campaign.TotalSales + order.Quantity);
                    campaign.TotalSales += order.Quantity;
                    _campaignDataDal.Update(x => x.ProductCode == order.ProductCode && x.Status == STATUS.ACTIVE, campaign);
                    product.Stock -= order.Quantity;
                    _productDataDal.Update(x => x.ProductCode == order.ProductCode, product);
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

        #endregion

        private COMMAND_TYPE ValidateCommand(string command)
        {
            var splittedCommand = command.Split(' ');
            if (string.Equals(splittedCommand[0], Command.CreateProduct.Split(' ')[0]))
            {
                if (splittedCommand.Length == Command.CreateProduct.Split(' ').Length && double.TryParse(splittedCommand[2], out _)
                    && int.TryParse(splittedCommand[3], out _))
                    return COMMAND_TYPE.CREATEPRODUCT;
            }
            else if (string.Equals(splittedCommand[0], Command.CreateCampaign.Split(' ')[0]))
            {
                if (splittedCommand.Length == Command.CreateCampaign.Split(' ').Length && int.TryParse(splittedCommand[3], out _)
                    && double.TryParse(splittedCommand[4], out _) && int.TryParse(splittedCommand[5], out _))
                    return COMMAND_TYPE.CREATECAMPAIGN;
            }
            else if (string.Equals(splittedCommand[0], Command.CreateOrder.Split(' ')[0]))
            {
                if (splittedCommand.Length == Command.CreateOrder.Split(' ').Length && int.TryParse(splittedCommand[2], out _))
                    return COMMAND_TYPE.CREATEORDER;
            }
            else if (string.Equals(splittedCommand[0], Command.GetCampaignInfo.Split(' ')[0]))
            {
                if (splittedCommand.Length == Command.GetCampaignInfo.Split(' ').Length)
                    return COMMAND_TYPE.GETCAMPAINGINFO;
            }
            else if (string.Equals(splittedCommand[0], Command.GetProductInfo.Split(' ')[0]))
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
                return (itemPrice * (100 - manipulationLimit) / 100);
        }
    }
}
