using System;

namespace FakeUserGenerator.Models
{
    public class User
    {
        public int Number { get; set; }
        public Guid? Id { get; set; }
        public string? FullName { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
    }
}
