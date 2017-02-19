﻿using System;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using Microsoft.FSharp.Compiler.SourceCodeServices;

namespace JetBrains.ReSharper.Psi.FSharp.Tree
{
  public partial interface IFSharpFile : IFileImpl
  {
    [CanBeNull]
    FSharpParseFileResults ParseResults { get; set; }

    [CanBeNull]
    FSharpCheckFileResults GetCheckResults(Action interruptChecker = null);

    /// <summary>
    /// True when SetResolvedSymbolsStageProcess is finished.
    /// </summary>
    bool ReferencesResolved { get; set; }

  }
}