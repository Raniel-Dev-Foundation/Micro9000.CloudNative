using Core.Data;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestsShared.Mocks;
public abstract class EntityBuilder<TEntity> where TEntity : Entity
{
    public abstract TEntity Build();

    public virtual EntityBuilder<TEntity> BeforeBuild(CloudNativeDbContext context)
    {
        return this;
    }

    public TEntity BuildInto (CloudNativeDbContext context)
    {
        BeforeBuild(context);
        var entity = Build();

        if (entity.Id == 0)
        {
            context.Add(entity);
        }
        else
        {
            context.Update(entity);
        }

        context.SaveChanges();
        return entity;
    }
}
