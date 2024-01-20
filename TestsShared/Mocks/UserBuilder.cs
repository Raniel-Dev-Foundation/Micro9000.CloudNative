using Core.Data;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestsShared.Mocks;
public class UserBuilder : EntityBuilder<User>
{
    private string? _name;

    public UserBuilder WithName (string name)
    {
        _name = name;
        return this;
    }

    public override EntityBuilder<User> BeforeBuild(CloudNativeDbContext context)
    {
        if (_name == null) WithName("Default username");
        return this;
    }

    public override User Build()
    {
        return new User
        {
            Name = _name
        };
    }
}
