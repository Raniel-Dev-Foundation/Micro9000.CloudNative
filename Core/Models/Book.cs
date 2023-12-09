using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models;

public class Book : Entity
{
    public string? Title { get; set; }
    public int AuthorId { get; set; }
    public Author? Author { get; set; }
}
