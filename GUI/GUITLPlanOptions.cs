//
// Copyright (c) 2009 Froduald Kabanza and the Université de Sherbrooke.
// Use of this software is permitted for non-commercial research purposes, and
// it may be copied or applied only for that use. All copies must include this
// copyright message.
// 
// This is a research prototype and it has not gone through intensive tests and
// is delivered as is. It may still contain bugs. Froduald Kabanza and the
// Université de Sherbrooke disclaim any responsibility for damage that may be
// caused by using it.
// 
// Implementation: Daniel Castonguay
// Project Manager: Froduald Kabanza
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;

using GUI.Reflection;
using TLPlan;
using PDDLParser.Extensions;

namespace GUI
{
  /// <summary>
  /// Represents the GUI version of TLPlan options.
  /// </summary>
  /// <remarks>
  /// All properties have been decorated with different attributes, so that
  /// they show up properly in the PropertyGrid control.
  /// </remarks>
  public class GUITLPlanOptions : ICustomTypeDescriptor
  {
    #region Nested Classes

    #region Type Converters

    /// <summary>
    /// Represents a type converter for <see cref="System.TimeSpan"/> which represent time limits.
    /// </summary>
    /// <seealso cref="System.TimeSpan"/>
    private class TimeLimitConverter : TypeConverter
    {
      /// <summary>
      /// Returns whether this converter can convert an object of the given type to
      /// <see cref="System.TimeSpan"/>, using the specified context.
      /// </summary>
      /// <remarks>
      /// The only type that can be converted from is <see cref="System.String"/>.
      /// </remarks>
      /// <param name="context">An <see cref="System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
      /// <param name="sourceType">A <see cref="System.Type"/> that represents the type you want to convert from. The only supported type is <see cref="System.String"/>.</param>
      /// <returns>True if this converter can perform the conversion; otherwise, false.</returns>
      public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
      {
        if (sourceType == typeof(string))
          return true;

        return base.CanConvertFrom(context, sourceType);
      }

      /// <summary>
      /// Converts the given value to <see cref="System.TimeSpan"/>, using the specified
      /// context and culture information.
      /// </summary>
      /// <remarks>The only type that can be converted from is <see cref="System.String"/>.</remarks>
      /// <param name="context">An <see cref="System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
      /// <param name="culture">The <see cref="System.Globalization.CultureInfo"/> to use as the current culture.</param>
      /// <param name="value">The <see cref="System.Object"/> to convert. This has to be of type <see cref="System.String"/>.</param>
      /// <returns>An <see cref="System.Object"/> that represents the converted value. If the conversion worked, it is of type <see cref="System.TimeSpan"/>.</returns>
      public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
      {
        if (value is string)
        {
          string strValue = (string)value;
          return TimeSpan.FromSeconds(int.Parse(strValue, culture));
        }

        return base.ConvertFrom(context, culture, value);
      }

      /// <summary>
      /// Returns whether this converter can convert the object to <see cref="System.TimeSpan"/>,
      /// using the specified context.
      /// </summary>
      /// <param name="context">An <see cref="System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
      /// <param name="destinationType">A <see cref="System.Type"/> that represents the type you want to convert to. The only conversion available is to <see cref="System.String"/>.</param>
      /// <returns>True if this converter can perform the conversion; otherwise, false.</returns>
      public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
      {
        if (destinationType == typeof(string))
          return true;

        return base.CanConvertTo(context, destinationType);
      }

      /// <summary>
      /// Converts the given value object to <see cref="System.TimeSpan"/>, using the specified
      /// context and culture information.
      /// </summary>
      /// <param name="context">An <see cref="System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
      /// <param name="culture">The <see cref="System.Globalization.CultureInfo"/> to use as the current culture.</param>
      /// <param name="value">The <see cref="System.Object"/> to convert.</param>
      /// <param name="destinationType">The <see cref="System.Type"/> to convert the value parameter to. This can only be <see cref="System.String"/>.</param>
      /// <returns>An <see cref="System.Object"/> that represents the converted value.</returns>
      public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
      {
        if (destinationType == typeof(string))
        {
          TimeSpan span = (TimeSpan)value;
          return ((int)span.TotalSeconds).ToString();
        }

        return base.ConvertTo(context, culture, value, destinationType);
      }
    }

    /// <summary>
    /// Represents an localizable enumeration converter. The localization is done through the use of resources.
    /// </summary>
    private class LocalizableEnumConverter : EnumConverter
    {
      #region Private Fields

      /// <summary>
      /// The resource manager holding the localized strings for the enumeration values.
      /// </summary>
      /// <remarks>
      /// Localized strings representing enumeration values are stored in resources using a
      /// key/value scheme. The key's format is "EnumerationType_EnumerationValue", and the value
      /// is the localized string.
      /// </remarks>
      /// <example>
      /// This example illustrates the key formatting. Let's assume we have an enumeration called <code>EnumExample</code>:
      /// <code>
      /// public enum EnumExample
      /// {
      ///   Value1, 
      ///   Value2,
      ///   Value3
      /// }
      /// </code>>
      /// The actual keys used in the resources will be:
      /// <code>
      /// EnumExample_Value1
      /// EnumExample_Value2
      /// EnumExample_Value3
      /// </code>
      /// We could therefore manually extract localized values for the current culture using the following code,
      /// assuming resourceManager has been properly set:
      /// <code>
      /// string localizedValue = resourceManager.GetString("EnumExample_Value2", CultureInfo.CurrentCulture);
      /// </code>
      /// </example>
      private ResourceManager m_resourceManager;
      /// <summary>
      /// A dictionary that maps, for each culture, the localized string representation of an enumeration value to the actual enumeration value.
      /// </summary>
      private IDictionary<CultureInfo, IDictionary<string, object>> m_cultureLookups;

      #endregion

      #region Constructors

      /// <summary>
      /// Creates a new localizable enumeration converter.
      /// </summary>
      /// <param name="enumType">The <see cref="System.Type"/> of enumeration.</param>
      public LocalizableEnumConverter(Type enumType/*, ResourceManager resourceManager*/)
        : base(enumType)
      {
        //m_resourceManager = resourceManager;
        m_resourceManager = Properties.Resources.ResourceManager;
        m_cultureLookups = new Dictionary<CultureInfo, IDictionary<string, object>>();
      }

      #endregion

      #region Private Methods

      /// <summary>
      /// Retrieves the string representation of an enumeration value in the given culture.
      /// </summary>
      /// <remarks>
      /// If no correspondance for the enumeration value is found in the given culture, the enumeration
      /// value's string key is returned instead.
      /// </remarks>
      /// <param name="culture">The <see cref="System.Globalization.CultureInfo"/> to use as the current culture.</param>
      /// <param name="value">The enumeration value.</param>
      /// <returns>The string representation of the enumeration value. If it is not found in the current resources, the string key is returned. See <see cref="LocalizableEnumConverter"/>.</returns>
      /// <seealso cref="LocalizableEnumConverter"/>
      private string GetStringValueFromResources(CultureInfo culture, object value)
      {
        string resourceName = string.Format("{0}_{1}", value.GetType().Name, value.ToString());

        // Returns the resource key if it is not found in the current culture.
        return m_resourceManager.GetString(resourceName, culture) ?? resourceName;
      }

      /// <summary>
      /// Retrieves the enumeration value based on the localized string value.
      /// </summary>
      /// <param name="culture">The <see cref="System.Globalization.CultureInfo"/> to use as the current culture.
      /// If this is a <see langword="null"/>, <see cref="System.Globalization.CultureInfo.CurrentUICulture"/> is used instead.</param>
      /// <param name="value">The localized string value.</param>
      /// <returns>The enumeration value, or null if no correspondance was found.</returns>
      private object GetEnumValueFromString(CultureInfo culture, string value)
      {
        IDictionary<string, object> lookupTable = GetCultureLookup(culture);
        object result = null;

        lookupTable.TryGetValue(value, out result);

        return result;
      }

      /// <summary>
      /// Retrieves the dictionary that maps localized string values to the actual enumeration value,
      /// for a given culture.
      /// </summary>
      /// <param name="culture">The <see cref="System.Globalization.CultureInfo"/> to use as the current culture.
      /// If this is a <see langword="null"/>, <see cref="System.Globalization.CultureInfo.CurrentUICulture"/> is used instead.</param>
      /// <returns>The dictionary that maps localized string values to the actual enumeration value, for the given culture.</returns>
      private IDictionary<string, object> GetCultureLookup(CultureInfo culture)
      {
        IDictionary<string, object> lookupTable;
        culture = culture ?? CultureInfo.CurrentUICulture;

        if (!m_cultureLookups.TryGetValue(culture, out lookupTable))
        {
          // Dictionaries do not override GetHashCode() so this is okay.
          m_cultureLookups[culture] = lookupTable = new Dictionary<string, object>();
          foreach (object value in GetStandardValues())
            lookupTable.Add(GetStringValueFromResources(culture, value), value);
        }

        return lookupTable;
      }

      #endregion

      #region EnumConverter Interface Overrides

      /// <summary>
      /// Converts the specified value object to an enumeration object.
      /// </summary>
      /// <remarks>The only type that can be converted from is <see cref="System.String"/>.</remarks>
      /// <param name="context">An <see cref="System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
      /// <param name="culture">The <see cref="System.Globalization.CultureInfo"/> to use as the current culture.</param>
      /// <param name="value">The <see cref="System.Object"/> to convert. This has to be of type <see cref="System.String"/>.</param>
      /// <returns>An <see cref="System.Object"/> that represents the converted value.</returns>
      public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
      {
        if (value is string)
          return GetEnumValueFromString(culture, (string)value) ?? base.ConvertFrom(context, culture, value);

        return base.ConvertFrom(context, culture, value);
      }

      /// <summary>
      /// Converts the given value object to the specified destination type.
      /// </summary>
      /// <param name="context">An <see cref="System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
      /// <param name="culture">The <see cref="System.Globalization.CultureInfo"/> to use as the current culture.</param>
      /// <param name="value">The <see cref="System.Object"/> to convert.</param>
      /// <param name="destinationType">The <see cref="System.Type"/> to convert the value parameter to. This can only be <see cref="System.String"/>.</param>
      /// <returns>An <see cref="System.Object"/> that represents the converted value.</returns>
      public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
      {
        if (value != null && destinationType == typeof(string))
          return GetStringValueFromResources(culture, value);

        return base.ConvertTo(context, culture, value, destinationType);
      }

      #endregion
    }

    #endregion

    #region UI Type Editors

    /// <summary>
    /// Represents a <see cref="System.Windows.Forms.Design.FileNameEditor"/> that can be set to read-only
    /// and look like it is read-only.
    /// </summary>
    /// <remarks>
    /// This class exists solely to overcome what we view as a problem with the original <see cref="System.Windows.Forms.Design.FileNameEditor"/>,
    /// since it still looks editable, even when it is read-only.
    /// </remarks>
    /// <seealso cref="System.Windows.Forms.Design.FileNameEditor"/>
    private class ReadOnlyFileNameEditor : System.Windows.Forms.Design.FileNameEditor
    {
      /// <summary>
      /// Gets the editing style used by the <see cref="System.Windows.Forms.Design.FileNameEditor.EditValue(System.ComponentModel.ITypeDescriptorContext,System.IServiceProvider,System.Object)"/>
      /// method.
      /// </summary>
      /// <param name="context">An <see cref="System.ComponentModel.ITypeDescriptorContext"/> that can be used to gain
      /// additional context information.</param>
      /// <returns>If it is read-only, returns <see cref="System.Drawing.Design.UITypeEditorEditStyle.None"/>. Otherwise, it returns one
      /// of the <see cref="System.Drawing.Design.UITypeEditorEditStyle"/> values indicating the provided editing style.</returns>
      public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
      {
        // If this is not done, the field does not appear to be readonly in the property grid.
        if (context.PropertyDescriptor.IsReadOnly)
          return UITypeEditorEditStyle.None;
        
        return base.GetEditStyle(context);
      }
    }

    #endregion

    #region Property Descriptors

    /// <summary>
    /// Represents a <see cref="System.ComponentModel.PropertyDescriptor"/> for which the "browsable"
    /// attribute can be changed at runtime.
    /// </summary>
    private class BrowsablePropertyDescriptor : ForwardingPropertyDescriptor
    {
      #region Private Fields

      /// <summary>
      /// Whether the property is browsable.
      /// </summary>
      private bool m_isBrowsable;

      #endregion

      #region Properties

      /// <summary>
      /// Gets whether the property is browsable.
      /// </summary>
      public override bool IsBrowsable
      {
        get { return m_isBrowsable; }
      }

      #endregion

      #region Constructors

      /// <summary>
      /// Creates a new property descriptor, wrapping the given one.
      /// </summary>
      /// <param name="root">The <see cref="System.ComponentModel.PropertyDescriptor"/> to wrap and to forward method calls to.</param>
      public BrowsablePropertyDescriptor(PropertyDescriptor root)
        : base(root)
      {
        SetIsBrowsable(root.IsBrowsable);
      }

      #endregion

      #region Public Methods

      /// <summary>
      /// Sets whether the property is browsable.
      /// </summary>
      /// <param name="isBrowsable">Whether the property is browsable.</param>
      public void SetIsBrowsable(bool isBrowsable)
      {
        this.m_isBrowsable = isBrowsable;
      }

      #endregion

      #region ForwardingPropertyDescriptor Interface Overrides

      /// <summary>
      /// Gets the collection of attributes for this member.
      /// </summary>
      /// <remarks>
      /// This replaces the <see cref="System.ComponentModel.BrowsableAttribute"/>, if any,
      /// with a new one to represent its chosen value.
      /// </remarks>
      public override AttributeCollection Attributes
      {
        get
        {
          AttributeCollection attributes = base.Attributes;
          List<Attribute> lstAttr = new List<Attribute>();
          foreach (Attribute attr in attributes)
            if (!(attr is BrowsableAttribute))
              lstAttr.Add(attr);

          // The Browsable attribute is not necessarily there, as it defaults to true.
          lstAttr.Add(new BrowsableAttribute(m_isBrowsable));

          return new AttributeCollection(lstAttr.ToArray());
        }
      }

      #endregion
    }

    /// <summary>
    /// Represents a <see cref="System.ComponentModel.PropertyDescriptor"/> for which the "readonly"
    /// attribute can be changed at runtime.
    /// </summary>
    private class ReadOnlyPropertyDescriptor : ForwardingPropertyDescriptor
    {
      #region Private Fields

      /// <summary>
      /// Whether the property is read-only.
      /// </summary>
      private bool m_readOnly;

      #endregion

      #region Properties

      /// <summary>
      /// Gets whether the property is read-only.
      /// </summary>
      public override bool IsReadOnly
      {
        get { return this.m_readOnly; }
      }

      #endregion

      #region Constructors

      /// <summary>
      /// Creates a new property descriptor, wrapping the given one.
      /// </summary>
      /// <param name="root">The <see cref="System.ComponentModel.PropertyDescriptor"/> to wrap and to forward method calls to.</param>
      public ReadOnlyPropertyDescriptor(PropertyDescriptor root)
        : base(root)
      {
        this.SetReadOnly(false);
      }

      #endregion

      #region Public Methods

      /// <summary>
      /// Sets whether the property is read-only.
      /// </summary>
      /// <param name="readOnly">Whether the property is read-only.</param>
      public void SetReadOnly(bool readOnly)
      {
        this.m_readOnly = readOnly;
      }

      #endregion

      #region ForwardingPropertyDescriptor Interface Overrides

      /// <summary>
      /// Gets the collection of attributes for this member.
      /// </summary>
      /// <remarks>
      /// This replaces the <see cref="System.ComponentModel.ReadOnlyAttribute"/>, if any,
      /// with a new one to represent its chosen value.
      /// </remarks>
      public override AttributeCollection Attributes
      {
        get
        {
          AttributeCollection attributes = base.Attributes;
          List<Attribute> lstAttr = new List<Attribute>();
          foreach (Attribute attr in attributes)
            if (attr is ReadOnlyAttribute)
              lstAttr.Add(new ReadOnlyAttribute(this.m_readOnly));
            else
              lstAttr.Add(attr);

          return new AttributeCollection(lstAttr.ToArray());
        }
      }

      #endregion
    }

    #endregion

    #endregion

    #region Private Fields

    /// <summary>
    /// The graph search strategy.
    /// </summary>
    private GraphSearchStrategyOrAll m_graphSearchStrategy;
    /// <summary>
    /// The world implementation type.
    /// </summary>
    private WorldTypeOrAll m_worldType;
    /// <summary>
    /// Whether to validate the plan.
    /// </summary>
    private bool m_validatePlan;

    /// <summary>
    /// A dictionary that maps property names to their corresponding <see cref="System.ComponentModel.PropertyDescriptor"/>.
    /// </summary>
    private IDictionary<string, PropertyDescriptor> m_allPropertyDescriptors;
    /// <summary>
    /// A dictionary that maps property names to their corresponding <see cref="GUI.GUITLPlanOptions.ReadOnlyPropertyDescriptor"/>.
    /// </summary>
    private IDictionary<string, ReadOnlyPropertyDescriptor> m_readOnlyDescriptors;
    /// <summary>
    /// A dictionary that maps property names to their corresponding <see cref="GUI.GUITLPlanOptions.BrowsablePropertyDescriptor"/>.
    /// </summary>
    private IDictionary<string, BrowsablePropertyDescriptor> m_browsableDescriptors;

    #endregion

    #region Enumerations

    /// <summary>
    /// A boolean value.
    /// </summary>
    /// <remarks>The <see cref="BoolOrAll.All"/> value means all the enumeration values will
    /// be used, each with a different planner.</remarks>
    [TypeConverter(typeof(LocalizableEnumConverter))]
    public enum BoolOrAll
    {
      /// <summary>
      /// The true value.
      /// </summary>
      True,
      /// <summary>
      /// The false value.
      /// </summary>
      False,
      /// <summary>
      /// Use all values.
      /// </summary>
      All
    }

    /// <summary>
    /// All possible world types.
    /// </summary>
    /// <remarks>The <see cref="WorldTypeOrAll.All"/> value means all the enumeration values will
    /// be used, each with a different planner.</remarks>
    [TypeConverter(typeof(LocalizableEnumConverter))]
    public enum WorldTypeOrAll
    {
      /// <summary>
      /// A qualified world uses a hashset of qualified predicates and fluents.
      /// </summary>
      Qualified,
      /// <summary>
      /// A custom world may be initialized with any combination of facts/fluents container.
      /// </summary>
      Custom,
      /// <summary>
      /// Use all values.
      /// </summary>
      All
    }

    /// <summary>
    /// All possible facts container types. Note that the facts container type is 
    /// defined only if the world type is Custom.
    /// </summary>
    /// <remarks>The <see cref="FactsContainerTypeOrAll.All"/> value means all the enumeration values will
    /// be used, each with a different planner.</remarks>
    [TypeConverter(typeof(LocalizableEnumConverter))]
    public enum FactsContainerTypeOrAll
    {
      /// <summary>
      /// A bitset facts container stores predicates in a fixed bitset.
      /// </summary>
      Bitset,
      /// <summary>
      /// A hashset facts container stores predicates in a hashset.
      /// </summary>
      Hashset,
      /// <summary>
      /// A treeset facts container stores predicates in a treeset.
      /// </summary>
      Treeset,
      /// <summary>
      /// Use all values.
      /// </summary>
      All
    };

    /// <summary>
    /// All possible fluents container types. Note that the fluents container type is
    /// defined only if the world type is Custom.
    /// </summary>
    /// <remarks>The <see cref="FluentsContainerTypeOrAll.All"/> value means all the enumeration values will
    /// be used, each with a different planner.</remarks>
    [TypeConverter(typeof(LocalizableEnumConverter))]
    public enum FluentsContainerTypeOrAll
    {
      /// <summary>
      /// An array fluents container stores fluents in a fixed array.
      /// </summary>
      Array,
      /// <summary>
      /// A hashmap fluents container stores fluents in a hashmap.
      /// </summary>
      Hashmap,
      /// <summary>
      /// A treemap fluents container stores fluents in a treemap.
      /// </summary>
      Treemap,
      /// <summary>
      /// Use all values.
      /// </summary>
      All
    };

    /// <summary>
    /// All possible sets implementation type. This is used to generate the appropriate
    /// implementation for the open and closed set needed in certain search strategies.
    /// </summary>
    /// <remarks>The <see cref="SetImplementationTypeOrAll.All"/> value means all the enumeration values will
    /// be used, each with a different planner.</remarks>
    [TypeConverter(typeof(LocalizableEnumConverter))]
    public enum SetImplementationTypeOrAll
    {
      /// <summary>
      /// Generates hashsets, whose elements are not sorted.
      /// </summary>
      Hashset,
      /// <summary>
      /// Generates treesets, whose elements are sorted.
      /// </summary>
      Treeset,
      /// <summary>
      /// Use all values.
      /// </summary>
      All
    }

    /// <summary>
    /// All possible graph search strategies.
    /// </summary>
    /// <remarks>The <see cref="GraphSearchStrategyOrAll.All"/> value means all the enumeration values will
    /// be used, each with a different planner.</remarks>
    [TypeConverter(typeof(LocalizableEnumConverter))]
    public enum GraphSearchStrategyOrAll
    {
      /// <summary>
      /// Explores the most promising worlds first.
      /// </summary>
      BestFirst,
      /// <summary>
      /// Explores all neighborhing nodes first.
      /// </summary>
      BreadthFirst,
      /// <summary>
      /// Explores all neighborhing nodes first, try the actions with the highest priority first.
      /// </summary>
      BreadthFirstPriority,
      /// <summary>
      /// Explores as far as possible along each branch before backtracking.
      /// </summary>
      DepthFirst,
      /// <summary>
      /// Explores as far as possible along each branch before backtracking, 
      /// try the actions with the highest priority first.
      /// </summary>
      DepthFirstPriority,
      /// <summary>
      /// Explores as far as possible along a single branch. The search fails if a node
      /// has no valid successors.
      /// </summary>
      DepthFirstNoBacktracking,
      /// <summary>
      /// Explores as far as possible along each branch before backtracking,
      /// try the most promising worlds first.
      /// </summary>
      DepthBestFirst,
      /// <summary>
      /// Use all values.
      /// </summary>
      All
    };

    /// <summary>
    /// All possible levels of preprocessing. The preprocessing phase takes place before the
    /// actual graph search process and uses the invariants to simplify actions preconditions.
    /// </summary>
    /// <remarks>The <see cref="PreprocessingLevelOrAll.All"/> value means all the enumeration values will
    /// be used, each with a different planner.</remarks>
    [TypeConverter(typeof(LocalizableEnumConverter))]
    public enum PreprocessingLevelOrAll
    {
      /// <summary>
      /// Do not preprocess preconditions.
      /// </summary>
      NoPreprocessing = 0,
      /// <summary>
      /// Preprocess all preconditions except the defined formulas.
      /// </summary>
      PreprocessPreconditions = 1,
      /// <summary>
      /// Preprocess all preconditions.
      /// </summary>
      PreprocessPreconditionsAndDefinedFormulas = 2,
      /// <summary>
      /// Use all values.
      /// </summary>
      All = 3
    };


    #endregion

    #region Properties

    /// <summary>
    /// The domain file to use.
    /// </summary>
    [Category("Files")]
    [DisplayName("Domain File")]
    [Description("The domain PDDL file.")]
    [EditorAttribute(typeof(ReadOnlyFileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
    public string DomainFile { get; set; }

    /// <summary>
    /// The problem file to use.
    /// </summary>
    [Category("Files")]
    [DisplayName("Problem File")]
    [Description("The problem PDDL file, which corresponds to the given domain.")]
    [EditorAttribute(typeof(ReadOnlyFileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
    public string ProblemFile { get; set; }


    /// <summary>
    /// The graph search strategy to use.
    /// </summary>
    [Category("Options")]
    [DisplayName("Search Strategy")]
    [Description("The graph search strategy used when solving.")]
    [DefaultValue(GraphSearchStrategyOrAll.BestFirst)]
    [RefreshProperties(RefreshProperties.All)]
    public GraphSearchStrategyOrAll SearchStrategy
    {
      get
      {
        return m_graphSearchStrategy;
      }
      set
      {
        m_graphSearchStrategy = value;
        if (value == GraphSearchStrategyOrAll.DepthFirstNoBacktracking)
          this.UseBacktracking = BoolOrAll.False;

        m_readOnlyDescriptors["UseBacktracking"].SetReadOnly(value == GraphSearchStrategyOrAll.DepthFirstNoBacktracking);
      }
    }

    /// <summary>
    /// The world implementation to use.
    /// </summary>
    [Category("Options")]
    [DisplayName("World Implementation")]
    [Description("The world implementation used when solving.")]
    [DefaultValue(WorldTypeOrAll.Qualified)]
    [RefreshProperties(RefreshProperties.All)]
    public WorldTypeOrAll WorldImplementation 
    {
      get { return m_worldType; }
      set
      {
        m_worldType = value;
        if (value == WorldTypeOrAll.Qualified)
        {
          this.FactsContainerImplementation = FactsContainerTypeOrAll.Hashset;
          this.FluentsContainerImplementation = FluentsContainerTypeOrAll.Hashmap;
        }

        m_readOnlyDescriptors["FactsContainerImplementation"].SetReadOnly(m_worldType == WorldTypeOrAll.Qualified);
        m_readOnlyDescriptors["FluentsContainerImplementation"].SetReadOnly(m_worldType == WorldTypeOrAll.Qualified);
      }
    }

    /// <summary>
    /// The facts container implementation to use. Note that this is only defined if the world
    /// implementation is custom.
    /// </summary>
    [Category("Options")]
    [DisplayName("Facts Container Implementation")]
    [Description("The implementation of the facts container used when solving.")]
    [DefaultValue(FactsContainerTypeOrAll.Hashset)]
    public FactsContainerTypeOrAll FactsContainerImplementation { get; set; }

    /// <summary>
    /// The fluents container implementation to use. Note that this is only defined if the world
    /// implementation is custom.
    /// </summary>
    [Category("Options")]
    [DisplayName("Fluents Container Implementation")]
    [Description("The implementation of the fluents container used when solving.")]
    [DefaultValue(FluentsContainerTypeOrAll.Hashmap)]
    public FluentsContainerTypeOrAll FluentsContainerImplementation { get; set; }

    /// <summary>
    /// The sets implementation. This is used to generate the appropriate
    /// implementation for the open and closed set needed in certain search strategies.
    /// </summary>
    [Category("Options")]
    [DisplayName("Algorithm Structures Implementation")]
    [Description("The implementation of the search algorithm's structures.")]
    [DefaultValue(SetImplementationTypeOrAll.Hashset)]
    public SetImplementationTypeOrAll SetStructureImplementation { get; set; }

    /// <summary>
    /// Whether invariants should be computed and stored separately from the each 
    /// world generated during the search process.
    /// </summary>
    [Category("Options")]
    [DisplayName("Compute Invariants")]
    [Description("Whether to compute information about invariants. If this option is not set, all invariant information is copied in each new world.")]
    [DefaultValue(BoolOrAll.True)]
    public BoolOrAll ComputeInvariants { get; set; }

    /// <summary>
    /// The level of preprocessing to complete before starting the actual graph search process.
    /// The preprocessing phase is used to simplify actions preconditions.
    /// </summary>
    [Category("Options")]
    [DisplayName("Preprocessing level")]
    [Description("Specifies the preprocessing level of the actions' preconditions and of the goal.")]
    [DefaultValue(PreprocessingLevelOrAll.PreprocessPreconditionsAndDefinedFormulas)]
    public PreprocessingLevelOrAll PreprocessLevel { get; set; }

    /// <summary>
    /// If true, immediately prune successors using the temporal control formula.
    /// Else, discard successors only when they are the next ones to be explored.
    /// </summary>
    [Category("Options")]
    [DisplayName("Immediately Prune Successors")]
    [Description("Whether to immediately prune all successors using the temporal control formula.")]
    [DefaultValue(BoolOrAll.False)]
    public BoolOrAll ImmediatelyPruneSuccessors { get; set; }

    /// <summary>
    /// Whether to check for cycles when exploring the graph.
    /// If set to false, the search may never return since it may explore cycles 
    /// ad vitam eternam.
    /// </summary>
    [Category("Options")]
    [DisplayName("Check Cycles")]
    [Description("Whether to verify that a newly created world has already been visited. Setting this to false may lead to an infite loop while planning. In specific domains, disabling cycle checking may lead to better performance.")]
    [DefaultValue(BoolOrAll.True)]
    public BoolOrAll CycleChecking { get; set; }

    /// <summary>
    /// Whether to use backtracking when exploring the graph.
    /// If set to false, all successors of a world, except the first one, are discarded.
    /// </summary>
    [Category("Options")]
    [DisplayName("Use Backtracking")]
    [Description("Whether to allow backtracking while planning. When disabled, the planner effectively discards all successors except the first (viable) one.\n\nEvent if a trajectory to the goal exists, when backtracking is disabled, the planner may nevertheless fail to find a plan.")]
    [DefaultValue(BoolOrAll.True)]
    public BoolOrAll UseBacktracking { get; set; }

    /// <summary>
    /// Whether to support concurrency in planning.
    /// If set to false, only sequential plans are generated.
    /// </summary>
    [Category("Options")]
    [DisplayName("Use Concurrent Actions")]
    [Description("Whether to allow concurrent actions (non-concurrent actions leads to STRIPS-like planning).")]
    [DefaultValue(BoolOrAll.True)]
    public BoolOrAll ConcurrentActions { get; set; }

    /// <summary>
    /// Whether to allow the use of undefined fluents.
    /// If set to false, the program throws an exception whenever an undefined fluent is
    /// evaluated.
    /// </summary>
    [Category("Options")]
    [DisplayName("Allow Undefined Fluents")]
    [Description("Whether to allow the use of undefined fluents. If this options is not set, the planner will stop if it encounters an undefined fluent.")]
    [DefaultValue(true)]
    public bool AllowUndefinedFluents { get; set; }

    /// <summary>
    /// Whether to hide the wait-for-next-event in plans.
    /// </summary>
    [Category("Options")]
    [DisplayName("Elide Wait Event")]
    [Description("Whether to elide the wait-for-next-event from plans. When disabled, the plan cannot be validated.")]
    [DefaultValue(true)]
    public bool ElideWaitEvent { get; set; }

    /// <summary>
    /// Whether to validate the plan (using the validator) when one is found.
    /// </summary>
    [Category("Validation")]
    [DisplayName("Validate Plan")]
    [Description("Whether to validate the plan computed by the planner.")]
    [DefaultValue(false)]
    [RefreshProperties(RefreshProperties.All)]
    public bool ValidatePlan 
    {
      get { return m_validatePlan; }
      set
      {
        m_validatePlan = value;
        m_readOnlyDescriptors["ValidationDomain"].SetReadOnly(!value);
        if (value && string.IsNullOrEmpty(ValidationDomain))
          ValidationDomain = DomainFile;
      }
    }

    /// <summary>
    /// The PDDL domain used for validation. This domain must be PDDL3.0-compliant
    /// (no TLPlan special operators, no object fluents, ...).
    /// </summary>
    [Category("Validation")]
    [DisplayName("Validation Domain File")]
    [Description("The PDDL3.0-compliant domain (which should be equivalent to the main domain) used by the validator.\n\nIf this field is empty and validation is required, the main PDDL domain will be used.")]
    [EditorAttribute(typeof(ReadOnlyFileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
    public string ValidationDomain { get; set; }

    /// <summary>
    /// The maximum number of samples to visit during the search.
    /// Set this value to 0 for no limit.
    /// </summary>
    [Category("Limits")]
    [DisplayName("Search Limit")]
    [Description("Specifies the limit on the number of worlds to explore. A value of zero means no limit.")]
    [DefaultValue(0)]
    public int SearchLimit { get; set; }

    /// <summary>
    /// The maximum time limit during the search.
    /// Set this value to 0 for no limit.
    /// </summary>
    [Category("Limits")]
    [DisplayName("Time Limit")]
    [Description("Specifies the time limit within which the planner must return. A value of zero means no limit.")]
    [DefaultValue(typeof(TimeSpan), "0")]
    [TypeConverter(typeof(TimeLimitConverter))]
    public TimeSpan TimeLimit { get; set; }

    /// <summary>
    /// Whether to pause on the initial world when planning.
    /// </summary>
    [Category("Flow Options")]
    [DisplayName("Pause On Initial World")]
    [Description("Whether to pause when the initial world is reached (i.e. at the start of the search).")]
    [DefaultValue(true)]
    public bool PauseOnInitialWorld { get; set; }

    /// <summary>
    /// Whether to pause on the goal world when planning.
    /// </summary>
    [Category("Flow Options")]
    [DisplayName("Pause On Goal World")]
    [Description("Whether to pause when the goal world is reached (i.e. at the end of the search).")]
    [DefaultValue(false)]
    public bool PauseOnGoalWorld { get; set; }

    /// <summary>
    /// Gets the number of <see cref="TLPlan.TLPlanOptions"/> instances that will be created from
    /// the current options.
    /// </summary>
    [Browsable(false)]
    [ReadOnly(true)]
    public int Count
    {
      get
      {
        int optionCount = 1;
        int worldOptionCount = 1;
        int allWorldImplementation = 0;

        foreach (PropertyDescriptor prop in  ((ICustomTypeDescriptor)this).GetProperties())
        {
          if (prop.PropertyType.IsSubclassOf(typeof(Enum)) && !prop.IsReadOnly)
          {
            Array values = Enum.GetValues(prop.PropertyType);
            if (prop.GetValue(this).Equals(values.GetValue(values.Length - 1)))
              if (prop.Name == "WorldImplementation")
                ++allWorldImplementation;
              else if (prop.Name == "FactsContainerImplementation" || prop.Name == "FluentsContainerImplementation")
                worldOptionCount *= (values.Length - 1);
              else
                optionCount *= (values.Length - 1);
          }
        }

        return optionCount * (allWorldImplementation + worldOptionCount);
      }
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a new instance of GUI options with default values.
    /// </summary>
    public GUITLPlanOptions()
    {
      InitializePropertyDescriptors();

      DomainFile = string.Empty;
      ProblemFile = string.Empty;
      ValidationDomain = string.Empty;

      ComputeInvariants = BoolOrAll.True;
      PreprocessLevel = PreprocessingLevelOrAll.PreprocessPreconditionsAndDefinedFormulas;
      ImmediatelyPruneSuccessors = BoolOrAll.False;
      ConcurrentActions = BoolOrAll.True;
      ElideWaitEvent = true;
      AllowUndefinedFluents = true;
      CycleChecking = BoolOrAll.True;
      UseBacktracking = BoolOrAll.True;
      ValidatePlan = false;

      SearchStrategy = GraphSearchStrategyOrAll.BestFirst;
      WorldImplementation = WorldTypeOrAll.Qualified;
      SetStructureImplementation = SetImplementationTypeOrAll.Hashset;

      FactsContainerImplementation = FactsContainerTypeOrAll.Hashset;
      FluentsContainerImplementation = FluentsContainerTypeOrAll.Hashmap;

      SearchLimit = 0;
      TimeLimit = new TimeSpan(0);

      PauseOnInitialWorld = true;
      PauseOnGoalWorld = false;
    }

    /// <summary>
    /// Creates a new instance of GUI options with the specified values.
    /// </summary>
    /// <param name="options">The desired options.</param>
    public GUITLPlanOptions(TLPlanOptions options)
    {
      InitializePropertyDescriptors();

      DomainFile = options.Domain;
      ProblemFile = options.Problem;
      ValidationDomain = options.ValidationDomain;

      ComputeInvariants = options.ComputeInvariants ? BoolOrAll.True : BoolOrAll.False;
      PreprocessLevel = (PreprocessingLevelOrAll)options.PreprocessLevel;
      ImmediatelyPruneSuccessors = options.ImmediatelyPruneSuccessors ? BoolOrAll.True : BoolOrAll.False;
      ConcurrentActions = options.ConcurrentActions ? BoolOrAll.True : BoolOrAll.False;
      ElideWaitEvent = options.ElideWaitEvent;
      AllowUndefinedFluents = options.AllowUndefinedFluents;
      CycleChecking = options.CycleChecking ? BoolOrAll.True : BoolOrAll.False;
      UseBacktracking = options.UseBacktracking ? BoolOrAll.True : BoolOrAll.False;
      ValidatePlan = options.ValidatePlan;

      SearchStrategy = (GraphSearchStrategyOrAll)options.SearchStrategy;
      WorldImplementation = (WorldTypeOrAll)options.WorldImp;
      SetStructureImplementation = (SetImplementationTypeOrAll)options.SetStructureImp;

      FactsContainerImplementation = (FactsContainerTypeOrAll)options.FactsContainerImp;
      FluentsContainerImplementation = (FluentsContainerTypeOrAll)options.FluentsContainerImp;

      SearchLimit = options.SearchLimit;
      TimeLimit = options.TimeLimit;

      PauseOnInitialWorld = true;
      PauseOnGoalWorld = false;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Makes all browsables properties read-only.
    /// </summary>
    public void MakeReadOnly()
    {
      foreach (ReadOnlyPropertyDescriptor prop in m_readOnlyDescriptors.Values)
        prop.SetReadOnly(true);
    }

    /// <summary>
    /// Make all non-TLPlan options non-browsable.
    /// </summary>
    public void HideNonTLPlanOptions()
    {
      foreach (BrowsablePropertyDescriptor prop in m_browsableDescriptors.Values)
        prop.SetIsBrowsable(false);
    }

    /// <summary>
    /// Returns these options' equivalent instance of <see cref="TLPlan.TLPlanOptions"/>.
    /// </summary>
    /// <remarks>
    /// This method only works on instances that have no option set to "All".
    /// </remarks>
    /// <returns>An instance of <see cref="TLPlan.TLPlanOptions"/> that has the same options as this object.</returns>
    public TLPlanOptions ConvertToTLPlanOptions()
    {
      // This method only works on GUITLPlanOptions instances that have no option set to "All".
      TLPlanOptions options = new TLPlanOptions();

      options.SetDomain(this.DomainFile);
      options.SetProblem(this.ProblemFile);
      options.SetValidationDomain(this.ValidationDomain);

      options.SearchStrategy = (TLPlanOptions.GraphSearchStrategy)this.SearchStrategy;
      options.WorldImp = (TLPlanOptions.WorldType)this.WorldImplementation;
      options.SetStructureImp = (TLPlanOptions.SetImplementationType)this.SetStructureImplementation;

      options.FactsContainerImp = (TLPlanOptions.FactsContainerType)this.FactsContainerImplementation;
      options.FluentsContainerImp = (TLPlanOptions.FluentsContainerType)this.FluentsContainerImplementation;

      options.PreprocessLevel = (TLPlanOptions.PreprocessingLevel)this.PreprocessLevel;
      options.ComputeInvariants = this.ComputeInvariants == BoolOrAll.True;
      options.ImmediatelyPruneSuccessors = this.ImmediatelyPruneSuccessors == BoolOrAll.True;
      options.ConcurrentActions = this.ConcurrentActions == BoolOrAll.True;
      options.ElideWaitEvent = this.ElideWaitEvent;
      options.AllowUndefinedFluents = this.AllowUndefinedFluents;
      options.CycleChecking = this.CycleChecking == BoolOrAll.True;
      options.UseBacktracking = this.UseBacktracking == BoolOrAll.True;
      options.ValidatePlan = this.ValidatePlan;

      options.SearchLimit = this.SearchLimit;
      options.TimeLimit = this.TimeLimit;

      return options;
    }

    /// <summary>
    /// Returns an enumeration of all <see cref="GUITLPlanOptions"/> corresponding to this object, with no option set to "All".
    /// </summary>
    /// <returns>An enumeration of all <see cref="GUITLPlanOptions"/> corresponding to this object, with no option set to "All".</returns>
    public IEnumerable<GUITLPlanOptions> GetSelectedGUITLPlanOptions()
    {
      foreach (TLPlanOptions options in GetSelectedTLPlanOptions())
      {
        GUITLPlanOptions guiOptions = new GUITLPlanOptions(options);

        guiOptions.PauseOnInitialWorld = this.PauseOnInitialWorld;
        guiOptions.PauseOnGoalWorld = this.PauseOnGoalWorld;

        yield return guiOptions;
      }
    }

    /// <summary>
    /// Returns an enumeration of all <see cref="TLPlan.TLPlanOptions"/> corresponding to this object.
    /// </summary>
    /// <returns>An enumeration of all <see cref="TLPlan.TLPlanOptions"/> corresponding to this object.</returns>
    public IEnumerable<TLPlanOptions> GetSelectedTLPlanOptions()
    {
      TLPlanOptions options = new TLPlanOptions();
      List<string> traversedProperties = new List<string>();

      options.SetDomain(this.DomainFile);
      options.SetProblem(this.ProblemFile);
      options.SetValidationDomain(this.ValidationDomain);

      options.ElideWaitEvent = this.ElideWaitEvent;
      options.AllowUndefinedFluents = this.AllowUndefinedFluents;
      options.ValidatePlan = this.ValidatePlan;

      options.SearchLimit = this.SearchLimit;
      options.TimeLimit = this.TimeLimit;

      if (this.SearchStrategy != GraphSearchStrategyOrAll.All)
        options.SearchStrategy = (TLPlanOptions.GraphSearchStrategy)this.SearchStrategy;
      else
        traversedProperties.Add("SearchStrategy");

      if (this.WorldImplementation != WorldTypeOrAll.All)
        options.WorldImp = (TLPlanOptions.WorldType)this.WorldImplementation;
      else
        traversedProperties.Add("WorldImp");

      if (this.FactsContainerImplementation != FactsContainerTypeOrAll.All)
        options.FactsContainerImp = (TLPlanOptions.FactsContainerType)this.FactsContainerImplementation;
      else
        traversedProperties.Add("FactsContainerImp");

      if (this.FluentsContainerImplementation != FluentsContainerTypeOrAll.All)
        options.FluentsContainerImp = (TLPlanOptions.FluentsContainerType)this.FluentsContainerImplementation;
      else
        traversedProperties.Add("FluentsContainerImp");

      if (this.SetStructureImplementation != SetImplementationTypeOrAll.All)
        options.SetStructureImp = (TLPlanOptions.SetImplementationType)this.SetStructureImplementation;
      else
        traversedProperties.Add("StructureImp");

      if (this.ComputeInvariants != BoolOrAll.All)
        options.ComputeInvariants = this.ComputeInvariants == BoolOrAll.True;
      else
        traversedProperties.Add("ComputeInvariants");

      if (this.PreprocessLevel != PreprocessingLevelOrAll.All)
        options.PreprocessLevel = (TLPlanOptions.PreprocessingLevel)this.PreprocessLevel;
      else
        traversedProperties.Add("PreprocessLevel");

      if (this.ImmediatelyPruneSuccessors != BoolOrAll.All)
        options.ImmediatelyPruneSuccessors = this.ImmediatelyPruneSuccessors == BoolOrAll.True;
      else
        traversedProperties.Add("ImmediatelyPruneSuccessors");

      if (this.ConcurrentActions != BoolOrAll.All)
        options.ConcurrentActions = this.ConcurrentActions == BoolOrAll.True;
      else
        traversedProperties.Add("ConcurrentActions");

      if (this.UseBacktracking != BoolOrAll.All)
        options.UseBacktracking = this.UseBacktracking == BoolOrAll.True;
      else
        traversedProperties.Add("UseBacktracking");

      if (this.CycleChecking != BoolOrAll.All)
        options.CycleChecking = this.CycleChecking == BoolOrAll.True;
      else
        traversedProperties.Add("CycleChecking");

      if (traversedProperties.Count == 0)
        return options.Once();
      else
      {
        // Retrieve all properties that need be traversed
        List<PropertyInfo> properties = new List<PropertyInfo>(typeof(TLPlanOptions).GetProperties()
          .Where(p => traversedProperties.Contains(p.Name)));

        return TLPlanOptions.GetAllOptions<TLPlanOptions.BehaviorAttribute>(properties, 0, options, true);
      }
    }

    #endregion

    #region ICustomTypeDescriptor Interface

    /// <summary>
    /// Returns a collection of custom attributes for this instance of a component.
    /// </summary>
    /// <returns>An <see cref="System.ComponentModel.AttributeCollection"/> containing the attributes for
    /// this object.</returns>
    AttributeCollection ICustomTypeDescriptor.GetAttributes()
    {
      return TypeDescriptor.GetAttributes(GetType());
    }

    /// <summary>
    /// Returns the class name of this instance of a component.
    /// </summary>
    /// <returns>The class name of the object, or null if the class does not have a name.</returns>
    string ICustomTypeDescriptor.GetClassName()
    {
      return TypeDescriptor.GetClassName(GetType());
    }

    /// <summary>
    /// Returns the name of this instance of a component.
    /// </summary>
    /// <returns>The name of the object, or null if the object does not have a name.</returns>
    string ICustomTypeDescriptor.GetComponentName()
    {
      return TypeDescriptor.GetComponentName(GetType());
    }

    /// <summary>
    /// Returns a type converter for this instance of a component.
    /// </summary>
    /// <returns>A <see cref="System.ComponentModel.TypeConverter"/> that is the converter for this object,
    /// or null if there is no <see cref="System.ComponentModel.TypeConverter"/> for this object.</returns>
    TypeConverter ICustomTypeDescriptor.GetConverter()
    {
      return TypeDescriptor.GetConverter(GetType());
    }

    /// <summary>
    /// Returns the default event for this instance of a component.
    /// </summary>
    /// <returns>An <see cref="System.ComponentModel.EventDescriptor"/> that represents the default event
    /// for this object, or null if this object does not have events.</returns>
    EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
    {
      return TypeDescriptor.GetDefaultEvent(GetType());
    }
    
    /// <summary>
    /// Returns the default property for this instance of a component.
    /// </summary>
    /// <returns>A <see cref="System.ComponentModel.PropertyDescriptor"/> that represents the default property
    /// for this object, or null if this object does not have properties.</returns>
    PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
    {
      return TypeDescriptor.GetDefaultProperty(GetType());
    }

    /// <summary>
    /// Returns an editor of the specified type for this instance of a component.
    /// </summary>
    /// <param name="editorBaseType">A <see cref="System.Type"/> that represents the editor for this object.</param>
    /// <returns>An <see cref="System.Object"/> of the specified type that is the editor for this object,
    /// or null if the editor cannot be found.</returns>
    object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
    {
      return TypeDescriptor.GetEditor(GetType(), editorBaseType);
    }

    /// <summary>
    /// Returns the events for this instance of a component using the specified attribute
    /// array as a filter.
    /// </summary>
    /// <param name="attributes">An array of type <see cref="System.Attribute"/> that is used as a filter.</param>
    /// <returns>An <see cref="System.ComponentModel.EventDescriptorCollection"/> that represents the filtered
    /// events for this component instance.</returns>
    EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
    {
      return TypeDescriptor.GetEvents(GetType(), attributes);
    }

    /// <summary>
    /// Returns the events for this instance of a component.
    /// </summary>
    /// <returns>An <see cref="System.ComponentModel.EventDescriptorCollection"/> that represents the events
    /// for this component instance.</returns>
    EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
    {
      return TypeDescriptor.GetEvents(GetType());
    }

    /// <summary>
    /// Returns the properties for this instance of a component using the attribute
    /// array as a filter.
    /// </summary>
    /// <param name="attributes">An array of type <see cref="System.Attribute"/> that is used as a filter.</param>
    /// <returns>A <see cref="System.ComponentModel.PropertyDescriptorCollection"/> that represents the
    /// filtered properties for this component instance.</returns>
    PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
    {
      return SwitchProperties(TypeDescriptor.GetProperties(GetType(), attributes));
    }

    /// <summary>
    /// Returns the properties for this instance of a component.
    /// </summary>
    /// <returns>A <see cref="System.ComponentModel.PropertyDescriptorCollection"/> that represents the
    /// properties for this component instance.</returns>
    PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
    {
      return SwitchProperties(TypeDescriptor.GetProperties(GetType()));
    }

    /// <summary>
    /// Returns an object that contains the property described by the specified property
    /// descriptor.
    /// </summary>
    /// <param name="pd">A <see cref="System.ComponentModel.PropertyDescriptor"/> that represents the property whose
    /// owner is to be found.</param>
    /// <returns>An <see cref="System.Object"/> that represents the owner of the specified property.</returns>
    object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
    {
      return this;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Switches the property descriptors in the collection by the ones we modified.
    /// </summary>
    /// <param name="properties">The collection of property descriptors to modify.</param>
    /// <returns>A collection of property descriptors containing the modifiable ones.</returns>
    /// <seealso cref="System.ComponentModel.PropertyDescriptor"/>
    private PropertyDescriptorCollection SwitchProperties(PropertyDescriptorCollection properties)
    {
      List<PropertyDescriptor> propList = new List<PropertyDescriptor>();
      foreach (PropertyDescriptor prop in properties)
      {
        PropertyDescriptor outProp;
        if (m_allPropertyDescriptors.TryGetValue(prop.Name, out outProp))
          propList.Add(outProp);
        else
          propList.Add(prop);
      }

      return new PropertyDescriptorCollection(propList.ToArray(), true);
    }

    /// <summary>
    /// Initializes all the modifiable property descriptors.
    /// </summary>
    private void InitializePropertyDescriptors()
    {
      this.m_allPropertyDescriptors = new Dictionary<string, PropertyDescriptor>();
      this.m_readOnlyDescriptors = new Dictionary<string, ReadOnlyPropertyDescriptor>();
      this.m_browsableDescriptors = new Dictionary<string, BrowsablePropertyDescriptor>();

      foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(typeof(GUITLPlanOptions)))
      {
        PropertyDescriptor modifiedProp = prop;

        if (modifiedProp.Attributes[typeof(BrowsableAttribute)].Equals(BrowsableAttribute.Yes))
        {
          modifiedProp = new ReadOnlyPropertyDescriptor(modifiedProp);
          this.m_readOnlyDescriptors[prop.Name] = (ReadOnlyPropertyDescriptor)modifiedProp;
        }

        switch (modifiedProp.Name)
        {
          case "PauseOnInitialWorld":
          case "PauseOnGoalWorld":
            modifiedProp = new BrowsablePropertyDescriptor(modifiedProp);
            this.m_browsableDescriptors[modifiedProp.Name] = (BrowsablePropertyDescriptor) modifiedProp;
            break;
        }

        // Contains all property descriptors.
        if (modifiedProp != prop)
          this.m_allPropertyDescriptors[modifiedProp.Name] = modifiedProp;
      }
    }

    #endregion
  }
}
