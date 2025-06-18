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
            _clientRepository = clientRepository ?? throw new ArgumentNullException(nameof(clientRepository));
        }

        public async Task<ClientDto> GetByIdAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("Id must be greater than zero.", nameof(id));
            var client = await _clientRepository.GetByIdAsync(id);
            return client != null ? MapToDto(client) : null;
        }

        public async Task<ClientDto> GetByIdentificationAsync(string identification)
        {
            if (string.IsNullOrWhiteSpace(identification))
                throw new ArgumentException("Identification is required.", nameof(identification));

            var client = await _clientRepository.GetByIdentificationAsync(identification);
            return client != null ? MapToDto(client) : null;
        }

        public async Task<IEnumerable<ClientDto>> GetAllAsync(int pageNumber, int pageSize, string searchTerm = null)
        {
            if (pageNumber <= 0) throw new ArgumentException("Page number must be greater than zero.", nameof(pageNumber));
            if (pageSize <= 0) throw new ArgumentException("Page size must be greater than zero.", nameof(pageSize));

            var clients = await _clientRepository.GetAllAsync(pageNumber, pageSize, searchTerm?.Trim());
            return clients?.Select(MapToDto) ?? Enumerable.Empty<ClientDto>();
        }

        public async Task AddAsync(ClientCreateDto clientDto)
        {
            if (clientDto == null) throw new ArgumentNullException(nameof(clientDto));

            // Validación de unicidad
            if (await _clientRepository.ExistsAsync(clientDto.IdentificationNumber))
                throw new InvalidOperationException("A client with this identification already exists.");

            var client = new Client
            {
                IdentificationType = clientDto.IdentificationType,
                IdentificationNumber = clientDto.IdentificationNumber,
                FirstName = clientDto.FirstName?.Trim(),
                LastName = clientDto.LastName?.Trim(),
                Phone = clientDto.Phone?.Trim(),
                Email = clientDto.Email?.Trim(),
                Address = clientDto.Address?.Trim()
            };

            await _clientRepository.AddAsync(client);
        }

        public async Task UpdateAsync(ClientUpdateDto clientDto)
        {
            if (clientDto == null) throw new ArgumentNullException(nameof(clientDto));

            var existingClient = await _clientRepository.GetByIdAsync(clientDto.ClientId);
            if (existingClient == null)
                throw new InvalidOperationException("Client does not exist.");

            // (Opcional) Validar unicidad si cambia la identificación
            if (!string.Equals(existingClient.IdentificationNumber, clientDto.IdentificationNumber, StringComparison.OrdinalIgnoreCase))
            {
                if (await _clientRepository.ExistsAsync(clientDto.IdentificationNumber))
                    throw new InvalidOperationException("A client with this identification already exists.");
            }

            existingClient.IdentificationType = clientDto.IdentificationType;
            existingClient.IdentificationNumber = clientDto.IdentificationNumber;
            existingClient.FirstName = clientDto.FirstName?.Trim();
            existingClient.LastName = clientDto.LastName?.Trim();
            existingClient.Phone = clientDto.Phone?.Trim();
            existingClient.Email = clientDto.Email?.Trim();
            existingClient.Address = clientDto.Address?.Trim();

            await _clientRepository.UpdateAsync(existingClient);
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("Id must be greater than zero.", nameof(id));

            var client = await _clientRepository.GetByIdAsync(id);
            if (client == null)
                throw new InvalidOperationException("Client does not exist.");

            await _clientRepository.DeleteAsync(id);
        }

        public Task<bool> ExistsAsync(string identification)
        {
            if (string.IsNullOrWhiteSpace(identification))
                throw new ArgumentException("Identification is required.", nameof(identification));
            return _clientRepository.ExistsAsync(identification);
        }

        public Task<int> CountAsync(string searchTerm = null)
        {
            return _clientRepository.CountAsync(searchTerm?.Trim());
        }

        private ClientDto MapToDto(Client client)
        {
            if (client == null) return null;
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
