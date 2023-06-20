using DataLayer.Entities;
using DataLayer.Repositories;
using DataLayer.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public class CustomersService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CustomersService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<Customer>> GetAll()
        {
            var customers = await _unitOfWork.GetRepository<CustomersRepository, Customer>().GetAllAsync();
            return customers;
        }
    }
}
