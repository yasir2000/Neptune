using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace GUI.Reflection
{
  /// <summary>
  /// Represents a <see cref="System.ComponentModel.PropertyDescriptor"/> which forwards
  /// all methods to an underlying <see cref="System.ComponentModel.PropertyDescriptor"/>.
  /// This can be used, in subclasses, to allow changing the value of attributes at runtime.
  /// </summary>
  /// <seealso cref="System.ComponentModel.PropertyDescriptor"/>
  public abstract class ForwardingPropertyDescriptor : PropertyDescriptor
  {
    #region Private Fields

    /// <summary>
    /// The wrapped <see cref="System.ComponentModel.PropertyDescriptor"/>.
    /// </summary>
    private readonly PropertyDescriptor m_root;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the wrapped <see cref="System.ComponentModel.PropertyDescriptor"/>.
    /// </summary>
    protected PropertyDescriptor Root { get { return m_root; } }

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a new property descriptor, wrapping the given one.
    /// </summary>
    /// <param name="root">The <see cref="System.ComponentModel.PropertyDescriptor"/> to wrap and to forward method calls to.</param>
    protected ForwardingPropertyDescriptor(PropertyDescriptor root)
      : base(root)
    {
      m_root = root;
    }

    #endregion

    #region PropertyDescriptor Interface Overrides

    /// <summary>
    /// Gets the collection of attributes for this member.
    /// </summary>
    public override AttributeCollection Attributes
    {
      get { return Root.Attributes; }
    }

    /// <summary>
    /// Gets the name of the category to which the member belongs, as specified in
    /// the <see cref="System.ComponentModel.CategoryAttribute"/>.
    /// </summary>
    public override string Category
    {
      get { return Root.Category; }
    }

    /// <summary>
    /// When overridden in a derived class, gets the type of the component this property
    /// is bound to.
    /// </summary>
    public override Type ComponentType
    {
      get { return Root.ComponentType; }
    }

    /// <summary>
    /// Gets the type converter for this property.
    /// </summary>
    public override TypeConverter Converter
    {
      get { return Root.Converter; }
    }

    /// <summary>
    /// Gets the description of the member, as specified in the <see cref="System.ComponentModel.DescriptionAttribute"/>.
    /// </summary>
    public override string Description
    {
      get { return Root.Description; }
    }

    /// <summary>
    /// Gets whether this member should be set only at design time, as specified
    /// in the <see cref="System.ComponentModel.DesignOnlyAttribute"/>.
    /// </summary>
    public override bool DesignTimeOnly
    {
      get { return Root.DesignTimeOnly; }
    }

    /// <summary>
    /// Gets the name that can be displayed in a window, such as a Properties window.
    /// </summary>
    public override string DisplayName
    {
      get { return Root.DisplayName; }
    }

    /// <summary>
    /// Gets a value indicating whether the member is browsable, as specified in
    /// the <see cref="System.ComponentModel.BrowsableAttribute"/>.
    /// </summary>
    public override bool IsBrowsable
    {
      get { return Root.IsBrowsable; }
    }

    /// <summary>
    /// Gets a value indicating whether this property should be localized, as specified
    /// in the <see cref="System.ComponentModel.LocalizableAttribute"/>.
    /// </summary>
    public override bool IsLocalizable
    {
      get { return Root.IsLocalizable; }
    }

    /// <summary>
    /// When overridden in a derived class, gets a value indicating whether this
    /// property is read-only.
    /// </summary>
    public override bool IsReadOnly
    {
      get { return Root.IsReadOnly; }
    }

    /// <summary>
    /// Gets the name of the member.
    /// </summary>
    public override string Name
    {
      get { return Root.Name; }
    }

    /// <summary>
    /// When overridden in a derived class, gets the type of the property.
    /// </summary>
    public override Type PropertyType
    {
      get { return Root.PropertyType; }
    }

    /// <summary>
    /// Gets a value indicating whether value change notifications for this property
    /// may originate from outside the property descriptor.
    /// </summary>
    public override bool SupportsChangeEvents
    {
      get { return Root.SupportsChangeEvents; }
    }

    /// <summary>
    /// Enables other objects to be notified when this property changes.
    /// </summary>
    /// <param name="component">The component to add the handler for.</param>
    /// <param name="handler">The delegate to add as a listener.</param>
    public override void AddValueChanged(object component, EventHandler handler)
    {
      Root.AddValueChanged(component, handler);
    }

    /// <summary>
    /// When overridden in a derived class, returns whether resetting an object changes
    /// its value.
    /// </summary>
    /// <param name="component">The component to test for reset capability.</param>
    /// <returns>True if resetting the component changes its value; otherwise, false.</returns>
    public override bool CanResetValue(object component)
    {
      return Root.CanResetValue(component);
    }

    /// <summary>
    /// Returns a <see cref="System.ComponentModel.PropertyDescriptorCollection"/> for a given
    /// object using a specified array of attributes as a filter.
    /// </summary>
    /// <param name="instance">A component to get the properties for.</param>
    /// <param name="filter">An array of type System.Attribute to use as a filter.</param>
    /// <returns>A <see cref="System.ComponentModel.PropertyDescriptorCollection"/> with the properties
    /// that match the specified attributes for the specified component.</returns>
    public override PropertyDescriptorCollection GetChildProperties(object instance, Attribute[] filter)
    {
      return Root.GetChildProperties(instance, filter);
    }

    /// <summary>
    /// Gets an editor of the specified type.
    /// </summary>
    /// <param name="editorBaseType">The base type of editor, which is used to differentiate between multiple
    /// editors that a property supports.</param>
    /// <returns>An instance of the requested editor type, or null if an editor cannot be found.</returns>
    public override object GetEditor(Type editorBaseType)
    {
      return Root.GetEditor(editorBaseType);
    }

    /// <summary>
    /// When overridden in a derived class, gets the current value of the property
    /// on a component.
    /// </summary>
    /// <param name="component">The component with the property for which to retrieve the value.</param>
    /// <returns>The value of a property for a given component.</returns>
    public override object GetValue(object component)
    {
      return Root.GetValue(component);
    }

    /// <summary>
    /// Disables other objects to be notified when this property changes.
    /// </summary>
    /// <param name="component">The component to remove the handler for.</param>
    /// <param name="handler">The delegate to remove as a listener.</param>
    public override void RemoveValueChanged(object component, EventHandler handler)
    {
      Root.RemoveValueChanged(component, handler);
    }

    /// <summary>
    /// When overridden in a derived class, resets the value for this property of
    /// the component to the default value.
    /// </summary>
    /// <param name="component">The component with the property value that is to be reset to the default
    /// value.</param>
    public override void ResetValue(object component)
    {
      Root.ResetValue(component);
    }

    /// <summary>
    /// When overridden in a derived class, sets the value of the component to a
    /// different value.
    /// </summary>
    /// <param name="component">The component with the property value that is to be set.</param>
    /// <param name="value">The new value.</param>
    public override void SetValue(object component, object value)
    {
      Root.SetValue(component, value);
    }

    /// <summary>
    /// When overridden in a derived class, determines a value indicating whether
    /// the value of this property needs to be persisted.
    /// </summary>
    /// <param name="component">The component with the property to be examined for persistence.</param>
    /// <returns>True if the property should be persisted; otherwise, false.</returns>
    public override bool ShouldSerializeValue(object component)
    {
      return Root.ShouldSerializeValue(component);
    }

    #endregion
   
    #region Object Interface Overrides

    /// <summary>
    /// Compares this to another object to see if they are equivalent.
    /// </summary>
    /// <param name="obj">The object to compare to this <see cref="GUI.Reflection.ForwardingPropertyDescriptor"/>.</param>
    /// <returns>True if the values are equivalent; otherwise, false.</returns>
    public override bool Equals(object obj)
    {
      return Root.Equals(obj);
    }

    /// <summary>
    /// Returns the hash code for this object.
    /// </summary>
    /// <returns>The hash code for this object.</returns>
    public override int GetHashCode()
    {
      return Root.GetHashCode();
    }

    /// <summary>
    /// Returns a <see cref="System.String"/> that represents the current <see cref="GUI.Reflection.ForwardingPropertyDescriptor"/>.
    /// </summary>
    /// <returns>A <see cref="System.String"/> that represents the current <see cref="GUI.Reflection.ForwardingPropertyDescriptor"/>.</returns>
    public override string ToString()
    {
      return Root.ToString();
    }

    #endregion
  }
}
