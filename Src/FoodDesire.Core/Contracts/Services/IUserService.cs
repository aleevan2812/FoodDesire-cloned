﻿namespace FoodDesire.Core.Contracts.Services;
public interface IUserService<T> where T : BaseUser {
    Task<T> CreateAccount(T user);
    Task<T> GetById(int id);
    Task<List<T>> GetAll();
    Task<T> GetByEmailAndPassword(string email, string password);
    Task<T> GetByEmail(string email);
    Task<T> UpdateAccount(T user);
    Task<bool> DeleteAccountById(int id);
}

