namespace JetBrains.ReSharper.Plugins.FSharp.Services.Formatter

open Fantomas
open Fantomas.FormatConfig
open JetBrains.Application.Infra
open JetBrains.DocumentModel
open JetBrains.DocumentModel.Impl
open JetBrains.ReSharper.Feature.Services.CodeCleanup
open JetBrains.ReSharper.Plugins.FSharp.ProjectModelBase
open JetBrains.ReSharper.Psi
open JetBrains.ReSharper.Psi.CodeStyle
open JetBrains.ReSharper.Psi.FSharp
open JetBrains.ReSharper.Psi.FSharp.Tree
open JetBrains.ReSharper.Resources.Shell
open JetBrains.Util

[<CodeCleanupModule>]
type ReformatCode() =
    interface ICodeCleanupModule with
        member x.LanguageType = FSharpLanguage.Instance :> _
        member x.Descriptors = EmptyList<_>.Instance :> _
        member x.IsAvailableOnSelection = false
        member x.SetDefaultSetting(_,_) = ()
        member x.IsAvailable(file) = file.LanguageType :? FSharpProjectFileType
        
        member x.Process(file,_,_,_) =
            match file.GetTheOnlyPsiFile() with
            | :? IFSharpFile as fsFile
                    when fsFile.ParseResults.IsSome && fsFile.ParseResults.Value.ParseTree.IsSome ->
                let parsedInput = fsFile.ParseResults.Value.ParseTree.Value
                let filePath = file.GetLocation().FullPath
                let document = file.Document :?> DocumentBase
                let source = document.GetText()

                let settings = file.GetFormatterSettings(file.PrimaryPsiLanguage) :?> FSharpFormatSettingsKey
                let formatConfig = { FormatConfig.Default with
                                         PageWidth = settings.WRAP_LIMIT
                                         IndentSpaceNum = settings.INDENT_SIZE
                                         ReorderOpenDeclaration = settings.ReorderOpenDeclarations
                                         SpaceBeforeColon = settings.SpaceBeforeColon
                                         SpaceAfterComma = settings.SpaceAfterComma
                                         SpaceAfterSemicolon = settings.SpaceAfterSemicolon
                                         IndentOnTryWith = settings.IndentOnTryWith
                                         SpaceAroundDelimiter = settings.SpaceAroundDelimiter }

                let formatted = CodeFormatter.FormatAST(parsedInput, filePath, Some source, formatConfig)
                let stamp = document.LastModificationStamp
                let modificationSide = TextModificationSide.NotSpecified
                let change = DocumentChange(document, 0, source.Length, formatted, stamp, modificationSide)
                document.ChangeDocument(change, TimeStamp.NextValue)
                file.GetPsiServices().Files.CommitAllDocuments()
            | _ -> ()