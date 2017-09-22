﻿using System.Collections.Generic;
using JetBrains.ReSharper.Plugins.FSharp.Common.Checker;
using JetBrains.ReSharper.Plugins.FSharp.Psi.Impl.Cache2;
using JetBrains.ReSharper.Plugins.FSharp.Psi.Parsing;
//using JetBrains.ReSharper.Plugins.FSharp.Services.Formatter;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeStyle;
using JetBrains.ReSharper.Psi.CSharp.Impl;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Caches2;
using JetBrains.ReSharper.Psi.FSharp.Parsing;
using JetBrains.ReSharper.Psi.Impl;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace JetBrains.ReSharper.Plugins.FSharp.Psi.LanguageService
{
  [Language(typeof(FSharpLanguage))]
  public class FSharpLanguageService : ReSharper.Psi.LanguageService
  {
    private readonly FSharpCheckerService myFSharpCheckerService;
    private readonly ILogger myLogger;

    public FSharpLanguageService(PsiLanguageType psiLanguageType, IConstantValueService constantValueService,
      FSharpCheckerService fSharpCheckerService, ILogger logger)
      : base(psiLanguageType, constantValueService)
    {
      myFSharpCheckerService = fSharpCheckerService;
      myLogger = logger;
      CacheProvider = new FSharpCacheProvider(fSharpCheckerService);
    }

    public override ICodeFormatter CodeFormatter => null;
    public override ILexerFactory GetPrimaryLexerFactory() => new FSharpFakeLexerFactory();
    public override ILexer CreateFilteringLexer(ILexer lexer) => lexer;

    public override bool IsTypeMemberVisible(ITypeMember member) =>
      (member as IFSharpTypeMember)?.IsVisibleFromFSharp ?? true;

    public override IParser CreateParser(ILexer lexer, IPsiModule module, IPsiSourceFile sourceFile) =>
      new FSharpParser(sourceFile, myFSharpCheckerService, myLogger);

    public override IDeclaredElementPresenter DeclaredElementPresenter =>
      CSharpDeclaredElementPresenter.Instance; // todo: replace with F#-specific presenter

    public override IEnumerable<ITypeDeclaration> FindTypeDeclarations(IFile file) =>
      EmptyList<ITypeDeclaration>.Instance;

    public override ILanguageCacheProvider CacheProvider { get; }
    public override bool IsCaseSensitive => true;
    public override bool SupportTypeMemberCache => true;
    public override ITypePresenter TypePresenter => CLRTypePresenter.Instance;
  }
}