using Project.Application.Dtos;
using Project.Domain.Entities;
using Project.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Application.Services
{
    public class ClientServices : IClientServices
    {
        private readonly IClientRepository _clientRepository;

        public ClientServices(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public async Task<ClientDto> GetByIdAsync(int id)
        {
            var client = await _clientRepository.GetByIdAsync(id);
            return client == null ? null : MapToDto(client);
        }

        public async Task<ClientDto> GetByIdentificationAsync(string identification)
        {
            var client = await _clientRepository.GetByIdentificationAsync(identification);
            return client == null ? null : MapToDto(client);
        }

        public async Task<IEnumerable<ClientDto>> GetAllAsync(int pageNumber, int pageSize, string searchTerm = null)
        {
            var clients = await _clientRepository.GetAllAsync(pageNumber, pageSize, searchTerm);
            return clients.Select(MapToDto);
        }

        public async Task AddAsync(ClientCreateDto clientDto)
        {
            var client = new Client
            {
                IdentificationType = clientDto.IdentificationType,
                IdentificationNumber = clientDto.IdentificationNumber,
                FirstName = clientDto.FirstName,
                LastName = clientDto.LastName,
                Phone = clientDto.Phone,
                Email = clientDto.Email,
                Address = clientDto.Address
            };
            await _clientRepository.AddAsync(client);
        }

        public async Task UpdateAsync(ClientUpdateDto clientDto)
        {
            var client = new Client
            {
                ClientId = clientDto.ClientId,
                IdentificationType = clientDto.IdentificationType,
                IdentificationNumber = clientDto.IdentificationNumber,
                FirstName = clientDto.FirstName,
                LastName = clientDto.LastName,
                Phone = clientDto.Phone,
                Email = clientDto.Email,
                Address = clientDto.Address
            };
            await _clientRepository.UpdateAsync(client);
        }

        public Task DeleteAsync(int id)
        {
            return _clientRepository.DeleteAsync(id);
        }

        public Task<bool> ExistsAsync(string identification)
        {
            return _clientRepository.ExistsAsync(identification);
        }

        public Task<int> CountAsync(string searchTerm = null)
        {
            return _clientRepository.CountAsync(searchTerm);
        }

        private ClientDto MapToDto(Client client)
        {
            return new ClientDto
            {
                ClientId = client.ClientId,
                IdentificationNumber = client.IdentificationNumber,
                FirstName = client.FirstName,
                LastName = client.LastName,
                Phone = client.Phone,
                Email = client.Email,
                Address = client.Address
            };
        }
    }
}
