using System;

namespace Tubumu.Extensions;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
public class TubumuExtensionAttribute(bool generate) : Attribute
{
    internal bool Generate => generate;

    public Accessibility Accessibility { get; set; } = Accessibility.Public;
}


public enum Accessibility
{
    Public,
    Internal
}
