using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.DTOs
{
    public class UpdateTodoDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Priority? Priority { get; set; }
        public Guid? CategoryId { get; set; }
    }
}
