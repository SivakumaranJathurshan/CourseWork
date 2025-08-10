using InventoryManagement.Data.Repositories.Interfaces;
using InventoryManagement.Models;
using InventoryManagement.Models.DTO;
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
            var orderCreateDto = new OrderCreateDTO(
                "ORD-001",
                "John Doe",
                "123 Main St",
                "555-1234",
                OrderStatus.Pending,
                30m
            );

            var createdOrder = new Order
            {
                Id = 1,
                CustomerName = "John Doe",
                CustomerAddress = "123 Main St",
                CustomerPhone = "555-1234",
                OrderNumber = "ORD-001",
                Status = OrderStatus.Pending,
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { ProductId = 1, Quantity = 2, TotalPrice = 10m },
                    new OrderItem { ProductId = 2, Quantity = 3, TotalPrice = 20m }
                }
            };

            Order capturedOrder = null;
            _orderRepository.AddAsync(Arg.Do<Order>(order => capturedOrder = order)).Returns(createdOrder);

            var result = await _service.CreateOrderAsync(orderCreateDto);

            var testEndTime = DateTime.UtcNow;

            // Assert - Basic properties
            Assert.NotNull(result);
            Assert.NotNull(result.OrderNumber);
            Assert.Equal("John Doe", result.CustomerName);
            Assert.Equal("123 Main St", result.CustomerAddress);
            Assert.Equal("555-1234", result.CustomerPhone);
            Assert.Equal(OrderStatus.Pending, result.Status);

            // DateTime assertions with better error messages
            Assert.True(result.CreatedDate <= testEndTime,
                $"CreatedDate {result.CreatedDate} should be <= {testEndTime}");
            Assert.True(result.UpdatedDate <= testEndTime,
                $"UpdatedDate {result.UpdatedDate} should be <= {testEndTime}");
            Assert.True(result.OrderDate <= testEndTime,
                $"OrderDate {result.OrderDate} should be <= {testEndTime}");

            // Verify repository was called
            await _orderRepository.Received(1).AddAsync(Arg.Any<Order>());

            // Debug what was actually passed to the repository
            Assert.NotNull(capturedOrder);
            Assert.Equal(orderCreateDto.CustomerName, capturedOrder.CustomerName);
            Assert.Equal(orderCreateDto.CustomerAddress, capturedOrder.CustomerAddress);
            Assert.Equal(orderCreateDto.CustomerPhone, capturedOrder.CustomerPhone);
            Assert.Equal(orderCreateDto.Status, capturedOrder.Status);
            Assert.NotNull(capturedOrder.OrderNumber);
        }

        [Fact]
        public async Task CreateOrderAsync_ShouldLogAndThrow_WhenException()
        {
            var orderCreateDto = new OrderCreateDTO(
                "ORD-001",
                "John Doe",
                "123 Main St",
                "555-1234",
                OrderStatus.Pending,
                30m
            );
            var ex = new Exception("DB insert error");
            _orderRepository.AddAsync(Arg.Any<Order>()).Throws(ex);

            var thrown = await Assert.ThrowsAsync<Exception>(() => _service.CreateOrderAsync(orderCreateDto));
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
                CustomerAddress = "Old Address",
                CustomerPhone = "Old Phone",
                Status = OrderStatus.Pending,
                UpdatedDate = DateTime.UtcNow.AddDays(-1)
            };
            var update = new OrderUpdateDTO(
                5,
                "ORD-123",
                "New Customer",
                "New Address",
                "1234567890",
                OrderStatus.Processing,
                100m
            );

            _orderRepository.GetByIdAsync(5).Returns(existing);
            _orderRepository.UpdateAsync(Arg.Any<Order>()).Returns(call => call.Arg<Order>());

            var beforeUpdate = DateTime.UtcNow;

            var result = await _service.UpdateOrderAsync(5, update);

            Assert.Equal("New Customer", result.CustomerName);
            Assert.Equal("New Address", result.CustomerAddress);
            Assert.Equal("1234567890", result.CustomerPhone);
            Assert.Equal(OrderStatus.Processing, result.Status);

            // Assert UpdatedDate is recent (>= beforeUpdate)
            Assert.True(result.UpdatedDate >= beforeUpdate);
        }

        [Fact]
        public async Task UpdateOrderAsync_ShouldReturnNull_WhenOrderDoesNotExist()
        {
            var update = new OrderUpdateDTO(
                1,
                "ORD-123",
                "Customer",
                "Address",
                "Phone",
                OrderStatus.Pending,
                100m
            );
            _orderRepository.GetByIdAsync(Arg.Any<int>()).Returns((Order)null);

            var result = await _service.UpdateOrderAsync(1, update);

            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateOrderAsync_ShouldLogAndThrow_WhenException()
        {
            var update = new OrderUpdateDTO(
                1,
                "ORD-123",
                "Customer",
                "Address",
                "Phone",
                OrderStatus.Pending,
                100m
            );
            var ex = new Exception("DB Error");
            _orderRepository.GetByIdAsync(Arg.Any<int>()).Throws(ex);

            var thrown = await Assert.ThrowsAsync<Exception>(() => _service.UpdateOrderAsync(1, update));
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
