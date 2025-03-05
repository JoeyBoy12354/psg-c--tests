namespace psg_automated_nunit_shared.Attributes
{
    /// <summary>
    /// Add this attribute if test must be in QA.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class QAAttribute : Attribute
    {
    }

    /// <summary>
    /// Add this attribute if test must be in Prod.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class ProdAttribute : Attribute
    {
    }
}
