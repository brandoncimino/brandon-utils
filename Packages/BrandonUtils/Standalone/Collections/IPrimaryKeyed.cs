namespace BrandonUtils.Standalone.Collections {
    public interface IPrimaryKeyed<out T> {
        T PrimaryKey { get; }
    }
}