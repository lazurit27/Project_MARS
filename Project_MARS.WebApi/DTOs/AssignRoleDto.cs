using Project_MARS.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Project_MARS.WebApi.DTOs
{
    public class AssignRoleDto
    {
        [Required(ErrorMessage = "User ID is required.")]
        public string UserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role type is required.")]
        [EnumDataType(typeof(RolesType), ErrorMessage = "Invalid role type value.")]
        public RolesType Role { get; set; }
    }
}
