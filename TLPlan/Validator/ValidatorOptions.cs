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
using System.IO;
using System.Linq;
using System.Text;

namespace TLPlan.Validator
{
  /// <summary>
  /// Represents the options of the PDDL3.0 plan validator.
  /// </summary>
  public class ValidatorOptions
  {
    #region Private Fields

    /// <summary>
    /// The path of the validator executable. Default is "./validator.exe".
    /// </summary>
    private string m_validatorPath;

    /// <summary>
    /// The time difference used to distinguish whether actions are simultaneous. Actions with a time
    /// difference smaller than this value will be considered simultaneous. This defaults to 0.001.
    /// </summary>
    private double? m_concurrencyTolerance;

    /// <summary>
    /// Specifies whether the validator output should be verbose.
    /// </summary>
    private bool m_verbose;

    #endregion

    #region Properties

    /// <summary>
    /// Returns the path of the validator executable.
    /// </summary>
    public string ValidatorPath
    {
      get { return m_validatorPath; }
    }

    /// <summary>
    /// Gets/sets a value indicating whether the validator output should be verbose.
    /// </summary>
    public bool Verbose
    {
      get { return m_verbose; }
      set { m_verbose = value; }
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Creates an instance of this class with default values.
    /// </summary>
    public ValidatorOptions()
      : this(System.Environment.CurrentDirectory + "/validate.exe")
    { }

    /// <summary>
    /// Creates an instance of this class with the specified validator executable path.
    /// </summary>
    /// <param name="validatorPath">The path of the validator program.</param>
    public ValidatorOptions(string validatorPath)
    {
      SetValidatorPath(validatorPath);

      m_verbose = true;
      m_concurrencyTolerance = 0.0005;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Sets the validator executable paths.
    /// </summary>
    /// <param name="path">The path to the validator executable.</param>
    /// <exception cref="ArgumentException">Thrown if the file does not exist.</exception>
    public void SetValidatorPath(string path)
    {
      if (!File.Exists(path))
        throw new ArgumentException("Wrong validator path: file \"" + path + "\" does not exist.");

      m_validatorPath = path;
    }

    /// <summary>
    /// Set the concurrency tolerance to the given value. The concurrenty tolerance is used to
    /// determine whether two actions should be considered as simultaneous.
    /// </summary>
    /// <param name="tolerance">The tolerance threshold.</param>
    public void SetConcurrencyTolerance(double tolerance)
    {
      m_concurrencyTolerance = tolerance;
    }

    /// <summary>
    /// Unsets the concurrency tolerance, using the validator's default value.
    /// </summary>
    public void UnsetConcurrencyTolerance()
    {
      m_concurrencyTolerance = null;
    }

    #endregion

    #region Object Interface Overrides

    /// <summary>
    /// Returns a string representation of these validator options.
    /// </summary>
    /// <returns>A string representation of these validator options.</returns>
    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();

      if (m_verbose)
        sb.Append("-v ");
      if (m_concurrencyTolerance.HasValue)
        sb.Append("-t " + m_concurrencyTolerance.Value + " ");

      return sb.ToString();
    }

    #endregion
  }
}
