using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApp.Domain.Entities
{
    public class Category
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = null!;
        public ICollection<TodoItem> Todos { get; set; } = new List<TodoItem>();
    }
}
