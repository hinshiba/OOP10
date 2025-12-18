using Avalonia.Controls;
using Avalonia.Controls.Templates;
using AvaloniaApplication1.ViewModels;
using System;
using System.Diagnostics.CodeAnalysis;

namespace AvaloniaApplication1;

/// <summary>
/// Given a view model, returns the corresponding view if possible.
/// </summary>
[RequiresUnreferencedCode(
    "Default implementation of ViewLocator involves reflection which may be trimmed away.",
    Url = "https://docs.avaloniaui.net/docs/concepts/view-locator")]
public class ViewLocator: IDataTemplate {
    public Control? Build(object? param) {

        if(param is null) {
            return null;
        }

        string name = param.GetType().FullName!.Replace("ViewModel", "View", StringComparison.Ordinal);
        Type? type = Type.GetType(name);

        return type != null ? (Control)Activator.CreateInstance(type)! : new TextBlock { Text = "Not Found: " + name };
    }

    public bool Match(object? data) {
        return data is ViewModelBase;
    }
}
