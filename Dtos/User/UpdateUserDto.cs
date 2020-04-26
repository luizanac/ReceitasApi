using System;

namespace ReceitasApi.Dtos.User {
    public class UpdateUserDto {
        public string Name { get; set; }
        public string Email { get; set; }
        public bool Active { get; set; }
    }
}