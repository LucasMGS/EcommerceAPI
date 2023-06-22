using NSE.Core.Messages;

namespace NSE.Core.DomainObjects
{
    public abstract class Entity
    {
        public Guid Id { get; set; }

        protected Entity() 
        { 
            Id = Guid.NewGuid();
        }

        private List<Event> _notifications;
        public IReadOnlyCollection<Event>? Notifications => _notifications?.AsReadOnly();

        public void AddEvent(Event evt)
        {
            _notifications ??= new List<Event>();
            _notifications.Add(evt);
        }

        public void RemoveEvent(Event evt)
        {
            _notifications?.Remove(evt);
        }

        public void CleanEvents()
        {
            _notifications?.Clear();
        }

        #region Comparisons
        public override bool Equals(object obj)
        {
            var compareTo = obj as Entity;

            if (ReferenceEquals(this, compareTo)) return true;
            if (ReferenceEquals(null, compareTo)) return false;

            return Id.Equals(compareTo.Id);
        }

        public static bool operator ==(Entity a, Entity b)
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
                return true;

            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(Entity a, Entity b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return (GetType().GetHashCode() * 907) + Id.GetHashCode();
        }

        public override string ToString()
        {
            return $"{GetType().Name} [Id={Id}]";
        }
        #endregion

    }
}