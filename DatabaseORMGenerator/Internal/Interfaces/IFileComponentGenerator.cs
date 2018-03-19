namespace DatabaseORMGenerator.Internal.Interfaces
{
    public interface IFileComponentGenerator
    {
        string Generate();
        void AddComponentGenerator(IFileComponentGenerator Component);
    }
}