using System;

namespace ReceitasApi.Entities {
    public class Entity {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }

        public Entity () {
            Id = Guid.NewGuid ();
            CreatedAt = DateTime.Now;
        }
    }
}