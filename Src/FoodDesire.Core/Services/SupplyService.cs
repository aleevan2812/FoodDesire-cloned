﻿namespace FoodDesire.Core.Services;
public class SupplyService : ISupplyService {
    private readonly IRepository<Supply> _supplyRepository;
    private readonly IRepository<Ingredient> _ingredientRepository;
    private readonly IPaymentService _paymentService;

    public SupplyService(
        IRepository<Supply> supplyRepository,
        IRepository<Ingredient> ingredientRepository,
        IPaymentService paymentService
        ) {
        _supplyRepository = supplyRepository;
        _ingredientRepository = ingredientRepository;
        _paymentService = paymentService;
    }

    public async Task<Supply> AcceptSupply(int supplyId, int supplierId) {
        Supply supply = await _supplyRepository.GetByID(supplyId);
        supply.SupplierId = supplierId;
        supply.Status = SupplyStatus.Accepted;
        supply = await _supplyRepository.Update(supply);
        return supply;
    }

    public async Task<Supply> CompleteSupply(int supplyId, decimal value) {
        Supply supply = await _supplyRepository.GetByID(supplyId);
        supply.Status = SupplyStatus.Completed;
        supply.SuppliedDate = DateTime.Now;
        Ingredient ingredient = await _ingredientRepository.GetByID(supply.IngredientId);
        ingredient.CurrentPricePerUnit = (decimal)((double)value / supply.Amount);
        ingredient.CurrentQuantity += supply.Amount;
        ingredient.InSupply = 0;
        await _ingredientRepository.Update(ingredient);
        supply = await _supplyRepository.Update(supply);
        supply = await _paymentService.PaymentForSupply(supply, value);
        return supply;
    }

    public async Task<Supply> CreateSupply(int ingredientId, double amount) {
        Ingredient ingredient = await _ingredientRepository.GetByID(ingredientId);
        ingredient.InSupply = amount;
        await _ingredientRepository.Update(ingredient);

        Supply supply = new Supply() {
            IngredientId = ingredientId,
            Amount = amount,
        };
        supply = await _supplyRepository.Add(supply);
        return supply;
    }

    public async Task<List<Supply>> GetAllAcceptedSupplies() {
        Expression<Func<Supply, bool>> filterExpression = e => e.Status.Equals(SupplyStatus.Accepted);
        Expression<Func<Supply, object>> orderExpression = e => e.RequestedAt;

        IQueryable<Supply> filter(IQueryable<Supply> e) => e.Where(filterExpression);
        IOrderedQueryable<Supply> order(IQueryable<Supply> e) => e.OrderBy(orderExpression);
        IIncludableQueryable<Supply, object> include(IQueryable<Supply> e) => e.Include(e => e.Ingredient!);

        List<Supply> supplies = await _supplyRepository.Get(filter, order, include);
        return supplies;
    }

    public async Task<List<Supply>> GetAllCompletedSupplies() {
        Expression<Func<Supply, bool>> filterExpression = e => e.Status.Equals(SupplyStatus.Completed);
        Expression<Func<Supply, object>> orderExpression = e => e.RequestedAt;

        IQueryable<Supply> filter(IQueryable<Supply> e) => e.Where(filterExpression);
        IOrderedQueryable<Supply> order(IQueryable<Supply> e) => e.OrderBy(orderExpression);
        IIncludableQueryable<Supply, object> include(IQueryable<Supply> e) => e.Include(e => e.Ingredient!);

        List<Supply> supplies = await _supplyRepository.Get(filter, order, include);
        return supplies;
    }

    public async Task<List<Supply>> GetAllAcceptedSuppliesForSupplier(int supplierId) {
        Expression<Func<Supply, bool>> filterExpression = e => e.Status.Equals(SupplyStatus.Accepted) && e.SupplierId == supplierId;
        Expression<Func<Supply, object>> orderExpression = e => e.RequestedAt;

        IQueryable<Supply> filter(IQueryable<Supply> e) => e.Where(filterExpression);
        IOrderedQueryable<Supply> order(IQueryable<Supply> e) => e.OrderBy(orderExpression);
        IIncludableQueryable<Supply, object> include(IQueryable<Supply> e) => e.Include(e => e.Ingredient!);

        List<Supply> supplies = await _supplyRepository.Get(filter, order, include);
        return supplies;
    }

    public async Task<List<Supply>> GetAllPendingSupplies() {
        Expression<Func<Supply, bool>> filterExpression = e => e.Status.Equals(SupplyStatus.Pending);
        Expression<Func<Supply, object>> orderExpression = e => e.RequestedAt;

        IQueryable<Supply> filter(IQueryable<Supply> e) => e.Where(filterExpression);
        IOrderedQueryable<Supply> order(IQueryable<Supply> e) => e.OrderBy(orderExpression);
        IIncludableQueryable<Supply, object> include(IQueryable<Supply> e) => e.Include(e => e.Ingredient!);


        List<Supply> supplies = await _supplyRepository.Get(filter, order, include);
        return supplies;
    }

    public async Task<List<Supply>> GetAllSupplierSupplies(int supplierId) {
        Expression<Func<Supply, bool>> filterExpression = e => e.SupplierId == supplierId;
        Expression<Func<Supply, object>> orderExpression = e => e.RequestedAt;

        IQueryable<Supply> filter(IQueryable<Supply> e) => e.Where(filterExpression);
        IOrderedQueryable<Supply> order(IQueryable<Supply> e) => e.OrderBy(orderExpression);
        IIncludableQueryable<Supply, object> include(IQueryable<Supply> e) => e.Include(e => e.Ingredient!);


        List<Supply> supplies = await _supplyRepository.Get(filter, order, include);
        return supplies;
    }

    public async Task<List<Supply>> GetAllSupplies() {
        List<Supply> supplies = await _supplyRepository.GetAll();
        return supplies;
    }

    public async Task<Supply> GetSupplyById(int supplyId) {
        Supply supply = await _supplyRepository.GetByID(supplyId);
        return supply;
    }
}
