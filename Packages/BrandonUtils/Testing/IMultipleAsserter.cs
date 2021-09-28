namespace BrandonUtils.Testing {
    public interface IMultipleAsserter {
        void Invoke();

        int Indent { get; set; }
    }
}