using System;
using UnityEditor;
using UnityEngine;

/// This script will be Headers for inspector elements used in <see cref="Editor"/>



/// <summary>
/// Display a field as read-only in the inspector.
/// CustomPropertyDrawers will not work when this attribute is used.
/// </summary>
/// <seealso cref="BeginReadOnlyGroupAttribute"/>
/// <seealso cref="EndReadOnlyGroupAttribute"/>
public class ReadOnlyAttribute : PropertyAttribute { }

/// <summary>
/// Display one or more fields as read-only in the inspector.
/// Use <see cref="EndReadOnlyGroupAttribute"/> to close the group.
/// Works with CustomPropertyDrawers.
/// </summary>
/// <seealso cref="EndReadOnlyGroupAttribute"/>
/// <seealso cref="ReadOnlyAttribute"/>
public class BeginReadOnlyGroupAttribute : PropertyAttribute { }

/// <summary>
/// Use with <see cref="BeginReadOnlyGroupAttribute"/>.
/// Close the read-only group and resume editable fields.
/// </summary>
/// <seealso cref="BeginReadOnlyGroupAttribute"/>
/// <seealso cref="ReadOnlyAttribute"/>
public class EndReadOnlyGroupAttribute : PropertyAttribute { }

/// <summary>
/// Hides the Script field from the inspector.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class HideScriptField : Attribute
{
}

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
public class DrawIfAttribute : PropertyAttribute
{
    #region Fields
 
    public string comparedPropertyName { get; private set; }
    public object comparedValue { get; private set; }
    public DisablingType disablingType { get; private set; }
 
    /// <summary>
    /// Types of comperisons.
    /// </summary>
    public enum DisablingType
    {
        ReadOnly = 2,
        DontDraw = 3
    }
 
    #endregion
 
    /// <summary>
    /// Only draws the field only if a condition is met. Supports enum and bools.
    /// </summary>
    /// <param name="comparedPropertyName">The name of the property that is being compared (case sensitive).</param>
    /// <param name="comparedValue">The value the property is being compared to.</param>
    /// <param name="disablingType">The type of disabling that should happen if the condition is NOT met. Defaulted to DisablingType.DontDraw.</param>
    /// <example>
    /// <code>
    /// public enum ShowValueEnum
    ///{
    ///    ShowValue1,
    ///    ShowValue2,
    ///    None
    ///}
    /// public ShowValueEnum EnumTest = ShowValueEnum.None;
    /// [DrawIf("EnumTest", ShowValueEnum.ShowValue1)]  //Show if enum is equal to ShowValue1
    /// public int Value1 = 100;
    /// [DrawIf("EnumTest", ShowValueEnum.ShowValue2)]  //Show if enum is equal to ShowValue2
    /// public int Value2 = 200;
    ///
    /// public bool BoolTest = false;
    /// [DrawIf("BoolTest", true)] // Shows when bool is true
    /// public Vector3 Value;
    /// </code>
    /// </example>
    public DrawIfAttribute(string comparedPropertyName, object comparedValue, DisablingType disablingType = DisablingType.DontDraw)
    {
        this.comparedPropertyName = comparedPropertyName;
        this.comparedValue = comparedValue;
        this.disablingType = disablingType;
    }
}
public class FoldoutAttribute : PropertyAttribute
{
    public string name;
    public bool foldEverything;
    public bool readOnly;
    public bool styled;

    /// <summary>Adds the property to the specified foldout group.</summary>
    /// <param name="name">Name of the foldout group.</param>
    /// <param name="foldEverything">Toggle to put all properties to the specified group</param>
    /// <param name="readOnly">Toggle to put all properties to the specified group</param>
    public FoldoutAttribute(string name, bool foldEverything = true, bool readOnly = false, bool styled = false)
    {
        this.foldEverything = foldEverything;
        this.name = name;
        this.readOnly = readOnly;
        this.styled = styled;
    }
}