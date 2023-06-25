using DataLayer.Dtos;
using DataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Mapping
{
    public static class ProviderMappingExtensions
    {
        public static List<ProviderDisplayDto> ToProviderDisplayDtos(this List<Provider> providers)
        {
            if (providers == null)
            {
                return null;
            }
            return providers.Select(p => p.ToProviderDisplayDto()).ToList();
        }

        public static ProviderDisplayDto ToProviderDisplayDto(this Provider provider)
        {
            if (provider == null)
            {
                return null;
            }
            return new ProviderDisplayDto
            {
                UserId = provider.UserId,
                User = provider.User.ToUserDisplayDto(),
                Bio = provider.Bio,
                Rating = provider.Rating,
                AddressId = provider.AddressId,
                Address = provider.Address,
                Services = provider.Services.ToServiceForProviderDisplayDtos()
            };
        }
    }
}
