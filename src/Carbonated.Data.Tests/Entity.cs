namespace Carbonated.Data.Tests
{
    class Entity
    {
        public Entity(){}

        public Entity(int id, string name, string title)
        {
            Id = id;
            Name = name;
            Title = title;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }

        public override bool Equals(object obj)
        {
            var other = (Entity)obj;
            return other.Id == Id && other.Name == Name && other.Title == Title;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 31;
                hash = (17 * hash) + Id.GetHashCode();
                hash = (17 * hash) + Name.GetHashCode();
                hash = (17 * hash) + Title.GetHashCode();
                return hash;
            }
        }
    }
}
