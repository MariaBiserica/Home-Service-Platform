using DataLayer.Dtos;
using DataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Mapping
{
    public static class ServiceMappingExtensions
    {
        public static List<ServiceDisplayDto> ToServiceDisplayDtos (this List<Service> services)
        {
            if (services == null)
            {
                return null;
            }
            return services.Select(s => s.ToServiceDisplayDto()).ToList();
        }

        public static ServiceDisplayDto ToServiceDisplayDto (this Service service)
        {
            if (service == null)
            {
                return null;
            }

            return new ServiceDisplayDto
            {
                Title = service.Title,
                ServiceTypeId = service.ServiceTypeId,
                Type = service.Type,
                Description = service.Description,
                Prices = service.Prices,
                ProviderId = service.ProviderId,
                Provider = service.Provider.ToProviderForServiceDisplayDto(),
            };
        }

        public static List<ServiceDisplayDto> ToServiceForProviderDisplayDtos(this List<Service> services)
        {
            if (services == null)
            {
                return null;
            }
            return services.Select(s => s.ToServiceForProviderDisplayDto()).ToList();
        }

        public static ServiceDisplayDto ToServiceForProviderDisplayDto(this Service service)
        {
            if (service == null)
            {
                return null;
            }

            return new ServiceDisplayDto
            {
                Title = service.Title,
                ServiceTypeId = service.ServiceTypeId,
                Type = service.Type,
                Description = service.Description,
                Prices = service.Prices
            };
        }
    }
}
