using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RestaurantApp.DataAccess.DbContexts;
using RestaurantApp.Hubs;
using RestaurantApp.Models.DTOs;
using RestaurantApp.Models.Entities;
using RestaurantApp.Models.Enums;

namespace RestaurantApp.Services;

public class OrderService : IOrderService
{
    private readonly RestaurantAppDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IHubContext<OrderHub> _hubContext;

    public OrderService(RestaurantAppDbContext dbContext, IMapper mapper, IHubContext<OrderHub> hubContext)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _hubContext = hubContext;
    }
    
    public async Task<OrderDetailsDto> PlaceOrderAsync(int customerId, CreateOrderRequest request)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            var restaurant = await _dbContext.Restaurants.FindAsync(request.RestaurantId);
            if (restaurant == null)
            {
                throw new ArgumentException($"Restaurant with ID {request.RestaurantId} not found.");
            }
            
            var requestedItemIds = request.Items.Select(i => i.MenuItemId).ToList();
            var menuItemsFromDb = await _dbContext.MenuItems
                .Where(mi => requestedItemIds.Contains(mi.Id) && mi.RestaurantId == request.RestaurantId && !mi.Deleted.HasValue)
                .ToListAsync();

            if (menuItemsFromDb.Count != requestedItemIds.Count)
            {
                throw new ArgumentException("One or more menu items are invalid or do not belong to this restaurant.");
            }

            var order = new Order
            {
                UserId = customerId,
                RestaurantId = request.RestaurantId,
                DeliveryAddress = request.DeliveryAddress,
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Received,
                TotalPrice = 0
            };

            var orderItems = new List<OrderItem>();
            foreach (var itemRequest in request.Items)
            {
                var menuItem = menuItemsFromDb.First(mi => mi.Id == itemRequest.MenuItemId);
                var orderItem = new OrderItem
                {
                    MenuItemId = itemRequest.MenuItemId,
                    Quantity = itemRequest.Quantity,
                    UnitPrice = menuItem.Price,
                    Order = order
                };
                orderItems.Add(orderItem);
                order.TotalPrice += orderItem.Quantity * orderItem.UnitPrice;
            }

            order.OrderItems = orderItems;

            _dbContext.Orders.Add(order);
            await _dbContext.SaveChangesAsync();
            
            await transaction.CommitAsync();
            
            return await GetOrderByIdAsync(order.Id);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<OrderDetailsDto> GetOrderByIdAsync(int orderId)
    {
        var order = await _dbContext.Orders
            .Include(o => o.User)
            .Include(o => o.Restaurant)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.MenuItem)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null)
        {
            throw new KeyNotFoundException($"Order with ID {orderId} not found.");
        }

        return _mapper.Map<OrderDetailsDto>(order);
    }

    public async Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus)
    {
        var order = await _dbContext.Orders.FindAsync(orderId);

        if (order == null)
        {
            return false;
        }

        order.Status = newStatus;
        await _dbContext.SaveChangesAsync();
        
        await _hubContext.Clients.Group(orderId.ToString()).SendAsync("ReceiveOrderStatusUpdate", new 
        { 
            orderId = order.Id,
            newStatus = newStatus.ToString()
        });
        
        return true;
    }
}