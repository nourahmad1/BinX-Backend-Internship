namespace GenericRepositoryLab;

public class Repository<T> where T : class
{
    // The class constraint ensures that Repository only works with reference types.
    // This matches common repository usage for objects like Book or Member.

    private readonly List<T> _items = new();

    public void Add(T item)
    {
        _items.Add(item);
    }

    public IReadOnlyList<T> GetAll()
    {
        return _items.AsReadOnly();
    }

    public T? Find(Predicate<T> predicate)
    {
        return _items.Find(predicate);
    }
}