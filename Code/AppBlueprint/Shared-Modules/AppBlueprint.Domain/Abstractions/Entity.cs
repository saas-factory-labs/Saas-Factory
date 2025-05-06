//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace AppBlueprint.Domain.Abstractions;

//public abstract class Entity
//{
//    private readonly List<IDomainEvent> _domainEvents = new();

//    protected Entity(Guid id)
//    {
//        Id = id;
//    }

//    protected Entity()
//    {
//    }

//    public Guid Id { get; init; }

//    public IReadOnlyList<IDomainEvent> GetDomainEvents()
//    {
//        return _domainEvents.ToList();
//    }

//    public void ClearDomainEvents()
//    {
//        _domainEvents.Clear();
//    }

//    protected void RaiseDomainEvent(IDomainEvent domainEvent)
//    {
//        _domainEvents.Add(domainEvent);
//    }
//}



