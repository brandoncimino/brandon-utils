namespace BrandonUtils.Collections {
    public interface IPrimaryKeyed<out T> {
        T PrimaryKey { get; }
    }
}