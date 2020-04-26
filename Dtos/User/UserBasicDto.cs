using System;

namespace ReceitasApi.Dtos.User {
    public class UserBasicDto {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string UserType { get; set; }
        public bool Active { get; set; }

    }
}