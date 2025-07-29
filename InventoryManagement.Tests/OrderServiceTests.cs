using InventoryManagement.Data.Repositories.Interfaces;
using InventoryManagement.Models;
using InventoryManagement.Services;
using InventoryManagement.Services.Interfaces;
using InventoryManagement.Services.Utility;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace InventoryManagement.Tests
{
    public class OrderServiceTests
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IInventoryService _inventoryService;
        private readonly ILoggerService<OrderService> _logger;
        private readonly OrderService _service;

        public OrderServiceTests()
        {
            _orderRepository = Substitute.For<IOrderRepository>();
            _inventoryService = Substitute.For<IInventoryService>();
            _logger = Substitute.For<ILoggerService<OrderService>>();
            _service = new OrderService(_orderRepository, _inventoryService, _logger);
        }

        #region GetAllOrdersAsync

        [Fact]
        public async Task GetAllOrdersAsync_ShouldReturnOrders_WhenSuccessful()
        {
            var orders = new List<Order> { new Order { Id = 1 } };
            _orderRepository.GetAllAsync().Returns(orders);

            var result = await _service.GetAllOrdersAsync();

            Assert.Equal(orders, result);
        }

        [Fact]
        public async Task GetAllOrdersAsync_ShouldLogAndThrow_WhenException()
        {
            var ex = new Exception("DB Error");
            _orderRepository.GetAllAsync().Throws(ex);

            var thrown = await Assert.ThrowsAsync<Exception>(() => _service.GetAllOrdersAsync());
            Assert.Equal("Internal server Error", thrown.Message);
            _logger.Received(1).LogException("Internal server Error", ex);
        }

        #endregion

        #region GetOrderByIdAsync

        [Fact]
        public async Task GetOrderByIdAsync_ShouldReturnOrder_WhenFound()
        {
            var order = new Order { Id = 2 };
            _orderRepository.GetOrderWithItemsAsync(2).Returns(order);

            var result = await _service.GetOrderByIdAsync(2);

            Assert.Equal(order, result);
        }

        [Fact]
        public async Task GetOrderByIdAsync_ShouldLogAndThrow_WhenException()
        {
            var ex = new Exception("DB Error");
            _orderRepository.GetOrderWithItemsAsync(Arg.Any<int>()).Throws(ex);

            var thrown = await Assert.ThrowsAsync<Exception>(() => _service.GetOrderByIdAsync(1));
            Assert.Equal("Internal server Error", thrown.Message);
            _logger.Received(1).LogException("Internal server Error", ex);
        }

        #endregion

        #region CreateOrderAsync

        [Fact]
        public async Task CreateOrderAsync_ShouldCreateOrderAndUpdateInventory_WhenSuccessful()
        {
            var orderItems = new List<OrderItem>
            {
                new OrderItem { ProductId = 1, Quantity = 2, TotalPrice = 10m },
                new OrderItem { ProductId = 2, Quantity = 3, TotalPrice = 20m }
            };
            var order = new Order { OrderItems = orderItems };

            _orderRepository.AddAsync(Arg.Any<Order>()).Returns(call => call.Arg<Order>());

            var result = await _service.CreateOrderAsync(order);

            Assert.NotNull(result.OrderNumber);
            Assert.Equal(30m, result.TotalAmount);
            Assert.True(result.CreatedDate <= DateTime.UtcNow);
            Assert.True(result.UpdatedDate <= DateTime.UtcNow);
            Assert.True(result.OrderDate <= DateTime.UtcNow);

            foreach (var item in orderItems)
            {
                await _inventoryService.Received(1).UpdateStockAsync(item.ProductId, -item.Quantity);
            }
        }

        [Fact]
        public async Task CreateOrderAsync_ShouldLogAndThrow_WhenException()
        {
            var ex = new Exception("DB insert error");
            _orderRepository.AddAsync(Arg.Any<Order>()).Throws(ex);

            var thrown = await Assert.ThrowsAsync<Exception>(() => _service.CreateOrderAsync(new Order()));
            Assert.Equal("Internal server Error", thrown.Message);
            _logger.Received(1).LogException("Internal server Error", ex);
        }

        #endregion

        #region UpdateOrderAsync

        [Fact]
        public async Task UpdateOrderAsync_ShouldReturnUpdatedOrder_WhenOrderExists()
        {
            var existing = new Order
            {
                Id = 5,
                CustomerName = "Old",
                UpdatedDate = DateTime.UtcNow.AddDays(-1)
            };
            var update = new Order
            {
                CustomerName = "New",
                CustomerAddress = "Address",
                CustomerPhone = "1234567890",
                Status = OrderStatus.Processing
            };

            _orderRepository.GetByIdAsync(5).Returns(existing);
            _orderRepository.UpdateAsync(Arg.Any<Order>()).Returns(call => call.Arg<Order>());

            var beforeUpdate = DateTime.UtcNow;

            var result = await _service.UpdateOrderAsync(5, update);

            Assert.Equal("New", result.CustomerName);
            Assert.Equal("Address", result.CustomerAddress);
            Assert.Equal("1234567890", result.CustomerPhone);
            Assert.Equal(OrderStatus.Processing, result.Status);

            // Assert UpdatedDate is recent (>= beforeUpdate)
            Assert.True(result.UpdatedDate >= beforeUpdate);
        }

        [Fact]
        public async Task UpdateOrderAsync_ShouldReturnNull_WhenOrderDoesNotExist()
        {
            _orderRepository.GetByIdAsync(Arg.Any<int>()).Returns((Order)null);

            var result = await _service.UpdateOrderAsync(1, new Order());

            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateOrderAsync_ShouldLogAndThrow_WhenException()
        {
            var ex = new Exception("DB Error");
            _orderRepository.GetByIdAsync(Arg.Any<int>()).Throws(ex);

            var thrown = await Assert.ThrowsAsync<Exception>(() => _service.UpdateOrderAsync(1, new Order()));
            Assert.Equal("Internal server Error", thrown.Message);
            _logger.Received(1).LogException("Internal server Error", ex);
        }

        #endregion

        #region DeleteOrderAsync

        [Fact]
        public async Task DeleteOrderAsync_ShouldReturnTrue_WhenDeleted()
        {
            _orderRepository.DeleteAsync(1).Returns(true);

            var result = await _service.DeleteOrderAsync(1);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteOrderAsync_ShouldLogAndThrow_WhenException()
        {
            var ex = new Exception("DB Error");
            _orderRepository.DeleteAsync(Arg.Any<int>()).Throws(ex);

            var thrown = await Assert.ThrowsAsync<Exception>(() => _service.DeleteOrderAsync(1));
            Assert.Equal("Internal server Error", thrown.Message);
            _logger.Received(1).LogException("Internal server Error", ex);
        }

        #endregion

        #region GetOrdersWithItemsAsync

        [Fact]
        public async Task GetOrdersWithItemsAsync_ShouldReturnOrders_WhenSuccessful()
        {
            var orders = new List<Order> { new Order { Id = 3, OrderItems = new List<OrderItem> { new OrderItem() } } };
            _orderRepository.GetOrdersWithItemsAsync().Returns(orders);

            var result = await _service.GetOrdersWithItemsAsync();

            Assert.Single(result);
            Assert.NotEmpty(result.First().OrderItems);
        }

        [Fact]
        public async Task GetOrdersWithItemsAsync_ShouldLogAndThrow_WhenException()
        {
            var ex = new Exception("DB Error");
            _orderRepository.GetOrdersWithItemsAsync().Throws(ex);

            var thrown = await Assert.ThrowsAsync<Exception>(() => _service.GetOrdersWithItemsAsync());
            Assert.Equal("Internal server Error", thrown.Message);
            _logger.Received(1).LogException("Internal server Error", ex);
        }

        #endregion

        #region GetOrderByNumberAsync

        [Fact]
        public async Task GetOrderByNumberAsync_ShouldReturnOrder_WhenFound()
        {
            var order = new Order { OrderNumber = "ORD123" };
            _orderRepository.GetOrderByNumberAsync("ORD123").Returns(order);

            var result = await _service.GetOrderByNumberAsync("ORD123");

            Assert.Equal(order, result);
        }

        [Fact]
        public async Task GetOrderByNumberAsync_ShouldLogAndThrow_WhenException()
        {
            var ex = new Exception("DB Error");
            _orderRepository.GetOrderByNumberAsync(Arg.Any<string>()).Throws(ex);

            var thrown = await Assert.ThrowsAsync<Exception>(() => _service.GetOrderByNumberAsync("ORD123"));
            Assert.Equal("Internal server Error", thrown.Message);
            _logger.Received(1).LogException("Internal server Error", ex);
        }

        #endregion

        #region GetOrdersByStatusAsync

        [Fact]
        public async Task GetOrdersByStatusAsync_ShouldReturnOrders_WhenSuccessful()
        {
            var orders = new List<Order> { new Order { Status = OrderStatus.Pending } };
            _orderRepository.GetOrdersByStatusAsync(OrderStatus.Pending).Returns(orders);

            var result = await _service.GetOrdersByStatusAsync(OrderStatus.Pending);

            Assert.All(result, o => Assert.Equal(OrderStatus.Pending, o.Status));
        }

        [Fact]
        public async Task GetOrdersByStatusAsync_ShouldLogAndThrow_WhenException()
        {
            var ex = new Exception("DB Error");
            _orderRepository.GetOrdersByStatusAsync(Arg.Any<OrderStatus>()).Throws(ex);

            var thrown = await Assert.ThrowsAsync<Exception>(() => _service.GetOrdersByStatusAsync(OrderStatus.Pending));
            Assert.Equal("Internal server Error", thrown.Message);
            _logger.Received(1).LogException("Internal server Error", ex);
        }

        #endregion

        #region UpdateOrderStatusAsync

        [Fact]
        public async Task UpdateOrderStatusAsync_ShouldUpdateAndReturnTrue_WhenOrderExists()
        {
            var order = new Order { Id = 1, Status = OrderStatus.Pending, UpdatedDate = DateTime.UtcNow.AddDays(-1) };
            _orderRepository.GetByIdAsync(1).Returns(order);
            _orderRepository.UpdateAsync(Arg.Any<Order>()).Returns(Task.FromResult(order));

            var result = await _service.UpdateOrderStatusAsync(1, OrderStatus.Shipped);

            Assert.True(result);
            Assert.Equal(OrderStatus.Shipped, order.Status);
            Assert.True(order.UpdatedDate > DateTime.UtcNow.AddMinutes(-5));
        }

        [Fact]
        public async Task UpdateOrderStatusAsync_ShouldReturnFalse_WhenOrderDoesNotExist()
        {
            _orderRepository.GetByIdAsync(Arg.Any<int>()).Returns((Order)null);

            var result = await _service.UpdateOrderStatusAsync(1, OrderStatus.Shipped);

            Assert.False(result);
        }

        [Fact]
        public async Task UpdateOrderStatusAsync_ShouldLogAndThrow_WhenException()
        {
            var ex = new Exception("DB Error");
            _orderRepository.GetByIdAsync(Arg.Any<int>()).Throws(ex);

            var thrown = await Assert.ThrowsAsync<Exception>(() => _service.UpdateOrderStatusAsync(1, OrderStatus.Shipped));
            Assert.Equal("Internal server Error", thrown.Message);
            _logger.Received(1).LogException("Internal server Error", ex);
        }

        #endregion
    }
}
