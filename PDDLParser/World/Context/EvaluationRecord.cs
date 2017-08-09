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
// Implementation: Simon Chamberland
// Project Manager: Froduald Kabanza
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDDLParser.Exp.Struct;
using Double = PDDLParser.Exp.Struct.Double;

namespace PDDLParser.World.Context
{
  /// <summary>
  /// An evaluation records whether an evaluation is finished and the result of this
  /// evaluation.
  /// </summary>
  /// <typeparam name="T">The type of evaluation record.</typeparam>
  public interface IEvaluationRecord<T>
  {
    /// <summary>
    /// Whether the evaluation has finished.
    /// </summary>
    bool Finished
    {
      get;
      set;
    }

    /// <summary>
    /// The result of the evaluation, which is not defined if the evaluation is in progress.
    /// </summary>
    T Result
    {
      get;
      set;
    }
  }

  /// <summary>
  /// An evaluation record holds information about the evaluation status 
  /// and result of a logical expression.
  /// </summary>
  /// <typeparam name="T">The record type.</typeparam>
  public class EvaluationRecord<T> : IEvaluationRecord<T>
  {
    /// <summary>
    /// Whether the evaluation has finished.
    /// </summary>
    public bool m_finished;
    /// <summary>
    /// The result of the evaluation.
    /// </summary>
    public T m_result;

    /// <summary>
    /// Whether the evaluation has finished.
    /// </summary>
    public bool Finished
    {
      get { return m_finished; }
      set { m_finished = value; }
    }

    /// <summary>
    /// The result of the evaluation.
    /// </summary>
    public T Result
    {
      get { return m_result; }
      set { m_result = value; }
    }

    /// <summary>
    /// Creates a new evaluation record and assumes the evaluation has 
    /// not finished yet.
    /// </summary>
    public EvaluationRecord()
    {
      this.Finished = false;
    }

    /// <summary>
    /// Creates a new evaluation record and sets the result of the evaluation.
    /// </summary>
    /// <param name="result">The result of the evaluation.</param>
    public EvaluationRecord(T result)
    {
      this.Finished = true;
      this.Result = result;
    }
  }

  /// <summary>
  /// This wrapper wraps a Bool evaluation record into a FuzzyBool evaluation record.
  /// </summary>
  public class FuzzyCertaintyEvaluationRecordWrapper : IEvaluationRecord<FuzzyBoolValue>
  {
    /// <summary>
    /// The wrapped Bool evaluation record.
    /// </summary>
    private IEvaluationRecord<BoolValue> m_eval;

    /// <summary>
    /// Creates a new wrapper for the specified wrapped Bool evaluation record.
    /// </summary>
    /// <param name="eval">The wrapped Bool evaluation record.</param>
    public FuzzyCertaintyEvaluationRecordWrapper(IEvaluationRecord<BoolValue> eval)
    {
      this.m_eval = eval;
    }

    /// <summary>
    /// Whether the evaluation has finished.
    /// </summary>
    public bool Finished
    {
      get
      {
        return m_eval.Finished;
      }
      set
      {
        m_eval.Finished = value;
      }
    }

    /// <summary>
    /// The result of the evaluation.
    /// </summary>
    public FuzzyBoolValue Result
    {
      get
      {
        return FuzzyBool.BoolValueToFuzzyBoolValue(m_eval.Result);
      }
      set
      {
        m_eval.Result = FuzzyBool.FuzzyBoolValueToBoolValue(value);
      }
    }
  }

  /// <summary>
  /// This wrapper wraps a Double evaluation record into a FuzzyDouble evaluation record.
  /// </summary>
  public class FuzzyNumericEvaluationRecordWrapper : IEvaluationRecord<FuzzyDouble>
  {
    /// <summary>
    /// The wrapped Double evaluation record.
    /// </summary>
    private IEvaluationRecord<Double> m_eval;

    /// <summary>
    /// Creates a new wrapper for the specified wrapped Double evaluation record.
    /// </summary>
    /// <param name="eval">The wrapped Double evaluation record.</param>
    public FuzzyNumericEvaluationRecordWrapper(IEvaluationRecord<Double> eval)
    {
      this.m_eval = eval;
    }

    /// <summary>
    /// Whether the evaluation has finished.
    /// </summary>
    public bool Finished
    {
      get
      {
        return m_eval.Finished;
      }
      set
      {
        m_eval.Finished = value;
      }
    }

    /// <summary>
    /// The result of the evaluation.
    /// </summary>
    public FuzzyDouble Result
    {
      get
      {
        return new FuzzyDouble(m_eval.Result);
      }
      set
      {
        m_eval.Result = FuzzyDouble.FuzzyDoubleToDouble(value);
      }
    }
  }

  /// <summary>
  /// This wrapper wraps a ConstantExp evaluation record into a FuzzyConstantExp
  /// evaluation record.
  /// </summary>
  public class FuzzyObjectEvaluationRecordWrapper : IEvaluationRecord<FuzzyConstantExp>
  {
    /// <summary>
    /// The wrapped ConstantExp evaluation record.
    /// </summary>
    private IEvaluationRecord<ConstantExp> m_eval;

    /// <summary>
    /// Creates a new wrapper for the specified wrapped ConstantExp evaluation record.
    /// </summary>
    /// <param name="eval">The wrapped ConstantExp evaluation record.</param>
    public FuzzyObjectEvaluationRecordWrapper(IEvaluationRecord<ConstantExp> eval)
    {
      this.m_eval = eval;
    }

    /// <summary>
    /// Whether the evaluation has finished.
    /// </summary>
    public bool Finished
    {
      get
      {
        return m_eval.Finished;
      }
      set
      {
        m_eval.Finished = value;
      }
    }

    /// <summary>
    /// The result of the evaluation.
    /// </summary>
    public FuzzyConstantExp Result
    {
      get
      {
        return new FuzzyConstantExp(m_eval.Result);
      }
      set
      {
        m_eval.Result = FuzzyConstantExp.FuzzyConstantExpToConstantExp(value);
      }
    }
  }
}
