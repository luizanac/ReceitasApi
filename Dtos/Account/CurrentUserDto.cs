using System;

namespace ReceitasApi.Dtos.Account {
    public class CurrentUserDto {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool Active { get; set; }
        public string UserType { get; set; }
    }
}