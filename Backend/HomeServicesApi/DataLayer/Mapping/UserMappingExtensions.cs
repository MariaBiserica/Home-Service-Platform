using DataLayer.Dtos;
using DataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Mapping
{
    public static class UserMappingExtensions
    {
        public static List<UserDisplayDto> ToUserDisplayDtos (this List<User> users)
        {
            if (users == null)
            {
                return null;
            }

            return users.Select(u => u.ToUserDisplayDto()).ToList();
        }

        public static UserDisplayDto ToUserDisplayDto(this User user)
        {
            if (user == null)
            {
                return null;
            }

            return new UserDisplayDto
            {
                Username = user.Username,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email
            };
        }
    }
}
