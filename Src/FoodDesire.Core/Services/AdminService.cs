﻿namespace FoodDesire.Core.Services;
public class AdminService : IAdminService {
    private readonly IRepository<Admin> _adminRepository;
    private readonly ITrackingRepository<User> _userRepository;
    public AdminService(
        IRepository<Admin> adminRepository,
        ITrackingRepository<User> userRepository
        ) {
        _adminRepository = adminRepository;
        _userRepository = userRepository;
    }

    public async Task<Admin> CreateAccount(Admin user) {
        Admin newAdmin = await _adminRepository.Add(user);
        return newAdmin;
    }

    public async Task<bool> DeleteAccountById(int id) {
        Admin admin = await _adminRepository.GetByID(id);
        bool adminDeleted = await _userRepository.SoftDelete(admin.UserId);
        await _userRepository.SaveChanges();
        return adminDeleted;
    }

    public async Task<Admin> GetByEmailAndPassword(string email, string password) {
        Expression<Func<Admin, bool>> filterExpression = e =>
            e.User!.Account!.Email!.Equals(email) && e.User!.Account.Password!.Equals(password);

        IQueryable<Admin> filter(IQueryable<Admin> e) => e.Where(filterExpression);

        Admin? admin = await _adminRepository.GetOne(filter);
        return admin!;
    }

    public async Task<Admin> GetById(int id) {
        Admin admin = await _adminRepository.GetByID(id);
        return admin;
    }

    public async Task<Admin> UpdateAccount(Admin user) {
        Admin updatedAdmin = await _adminRepository.Update(user);
        return updatedAdmin;
    }

    public Task<List<Admin>> GetAll() {
        throw new NotImplementedException();
    }

    public async Task<Admin> GetByEmail(string email) {
        Expression<Func<Admin, bool>> filterExpression = e => e.User!.Account!.Email!.Equals(email);

        IQueryable<Admin> filter(IQueryable<Admin> e) => e.Where(filterExpression);

        Admin? admin = await _adminRepository.GetOne(filter);
        return admin!;
    }
}
