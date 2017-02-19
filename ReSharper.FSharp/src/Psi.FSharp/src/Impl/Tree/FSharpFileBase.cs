﻿using System;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.FSharp.Tree;
using JetBrains.ReSharper.Psi.FSharp.Util;
using Microsoft.FSharp.Compiler.SourceCodeServices;

namespace JetBrains.ReSharper.Psi.FSharp.Impl.Tree
{
  internal abstract class FSharpFileBase : FileElementBase, IFSharpFile
  {
    public FSharpParseFileResults ParseResults { get; set; }
    private FSharpCheckFileResults CheckResults { get; set; }


    public FSharpCheckFileResults GetCheckResults([CanBeNull] Action interruptChecker = null)
    {
      return CheckResults ?? (CheckResults = FSharpCheckerUtil.CheckFSharpFile(this, interruptChecker));
    }

    public bool ReferencesResolved { get; set; }

    public override PsiLanguageType Language => FSharpLanguage.Instance;

    public virtual void Accept(TreeNodeVisitor visitor)
    {
      visitor.VisitNode(this);
    }

    public virtual void Accept<TContext>(TreeNodeVisitor<TContext> visitor, TContext context)
    {
      visitor.VisitNode(this, context);
    }

    public virtual TReturn Accept<TContext, TReturn>(TreeNodeVisitor<TContext, TReturn> visitor, TContext context)
    {
      return visitor.VisitNode(this, context);
    }
  }
}