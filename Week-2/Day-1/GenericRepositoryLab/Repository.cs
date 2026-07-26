namespace GenericRepositoryLab;
public class Repository<T> where T : class
{
    private readonly List<T> _items=new ();
    public void Add(T item)
    {
        _items.Add(item);
    }
    public IReadOnlyList<T> GetAll()
    {
        return _items.AsReadOnly();
    } 
    public T?Find(Predicate<T> predicate)
    {
        return _items.Find(predicate);
    }
    
}