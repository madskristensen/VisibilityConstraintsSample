# VisibilityConstraints example

This sample shows how to use the `<VisibilityConstraints>` element in a Visual Studio extension to remove the need to autoload a package.

So if you use the `ProvideAutoload` attribute in an extension today, this example is for you. The benefit is that your extension won't negatively impact the performance of Visual Studio by loaded when not needed.

## What is VisibilityConstraints?

Often a package is being loaded to run a `BeforeQueryStatus` method for a button to determine its visibility. With `<VisibilityConstraints>` we can determine the visibility of a button without running BeforeQueryStatus and therefore don't need to load the package before the user clicks the button.

It is a best practice to never load a package before it is needed, and `<VisibilityConstraints>` allow us to load the package only when requested and not before.

## How to
First we must specify a rule for when a button should be visible. In this example, the rule is that the button should be visible when the user right-clicks a .cs or .vb file in Solution Explorer. We can express that in an attribute on the `Package` or `AsyncPackage` class like so:

```c#
[ProvideUIContextRule(_uiContextSupportedFiles,
    name: "Supported Files",
    expression: "CSharp | VisualBasic",
    termNames: new[] { "CSharp", "VisualBasic" },
    termValues: new[] { "HierSingleSelectionName:.cs$", "HierSingleSelectionName:.vb$" })]
```

Then we must register a `<VisibilityConstraint`> based on that rule in the .vsct file like so:

```xml
<VisibilityConstraints>
  <VisibilityItem guid="guidMyButtonPackageCmdSet" id="MyButtonId" context="uiContextSupportedFiles" />
</VisibilityConstraints>
```

...and remember to mark the button itself as dynamic visible:

```xml
<CommandFlag>DynamicVisibility</CommandFlag>
```

## Ful documentation
Read the docs for all the details surrounding these scenarios.

* [Use Rule-based UI Context for Visual Studio Extensions](https://docs.microsoft.com/visualstudio/extensibility/how-to-use-rule-based-ui-context-for-visual-studio-extensions)
* [VisibilityItem element](https://docs.microsoft.com/en-us/visualstudio/extensibility/visibilityitem-element)
* [Use AsyncPackage to Load VSPackages in the Background](https://docs.microsoft.com/visualstudio/extensibility/how-to-use-asyncpackage-to-load-vspackages-in-the-background)